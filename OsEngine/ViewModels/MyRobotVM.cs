﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using OkonkwoOandaV20;
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
            ServerMaster.ServerCreateEvent += ServerMaster_ServerCreateEvent;
        }

        

        #region Properties ==========================================================


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
                //StartSecurity(_security);
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
                _server = value;
                OnPropertyChanged(nameof(ServerType));
                StringPortfolios = GetStringPortfolios(_server);
                if (StringPortfolios != null && StringPortfolios.Count>0)
                {
                    StringPortfolio = StringPortfolios[0];
                }
                OnPropertyChanged(nameof(StringPortfolios));
            }
        }
        private IServer _server;

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
        private string _stringPortfolio;

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
        private Direction _direction;

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

        public StepType StepType
        {
            get => _stepType;

            set
            {
                _stepType = value;
                OnPropertyChanged(nameof(StepType));
            }
        }
        private StepType _stepType;

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

        public ObservableCollection<string> StringPortfolios { get; set; } = new ObservableCollection<string>();

        private Portfolio _portfolio;

        #endregion

        #region Commands ==========================================================

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

        private Portfolio GetPortfolio(string number)
        {
            if (Server != null)
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

        private void ServerMaster_ServerCreateEvent(IServer newServer)
        {
            if (newServer == Server)
            {
                return;
            }



            Server = newServer; //если нет добавляем в список

            Server.PortfoliosChangeEvent += NewServer_PortfoliosChangeEvent; ;//событие на обновление счета
            Server.NeadToReconnectEvent += NewServer_NeadToReconnectEvent;//событие на перезаказ данных с сервера
            Server.NewMarketDepthEvent += NewServer_NewMarketDepthEvent;//подписка на обновление стакана
            Server.NewTradeEvent += NewServer_NewTradeEvent;//подписка на обезличенные сделки
            Server.NewOrderIncomeEvent += NewServer_NewOrderIncomeEvent;//изменение ордера
            Server.NewMyTradeEvent += NewServer_NewMyTradeEvent;//произошла моя сделка
            Server.ConnectStatusChangeEvent += NewServer_ConnectStatusChangeEvent;//изменение статуса соединения
            Server.NeadToReconnectEvent += _server_NeadToReconnectEvent;
        }

        private void _server_NeadToReconnectEvent()
        {
            
        }

        private void NewServer_ConnectStatusChangeEvent(string obj)
        {
            
        }


        private void NewServer_PortfoliosChangeEvent(List<Portfolio> portfolios)
        {
            
        }

        private void NewServer_NewMyTradeEvent(MyTrade myTrade)
        {

        }

        private void NewServer_NewOrderIncomeEvent(Order order)
        {

        }

        private void NewServer_NewTradeEvent(List<Trade> trades)
        {

        }

        private void NewServer_NewMarketDepthEvent(MarketDepth marketDepth)
        {

        }

        private void NewServer_NeadToReconnectEvent()
        {
            
        }

        #endregion
    }
}
