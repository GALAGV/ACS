using AGVSystem.BLL;
using AGVSystem.Model;
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
        Ga_agvBLL GetAgvBLL = new Ga_agvBLL();
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
            List<int> Agvlist = GetAgvBLL.AGVList(Time);
            dt = new DataTable("TabAgvMoveInfo");
            dt.Columns.Add(new DataColumn("type"));
            dt.Columns.Add(new DataColumn("TagName"));
            dt.Columns.Add(new DataColumn("Speed"));
            dt.Columns.Add(new DataColumn("turn"));
            dt.Columns.Add(new DataColumn("Dir"));
            dt.Columns.Add(new DataColumn("Hook"));
            dt.Columns.Add(new DataColumn("Rfid"));
            dt.Columns.Add(new DataColumn("Program"));
            dt.Columns.Add(new DataColumn("Step"));

            for (int i = 0; i < Agvlist.Count; i++)
            {
                dt.Rows.Add(new object[] { "离线", Agvlist[i], "", "", "", "", "", "" });
                string agvNum = "";
                if (Agvlist[i] < 10)
                {
                    agvNum = "00" + Agvlist[i].ToString();
                }
                else if (Agvlist[i] < 10 && Agvlist[i] < 100)
                {
                    agvNum = "0" + Agvlist[i].ToString();
                }
                else
                {
                    agvNum = Agvlist[i].ToString();
                }
                TreeViewItem tree = new TreeViewItem() { Header = "agv-" + agvNum + "" };
                tree.Selected += Tree_Selected;
                agvlist.Items.Add(tree);
                MainInfo.agvNo.Add(Agvlist[i]);
            }
            if (Agvlist.Count > 0)
            {
                selAgv = Agvlist[0];
            }
            TabAgvMoveData.DataContext = dt.DefaultView;
            TabAgvMoveData.AutoGenerateColumns = false;
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
            DataTable AgvData = new DataTable("TabAgvInfo");
            AgvData.Columns.Add(new DataColumn("Agv"));
            AgvData.Columns.Add(new DataColumn("Info"));
            AgvData.Rows.Add(new object[] { "AGV", "" });

            AgvData.Rows.Add(new object[] { "报警信息", "" });
            AgvData.Rows.Add(new object[] { "Program", "" });
            AgvData.Rows.Add(new object[] { "Step", "" });
            AgvData.Rows.Add(new object[] { "出发地", "" });
            AgvData.Rows.Add(new object[] { "目的地", "" });
            AgvData.Rows.Add(new object[] { "任务名", "" });
            TabAgvData.ItemsSource = AgvData.DefaultView;
            //TabAgvData.ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.Star);
            TabAgvData.HeadersVisibility = DataGridHeadersVisibility.None;
            TabAgvData.AutoGenerateColumns = false;
        }

    }
}
