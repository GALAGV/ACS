using AGVSystem.APP.agv_System;
using AGVSystem.BLL;
using AGVSystem.IService.IO_BLL;
using AGVSystem.IService.IO_System;
using AGVSystem.Model;
using AGVSystem.UI.APP_UI.Map;
using AGVSystem.UI.APP_UI.Setting;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Threading;
using AGVSystem.Model.Ga_agvModels;

namespace AGVSystem.UI.APP_UI.Main
{
    /// <summary>
    /// Home_Main.xaml 的交互逻辑
    /// </summary>
    public partial class Home_Main : Window
    {
        public Home_Main()
        {
            InitializeComponent();
        }


        IO_AGVmanagement Get_AGVmanagement = new agvFunction();
        private int selAgv = 1; //默认显示AGV
        List<Ga_agv> Ga_agvNumArray;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TabAgvMoveInfo(1535037182);
            AgvInfo();
            LoadComInfo(1535037182);
        }

        /// <summary>
        /// 加载串口信息
        /// </summary>
        public void LoadComInfo(long Time)
        {
            SerialPortData.DataContext = Get_AGVmanagement.agvGather(Time);
            SerialPortData.AutoGenerateColumns = false;
        }







        /// <summary>
        /// 显示所有AGV初始信息
        /// </summary>
        /// <param name="Time"></param>
        public void TabAgvMoveInfo(long Time)
        {
            Ga_agvNumArray = Get_AGVmanagement.AgvInfo(Time, ref selAgv);
            TabAgvMoveData.DataContext = Ga_agvNumArray;
            TabAgvMoveData.AutoGenerateColumns = false;
        }

        /// <summary>
        ///加载agv初始信息
        /// </summary>
        public void AgvInfo()
        {
            TabAgvData.ItemsSource = Get_AGVmanagement.AgvInfo();
            TabAgvData.HeadersVisibility = DataGridHeadersVisibility.None;
            TabAgvData.AutoGenerateColumns = false;
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            SettingForm setting = new SettingForm();
            setting.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("确定退出程序？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void Com_Map_Click(object sender, RoutedEventArgs e)
        {
            CompileMap compile = new CompileMap();
            compile.Show();
        }
    }
}
