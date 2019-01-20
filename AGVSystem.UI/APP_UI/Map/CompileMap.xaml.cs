using MySql.Data.MySqlClient;
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
using AGVSystem.IService.IO_System;
using AGVSystem.APP.agv_Map;

namespace AGVSystem.UI.APP_UI.Map
{
    /// <summary>
    /// CompileMap.xaml 的交互逻辑
    /// </summary>
    public partial class CompileMap : Window
    {
        public CompileMap()
        {
            InitializeComponent();
            Compile();
        }

        IO_AGVMapService mapService = new agvMapRegulate();

        public void Compile()
        {
            TabSerialPortData.ItemsSource = mapService.GetMapRegulate();
            TabSerialPortData.AutoGenerateColumns = false;
        }

        private void TabSerialPortData_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void CheckBox_Click_2(object sender, RoutedEventArgs e)
        {
            CheckBox headercb = (CheckBox)sender;

            for (int i = 0; i < TabSerialPortData.Items.Count; i++)
            {
                //获取行
                DataGridRow neddrow = (DataGridRow)TabSerialPortData.ItemContainerGenerator.ContainerFromIndex(i);
                if (neddrow != null)
                {
                    CheckBox cb = (CheckBox)TabSerialPortData.Columns[0].GetCellContent(neddrow);
                    if (cb != null)
                        cb.IsChecked = headercb.IsChecked;
                }
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Export_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
