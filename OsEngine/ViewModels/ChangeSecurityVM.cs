using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Commands;
using OsEngine.Entity;
using OsEngine.Market;
using OsEngine.Market.Servers;
using OsEngine.MyEntity;
using OsEngine.Robots;
using OsEngine.Views;

namespace OsEngine.ViewModels
{
    public class ChangeSecurityVM:BaseVM
    {
        public ChangeSecurityVM(MyRobotVM robot)
        {
            _robot = robot;

            Init();
        }

        #region Properties ===========================================================================================

        public ObservableCollection<Exchange> Exchanges { get; set; } = new ObservableCollection<Exchange>();

        public ObservableCollection<EmitClass> EmitClasses { get; set; } = new ObservableCollection<EmitClass>();

        public ObservableCollection<Emitent> Securities { get; set; } = new ObservableCollection<Emitent>();

        public Emitent SelectedEmitent
        {
            get => _selectedEmitent;

            set
            {
                _selectedEmitent = value;
                OnPropertyChanged(nameof(SelectedEmitent));
            }
        }
        private Emitent _selectedEmitent;

        #endregion

        #region Fields ==============================================================================================

        Dictionary<string, List<Security>> _classes = new Dictionary<string, List<Security>>();

        private MyRobotVM _robot;

        #endregion

        #region Commands ============================================================================================

        private DelegateCommand _commandSetExchange;

        public DelegateCommand CommandSetExchange
        {
            get
            {
                if (_commandSetExchange == null)
                {
                    _commandSetExchange = new DelegateCommand(SetExchange);
                }
                return _commandSetExchange;
            }
        }

        private DelegateCommand _commandSetEmitClass;

        public DelegateCommand CommandSetEmitClass
        {
            get
            {
                if (_commandSetEmitClass == null)
                {
                    _commandSetEmitClass = new DelegateCommand(SetEmitClass);
                }
                return _commandSetEmitClass;
            }
        }

        private DelegateCommand _commandChange;

        public DelegateCommand CommandChange
        {
            get
            {
                if (_commandChange == null)
                {
                    _commandChange = new DelegateCommand(Change);
                }
                return _commandChange;
            }
        }

        #endregion

        #region Methods ============================================================================================

        void Change(object obj)
        {
            if (SelectedEmitent != null && SelectedEmitent.Security != null)
            {
                _robot.SelectedSecurity = SelectedEmitent.Security;
            }
        }

        void SetEmitClass(object obj)
        {
            string classEmit = (string)obj;

            List<Security> secsList = _classes[classEmit];

            ObservableCollection<Emitent> emits = new ObservableCollection<Emitent>();

            foreach (Security sec in secsList)
            {
                emits.Add(new Emitent(sec));
            }

            Securities = emits;
            OnPropertyChanged(nameof(Securities));
        }

        void Init()
        {
            List<IServer> servers = ServerMaster.GetServers();

            Exchanges.Clear();


            foreach (IServer server in servers)
            {
                Exchanges.Add(new Exchange(server.ServerType));
            }
            OnPropertyChanged(nameof(Exchanges));
        }

        void SetExchange(object obj)
        {
            ServerType type = (ServerType)obj;

            List<IServer> servers = ServerMaster.GetServers();

            List<Security> securities = null;

            foreach (IServer server in servers)
            {
                if (server.ServerType == type)
                {
                    securities = server.Securities;
                    break;
                }
            }

            if (securities == null)
            {
                return;
            }

            _classes.Clear();

            EmitClasses.Clear();

            foreach (Security sec in securities)
            {
                if (_classes.ContainsKey(sec.NameClass))
                {
                    _classes[sec.NameClass].Add(sec);
                }
                else
                {
                    List<Security> secs = new List<Security>();

                    secs.Add(sec);

                    _classes.Add(sec.NameClass, secs);

                    EmitClasses.Add(new EmitClass(sec.NameClass));
                }
            }
        }

        #endregion


    }
}
