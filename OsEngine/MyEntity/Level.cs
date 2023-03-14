using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OsEngine.Charts.CandleChart.Indicators;
using OsEngine.Entity;
using OsEngine.Market.Servers;
using OsEngine.Robots;
using OsEngine.ViewModels;

namespace OsEngine.MyEntity
{
    public class Level:BaseVM
    {
        #region Properties ====================================================================================

        /// <summary>
        /// Расчетная цена уровня
        /// </summary>
        public decimal PriceLevel
        {
            get => _priceLevel;

            set
            {
                _priceLevel = value;
                OnPropertyChanged(nameof(PriceLevel));
            }
        }
        private decimal _priceLevel = 0;

        public Side Side
        {
            get => _side;

            set
            {
                _side = value;
                OnPropertyChanged(nameof(Side));
            }
        }
        private Side _side = 0;

        /// <summary>
        /// Реальная цена открытой позиции
        /// </summary>
        public decimal OpenPrice
        {
            get => _openPrice;

            set
            {
                _openPrice = value;
                OnPropertyChanged(nameof(OpenPrice));
            }
        }
        private decimal _openPrice = 0;

        /// <summary>
        /// Объем открытой позиции
        /// </summary>
        public decimal Volume
        {
            get => _volume;

            set
            {
                _volume = value;
                Change();
            }
        }
        private decimal _volume = 0;

        
        public decimal Accum
        {
            get => _accum;

            set
            {
                _accum = value;
                OnPropertyChanged(nameof(Accum));
            }
        }
        private decimal _accum = 0;

        public decimal Margin
        {
            get => _margin;

            set
            {
                _margin = value;
                OnPropertyChanged(nameof(Margin));
            }
        }
        private decimal _margin = 0;

        public decimal TakePrice
        {
            get => _takePrice;

            set
            {
                _takePrice = value;
                OnPropertyChanged(nameof(TakePrice));
            }
        }
        private decimal _takePrice = 0;

        /// <summary>
        /// Лимитка на позицию
        /// </summary>
        public decimal LimitVolume
        {
            get => _limitVolume;

            set
            {
                _limitVolume = value;
                Change();
            }
        }
        private decimal _limitVolume = 0;

        /// <summary>
        /// Лимитка на тейк
        /// </summary>
        public decimal TakeVolume
        {
            get => _takeVolume;

            set
            {
                _takeVolume = value;
                Change();
            }
        }
        private decimal _takeVolume = 0;

        /// <summary>
        /// Флаг разрешение на выставление ордера
        /// </summary>
        public bool PassVolume
        {
            get => _passVolume;

            set
            {
                _passVolume = value;
                Change();
            }
        }
        private bool _passVolume = true;

        /// <summary>
        /// Флаг разрешение на выставление отейк-ордера
        /// </summary>
        public bool PassTake
        {
            get => _passTake;

            set
            {
                _passTake = value;
                Change();
            }
        }
        private bool _passTake = true;

        #endregion


        #region Fields =============================================================================

        private CultureInfo CultureInfo = new CultureInfo("ru-RU");

        /// <summary>
        /// Лимтка на тейк
        /// </summary>
        public List<Order> OrdersForClose = new List<Order>();

        /// <summary>
        /// Лимтка на открытие
        /// </summary>
        public List<Order> OrdersForOpen = new List<Order>();

        private List<MyTrade> _myTrades = new List<MyTrade>();

        private decimal _calcVolume = 0;

        #endregion


        #region Methods ====================================================================================

        public void SetVolumeStart()
        {
            if (Volume == 0 && LimitVolume == 0 && TakeVolume == 0)
            {
                ClearOrders(ref OrdersForOpen);
                ClearOrders(ref OrdersForClose);
            }
        }

        private void ClearOrders(ref List<Order> orders)
        {
            List<Order> temp = new List<Order>();

            foreach (Order order in orders)
            {
                if (order != null && (order.State != OrderStateType.Cancel || order.State != OrderStateType.Done))
                {
                    temp.Add(order);
                }
            }

            orders = temp;
        }

        public bool AddMyTrade(MyTrade newTrade, Security security)
        {
            foreach (var trade in _myTrades)
            {
                if (trade.NumberTrade == newTrade.NumberTrade)
                {
                    return false;
                }
            }

            if (IsMyTrade(newTrade))
            {
                _myTrades.Add(newTrade);

                CalculatePosition(newTrade, security);

                CalculateOrders();

                return true;
            }
            return false;
        }

        private void CalculatePosition(MyTrade myTrade, Security security)
        {
            string str = "myTrade = " + myTrade.Price + "\n";
            str += "Side = " + myTrade.Side + "\n";
            RobotWindowVM.Log(security.Name, str);

            decimal accum = 0;

            if (_calcVolume == 0)
            {
                OpenPrice = myTrade.Price;
            }
            else if (_calcVolume > 0)
            {
                if (myTrade.Side == Side.Buy)
                {
                    OpenPrice = (_calcVolume * OpenPrice + myTrade.Volume * myTrade.Price) / (_calcVolume + myTrade.Volume);
                }
                else
                {
                    if (myTrade.Volume <= _calcVolume)
                    {
                        accum = (myTrade.Price - OpenPrice) * myTrade.Volume;
                    }
                    else
                    {
                        accum = (myTrade.Price - OpenPrice) * _calcVolume;
                        OpenPrice = myTrade.Price;
                    }
                }
            }
            else if (_calcVolume < 0)
            {
                if (myTrade.Side == Side.Buy)
                {
                    if (myTrade.Volume <= Math.Abs(_calcVolume))
                    {
                        accum = (OpenPrice - myTrade.Price) * myTrade.Volume;
                    }
                    else
                    {
                        accum = (OpenPrice - myTrade.Price) * Math.Abs(_calcVolume);
                        OpenPrice = myTrade.Price;
                    }
                }
                else
                {
                    OpenPrice = (Math.Abs(_calcVolume) * OpenPrice + myTrade.Volume * myTrade.Price) / (Math.Abs(_calcVolume) + myTrade.Volume);
                }
            }

            if (myTrade.Side == Side.Buy)
            {
                _calcVolume += myTrade.Volume;
            }
            else
            {
                _calcVolume -= myTrade.Volume;
            }

            if (_calcVolume == 0)
            {
                OpenPrice = 0;
            }

            Accum += accum * security.Lot;

            OpenPrice = Math.Round(OpenPrice, security.Decimals);
        }

        private bool IsMyTrade(MyTrade newTrade)
        {
            foreach (var order in OrdersForOpen)
            {
                if (order.NumberMarket == newTrade.NumberOrderParent)
                {
                    return true;
                }
            }

            foreach (var order in OrdersForClose)
            {
                if (order.NumberMarket == newTrade.NumberOrderParent)
                {
                    return true;
                }
            }

            return false;
        }

        public bool NewOrder(Order newOrder)
        {
            for (int i = 0; i < OrdersForOpen.Count; i++)
            {
                if (OrdersForOpen[i].NumberMarket == newOrder.NumberMarket)
                {
                    CopyOrder(newOrder, OrdersForOpen[i]);

                    CalculateOrders();

                    return true;
                }
            }

            for (int i = 0; i < OrdersForClose.Count; i++)
            {
                if (OrdersForClose[i].NumberMarket == newOrder.NumberMarket)
                {
                    CopyOrder(newOrder, OrdersForClose[i]);

                    CalculateOrders();

                    return true;
                }
            }

            return false;
        }

        private void CalculateOrders()
        {
            decimal activeVolume = 0;
            decimal volumeExecute = 0;
            decimal activeTake = 0;
            bool passLimit = true;
            bool passTake = true;

            foreach (Order order in OrdersForOpen)
            {
                volumeExecute += order.VolumeExecute;

                if (order.State == OrderStateType.Activ || order.State == OrderStateType.Patrial)
                {
                    activeVolume += order.Volume - order.VolumeExecute;
                }
                else if (order.State == OrderStateType.Pending || order.State == OrderStateType.None)
                {
                    passLimit = false;
                }
            }

            foreach (Order order in OrdersForClose)
            {
                volumeExecute -= order.VolumeExecute;

                if (order.State == OrderStateType.Activ || order.State == OrderStateType.Patrial)
                {
                    activeTake += order.Volume - order.VolumeExecute;
                }
                else if (order.State == OrderStateType.Pending || order.State == OrderStateType.None)
                {
                    passTake = false;
                }
            }

            Volume = volumeExecute;
            if (Side == Side.Sell)
            {
                Volume *= -1;
            }

            LimitVolume = activeVolume;
            TakeVolume = activeTake;
            PassVolume = passLimit;
            PassTake = passTake;
        }

        private Order CopyOrder(Order newOrder, Order order)
        {
            order.State = newOrder.State;
            order.TimeCancel = newOrder.TimeCancel;
            order.Volume = newOrder.Volume;
            order.VolumeExecute = newOrder.VolumeExecute;
            order.TimeDone = newOrder.TimeDone;
            order.TimeCallBack = newOrder.TimeCallBack;
            order.NumberUser = newOrder.NumberUser;

            return order;
        }

        private void Change()
        {
            OnPropertyChanged(nameof(Volume));
            OnPropertyChanged(nameof(OpenPrice));
            OnPropertyChanged(nameof(LimitVolume));
            OnPropertyChanged(nameof(TakeVolume));
            OnPropertyChanged(nameof(PassTake));
            OnPropertyChanged(nameof(PassVolume));
            OnPropertyChanged(nameof(TakePrice));
        }

        public string GetStringForSave()
        {
            string str = "";

            str += "Volume = " + Volume.ToString(CultureInfo) + " | ";
            str += "PriceLevel = " + PriceLevel.ToString(CultureInfo) + " | ";
            str += "OpenPrice = " + OpenPrice.ToString(CultureInfo) + " | ";
            str += Side + " | ";
            str += "PassVolume = " + PassVolume.ToString(CultureInfo) + " | ";
            str += "PassTake = " + PassTake.ToString(CultureInfo) + " | ";
            str += "LimitVolume = " + LimitVolume.ToString(CultureInfo) + " | ";
            str += "TakeVolume = " + TakeVolume.ToString(CultureInfo) + " | ";
            str += "TakePrice = " + TakePrice.ToString(CultureInfo) + " | ";

            return str;
        }

        public void CancelAllOrders(IServer server, string header)
        {
            CancelOrders(OrdersForOpen, server);
            CancelOrders(OrdersForClose, server);
            RobotWindowVM.Log(header, GetStringForSave());
        }

        private void CancelOrders(List<Order> orders, IServer server)
        {
            foreach (var order in orders)
            {
                if (order != null)
                {
                    if (order.State == OrderStateType.Activ || order.State == OrderStateType.Patrial || order.State == OrderStateType.Pending)
                    {
                        server.CancelOrder(order);
                        Thread.Sleep(30);
                        
                    }
                }
            }
        }

        #endregion


    }
}
