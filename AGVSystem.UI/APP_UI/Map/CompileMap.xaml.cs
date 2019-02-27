using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AGVSystem.APP.agv_Map;
using AGVSystem.Model.Ga_agvModels;
using AGVSystem.Infrastructure.agvCommon;
using System.IO;
using System.Collections.ObjectModel;

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

        agvMapRegulate mapService = new agvMapRegulate(); //业务逻辑接口

        ObservableCollection<Ga_Map> GetMaps = new ObservableCollection<Ga_Map>(); //显示数据源


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
            sfd.FileName = "" + ga_Map.Name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            if (sfd.ShowDialog() == true)
            {
                if (mapService.Export_Map(UTC.ConvertDateTimeLong(Convert.ToDateTime(ga_Map.CreateTime)), sfd.FileName))
                {
                    GetMaps.ToList().ForEach(x => x.IsSelected = false); //取消选择
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
            AddMap map = new AddMap();
            this.Close();
            map.ShowDialog();
        }

        private void AllCkek_Click(object sender, RoutedEventArgs e)
        {
            GetMaps.ToList().ForEach(x => x.IsSelected = ((CheckBox)sender).IsChecked == true ? true : false); //全选or反选
        }


        /// <summary>
        /// 删除地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteMap_Click(object sender, RoutedEventArgs e)
        {
            if ((GetMaps.Where(a => a.IsSelected).ToList()).Count.Equals(0))
            {
                MessageBox.Show("请选择需要操作的列表项！", "提示", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                return;
            }
            MessageBoxResult result = MessageBox.Show("确定删除吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                if (mapService.Delete_Map(GetMaps.Where(a => a.IsSelected).ToList()) == true)
                {
                    GetMaps = mapService.GetMapRegulate();
                    TabSerialPortData.DataContext = GetMaps;
                    MessageBox.Show("删除成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.No);
                }
            }

        }



        /// <summary>
        /// 导入地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WriteMap_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".tll";
            dlg.Filter = "地图信息文件|*.tll";
            if (dlg.ShowDialog() == true)
            {
                string sqlText = File.ReadAllText(dlg.FileName);
                if (mapService.AGV_MapTolead(sqlText) == true)
                {
                    GetMaps = mapService.GetMapRegulate();
                    TabSerialPortData.DataContext = GetMaps;
                    MessageBox.Show("导入成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("导入失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// 编辑地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditMap_Click(object sender, RoutedEventArgs e)
        {
            Ga_Map ga_Map = SelectMap();
            if (ga_Map == null)
                return;

            MapEdit map = new MapEdit(ga_Map, true);
            map.Show();
            this.Close();
        }
    }
}
