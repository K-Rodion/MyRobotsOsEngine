using OsEngine.Robots.Arbitrager.Model;
using OsEngine.Robots.Arbitrager.ViewModel;
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

namespace OsEngine.Robots.Arbitrager.View
{
    /// <summary>
    /// Логика взаимодействия для ArbitragerUI.xaml
    /// </summary>
    public partial class ArbitragerUI : Window
    {
        public ArbitragerUI(ArbitragerBot robot)
        {
            InitializeComponent();

            vm = new VM(robot);

            DataContext = vm;
        }
        private VM vm;
    }
}
