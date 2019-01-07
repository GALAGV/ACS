using AGVSystem.APP.agv_System;
using AGVSystem.BLL;
using AGVSystem.IService.IO_BLL;
using AGVSystem.IService.IO_System;
using AGVSystem.Model;
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

        IO_agvBLL GetAgvBLL = new Ga_agvBLL();
        IO_AGVmanagement Get_AGVmanagement = new agvFunction();
        private int selAgv = 1; //默认显示AGV
        DataTable dt;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TabAgvMoveInfo(1524132399);
            AgvInfo();
        }

        /// <summary>
        /// 显示所有AGV初始信息
        /// </summary>
        /// <param name="Time"></param>
        public void TabAgvMoveInfo(long Time)
        {
            List<int> AgvNum = GetAgvBLL.AGVNumList(Time);
            dt = Get_AGVmanagement.AgvInfo(AgvNum, ref selAgv);
            TabAgvMoveData.DataContext = dt.DefaultView;
            TabAgvMoveData.AutoGenerateColumns = false;
            //agvlist.DataContext = AgvNum;
            //TreeViewItem tree = new TreeViewItem() { Header = "agv-" + agvNum + "" };
            //tree.Selected += Tree_Selected;
            //agvlist.Items.Add(tree);
        }

        /// <summary>
        /// Tree点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tree_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tree = sender as TreeViewItem;
            MessageBox.Show(tree.Header.ToString());
        }

        /// <summary>
        ///加载agv初始信息
        /// </summary>
        public void AgvInfo()
        {
            TabAgvData.ItemsSource = Get_AGVmanagement.AgvInfo().DefaultView;
            //TabAgvData.ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.Star);
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
    }
}
