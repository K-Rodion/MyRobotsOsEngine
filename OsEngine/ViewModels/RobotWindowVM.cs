using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using OsEngine.Charts.CandleChart.Indicators;
using OsEngine.Commands;
using OsEngine.Entity;
using OsEngine.Market;
using OsEngine.MyEntity;
using OsEngine.Robots;
using OsEngine.Views;

namespace OsEngine.ViewModels
{
    public class RobotWindowVM:BaseVM
    {
        public RobotWindowVM()
        {
            ServerMaster.ServerCreateEvent += ServerMaster_ServerCreateEvent;

            Task.Run(() =>
            {
                RecordLog();
            });

            Load();

            ServerMaster.ActivateAutoConnection();
        }

        

        #region Properties ================================================

        public ObservableCollection<MyRobotVM> Robots { get; set; } = new ObservableCollection<MyRobotVM>();

        public MyRobotVM SelectedRobot
        {
            get => _selectedRobot;

            set
            {
                _selectedRobot = value;
                OnPropertyChanged(nameof(SelectedRobot));
            }
        }
        private MyRobotVM _selectedRobot;

        #endregion

        #region Fields ====================================================

        public static ChangeSecurityWindow ChangeSecurityWindow = null;

        private static ConcurrentQueue<MessageForLog> _logMessages = new ConcurrentQueue<MessageForLog>();//коллекция для записи логов

        public static ConcurrentDictionary<string, ConcurrentDictionary<string, Order>> Orders =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, Order>>();

        public static ConcurrentDictionary<string, ConcurrentDictionary<string, MyTrade>> MyTrades =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, MyTrade>>();

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

        private void ServerMaster_ServerCreateEvent(Market.Servers.IServer server)
        {
            server.NewOrderIncomeEvent += Server_NewOrderIncomeEvent;
            server.NewMyTradeEvent += Server_NewMyTradeEvent;
            server.ConnectStatusChangeEvent += Server_ConnectStatusChangeEvent;
        }

        private void Server_ConnectStatusChangeEvent(string state)
        {
            if (state == "Connect")
            {
                Task.Run(async () =>
                {
                    DateTime dt = DateTime.Now;

                    while (dt.AddMinutes(1) > DateTime.Now)
                    {
                        await Task.Delay(5000);

                        foreach (var robot in Robots)
                        {
                            robot.CheckMissedOrders();

                            robot.CheckMissedMyTrades();
                        }
                    }
                });
            }
        }

        private void Server_NewMyTradeEvent(MyTrade myTrade)
        {
            ConcurrentDictionary<string, MyTrade> myTrades = null;

            if (MyTrades.TryGetValue(myTrade.SecurityNameCode, out myTrades))
            {
                myTrades.AddOrUpdate(myTrade.NumberTrade, myTrade, (key, value) => value = myTrade);
            }
            else
            {
                myTrades = new ConcurrentDictionary<string, MyTrade>();

                myTrades.AddOrUpdate(myTrade.NumberTrade, myTrade, (key, value) => value = myTrade);

                MyTrades.AddOrUpdate(myTrade.SecurityNameCode, myTrades, (key, value) => value = myTrades);
            }
        }

        private void Server_NewOrderIncomeEvent(Order order)
        {
            ConcurrentDictionary<string, Order> orders = null;

            if (Orders.TryGetValue(order.SecurityNameCode, out orders))
            {
                orders.AddOrUpdate(order.NumberMarket, order, (key, value) => value = order);
            }
            else
            {
                orders = new ConcurrentDictionary<string, Order>();

                orders.AddOrUpdate(order.NumberMarket, order, (key, value) => value = order);

                Orders.AddOrUpdate(order.SecurityNameCode, orders, (key, value) => value = orders);
            }
        }

        void ServersToConnect(object obj)
        {
            ServerMaster.ShowDialog(false);
        }

        void AddTabEmitent(object o)
        {
            AddTab("");
        }

        void AddTab(string name)
        {
            if (name != "")
            {
                Robots.Add(new MyRobotVM(name, Robots.Count + 1));
            }
            else
            {
                Robots.Add(new MyRobotVM("Tab " + Robots.Count + 1, Robots.Count + 1));
            }

            Robots.Last().OnSelectedSecurity += RobotWindowVM_OnSelectedSecurity;
            
        }

        private void RobotWindowVM_OnSelectedSecurity()
        {
            Save();
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

        public static void Log(string name, string str)
        {
            MessageForLog messageForLog = new MessageForLog()
            {
                Name = name,
                Message = str
            };

            _logMessages.Enqueue(messageForLog);
        }

        private static void RecordLog()
        {
            if (!Directory.Exists(@"Log"))
            {
                Directory.CreateDirectory(@"Log");
            }

            while (MainWindow.ProccesIsWorked)
            {
                MessageForLog mess;

                if (_logMessages.TryDequeue(out mess))
                {
                    string name = mess.Name + "_" + DateTime.Now.ToShortDateString() + ".log";

                    using (StreamWriter writer = new StreamWriter(@"Log\" + name, true))
                    {
                        writer.WriteLine(mess.Message);

                        writer.Close();
                    }
                }

                Thread.Sleep(5);
            }
        }

        private void Save()
        {
            if (!Directory.Exists(@"Parameters"))
            {
                Directory.CreateDirectory(@"Parameters");
            }

            string str = "";

            for (int i = 0; i < Robots.Count; i++)
            {
                str += Robots[i].Header + ";";
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(@"Parameters\param.txt", false))
                {
                    writer.WriteLine(str);

                    writer.WriteLine(SelectedRobot.NumberTab);

                    writer.Close();
                }

            }
            catch (Exception e)
            {
                Log("App", "Save error = " + e.Message);
            }
            
        }

        private void Load()
        {
            if (!Directory.Exists(@"Parameters"))
            {
                return;
            }

            string strtabs = "";

            int selectedNumber = 0;

            try
            {
                using (StreamReader reader = new StreamReader(@"Parameters\param.txt"))
                {
                    strtabs = reader.ReadLine();

                    selectedNumber = Convert.ToInt32(reader.ReadLine());

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Log("App", "Load error = " + e.Message);
            }

            string[] tabs = strtabs.Split(';');

            foreach (var tab in tabs)
            {
                if (tab != "")
                {
                    AddTab(tab);
                }
            }

            if (Robots.Count > selectedNumber - 1)
            {
                SelectedRobot = Robots[selectedNumber - 1];
            }
            

        }

        #endregion
    }
}
