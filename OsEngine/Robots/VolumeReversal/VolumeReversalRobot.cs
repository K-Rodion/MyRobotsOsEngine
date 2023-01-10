using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;

namespace OsEngine.Robots.VolumeReversal
{
    public class VolumeReversalRobot : BotPanel
    {
        public VolumeReversalRobot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            this.TabCreate(BotTabType.Simple);

            _tab = TabsSimple[0];

            CreateParameter("Mode", "Edit", new[] { "Edit", "Trade" });

            _risk = CreateParameter("Risk %", 1m, 0.1m, 10m, 0.1m);

            _profitKoef = CreateParameter("Koef Profit", 1m, 0.1m, 10m, 0.1m);

            _countDownCandles = CreateParameter("Count Down Candles", 1, 1, 5, 1);

            _koefVolume = CreateParameter("Koef volume", 2m, 2m, 10m, 0.5m);

            _countCandles = CreateParameter("Count candles", 10, 5, 50, 1);

            _tab.CandleFinishedEvent += _tab_CandleFinishedEvent;

            _tab.PositionOpeningSuccesEvent += _tab_PositionOpeningSuccesEvent;

            _tab.PositionClosingSuccesEvent += _tab_PositionClosingSuccesEvent;

            
        }

        





        #region Fields==========================================================================================

        private BotTabSimple _tab;

        /// <summary>
        /// Риск на сделку
        /// </summary>
        private StrategyParameterDecimal _risk;

        /// <summary>
        /// Во сколько раз тейк больше риска
        /// </summary>
        private StrategyParameterDecimal _profitKoef;

        /// <summary>
        /// Кол-во падающих свечей перед объемным разворотом
        /// </summary>
        private StrategyParameterInt _countDownCandles;

        /// <summary>
        /// Во сколько раз объем превышает средний
        /// </summary>
        private StrategyParameterDecimal _koefVolume;

        /// <summary>
        /// Средний объем
        /// </summary>
        private decimal _averageVolume;

        /// <summary>
        /// Кол-во свечей для вычисления среднего объема
        /// </summary>
        private StrategyParameterInt _countCandles;

        /// <summary>
        /// Кол-во пунктов до стоп-лосса
        /// </summary>
        private int _punkts = 0;

        private decimal _lowCandle;

        private decimal _priceBuy;




        #endregion


        #region Methods

        private void _tab_CandleFinishedEvent(List<Candle> candles)
        {
            List<Position> positions = _tab.PositionOpenLong;

            if (positions.Count > 0 && candles[candles.Count-1].Close > _priceBuy + _priceBuy - _lowCandle)
            {
                var position = _tab.PositionOpenLong.First();

                UpdateStopLoss(position);
            }

            if (positions.Count > 0)
            {
                return;
            }

            if (candles.Count < _countDownCandles.ValueInt +1
                || candles.Count < _countCandles.ValueInt+1)
            {
                return;
            }

            _averageVolume = 0;

            for (int i = candles.Count-2; i > candles.Count - 2 - _countCandles.ValueInt; i--)
            {
                _averageVolume += candles[i].Volume;
            }

            _averageVolume /= _countCandles.ValueInt;

            

            Candle candle = candles[candles.Count - 1];

            if (candle.Close < (candle.High + candle.Low)/2
                || candle.Volume < _averageVolume*_koefVolume.ValueDecimal)
            {
                return;
            }

            for (int i = candles.Count-2; i > candles.Count - 2 - _countDownCandles.ValueInt; i--)
            {
                if (candles[i].Close > candles[i].Open)
                {
                    return;
                }
            }

            _punkts = (int)((candle.Close - candle.Low)/_tab.Securiti.PriceStep);

            if (_punkts < 5)
            {
                return;
            }

            decimal amountStop = _punkts * _tab.Securiti.PriceStepCost;

            decimal amountRisk = _tab.Portfolio.ValueBegin * _risk.ValueDecimal / 100;

            decimal volume = amountRisk / amountStop;

            decimal go = 10000;

            if (_tab.Securiti.Go > 1)
            {
                go = _tab.Securiti.Go;
            }

            decimal maxLot = _tab.Portfolio.ValueBegin / go;

            if (volume < maxLot)
            {
                _lowCandle = candle.Low;

                _priceBuy = candles[candles.Count-1].Close;

                _tab.BuyAtMarket(volume);

            }
            
        }

        private void UpdateStopLoss(Position position)
        {
            _tab.SellAtStopCancel();

            _tab.CloseAtStop(position, _priceBuy, _priceBuy - 100 * _tab.Securiti.PriceStep);
        }

        private void _tab_PositionOpeningSuccesEvent(Position pos)
        {
            decimal priceTake = pos.EntryPrice + _punkts * _profitKoef.ValueDecimal;
            
            _tab.CloseAtProfit(pos, priceTake, priceTake);

            _tab.CloseAtStop(pos, _lowCandle, _lowCandle - 100 * _tab.Securiti.PriceStep);

        }

        private void _tab_PositionClosingSuccesEvent(Position pos)
        {
            SaveCSV(pos);
        }

        void SaveCSV(Position pos)
        {
            

            if (!File.Exists(@"Engine\trades.csv"))
            {
                string header = ";Позиция;Символ;Лоты;Изменение/Максимум " +
                                "Лотов;Исполнение входа;Сигнал входа;Бар " +
                                "входа;Дата входа;Время входа;Цена входа;" +
                                "Комиссия входа;Исполнение выхода;Сигнал " +
                                "выхода;Бар выхода;Дата выхода;Время выхода;" +
                                "Цена выхода;Комиссия выхода;Средневзвешенная " +
                                "цена входа;П/У;П/У сделки;П/У с одного лота;" +
                                "Зафиксированная П/У;Открытая П/У;Продолж. " +
                                "(баров);Доход/Бар;Общий П/У;% изменения;MAE;" +
                                "MAE %;MFE;MFE %";

                using (StreamWriter writer = new StreamWriter(@"Engine\trades.csv", false))
                {
                    writer.WriteLine(header);

                    writer.Close();
                }
            }

            using (StreamWriter writer = new StreamWriter(@"Engine\trades.csv", true))
            {
                string str = ";;;;;;;;" + pos.TimeOpen.ToShortDateString();
                str += ";" + pos.TimeOpen.TimeOfDay;
                str += ";;;;;;;;;;;;;;" + pos.ProfitPortfolioPunkt + ";;;;;;;;;";

                writer.WriteLine(str);



                writer.Close();
            }
        }

        public override string GetNameStrategyType()
        {
            return nameof(VolumeReversalRobot);
        }

        public override void ShowIndividualSettingsDialog()
        {
            WindowVolumeReversalRobot window = new WindowVolumeReversalRobot(this);

            window.ShowDialog();
        }

        #endregion

    }
}
