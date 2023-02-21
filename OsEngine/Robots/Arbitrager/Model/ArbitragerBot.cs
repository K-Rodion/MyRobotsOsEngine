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

        private BotTabSimple _spot;

        private BotTabSimple _fut;

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

        private Edit _edit;

        #endregion


        #region Methods ======================================================================

        public void _spot_MarketDepthUpdateEvent(MarketDepth marketDepthSpot)
        {
            
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

            if (Position == null)
            {
                
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
