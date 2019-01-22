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
using AGVSystem.Model.Ga_agvModels;

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

        IO_AGVMapService mapService = new agvMapRegulate(); //业务逻辑接口

        List<Ga_Map> GetMaps = new List<Ga_Map>(); //显示数据源


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }


        public void Compile()
        {
            GetMaps= mapService.GetMapRegulate();
            TabSerialPortData.DataContext = GetMaps;
            TabSerialPortData.AutoGenerateColumns = false;
        }



        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            Ga_Map ga_Map = SelectMap();
            if (ga_Map == null)
                return;

            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "地图信息文件|*.tll";
            sfd.FileName = "(" + ga_Map.Name + ")" + DateTime.Now.ToString("yyyyMMdd");
            if (sfd.ShowDialog() == true)
            {
                if (mapService.Export_Map(ga_Map.CreateTime, sfd.FileName))
                {
                    GetMaps.ForEach(x => x.IsSelected = false); //取消选择
                    MessageBox.Show("导出成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.No);
                }
                else
                {
                    MessageBox.Show("导出失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                }
            }
        }

        /// <summary>
        /// 获取选中列表项
        /// </summary>
        /// <returns></returns>
        private Ga_Map SelectMap()
        {
            List<Ga_Map> SelectedList = GetMaps.Where(a => a.IsSelected).ToList();
            if (SelectedList.Count.Equals(0))
            {
                MessageBox.Show("请选择需要操作的列表项！", "提示", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                return null;
            }
            else if (SelectedList.Count.Equals(1))
            {

                return SelectedList[0];
            }
            else
            {
                MessageBox.Show("请选择一项需要操作的列表项！", "提示", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                return null;
            }
        }

        /// <summary>
        /// 新建地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMap_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AllCkek_Click(object sender, RoutedEventArgs e)
        {
            GetMaps.ForEach(x => x.IsSelected = ((CheckBox)sender).IsChecked == true ? true : false); //全选or反选
        }


        /// <summary>
        /// 删除地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteMap_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
