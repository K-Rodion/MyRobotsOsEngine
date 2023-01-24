using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;
using OsEngine.Robots.FrontRunner.View;
using OsEngine.Robots.FrontRunner.ViewModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

            _tab.PositionOpeningSuccesEvent += _tab_PositionOpeningSuccesEvent;

           
        }

        











        #region Fields========================================================================

        public decimal BigVolume = 200;

        public int Offset = 1;

        public int Take = 20;

        public decimal Lot = 1;

        public Position PositionLong = null;

        public Position PositionShort = null;

        private BotTabSimple _tab;

        public decimal EntryPrice;

        public string SecururityName;

        public decimal CurrentLot;

        public string State;

        public decimal ClosePrice;

        #endregion

        #region Properties ==================================================================

        public Edit Edit
        {
            get => _edit;

            set
            {
                _edit = value;

                if (_edit == Edit.Stop && ((PositionLong != null && PositionLong.State == PositionStateType.Opening) 
                                           || (PositionShort != null && PositionShort.State == PositionStateType.Opening)))
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
            //Position = null;
        }

        private void _tab_PositionOpeningSuccesEvent(Position pos)
        {
            EntryPrice = pos.EntryPrice;

            SecururityName = pos.SecurityName;

            CurrentLot = pos.OpenVolume;

            State = pos.State.ToString();

            ClosePrice = pos.ClosePrice;
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

            List<Position> _positionsLong = _tab.PositionOpenLong;

            List<Position> _positionsShort = _tab.PositionOpenShort;

            if (PositionLong != null && PositionLong.State != PositionStateType.Open && PositionLong.State != PositionStateType.Opening)
            {
                PositionLong = null;
                Log("PositionLong 5 = null");
            }

            if (PositionShort != null && PositionShort.State != PositionStateType.Open && PositionShort.State != PositionStateType.Opening)
            {
                PositionShort = null;
                Log("PositionShort 5 = null");
            }

            if (PositionLong != null && PositionLong.State == PositionStateType.Open)
            {
                decimal takePrice = PositionLong.EntryPrice + Take * _tab.Securiti.PriceStep;

                _tab.CloseAtProfit(PositionLong, takePrice, takePrice); // выставляем тейк-профит
            }

            if (PositionShort != null && PositionShort.State == PositionStateType.Open)
            {
                decimal takePrice = PositionShort.EntryPrice - Take * _tab.Securiti.PriceStep;

                _tab.CloseAtProfit(PositionShort, takePrice, takePrice); // выставляем тейк-профит
            }

            for (int i = 0; i < marketDepth.Asks.Count; i++)
            {
                if (marketDepth.Asks[i].Ask >= BigVolume &&
                    PositionShort == null) // пробегаемся по стакану асков: если есть заявка на большой объем? нет открытых позиций и нет активных заявок, выставляем лимитку на продажу
                {
                    decimal price = marketDepth.Asks[i].Price - Offset * _tab.Securiti.PriceStep;

                    PositionShort = _tab.SellAtLimit(Lot, price);

                    Log("PositionShort 1 = " + PositionShort.GetStringForSave());

                    if (PositionShort.State != PositionStateType.Open && PositionShort.State != PositionStateType.Opening)
                    {
                        PositionShort = null;
                        Log("PositionShort 2 = null");
                    }
                }

                if (PositionShort != null && marketDepth.Asks[i].Price ==
                                          PositionShort.EntryPrice + Offset * _tab.Securiti.PriceStep
                                          && marketDepth.Asks[i].Ask <
                                          BigVolume /
                                          2) // если позиция открыта, текущая цена равна цене открытия и заявка с большим объемом сократилась более чем в 2? раза:
                {
                    if (PositionShort.State == PositionStateType.Open) // позиция уже открыта
                    {
                        _tab.CloseAtMarket(PositionShort, PositionShort.OpenVolume); // закрываем открытую позицию по рынку
                    }
                    else if
                        (PositionShort.State ==
                         PositionStateType.Opening) // позиция открывается, т.е лимитка еще не сработала
                    {
                        _tab.CloseAllOrderToPosition(PositionShort); // снимаем все заявки
                        PositionShort = null;
                        Log("PositionShort 3 = null");
                    }
                }
                else if (PositionShort != null && PositionShort.State == PositionStateType.Opening &&
                         marketDepth.Asks[i].Ask >= BigVolume && marketDepth.Asks[i].Price <
                         PositionShort.EntryPrice - Offset * _tab.Securiti.PriceStep)
                {
                    _tab.CloseAllOrderToPosition(PositionShort); // снимаем все заявки
                    PositionShort = null;
                    Log("PositionShort 4 = null");
                    break;
                }
            }

            for (int i = 0; i < marketDepth.Bids.Count; i++)
            {
                if (marketDepth.Bids[i].Bid >= BigVolume &&
                    PositionLong == null) // пробегаемся по стакану бидов: если есть заявка на большой объем, нет открытых позиций и нет активных заявок, выставляем лимитку на покупку
                {
                    decimal price = marketDepth.Bids[i].Price + Offset * _tab.Securiti.PriceStep;

                    PositionLong = _tab.BuyAtLimit(Lot, price);
                    Log("PositionLong 1 = " + PositionLong.GetStringForSave());

                    if (PositionLong.State != PositionStateType.Open && PositionLong.State != PositionStateType.Opening)
                    {
                        PositionLong = null;
                        Log("PositionLong 2 = null");
                    }
                }

                if (PositionLong != null && marketDepth.Bids[i].Price ==
                                         PositionLong.EntryPrice - Offset * _tab.Securiti.PriceStep
                                         && marketDepth.Bids[i].Bid <
                                         BigVolume /
                                         2) // если позиция открыта, текущая цена равна цене открытия и заявка с большим объемом сократилась более чем в 2? раза:
                {
                    if (PositionLong.State == PositionStateType.Open) // позиция уже открыта
                    {
                        _tab.CloseAtMarket(PositionLong, PositionLong.OpenVolume); // закрываем открытую позицию по рынку
                    }
                    else if
                        (PositionLong.State ==
                         PositionStateType.Opening) // позиция открывается, т.е лимитка еще не сработала
                    {
                        _tab.CloseAllOrderToPosition(PositionLong); // снимаем все заявки
                        PositionLong = null;
                        Log("PositionLong 3 = null");
                    }


                }
                else if (PositionLong != null && PositionLong.State == PositionStateType.Opening &&
                         marketDepth.Bids[i].Bid >= BigVolume && marketDepth.Bids[i].Price >
                         PositionLong.EntryPrice + Offset * _tab.Securiti.PriceStep)
                {
                    _tab.CloseAllOrderToPosition(PositionLong); // снимаем все заявки
                    PositionLong = null;
                    Log("PositionLong 4 = null");
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

            window.Show();
        }

        public void Log(string message)
        {
            if (!Directory.Exists(@"Log"))
            {
                Directory.CreateDirectory(@"Log");
            }

            DateTime dt = DateTime.Now;
            string name = "log_" + dt.ToShortDateString() + ".txt";

            try
            {
                using (StreamWriter writer = new StreamWriter(@"Log\" + name, true))
                {
                    string str = dt.ToShortTimeString() + "." + dt.Second + "." + dt.Millisecond;

                    writer.WriteLine(str);
                    writer.WriteLine(message);
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Log file error = " + e.Message);
            }

            
        }

        #endregion
    }
}
