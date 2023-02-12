using System;
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

namespace OsEngine.ViewModels
{
    public class MyRobotVM:BaseVM
    {
        public MyRobotVM()
        {
            ServerMaster.ServerCreateEvent += ServerMaster_ServerCreateEvent;
        }

        

        #region Properties ==========================================================

        public ObservableCollection<string> ListSecurities { get; set; } = new ObservableCollection<string>();

        public string SelectedSecurity
        {
            get => _selectedSecurity;

            set
            {
                _selectedSecurity = value;
                OnPropertyChanged(nameof(SelectedSecurity));
                _security = GetSecurityForName(_selectedSecurity);

                StartSecurity(_security);
            }
        }

        private string _selectedSecurity = "";

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


        #endregion

        #region Fields ==========================================================

        IServer _server;

        private List<Security> _securities = new List<Security>();

        private Security _security;

        #endregion

        #region Commands ==========================================================

        private DelegateCommand commandServersToConnect;

        public DelegateCommand CommandServersToConnect
        {
            get
            {
                if (commandServersToConnect == null)
                {
                    commandServersToConnect = new DelegateCommand(ServersToConnect);
                }
                return commandServersToConnect;
            }
        }

        #endregion

        #region Methods ==========================================================

        void ServersToConnect(object obj)
        {
            ServerMaster.ShowDialog(false);
        }

        private Security GetSecurityForName(string name)
        {
            for (int i = 0; i < _securities.Count; i++)
            {
                if (_securities[i].Name == name)
                {
                    return _securities[i];
                }
            }

            return null;
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
                    var series = _server.StartThisSecurity(security.Name, new TimeFrameBuilder(), security.NameClass);

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
            if (newServer == _server)
            {
                return;
            }



            _server = newServer; //если нет добавляем в список

            _server.PortfoliosChangeEvent += NewServer_PortfoliosChangeEvent; ;//событие на обновление счета
            _server.SecuritiesChangeEvent += NewServer_SecuritiesChangeEvent; ;//событие на обновление бумаг с биржи
            _server.NeadToReconnectEvent += NewServer_NeadToReconnectEvent;//событие на перезаказ данных с сервера
            _server.NewMarketDepthEvent += NewServer_NewMarketDepthEvent;//подписка на обновление стакана
            _server.NewTradeEvent += NewServer_NewTradeEvent;//подписка на обезличенные сделки
            _server.NewOrderIncomeEvent += NewServer_NewOrderIncomeEvent;//изменение ордера
            _server.NewMyTradeEvent += NewServer_NewMyTradeEvent;//произошла моя сделка
            _server.ConnectStatusChangeEvent += NewServer_ConnectStatusChangeEvent;//изменение статуса соединения
        }

        private void NewServer_ConnectStatusChangeEvent(string obj)
        {
            
        }

        private void NewServer_SecuritiesChangeEvent(List<Security> securities)
        {

            ObservableCollection<string> listSecurities = new ObservableCollection<string>();

            for (int i = 0; i < securities.Count; i++)
            {
                listSecurities.Add(securities[i].Name);
            }

            ListSecurities = listSecurities;
            OnPropertyChanged(nameof(ListSecurities));

            _securities = securities;
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
