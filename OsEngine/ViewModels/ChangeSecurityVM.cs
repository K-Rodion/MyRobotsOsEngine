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

namespace OsEngine.ViewModels
{
    public class ChangeSecurityVM:BaseVM
    {
        public ChangeSecurityVM()
        {
            Init();
        }

        #region Properties ===========================================================================================

        public ObservableCollection<Exchange> Exchanges { get; set; } = new ObservableCollection<Exchange>();

        public ObservableCollection<string> EmitClasses { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<Emitent> Securities { get; set; } = new ObservableCollection<Emitent>();

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


        #endregion

        #region Methods ============================================================================================

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

        }

        #endregion


    }
}
