using OsEngine.OsTrader.Gui;
using OsEngine.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OsEngine.Views
{
    /// <summary>
    /// Логика взаимодействия для ChangeSecurityWindow.xaml
    /// </summary>
    public partial class ChangeSecurityWindow : Window
    {
        public ChangeSecurityWindow(MyRobotVM robot)
        {
            InitializeComponent();

            DataContext = new ChangeSecurityVM(robot);

        }
        
    }
}
