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

            _tab.PositionOpeningSuccesEvent += _tab_PositionOpeningSuccesEvent; // событие на исполнение лимитки
        }

        



        #region Fields========================================================================

        public decimal BigVolume;

        public int Offset;

        public int Take;

        public decimal Lot;

        public Position Position = null;

        private BotTabSimple _tab;

        private bool _stateBid = true;

        private bool _stateAsk = true;

        #endregion

        #region Methods =====================================================================

        private void _tab_PositionOpeningSuccesEvent(Position pos)
        {
            Position = pos;  // обновление позиции

            _tab.CloseAllOrderInSystem(); // снимаем выставленные до этого лимитные заявки

            if (pos.Direction == Side.Sell) // если позиция открылась в шорт:
            {
                decimal takePrice = Position.EntryPrice - Take * _tab.Securiti.PriceStep;

                _tab.CloseAtProfit(Position, takePrice, takePrice); // выставляем тейк-профит
            }
            else if (pos.Direction == Side.Buy) // если позиция открылась в лонг:
            {
                decimal takePrice = Position.EntryPrice + Take * _tab.Securiti.PriceStep;

                _tab.CloseAtProfit(Position, takePrice, takePrice); // выставляем тейк-профит
            }
        }

        private void _tab_MarketDepthUpdateEvent(MarketDepth marketDepth)
        {
            if (marketDepth.SecurityNameCode != _tab.Securiti.Name)
            {
                return;
            }

            for (int i = 0; i < marketDepth.Asks.Count; i++)
            {
                if (marketDepth.Asks[i].Ask >= BigVolume && Position == null
                    && _stateAsk)  // пробегаемся по стакану асков: если есть заявка на большой объем? нет открытых позиций и нет активных заявок, выставляем лимитку на продажу
                {
                    decimal price = marketDepth.Asks[i].Price - Offset * _tab.Securiti.PriceStep;

                    _stateAsk = false;

                    _tab.SellAtLimit(Lot, price);
                }

                if (Position != null && marketDepth.Asks[i].Price == Position.EntryPrice 
                                     && marketDepth.Asks[i].Ask < BigVolume/3)// если позиция открыта, текущая цена равна цене открытия и заявка с большим объемом сократилась более чем в 3? раза:
                {
                    _tab.CloseAtMarket(Position, Position.OpenVolume);// закрываем открытую позицию по рынку
                }
            }

            for (int i = 0; i < marketDepth.Bids.Count; i++)    
            {
                if (marketDepth.Bids[i].Bid >= BigVolume && Position == null && _stateBid)  // пробегаемся по стакану бидов: если есть заявка на большой объем, нет открытых позиций и нет активных заявок, выставляем лимитку на покупку
                {
                    decimal price = marketDepth.Bids[i].Price + Offset * _tab.Securiti.PriceStep;

                    _stateBid = false;

                    _tab.BuyAtLimit(Lot, price);
                }

                if (Position != null && marketDepth.Bids[i].Price == Position.EntryPrice
                                     && marketDepth.Bids[i].Bid < BigVolume / 3)// если позиция открыта, текущая цена равна цене открытия и заявка с большим объемом сократилась более чем в 3? раза:
                {
                    _tab.CloseAtMarket(Position, Position.OpenVolume);// закрываем открытую позицию по рынку
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
