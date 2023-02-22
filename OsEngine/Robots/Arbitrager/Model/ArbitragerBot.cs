using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;
using OsEngine.Robots.Arbitrager.View;
using OsEngine.Robots.Arbitrager.ViewModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static OsEngine.Robots.Arbitrager.ViewModel.VM;

namespace OsEngine.Robots.Arbitrager.Model
{
    public class ArbitragerBot : BotPanel
    {
        public ArbitragerBot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple);

            TabCreate(BotTabType.Simple);

            _spot = TabsSimple[0];

            _fut = TabsSimple[1];

            _fut.MarketDepthUpdateEvent += _fut_MarketDepthUpdateEvent;

            _spot.MarketDepthUpdateEvent += _spot_MarketDepthUpdateEvent;

        }

        



        #region Fields========================================================================

        public decimal Spread = -500;

        public decimal Take = 50;

        public decimal Lot = 1;

        public Position Position = null;

        public List<Position> Positions = new List<Position>();

        private BotTabSimple _spot;

        private BotTabSimple _fut;

        private MarketDepth _marketDepthSpot;

        private decimal _openPrice = 0;

        #endregion

        #region Properties ===================================================================

        public Edit EDIT
        {
            get => _edit;

            set
            {
                _edit = value;
            }
        }

        private Edit _edit = VM.Edit.Stop;

        #endregion


        #region Methods ======================================================================

        public void _spot_MarketDepthUpdateEvent(MarketDepth marketDepthSpot)
        {
            _marketDepthSpot = marketDepthSpot.GetCopy();
        }

        public void _fut_MarketDepthUpdateEvent(MarketDepth marketDepthFut)
        {
            if (marketDepthFut.SecurityNameCode != _fut.Securiti.Name)
            {
                return;
            }

            if (EDIT == Edit.Stop)
            {
                return;
            }

            
            if (Position == null&& _marketDepthSpot != null && ((marketDepthFut.Asks[0].Price - _marketDepthSpot.Bids[0].Price * 1000) < Spread))
            {
                Position = _spot.SellAtMarket(Lot);

                Positions.Add(Position);

                Position = _fut.BuyAtMarket(Lot);

                Positions.Add(Position);
                
                _openPrice = _fut.GetJournal().LastPosition.EntryPrice - _spot.GetJournal().LastPosition.EntryPrice * 1000;
            }

            if (Position != null && _marketDepthSpot != null && ((marketDepthFut.Bids[0].Price - _marketDepthSpot.Asks[0].Price * 1000) > (Take + _openPrice)))
            {
                _spot.BuyAtMarket(Lot);
                _fut.SellAtMarket(Lot);

                Position = null;
            }

        }
        public override string GetNameStrategyType()
        {
            return nameof(ArbitragerBot);
        }

        public override void ShowIndividualSettingsDialog()
        {
            ArbitragerUI window = new ArbitragerUI(this);

            window.Show();
        }

        #endregion


    }
}
