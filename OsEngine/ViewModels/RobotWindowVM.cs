using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OsEngine.Commands;
using OsEngine.Market;
using OsEngine.Robots;
using OsEngine.Views;

namespace OsEngine.ViewModels
{
    public class RobotWindowVM:BaseVM
    {
        public RobotWindowVM()
        {
            
        }

        #region Properties ================================================

        public ObservableCollection<MyRobotVM> Robots { get; set; } = new ObservableCollection<MyRobotVM>();



        #endregion

        #region Fields ====================================================

        public static ChangeSecurityWindow ChangeSecurityWindow = null;

        #endregion

        #region Commands =================================================

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

        private DelegateCommand commandAddEmitent;

        public DelegateCommand CommandAddEmitent
        {
            get
            {
                if (commandAddEmitent == null)
                {
                    commandAddEmitent = new DelegateCommand(AddTabEmitent);
                }
                return commandAddEmitent;
            }
        }

        private DelegateCommand commandDeleteTab;

        public DelegateCommand CommandDeleteTab
        {
            get
            {
                if (commandDeleteTab == null)
                {
                    commandDeleteTab = new DelegateCommand(DeleteTabEmitent);
                }
                return commandDeleteTab;
            }
        }


        #endregion

        #region Methods ==================================================

        void ServersToConnect(object obj)
        {
            ServerMaster.ShowDialog(false);
        }

        void AddTabEmitent(object o)
        {
            Robots.Add(new MyRobotVM()
            {
                Header = "Tab "+ (Robots.Count+1)
            });
        }

        void DeleteTabEmitent(object obj)
        {
            string header = obj as string;

            MyRobotVM delRobot = null;

            foreach (var robot in Robots)
            {
                if (robot.Header == header)
                {
                    delRobot = robot;
                    break;
                }
            }

            if (delRobot != null)
            {
                MessageBoxResult res = MessageBox.Show("Remove tab " + header + "?", header, MessageBoxButton.YesNo);


                if (res == MessageBoxResult.Yes)
                {
                    Robots.Remove(delRobot);
                }
                
            }
        }

        #endregion
    }
}
