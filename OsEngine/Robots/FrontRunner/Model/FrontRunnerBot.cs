using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;
using OsEngine.Robots.FrontRunner.ViewModel;

namespace OsEngine.Robots.FrontRunner.Model
{
    public class FrontRunnerBot : BotPanel
    {
        public FrontRunnerBot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple);

            _tab = TabsSimple[0];

            _tab.MarketDepthUpdateEvent += _tab_MarketDepthUpdateEvent;
        }

        

        #region Fields========================================================================

        public decimal BigVolume;

        public int Offset;

        public decimal Take;

        public decimal Lot;

        public Position Position = null;

        private BotTabSimple _tab;

        private bool _stateBid = true;

        #endregion

        #region Methods =====================================================================

        private void _tab_MarketDepthUpdateEvent(MarketDepth marketDepth)
        {
            if (marketDepth.SecurityNameCode != _tab.Securiti.Name)
            {
                return;
            }

            for (int i = 0; i < marketDepth.Asks.Count; i++)
            {
                if (marketDepth.Asks[i].Ask >= BigVolume && Position == null)
                {
                    decimal price = marketDepth.Asks[i].Price - Offset * _tab.Securiti.PriceStep;


                    _tab.SellAtLimit(Lot, price);
                }
            }

            for (int i = 0; i < marketDepth.Bids.Count; i++)
            {
                if (marketDepth.Bids[i].Bid >= BigVolume && Position == null)
                {
                    decimal price = marketDepth.Bids[i].Price + Offset * _tab.Securiti.PriceStep;


                    _tab.BuyAtLimit(Lot, price);
                }
            }
        }

        public override string GetNameStrategyType()
        {
            return nameof(FrontRunnerBot);
        }

        public override void ShowIndividualSettingsDialog()
        {

        }

        #endregion
    }
}
