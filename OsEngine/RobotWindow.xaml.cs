using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Threading;
using System.Diagnostics;
using OsEngine.Market;
using System.Threading;
using OsEngine.ViewModels;

namespace OsEngine
{
    /// <summary>
    /// Логика взаимодействия для RobotWindow.xaml
    /// </summary>
    public partial class RobotWindow : MetroWindow
    {
        public RobotWindow()
        {
            Process ps = Process.GetCurrentProcess();
            ps.PriorityClass = ProcessPriorityClass.RealTime;

            Dispatcher = Dispatcher.CurrentDispatcher;

            InitializeComponent();

            MainWindow.ProccesIsWorked = true;
            _window = this;

            ServerMaster.ActivateLogging();

            this.Closed += RobotWindow_Closed;

            DataContext = new RobotWindowVM();
        }

        private void RobotWindow_Closed(object sender, EventArgs e)
        {
            MainWindow.ProccesIsWorked = false;

            Thread.Sleep(10000);

            Process.GetCurrentProcess().Kill();
        }

        private static RobotWindow _window;

        public static Dispatcher Dispatcher;


        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
