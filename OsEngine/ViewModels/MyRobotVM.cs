using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using Newtonsoft.Json;
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
    public class MyRobotVM : BaseVM
    {
        public MyRobotVM(string header, int number)
        {
            Header = header;
            NumberTab = number;

            ServerMaster.ServerCreateEvent += ServerMaster_ServerCreateEvent;

            Load();
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
                    OnSelectedSecurity?.Invoke();
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
                    return _serverType;
                }

                return Server.ServerType;
            }

            set
            {
                if (value != _serverType)
                {
                    _serverType = value;
                }
            }

        }
        private ServerType _serverType = ServerType.None;

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
                
                ServerState = Server.ServerStatus.ToString();

                
            }
        }

        private IServer _server = null;

        public string ServerState
        {
            get => _serverState;

            set
            {
                _serverState = value;
                OnPropertyChanged(nameof(ServerState));
            }
        }
        private string _serverState = "";

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

        public decimal VarMarginMoney
        {
            get => _varMarginMoney;

            set
            {
                _varMarginMoney = value;
                OnPropertyChanged(nameof(VarMarginMoney));
            }
        }
        private decimal _varMarginMoney;

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

        public decimal AccumMoney
        {
            get => _accumMoney;

            set
            {
                _accumMoney = value;
                OnPropertyChanged(nameof(AccumMoney));
            }
        }
        private decimal _accumMoney;

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

        public decimal TotalMoney
        {
            get => _totalMoney;

            set
            {
                _totalMoney = value;
                OnPropertyChanged(nameof(TotalMoney));
            }
        }
        private decimal _totalMoney;

        public decimal Depo
        {
            get => _depo;

            set
            {
                _depo = value;
                OnPropertyChanged(nameof(Depo));
            }
        }
        private decimal _depo;

        public decimal BlockedMoney
        {
            get => _blockedMoney;

            set
            {
                _blockedMoney = value;
                OnPropertyChanged(nameof(BlockedMoney));
            }
        }
        private decimal _blockedMoney;

        public decimal ProfitPortfolio
        {
            get => _profitPortfolio;

            set
            {
                _profitPortfolio = value;
                OnPropertyChanged(nameof(ProfitPortfolio));
            }
        }
        private decimal _profitPortfolio;

        public bool IsChekCurrency
        {
            get => _isChekCurrency;

            set
            {
                _isChekCurrency = value;
                OnPropertyChanged(nameof(IsChekCurrency));
            }
        }
        private bool _isChekCurrency = false;



        #endregion

        #region Fields ==========================================================

        private Portfolio _portfolio;

        public int NumberTab = 0;

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

        private void ServerMaster_ServerCreateEvent(IServer server)
        {
            if (server.ServerType == ServerType)
            {
                Server = server;
            }
        }

        private void TradeLogic()
        {
            if (IsRun == false || SelectedSecurity == null)
            {
                return;
            }

            foreach (Level level in Levels)
            {
                TradeLogicOpen(level);
                TradeLogicClose(level);
            }
            
        }

        private decimal GetStepLevel()
        {
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

            return stepLevel;
        }

        private void TradeLogicOpen(Level level)
        {
            decimal stepLevel = GetStepLevel();

            decimal borderUp = Price + stepLevel * MaxActiveLevel;

            decimal borderDown = Price - stepLevel * MaxActiveLevel;

            if (level.PassVolume && level.PriceLevel != 0
                                 && Math.Abs(level.Volume) + level.LimitVolume < Lot)
            {
                if ((level.Side == Side.Buy && level.PriceLevel >= borderDown)
                    || (level.Side == Side.Sell && level.PriceLevel <= borderUp))
                {
                    decimal lot = CalcWorkLot(Lot, level.PriceLevel);

                    decimal workLot = lot - Math.Abs(level.Volume) - level.LimitVolume;

                    if (workLot == 0) return;

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

        private void TradeLogicClose(Level level)
        {
            decimal stepLevel = GetStepLevel();

            if (level.PassTake && level.PriceLevel != 0
                               && level.Volume != 0
                               && Math.Abs(level.Volume) != level.TakeVolume)
            {
                Side side = Side.None;

                if (level.Volume > 0)
                {
                    side = Side.Sell;
                }
                else if (level.Volume < 0)
                {
                    side = Side.Buy;
                }

                RobotWindowVM.Log(Header, "Level = " + level.GetStringForSave());

                decimal workLot = Math.Abs(level.Volume) - level.TakeVolume;

                RobotWindowVM.Log(Header, "workLot = " + workLot);

                if (workLot > 0)
                {
                    level.PassTake = false;

                    Order order = SendOrder(SelectedSecurity, level.TakePrice, workLot, side);

                    if (order != null)
                    {
                        level.OrdersForClose.Add(order);

                        RobotWindowVM.Log(Header, "Send take order = " + GetStringForSave(order));
                    }
                    else
                    {
                        level.PassTake = true;
                    }
                }

            }
        }

        private void CalculateMargin()
        {
            if (Levels.Count == 0 || SelectedSecurity == null)
            {
                return;
            }

            decimal volume = 0;
            decimal accum = 0;
            decimal margine = 0;
            decimal averPrice = 0;

            foreach (var level in Levels)
            {
                if (level.Volume + volume != 0)
                {
                    averPrice = (level.OpenPrice * level.Volume + volume * averPrice) / (level.Volume + volume);
                }

                level.Margin = (Price - level.OpenPrice) * level.Volume * SelectedSecurity.Lot;

                margine += level.Margin;
                volume += level.Volume;
                accum += level.Accum;

            }

            AllPositionsCount = Math.Round(volume, SelectedSecurity.Decimals);
            PriceAverage = Math.Round(averPrice, SelectedSecurity.Decimals);
            VarMargin = Math.Round(margine, SelectedSecurity.Decimals);
            VarMarginMoney = Math.Round(VarMargin / SelectedSecurity.PriceStep * SelectedSecurity.PriceStepCost, 1);
            Accum = Math.Round(accum, SelectedSecurity.Decimals);
            AccumMoney = Math.Round(Accum / SelectedSecurity.PriceStep * SelectedSecurity.PriceStepCost, 1);
            Total = Accum + VarMargin;
            TotalMoney = Math.Round(Total / SelectedSecurity.PriceStep * SelectedSecurity.PriceStepCost, 1);

            Depo = _portfolio.ValueCurrent;
            BlockedMoney = _portfolio.ValueBlocked;
            ProfitPortfolio = _portfolio.Profit;


        }

        private decimal CalcWorkLot(decimal lot, decimal price)
        {
            decimal workLot = lot;

            if (IsChekCurrency)
            {
                workLot = lot / price;
            }

            workLot = decimal.Round(workLot, SelectedSecurity.DecimalsVolume);

            return workLot;
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
            if (Server == null || Server.ServerStatus == ServerConnectStatus.Disconnect)
            {
                return;
            }

            IsRun = !IsRun;

            RobotWindowVM.Log(Header, "StartStop = " + IsRun);

            if (IsRun)
            {
                foreach (var level in Levels)
                {
                    level.SetVolumeStart();

                    level.PassVolume = true;
                    level.PassTake = true;
                }
            }
            else
            {
                

                Task.Run(() =>
                {
                    while (true)
                    {
                        foreach (var level in Levels)
                        {
                            level.CancelAllOrders(Server, Header);
                        }

                        Thread.Sleep(3000);

                        bool flag = true;

                        foreach (var level in Levels)
                        {
                            if (level.LimitVolume != 0 || level.TakeVolume != 0)
                            {
                                flag = false; 
                                break;
                            }
                        }

                        if (flag)
                        {
                            break;
                        }
                    }
                });
                
            }
        }

        private void SubscribeToServer()
        {
            _server.NewMyTradeEvent += Server_NewMyTradeEvent;//пришла новая сделка
            _server.NewOrderIncomeEvent += Server_NewOrderIncomeEvent;//изменение ордера
            _server.NewCandleIncomeEvent += Server_NewCandleIncomeEvent;//пришла новая свеча
            _server.NewTradeEvent += Server_NewTradeEvent;//пришла новая обезличенная сделка
            _server.ConnectStatusChangeEvent += Server_ConnectStatusChangeEvent;//изменился статус сервера
            _server.SecuritiesChangeEvent += Server_SecuritiesChangeEvent;
            _server.PortfoliosChangeEvent += _server_PortfoliosChangeEvent;
        }

        private void _server_PortfoliosChangeEvent(List<Portfolio> portfolios)
        {
            if (portfolios == null || portfolios.Count == 0)
            {
                return;
            }

            StringPortfolios = GetStringPortfolios(_server);
            OnPropertyChanged(nameof(StringPortfolios));

            if (StringPortfolios != null && StringPortfolios.Count > 0)
            {
                if (StringPortfolio == "")
                {
                    StringPortfolio = StringPortfolios[0];
                }

                for (int i = 0; i < portfolios.Count; i++)
                {
                    if (portfolios[i].Number == StringPortfolio)
                    {
                        _portfolio = portfolios[i];
                    }
                }
            }
        }

        private void Server_ConnectStatusChangeEvent(string state)
        {
            ServerState = state;

            if (state == "Connect")
            {
                GetStateOrders();
            }
            else
            {
                IsRun = false;
            }
        }

        private void UnSubscribeToServer()
        {
            _server.NewMyTradeEvent -= Server_NewMyTradeEvent;//пришла новая сделка
            _server.NewOrderIncomeEvent -= Server_NewOrderIncomeEvent;//изменение ордера
            _server.NewCandleIncomeEvent -= Server_NewCandleIncomeEvent;//пришла новая свеча
            _server.NewTradeEvent -= Server_NewTradeEvent;//пришла новая обезличенная сделка
            _server.ConnectStatusChangeEvent -= Server_ConnectStatusChangeEvent;
            _server.SecuritiesChangeEvent -= Server_SecuritiesChangeEvent;
            _server.PortfoliosChangeEvent -= _server_PortfoliosChangeEvent;
        }

        private void Server_SecuritiesChangeEvent(List<Security> securities)
        {
            for (int i = 0; i < securities.Count; i++)
            {
                if (securities[i].Name == Header)
                {
                    SelectedSecurity = securities[i];
                    break;
                }
            }
        }

        private void Server_NewTradeEvent(List<Trade> trades)
        {
            if (trades != null && trades[0].SecurityNameCode == SelectedSecurity.Name)
            {
                Trade trade = trades.Last();

                Price = trade.Price;

                CalculateMargin();

                if (trade.Time.Second % 10 == 0)
                {
                    TradeLogic();
                }
            }

            
        }

        private void Server_NewCandleIncomeEvent(CandleSeries series)
        {
            
        }

        public void CheckMissedOrders()
        {
            if (SelectedSecurity == null)
            {
                return;
            }

            if (RobotWindowVM.Orders == null || RobotWindowVM.Orders.Count == 0)
            {
                return;
            }

            foreach (var val in RobotWindowVM.Orders)
            {
                if (val.Key == SelectedSecurity.Name)
                {
                    foreach (var value in val.Value)
                    {
                        Server_NewOrderIncomeEvent(value.Value);
                    }
                }
            }
        }

        public void CheckMissedMyTrades()
        {
            if (SelectedSecurity == null)
            {
                return;
            }

            if (RobotWindowVM.MyTrades == null || RobotWindowVM.MyTrades.Count == 0)
            {
                return;
            }

            foreach (var val in RobotWindowVM.MyTrades)
            {
                if (val.Key == SelectedSecurity.Name)
                {
                    foreach (var value in val.Value)
                    {
                        Server_NewMyTradeEvent(value.Value);
                    }
                }
            }
        }

        private void Server_NewOrderIncomeEvent(Order order)
        {
            if (order == null)
            {
                return;
            }

            if (SelectedSecurity != null 
                && order.SecurityNameCode == SelectedSecurity.Name
                && order.ServerType == Server.ServerType
                && order.PortfolioNumber == StringPortfolio)
            {
                bool isRec = true;

                if (order.State == OrderStateType.Activ && order.TimeCallBack.AddSeconds(10) < Server.ServerTime) isRec = false;

                
                if (isRec)
                {
                    RobotWindowVM.Log(Header, "NewOrderIncomeEvent = " + GetStringForSave(order));
                }

                if (order.NumberMarket != "")
                {
                    foreach (Level level in Levels)
                    {
                        bool res = level.NewOrder(order);

                        if (res)
                        {
                            
                            RobotWindowVM.Log(Header, "UpDate Level = " + level.GetStringForSave());
                        }
                    }
                }

                Save();
            }
        }

        private void Server_NewMyTradeEvent(MyTrade myTrade)
        {
            if (myTrade == null || SelectedSecurity == null
                || myTrade.SecurityNameCode != SelectedSecurity.Name)
            {
                return;
            }

            foreach (var level in Levels)
            {
                bool res = level.AddMyTrade(myTrade, SelectedSecurity);

                if (res)
                {
                    RobotWindowVM.Log(Header, GetStringForSave(myTrade));

                    if (myTrade.Side == level.Side)
                    {
                       TradeLogicClose(level);
                    }
                    else
                    {
                        TradeLogicOpen(level);
                    }
                    Save();
                }
            }
        }

        private void Calculate(object o)
        {
            RobotWindowVM.Log(Header, "\n\n Calculate");

            ObservableCollection<Level> levels = new ObservableCollection<Level>();

            decimal stepTake = 0;

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

                    stepTake = TakeLevel * SelectedSecurity.PriceStep;
                }
                else if (StepType == StepType.PERCENT)
                {
                    currBuyPrice -= StepLevel * currBuyPrice / 100;
                    currBuyPrice = Decimal.Round(currBuyPrice, SelectedSecurity.Decimals);

                    currSellPrice += StepLevel * currSellPrice / 100;
                    currSellPrice = Decimal.Round(currSellPrice, SelectedSecurity.Decimals);

                    stepTake = TakeLevel * currBuyPrice / 100;
                    stepTake = Decimal.Round(stepTake, SelectedSecurity.Decimals);
                }

                levelBuy.PriceLevel = currBuyPrice;
                levelSell.PriceLevel = currSellPrice;

                if (Direction == Direction.BUY || Direction == Direction.BUYSELL)
                {
                    levelBuy.TakePrice = levelBuy.PriceLevel + stepTake;

                    levels.Add(levelBuy);
                }

                if (Direction == Direction.SELL || Direction == Direction.BUYSELL)
                {
                    levelSell.TakePrice = levelSell.PriceLevel - stepTake;
                    
                    levels.Insert(0, levelSell);
                }
                
                
            }

            Levels = levels;
            OnPropertyChanged(nameof(Levels));
            Save();
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

        public ObservableCollection<string> GetStringPortfolios(IServer server)
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

        public void Save()
        {
            if (!Directory.Exists(@"Parameters\Tabs"))
            {
                Directory.CreateDirectory(@"Parameters\Tabs");
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(@"Parameters\Tabs\param_" + NumberTab + ".txt", false))
                {
                    writer.WriteLine(Header);
                    writer.WriteLine(ServerType);
                    writer.WriteLine(StringPortfolio);
                    writer.WriteLine(StartPoint);
                    writer.WriteLine(CountLevels);
                    writer.WriteLine(Direction);
                    writer.WriteLine(Lot);
                    writer.WriteLine(StepType);
                    writer.WriteLine(StepLevel);
                    writer.WriteLine(TakeLevel);
                    writer.WriteLine(MaxActiveLevel);
                    writer.WriteLine(PriceAverage);
                    writer.WriteLine(Accum);
                    writer.WriteLine(JsonConvert.SerializeObject(Levels));

                    writer.Close();
                }
            }
            catch (Exception e)
            {
                RobotWindowVM.Log(Header + NumberTab, "Save error = " + e.Message);
            }
        }

        private void Load()
        {
            if (!Directory.Exists(@"Parameters\Tabs"))
            {
                return;
            }

            string serverType = "";
            ObservableCollection < Level > levels = new ObservableCollection<Level>();

            try
            {
                using (StreamReader reader = new StreamReader(@"Parameters\Tabs\param_" + NumberTab + ".txt"))
                {
                    Header = reader.ReadLine();
                    serverType = reader.ReadLine();
                    StringPortfolio = reader.ReadLine();
                    StartPoint = GetDecimalForString(reader.ReadLine());
                    CountLevels = (int)GetDecimalForString(reader.ReadLine());

                    Direction direction = Direction.BUYSELL;
                    if (Enum.TryParse(reader.ReadLine(), out direction))
                    {
                        Direction = direction;
                    }

                    Lot = GetDecimalForString(reader.ReadLine());

                    StepType type = StepType.PUNKT;
                    if (Enum.TryParse(reader.ReadLine(), out type))
                    {
                        StepType = type;
                    }

                    StepLevel = GetDecimalForString(reader.ReadLine());
                    TakeLevel = GetDecimalForString(reader.ReadLine());
                    MaxActiveLevel = (int)GetDecimalForString(reader.ReadLine());
                    PriceAverage = GetDecimalForString(reader.ReadLine());
                    Accum = GetDecimalForString(reader.ReadLine());


                    levels = JsonConvert.DeserializeAnonymousType(reader.ReadLine(), new ObservableCollection<Level>());

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                RobotWindowVM.Log(Header + NumberTab, "Load error = " + e.Message);
            }

            if (levels != null)
            {
                Levels = levels;
            }

            StartServer(serverType);
        }

        private void StartServer(string serverType)
        {
            ServerType type = ServerType.None;

            if (Enum.TryParse(serverType, out type))
            {
                ServerType = type;

                ServerMaster.SetNeedServer(ServerType);
            }
        }

        private void GetStateOrders()
        {
            if (Server != null)
            {
                if (Server.ServerType == ServerType.BinanceFutures)
                {
                    AServer aServer = (AServer)Server;

                    List<Order> orders = new List<Order>();

                    foreach (var level in Levels)
                    {
                        GetStateOrders(level.OrdersForOpen, ref orders);

                        GetStateOrders(level.OrdersForClose, ref orders);
                    }

                    if (orders.Count > 0)
                    {
                        aServer.ServerRealization.GetOrdersState(orders);
                    }
                }
            }
        }

        private void GetStateOrders(List<Order> orders, ref List<Order> stateOrders)
        {
            foreach (var order in orders)
            {
                if (order != null)
                {
                    if (order.State == OrderStateType.Activ || order.State == OrderStateType.Patrial || order.State == OrderStateType.Pending)
                    {
                        stateOrders.Add(order);
                    }
                }
            }
        }

        private decimal GetDecimalForString(string str)
        {
            decimal value = 0;

            decimal.TryParse(str, out value);

            return value;
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

        #region Events ====================================================

        public delegate void onSelectedSecurity();

        public event onSelectedSecurity OnSelectedSecurity;

        #endregion
    }
}
