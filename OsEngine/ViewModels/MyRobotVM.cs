using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;
using OkonkwoOandaV20;
using OkonkwoOandaV20.TradeLibrary.DataTypes.Pricing;
using OsEngine.Commands;
using OsEngine.Entity;
using OsEngine.Market;
using OsEngine.Market.Servers;
using OsEngine.MyEntity;
using OsEngine.Robots;
using OsEngine.Views;

namespace OsEngine.ViewModels
{
    public class MyRobotVM:BaseVM
    {
        public MyRobotVM()
        {
            
        }



        #region Properties ==========================================================

        public ObservableCollection<string> StringPortfolios { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<Level> Levels { get; set; } = new ObservableCollection<Level>();

        public string Header
        {
            get
            {
                if (SelectedSecurity != null)
                {
                    return SelectedSecurity.Name;
                }
                else
                {
                    return _header;
                }
            }

            set
            {
                _header = value;
                OnPropertyChanged(nameof(Header));
            }

        }
        private string _header;

        public Security SelectedSecurity
        {
            get => _selectedSecurity;

            set
            {
                _selectedSecurity = value;
                OnPropertyChanged(nameof(SelectedSecurity));
                OnPropertyChanged(nameof(Header));

                if (SelectedSecurity != null)
                {
                    StartSecurity(SelectedSecurity);
                }
                
            }
        }
        private Security _selectedSecurity = null;

        public decimal StartPoint
        {
            get => _startPoint;

            set
            {
                _startPoint = value;
                OnPropertyChanged(nameof(StartPoint));
            }
        }
        private decimal _startPoint;

        public ServerType ServerType
        {
            get
            {
                if (Server == null)
                {
                    return ServerType.None;
                }
                return Server.ServerType;
            }

        }

        public IServer Server
        {
            get => _server;

            set
            {
                if (Server != null)
                {
                    UnSubscribeToServer();
                    _server = null;
                }

                _server = value;
                OnPropertyChanged(nameof(ServerType));

                SubscribeToServer();

                StringPortfolios = GetStringPortfolios(_server);

                if (StringPortfolios != null && StringPortfolios.Count>0)
                {
                    StringPortfolio = StringPortfolios[0];
                }
                OnPropertyChanged(nameof(StringPortfolios));
            }
        }
        private IServer _server = null;

        public string StringPortfolio
        {
            get => _stringPortfolio;

            set
            {
                _stringPortfolio = value;
                OnPropertyChanged(nameof(StringPortfolio));

                _portfolio = GetPortfolio(_stringPortfolio);
            }
        }
        private string _stringPortfolio = "";

        public int CountLevels
        {
            get => _countLevels;

            set
            {
                _countLevels = value;
                OnPropertyChanged(nameof(CountLevels));
            }
        }
        private int _countLevels;

        public Direction Direction
        {
            get => _direction;

            set
            {
                _direction = value;
                OnPropertyChanged(nameof(Direction));
            }
        }
        private Direction _direction = Direction.BUY;

        public List<Direction> Directions { get; set; } = new List<Direction>()
        {
            Direction.BUY, Direction.SELL, Direction.BUYSELL
        };

        public decimal Lot
        {
            get => _lot;

            set
            {
                _lot = value;
                OnPropertyChanged(nameof(Lot));
            }
        }
        private decimal _lot;

        public bool IsRun
        {
            get => _isRun;

            set
            {
                _isRun = value;
                OnPropertyChanged(nameof(IsRun));

                if (IsRun)
                {
                    TradeLogic();

                }
            }
        }
        private bool _isRun;

        public StepType StepType
        {
            get => _stepType;

            set
            {
                _stepType = value;
                OnPropertyChanged(nameof(StepType));
            }
        }
        private StepType _stepType = StepType.PUNKT;

        public List<StepType> StepTypes { get; set; } = new List<StepType>()
        {
            StepType.PUNKT, StepType.PERCENT
        };

        public decimal StepLevel
        {
            get => _stepLevel;

            set
            {
                _stepLevel = value;
                OnPropertyChanged(nameof(StepLevel));
            }
        }
        private decimal _stepLevel;

        public decimal TakeLevel
        {
            get => _takeLevel;

            set
            {
                _takeLevel = value;
                OnPropertyChanged(nameof(TakeLevel));
            }
        }
        private decimal _takeLevel;

        public int MaxActiveLevel
        {
            get => _maxActiveLevel;

            set
            {
                _maxActiveLevel = value;
                OnPropertyChanged(nameof(MaxActiveLevel));
            }
        }
        private int _maxActiveLevel;

        public decimal AllPositionsCount
        {
            get => _allPositionsCount;

            set
            {
                _allPositionsCount = value;
                OnPropertyChanged(nameof(AllPositionsCount));
            }
        }
        private decimal _allPositionsCount;

        public decimal PriceAverage
        {
            get => _priceAverage;

            set
            {
                _priceAverage = value;
                OnPropertyChanged(nameof(PriceAverage));
            }
        }
        private decimal _priceAverage;

        public decimal Price
        {
            get => _price;

            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }
        private decimal _price;

        public decimal VarMargin
        {
            get => _varMargin;

            set
            {
                _varMargin = value;
                OnPropertyChanged(nameof(VarMargin));
            }
        }
        private decimal _varMargin;

        public decimal Accum
        {
            get => _accum;

            set
            {
                _accum = value;
                OnPropertyChanged(nameof(Accum));
            }
        }
        private decimal _accum;

        public decimal Total
        {
            get => _total;

            set
            {
                _total = value;
                OnPropertyChanged(nameof(Total));
            }
        }
        private decimal _total;




        #endregion

        #region Fields ==========================================================

        private Portfolio _portfolio;

        #endregion

        #region Commands ==========================================================

        private DelegateCommand _commandStartStop;
        public DelegateCommand CommandStartStop
        {
            get
            {
                if (_commandStartStop == null)
                {
                    _commandStartStop = new DelegateCommand(StartStop);
                }

                return _commandStartStop;
            }
        }


        private DelegateCommand _commandCalculate;
        public DelegateCommand CommandCalculate
        {
            get
            {
                if (_commandCalculate == null)
                {
                    _commandCalculate = new DelegateCommand(Calculate);
                }

                return _commandCalculate;
            }
        }

        private DelegateCommand _commandSelectSecurity;
        public DelegateCommand CommandSelectSecurity
        {
            get
            {
                if (_commandSelectSecurity == null)
                {
                    _commandSelectSecurity = new DelegateCommand(SelectSecurity);
                }

                return _commandSelectSecurity;
            }
        }

        #endregion

        #region Methods ==========================================================

        private void TradeLogic()
        {
            if (IsRun == false)
            {
                return;
            }

            decimal stepLevel = 0;

            if (StepType == StepType.PUNKT)
            {
                stepLevel = StepLevel * SelectedSecurity.PriceStep;
            }
            else if (StepType == StepType.PERCENT)
            {
                stepLevel = StepLevel * Price / 100;

                stepLevel = Decimal.Round(stepLevel, SelectedSecurity.Decimals);
            }

            decimal borderUp = Price + stepLevel * MaxActiveLevel;

            decimal borderDown = Price - stepLevel * MaxActiveLevel;

            foreach (Level level in Levels)
            {
                if (level.PassVolume && level.PriceLevel != 0
                    && Math.Abs(level.Volume) + level.LimitVolume < Lot)
                {
                    if ((level.Side == Side.Buy && level.PriceLevel >= borderDown)
                        || (level.Side == Side.Sell && level.PriceLevel <= borderUp))
                    {
                        decimal workLot = Lot - Math.Abs(level.Volume) - level.LimitVolume;

                        RobotWindowVM.Log(Header, "Level = " + level.GetStringForSave());
                        RobotWindowVM.Log(Header, "workLot = " + workLot);


                        level.PassVolume = false;

                        Order order = SendOrder(SelectedSecurity, level.PriceLevel, workLot, level.Side);

                        if (order != null)
                        {
                            level.OrdersForOpen.Add(order);

                            RobotWindowVM.Log(Header, "Send linit order = " + GetStringForSave(order));
                        }
                        else
                        {
                            level.PassVolume = true;
                        }
                    }



                    
                }

                
            }
        }

        private Order SendOrder(Security sec , decimal price, decimal volume, Side side)
        {
            if (string.IsNullOrEmpty(StringPortfolio))
            {
                MessageBox.Show("StringPortfolio == null ");
                return null;
            }

            Order order = new Order()
            {
                Price = price,
                Volume = volume,
                Side = side,
                PortfolioNumber = StringPortfolio,
                TypeOrder = OrderPriceType.Limit,
                NumberUser = NumberGen.GetNumberOrder(StartProgram.IsOsTrader),
                SecurityNameCode = sec.Name,
                SecurityClassCode = sec.NameClass
            };

            Server.ExecuteOrder(order);

            return order;
        }

        private void StartStop(object o)
        {
            IsRun = !IsRun;
        }

        private void SubscribeToServer()
        {
            Server.NewMyTradeEvent += Server_NewMyTradeEvent;//пришла новая сделка
            Server.NewOrderIncomeEvent += Server_NewOrderIncomeEvent;//изменение ордера
            Server.NewCandleIncomeEvent += Server_NewCandleIncomeEvent;//пришла новая свеча
            Server.NewTradeEvent += Server_NewTradeEvent;//пришла новая обезличенная сделка
        }

        private void UnSubscribeToServer()
        {
            Server.NewMyTradeEvent -= Server_NewMyTradeEvent;//пришла новая сделка
            Server.NewOrderIncomeEvent -= Server_NewOrderIncomeEvent;//изменение ордера
            Server.NewCandleIncomeEvent -= Server_NewCandleIncomeEvent;//пришла новая свеча
            Server.NewTradeEvent -= Server_NewTradeEvent;//пришла новая обезличенная сделка
        }

        private void Server_NewTradeEvent(List<Trade> trades)
        {
            if (trades != null && trades[0].SecurityNameCode == SelectedSecurity.Name)
            {
                Price = trades.Last().Price;
            }

            
        }

        private void Server_NewCandleIncomeEvent(CandleSeries series)
        {
            
        }

        private void Server_NewOrderIncomeEvent(Order order)
        {
            
        }

        private void Server_NewMyTradeEvent(MyTrade myTrade)
        {
            
        }

        private void Calculate(object o)
        {
            ObservableCollection<Level> levels = new ObservableCollection<Level>();

            if (CountLevels <= 0)
            {
                return;
            }

            decimal currBuyPrice = StartPoint;

            decimal currSellPrice = StartPoint;

            for (int i = 0; i < CountLevels; i++)
            {
                Level levelBuy = new Level() {Side = Side.Buy};
                Level levelSell = new Level(){Side = Side.Sell};

                if (StepType == StepType.PUNKT)
                {
                    currBuyPrice -= StepLevel * SelectedSecurity.PriceStep;
                    currSellPrice += StepLevel * SelectedSecurity.PriceStep;
                }
                else if (StepType == StepType.PERCENT)
                {
                    currBuyPrice -= StepLevel * currBuyPrice / 100;
                    currBuyPrice = Decimal.Round(currBuyPrice, SelectedSecurity.Decimals);

                    currSellPrice += StepLevel * currSellPrice / 100;
                    currSellPrice = Decimal.Round(currSellPrice, SelectedSecurity.Decimals);
                }

                levelBuy.PriceLevel = currBuyPrice;
                levelSell.PriceLevel = currSellPrice;

                if (Direction == Direction.BUY || Direction == Direction.BUYSELL)
                {
                    levels.Add(levelBuy);
                }

                if (Direction == Direction.SELL || Direction == Direction.BUYSELL)
                {
                    levels.Insert(0, levelSell);
                }
                
                
            }

            Levels = levels;
            OnPropertyChanged(nameof(Levels));
        }

        private Portfolio GetPortfolio(string number)
        {
            if (Server != null && Server.Portfolios != null)
            {
                foreach (var portf in Server.Portfolios)
                {
                    if (portf.Number == number)
                    {
                        return portf;
                    }
                }
            }
            return null;
        }

        private ObservableCollection<string> GetStringPortfolios(IServer server)
        {

            ObservableCollection<string> stringportfolios = new ObservableCollection<string>();


            if (server == null)
            {
                return stringportfolios;
            }

            foreach (var portf in server.Portfolios)
            {
                stringportfolios.Add(portf.Number);
            }

            return stringportfolios;
        }

        void SelectSecurity(object o)
        {
            if (RobotWindowVM.ChangeSecurityWindow != null)
            {
                return;
            }

            RobotWindowVM.ChangeSecurityWindow = new ChangeSecurityWindow(this);

            RobotWindowVM.ChangeSecurityWindow.ShowDialog();

            RobotWindowVM.ChangeSecurityWindow = null;
        }



        private void StartSecurity(Security security)
        {
            if (security == null)
            {
                Debug.WriteLine("StartSecurity security = null");
                return;
            }

            Task.Run(() =>
            {
                while (true)
                {
                    var series = Server.StartThisSecurity(security.Name, new TimeFrameBuilder(), security.NameClass);

                    if (series != null)
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                }
            });


        }

        private string GetStringForSave(Order order)
        {
            string str = "";

            str += order.SecurityNameCode + " | ";
            str += order.PortfolioNumber + " | ";
            str += order.TimeCreate + " | ";
            str += order.State + " | ";
            str += order.Side + " | ";
            str += "Volume = " + order.Volume + " | ";
            str += "Price = " + order.Price + " | ";
            str += "NumberUser = " + order.NumberUser + " | ";
            str += "NumberMarket = " + order.NumberMarket + " | ";

            return str;
        }

        private string GetStringForSave(MyTrade myTrade)
        {
            string str = "";

            str += myTrade.SecurityNameCode + " | ";
            str += myTrade.Time + " | ";
            str += myTrade.Side + " | ";
            str += "Volume = " + myTrade.Volume + " | ";
            str += "Price = " + myTrade.Price + " | ";
            str += "NumberOrderParent = " + myTrade.NumberOrderParent + " | ";
            str += "NumberTrade = " + myTrade.NumberTrade + " | ";

            return str;
        }



        #endregion
    }
}
