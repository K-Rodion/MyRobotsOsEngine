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

namespace OsEngine.Robots.VolumeReversal
{
    /// <summary>
    /// Логика взаимодействия для WindowVolumeReversalRobot.xaml
    /// </summary>
    public partial class WindowVolumeReversalRobot : Window
    {
        public WindowVolumeReversalRobot(VolumeReversalRobot robot)
        {
            InitializeComponent();

            vm = new VM(robot);

            DataContext = vm;
        }

        private VM vm;
    }
}
