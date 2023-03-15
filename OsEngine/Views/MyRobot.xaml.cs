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
using MahApps.Metro.Controls;
using OsEngine.Market;
using OsEngine.Market.Servers;
using OsEngine.ViewModels;

namespace OsEngine.Views
{
    /// <summary>
    /// Логика взаимодействия для MyRobot.xaml
    /// </summary>
    public partial class MyRobot : UserControl
    {
        public MyRobot()
        {
            InitializeComponent();

            scroll = myScrollViewer;
            
        }

        private ScrollViewer scroll;


        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            PreviewMouseWheel += MyRobot_PreviewMouseWheel;
        }

        private void MyRobot_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta);
        }
    }
}
