using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;
using OsEngine.Robots.FrontRunner.View;
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

            _tab.PositionOpeningFailEvent += _tab_PositionOpeningFailEvent;
        }

       





        #region Fields========================================================================

        public decimal BigVolume = 200;

        public int Offset = 1;

        public int Take = 20;

        public decimal Lot = 1;

        public Position Position = null;

        private BotTabSimple _tab;

        #endregion

        #region Properties ==================================================================

        public Edit Edit
        {
            get => _edit;

            set
            {
                _edit = value;

                if (_edit == Edit.Stop && Position != null && Position.State == PositionStateType.Opening)
                {
                    _tab.CloseAllOrderInSystem();
                }
            }
        }

        Edit _edit = ViewModel.Edit.Stop;

        #endregion

        #region Methods =====================================================================

        private void _tab_PositionOpeningFailEvent(Position pos)
        {
            Position = null;
        }

       
        private void _tab_MarketDepthUpdateEvent(MarketDepth marketDepth)
        {
            if (marketDepth.SecurityNameCode != _tab.Securiti.Name)
            {
                return;
            }

            if (Edit == Edit.Stop)
            {
                return;
            }

            List<Position> _positions = _tab.PositionsOpenAll;

            if (_positions != null && _positions.Count > 0)
            {
                foreach (Position pos in _positions)
                {
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
            }

            

            for (int i = 0; i < marketDepth.Asks.Count; i++)
            {
                if (marketDepth.Asks[i].Ask >= BigVolume && Position == null)  // пробегаемся по стакану асков: если есть заявка на большой объем? нет открытых позиций и нет активных заявок, выставляем лимитку на продажу
                {
                    decimal price = marketDepth.Asks[i].Price - Offset * _tab.Securiti.PriceStep;

                    //_tab.SellAtLimit(Lot, price);
                }

                if (Position != null && marketDepth.Asks[i].Price == Position.EntryPrice 
                                     && marketDepth.Asks[i].Ask < BigVolume/3)// если позиция открыта, текущая цена равна цене открытия и заявка с большим объемом сократилась более чем в 3? раза:
                {
                    //_tab.CloseAtMarket(Position, Position.OpenVolume);// закрываем открытую позицию по рынку
                }
            }

            for (int i = 0; i < marketDepth.Bids.Count; i++)    
            {
                if (marketDepth.Bids[i].Bid >= BigVolume && Position == null)  // пробегаемся по стакану бидов: если есть заявка на большой объем, нет открытых позиций и нет активных заявок, выставляем лимитку на покупку
                {
                    decimal price = marketDepth.Bids[i].Price + Offset * _tab.Securiti.PriceStep;

                    Position = _tab.BuyAtLimit(Lot, price);

                    if (Position.State != PositionStateType.Open && Position.State != PositionStateType.Opening)
                    {
                        Position = null;
                    }
                }

                if (Position != null && marketDepth.Bids[i].Price == Position.EntryPrice - Offset * _tab.Securiti.PriceStep
                                     && marketDepth.Bids[i].Bid < BigVolume / 2)// если позиция открыта, текущая цена равна цене открытия и заявка с большим объемом сократилась более чем в 3? раза:
                {
                    if (Position.State == PositionStateType.Open) // позиция уже открыта
                    {
                        _tab.CloseAtMarket(Position, Position.OpenVolume);// закрываем открытую позицию по рынку
                    }
                    else if (Position.State == PositionStateType.Opening) // позиция открывается, т.е лимитка еще не сработала
                    {
                        _tab.CloseAllOrderInSystem(); // снимаем все заявки
                    }
                    
                    
                }
                else if (Position != null && Position.State == PositionStateType.Opening &&
                         marketDepth.Bids[i].Bid >= BigVolume && marketDepth.Bids[i].Price > Position.EntryPrice - Offset * _tab.Securiti.PriceStep)
                {
                    _tab.CloseAllOrderInSystem(); // снимаем все заявки
                    Position = null;
                    break;
                }
            }
        }

        public override string GetNameStrategyType()
        {
            return nameof(FrontRunnerBot);
        }

        public override void ShowIndividualSettingsDialog()
        {
            FrontRunnerUI window = new FrontRunnerUI(this);

            window.ShowDialog();
        }

        #endregion
    }
}
