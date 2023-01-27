using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.Indicators;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;

namespace OsEngine.Robots.PriceChanel
{
    public class PriceChanelFix : BotPanel
    {
        public PriceChanelFix(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple);

            _tab = TabsSimple[0];

            LenghtUp = CreateParameter("Lenght Up", 12, 5, 80, 2);

            LenghtDown = CreateParameter("Lenght Down", 12, 5, 80, 2);

            Mode = CreateParameter("Mode", "Off", new[] { "Off", "On", "OnlyLong", "OnlyShort"});

            Lot = CreateParameter("Lot", 10, 5, 20, 1);

            Risk = CreateParameter("Risk", 1m, 0.25m, 3m, 0.1m);

            _pc = IndicatorsFactory.CreateIndicatorByName("PriceChannel", name + "PriceChannel", false);//создание индикатора

            _pc.ParametersDigit[0].Value = LenghtUp.ValueInt;

            _pc.ParametersDigit[1].Value = LenghtDown.ValueInt;

            _pc = (Aindicator)_tab.CreateCandleIndicator(_pc, "Prime");// переносим индикатор в _tab для отображнения на графике

            _pc.Save();

            _tab.CandleFinishedEvent += _tab_CandleFinishedEvent;
        }

        

        #region Fields ================================================================================

        private BotTabSimple _tab;

        private Aindicator _pc;

        private StrategyParameterInt LenghtUp;

        private StrategyParameterInt LenghtDown;

        private StrategyParameterString Mode;

        private StrategyParameterInt Lot;

        private StrategyParameterDecimal Risk;

        #endregion

        #region Methods ===============================================================================

        private void _tab_CandleFinishedEvent(List<Candle> candles)
        {
            if (Mode.ValueString == "Off")
            {
                return;
            }

            if (_pc.DataSeries[0].Values == null || _pc.DataSeries[1].Values == null
                || _pc.DataSeries[0].Values.Count < LenghtUp.ValueInt +1
                || _pc.DataSeries[1].Values.Count < LenghtDown.ValueInt +1)
            {
                return;
            }

            Candle candle = candles[candles.Count - 1];// последняя свеча

            decimal lastUp = _pc.DataSeries[0].Values[_pc.DataSeries[0].Values.Count - 2];

            decimal lastDown = _pc.DataSeries[1].Values[_pc.DataSeries[1].Values.Count - 2];

            List<Position> positions = _tab.PositionsOpenAll;

            if (candle.Close > lastUp
                && candle.Open < lastUp
                && positions.Count ==0
                && (Mode.ValueString == "On" || Mode.ValueString == "OnlyLong"))
            {
                decimal riskMoney = _tab.Portfolio.ValueBegin * Risk.ValueDecimal / 100;

                decimal costPriceStep = _tab.Securiti.PriceStepCost;

                costPriceStep = 1;

                decimal steps = (lastUp - lastDown) / _tab.Securiti.PriceStep;

                decimal lot = riskMoney / (steps * costPriceStep);

                _tab.BuyAtMarket((int)lot);
            }

            if (candle.Close < lastDown
                && candle.Open > lastDown
                && positions.Count == 0
                && (Mode.ValueString == "On" || Mode.ValueString == "OnlyShort"))
            {
                decimal riskMoney = _tab.Portfolio.ValueBegin * Risk.ValueDecimal / 100;

                decimal costPriceStep = _tab.Securiti.PriceStepCost;

                costPriceStep = 1;

                decimal steps = (lastUp - lastDown) / _tab.Securiti.PriceStep;

                decimal lot = riskMoney / (steps * costPriceStep);

                _tab.SellAtMarket((int)lot);
            }

            if (positions.Count > 0)
            {
                Trailing(positions);
            }
        }

        private void Trailing(List<Position> positions)
        {
            decimal lastDown = _pc.DataSeries[1].Values.Last();

            decimal lastUp = _pc.DataSeries[0].Values.Last();

            foreach (Position pos in positions)
            {
                if (pos.State == PositionStateType.Open)
                {
                    if (pos.Direction == Side.Buy)
                    {
                        _tab.CloseAtTrailingStop(pos, lastDown, lastDown - 100*_tab.Securiti.PriceStep);
                    }
                    else if (pos.Direction == Side.Sell)
                    {
                        _tab.CloseAtTrailingStop(pos, lastUp, lastUp + 100 * _tab.Securiti.PriceStep);
                    }
                }
            }
        }

        public override string GetNameStrategyType()
        {
            return nameof(PriceChanelFix);
        }

        public override void ShowIndividualSettingsDialog()
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
