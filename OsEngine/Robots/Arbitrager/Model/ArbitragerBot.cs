using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.Market.Servers.SmartCom;
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

            _spot.MyTradeEvent += _spot_MyTradeEvent;

            _fut.MyTradeEvent += _fut_MyTradeEvent;


        }

        







        #region Fields========================================================================

        public decimal Spread;

        public decimal Take = 50;

        public decimal Step = 50;

        public decimal Lot = 1;

        public List<MyTrade> Trades = new List<MyTrade>();

        private List<decimal> _takes = new List<decimal>();

        private decimal _spread;

        private BotTabSimple _spot;

        private BotTabSimple _fut;

        private MarketDepth _marketDepthSpot;

        private decimal _openPrice = 0;

        private decimal _openPriceSpot = 0;

        private decimal _openPriceFut = 0;

        private bool _flag = true;

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

            if (Trades.Count == 0)
            {
                _spread = Spread;
            }

            
            if ((Trades.Count == 0 || Trades.Count % 2 == 0) && _flag && _marketDepthSpot != null && ((marketDepthFut.Asks[0].Price - _marketDepthSpot.Bids[0].Price * 1000) < _spread))
            {
                _spot.SellAtMarket(Lot);

                _fut.BuyAtMarket(Lot);

                _spread -= Step;

                _flag = false;

            }

            if (_flag && Trades.Count > 0 && _takes != null && _takes.Count != 0 && _marketDepthSpot != null && ((marketDepthFut.Bids[0].Price - _marketDepthSpot.Asks[0].Price * 1000) > (_takes.Last())))
            {
                _spot.BuyAtMarket(Lot);
                _fut.SellAtMarket(Lot);

                _takes.RemoveAt(_takes.Count-1);

                _spread += Step;

            }

        }

        private void _spot_MyTradeEvent(MyTrade trade)
        {
            if (trade == null || trade.SecurityNameCode != (_spot.Securiti.Name + " TestPaper")) return;

            if (trade.Side == Side.Sell)
            {
                Trades.Add(trade);

                GetOpenPrice(trade);
            }
        }

        private void _fut_MyTradeEvent(MyTrade trade)
        {
            if (trade == null || trade.SecurityNameCode != (_fut.Securiti.Name + " TestPaper")) return;

            if (trade.Side == Side.Buy)
            {
                Trades.Add(trade);

                GetOpenPrice(trade);
            }
        }

        private void GetOpenPrice(MyTrade trade)
        {
            if (trade.SecurityNameCode == _spot.Securiti.Name + " TestPaper")
            {
                _openPriceSpot = trade.Price * 1000;
            }

            if (trade.SecurityNameCode == _fut.Securiti.Name + " TestPaper")
            {
                _openPriceFut = trade.Price;
            }

            if (_openPriceFut != 0 && _openPriceSpot != 0)
            {
                _openPrice =  _openPriceFut - _openPriceSpot;

                _takes.Add(Take + _openPrice);

                _flag = true;

                _openPriceSpot = 0;
                _openPriceFut = 0;


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
