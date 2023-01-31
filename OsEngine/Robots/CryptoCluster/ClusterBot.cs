using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Charts.CandleChart.Indicators;
using OsEngine.Entity;
using OsEngine.Indicators;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OsEngine.Robots.CryptoCluster
{
    public class ClusterBot : BotPanel
    {
        public ClusterBot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple);

            _tabSimple = TabsSimple[0];

            TabCreate(BotTabType.Cluster);

            _tabCluster = TabsCluster[0];

            Mode = CreateParameter("Mode", false);
            Koef = CreateParameter("Koef", 3, 3, 9, 1);
            CountCandles = CreateParameter("CountCandles", 5, 3, 9, 1);
            Risk = CreateParameter("Risk %", 1m, 0.1m, 2m, 0.1m);
            Stop = CreateParameter("Stop ATR", 1, 1, 5, 1);
            Take = CreateParameter("Take ATR", 3, 1, 5, 1);

            _atr = IndicatorsFactory.CreateIndicatorByName("ATR", name + "ATR", false);
            _atr.ParametersDigit[0].Value = 100;
            _atr = (Aindicator)_tabSimple.CreateCandleIndicator(_atr, "Second");
            _atr.PaintOn = true;
            _atr.Save();

            _tabSimple.CandleFinishedEvent += _tabSimple_CandleFinishedEvent;
        }

        

        #region Fields ========================================================================

        private BotTabSimple _tabSimple;

        private BotTabCluster _tabCluster;

        public StrategyParameterBool Mode;

        public StrategyParameterInt Koef;

        public StrategyParameterInt CountCandles;

        public StrategyParameterDecimal Risk;

        public StrategyParameterInt Stop;

        public StrategyParameterInt Take;

        private Aindicator _atr;

        private decimal _stopPrice = 0;

        private decimal _takePrice = 0;

        #endregion

        #region Methods =======================================================================

        private void _tabSimple_CandleFinishedEvent(List<Candle> candles)
        {
            if (candles.Count < CountCandles.ValueInt
                || _tabCluster.VolumeClusters.Count < CountCandles.ValueInt)
            {
                return;
            }

            List<Position> positions = _tabSimple.PositionOpenLong;

            if (positions == null || positions.Count == 0)
            {
                decimal average = 0;
                for (int i = _tabCluster.VolumeClusters.Count - CountCandles.ValueInt; i < _tabCluster.VolumeClusters.Count-1; i++)
                {
                    average += _tabCluster.VolumeClusters[i].MaxSummVolumeLine.VolumeSumm;
                }

                average /= (CountCandles.ValueInt - 1);

                HorizontalVolumeLine last = _tabCluster.VolumeClusters[_tabCluster.VolumeClusters.Count - 1].MaxSummVolumeLine;

                if (last.VolumeSumm > average * Koef.ValueInt 
                    && last.VolumeDelta < 0)
                {
                    decimal lastAtr = _atr.DataSeries[0].Last;

                    decimal moneyRisk = _tabSimple.Portfolio.ValueBegin * Risk.ValueDecimal / 100;

                    decimal volume = moneyRisk / (lastAtr * Stop.ValueInt);

                    _tabSimple.BuyAtMarket();
                }
            }
            else
            {
                
            }
        }

        public override string GetNameStrategyType()
        {
            return nameof(ClusterBot);
        }

        public override void ShowIndividualSettingsDialog()
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
