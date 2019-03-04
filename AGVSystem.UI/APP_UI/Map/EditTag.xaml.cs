using AGVSystem.APP.agv_Map;
using AGVSystem.Infrastructure.agvCommon;
using AGVSystem.Model.Ga_agvModels;
using AGVSystem.Model.MapData;
using OperateIni;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AGVSystem.UI.APP_UI.Map
{
    /// <summary>
    /// EditTag.xaml 的交互逻辑
    /// </summary>
    public partial class EditTag : Window
    {
        private Painting GetPainting = new Painting();
        private MapInstrument map = new MapInstrument();
        private agvMapRegulate mapService = new agvMapRegulate();
        private double CanvasWidth, CanvasHeight; //初始宽高
        private ObservableCollection<Ga_Map> MapList = new ObservableCollection<Ga_Map>();
        private ObservableCollection<MapTag> Tags = new ObservableCollection<MapTag>();
        private long UTCTime { get; set; }
        public EditTag()
        {
            InitializeComponent();
            Setting_Map();
        }
        /// <summary>
        /// 绑定数据
        /// </summary>
        public void Setting_Map()
        {
            MapList = mapService.GetMapRegulate(); ;
            MapMenu.ItemsSource = MapList;
            LoadMap(MapList[0]);
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\setting.ini"))
            {
                string Size = IniFile.ReadIniData("AGV", "MapSise", "", AppDomain.CurrentDomain.BaseDirectory + "\\setting.ini");
                map.MapSise = Convert.ToDouble(Size);
            }
        }

        private void MapMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MapMenu.Items.Count > 0 && !string.IsNullOrEmpty(MapMenu.Text))
            {
                SerialPortData.DataContext = null;
                LoadMap(MapList.FirstOrDefault(x => x.CreateTime.Equals(MapMenu.SelectedValue.ToString())));
            }
        }

        private void Magnify_Click(object sender, RoutedEventArgs e)
        {
            map.MapSise += 0.2;
            if (map.MapSise > 5)
            {
                return;
            }
            CanvasMapZoom();
        }


        /// <summary>
        /// 缩放
        /// </summary>
        private void CanvasMapZoom()
        {
            map.Mapmagnify(map.MapSise, CanvasWidth, CanvasHeight, true);
        }

        private void Shrink_Click(object sender, RoutedEventArgs e)
        {
            map.MapSise -= 0.2;
            if (map.MapSise < 1)
            {
                map.MapSise = 1;
                return;
            }
            CanvasMapZoom();
        }

        private void Recover_Click(object sender, RoutedEventArgs e)
        {
            map.MapSise = 1;
            CanvasMapZoom();
        }

        private void SrcCount_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            SrcX.ScrollToHorizontalOffset(e.HorizontalOffset);//X轴标尺跟随移动
            SrcY.ScrollToVerticalOffset(e.VerticalOffset); //Y轴标尺等随移动
        }

        private void SerialPortData_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            int Columnindex = e.Column.DisplayIndex;
            int Rowsindex = e.Row.GetIndex();
            MapTag route = e.Row.DataContext as MapTag;
            if (route != null)
            {
                List<Ga_agvStatus> agv = new List<Ga_agvStatus>();
                agv.Add(new Ga_agvStatus() { StatusName = "N/A", StatusValue = "N/A" });
                mapService.SelectTagSystem(UTCTime, route.TagName).Select(p => new Ga_agvStatus() { StatusName = p, StatusValue = p }).ToList().ForEach(p => agv.Add(p));
                if (Columnindex.Equals(1))
                {
                    Tags[Rowsindex].NextTagList = agv;
                }
                else if (Columnindex.Equals(2))
                {
                    Tags[Rowsindex].NextLeftTagList = agv;
                }
                else if (Columnindex.Equals(3))
                {
                    Tags[Rowsindex].NextRightTagList = agv;
                }
                else if (Columnindex.Equals(4))
                {
                    Tags[Rowsindex].PreTagList = agv;
                }
                else if (Columnindex.Equals(5))
                {
                    Tags[Rowsindex].PreLeftTagList = agv;
                }
                else if (Columnindex.Equals(6))
                {
                    Tags[Rowsindex].PreRightTagList = agv;
                }
                else if (Columnindex.Equals(7))
                {
                    Tags[Rowsindex].SpeedList = MapRegulate.speed;
                }
                else if (Columnindex.Equals(8))
                {
                    Tags[Rowsindex].SpeedRevList = MapRegulate.speed;
                }
                else if (Columnindex.Equals(9))
                {
                    Tags[Rowsindex].PreTurnSpeedList = MapRegulate.speed;
                }
                else if (Columnindex.Equals(10))
                {
                    Tags[Rowsindex].RevTurnSpeedList = MapRegulate.speed;
                }
                else if (Columnindex.Equals(11))
                {
                    Tags[Rowsindex].PbsList = MapRegulate.pbs;
                }
                else if (Columnindex.Equals(12))
                {
                    Tags[Rowsindex].PbsRevList = MapRegulate.pbs;
                }
                else if (Columnindex.Equals(13))
                {
                    Tags[Rowsindex].PreTurnPBSList = MapRegulate.pbs;
                }
                else if (Columnindex.Equals(14))
                {
                    Tags[Rowsindex].RevTurnPBSList = MapRegulate.pbs;
                }
                AssociatedTag(Convert.ToInt32(route.TagName));
            }
        }

        private void ListT_DropDownClosed(object sender, EventArgs e)
        {
            this.SerialPortData.CommitEdit();
        }

        private void SerialPortData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int indexs = SerialPortData.SelectedIndex;
            int index = SerialPortData.CurrentCell.Column.DisplayIndex;

            if (!index.Equals(0))
            {
                this.SerialPortData.BeginEdit();

                DataGridTemplateColumn templeColumn = SerialPortData.Columns[index] as DataGridTemplateColumn;
                if (templeColumn == null) return;

                object item = SerialPortData.CurrentCell.Item;
                FrameworkElement element = templeColumn.GetCellContent(item);
                if (element != null)
                {
                    ComboBox expander = templeColumn.CellEditingTemplate.FindName("listT", element) as ComboBox;
                    if (expander != null)
                    {
                        expander.IsDropDownOpen = true;
                    }
                }
            }
        }

        private void TagSave_Click(object sender, RoutedEventArgs e)
        {
            if (mapService.UpdateTagInfoSystem(UTCTime, Tags))
            {
                MessageBox.Show("保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("保存失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SerialPortData_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            int indexs = e.Column.DisplayIndex;
            int index = e.Row.GetIndex();
            MapTag route = e.Row.DataContext as MapTag;
            if (route != null)
            {
                if (indexs.Equals(1))
                {
                    Tags[index].NextTagColor = route.NextTag != "N/A" ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Black);
                }
                else if (indexs.Equals(2))
                {
                    Tags[index].NextLeftTagColor = route.NextLeftTag != "N/A" ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Black);
                }
                else if (indexs.Equals(3))
                {
                    Tags[index].NextRightTagColor = route.NextRightTag != "N/A" ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Black);
                }
                else if (indexs.Equals(4))
                {
                    Tags[index].PreTagColor = route.PreTag != "N/A" ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Black);
                }
                else if (indexs.Equals(5))
                {
                    Tags[index].PreLeftTagColor = route.PreLeftTag != "N/A" ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Black);
                }
                else if (indexs.Equals(6))
                {
                    Tags[index].PreRightTagColor = route.PreRightTag != "N/A" ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Black);
                    AssociatedTag(Convert.ToInt32(route.TagName));
                }
                else if (indexs.Equals(7))
                {
                    Tags[index].SpeedColor = route.Speed != "0: 5" && route.Speed != "无变化" ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black);
                }
                else if (indexs.Equals(8))
                {
                    Tags[index].SpeedRevColor = route.SpeedRev != "0: 5" && route.SpeedRev != "无变化" ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black);
                }
                else if (indexs.Equals(9))
                {
                    Tags[index].PreTurnSpeedColor = route.PreTurnSpeed != "0: 5" && route.PreTurnSpeed != "无变化" ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black);
                }
                else if (indexs.Equals(10))
                {
                    Tags[index].RevTurnSpeedColor = route.RevTurnSpeed != "0: 5" && route.RevTurnSpeed != "无变化" ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black);
                }
                else if (indexs.Equals(11))
                {
                    Tags[index].PbsColor = route.Pbs != "区域0" && route.Pbs != "无变化" ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black);
                }
                else if (indexs.Equals(12))
                {
                    Tags[index].PbsRevColor = route.PbsRev != "区域0" && route.PbsRev != "无变化" ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black);
                }
                else if (indexs.Equals(13))
                {
                    Tags[index].PreTurnPBSColor = route.PreTurnPBS != "区域0" && route.PreTurnPBS != "无变化" ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black);
                }
                else if (indexs.Equals(14))
                {
                    Tags[index].RevTurnPBSColor = route.RevTurnPBS != "区域0" && route.RevTurnPBS != "无变化" ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black);
                }
                TagRecover();
            }
        }

        private void TagRecover()
        {
            map.valuePairs.Select(p => p.Value).ToList().ForEach(p => p.Background = new SolidColorBrush(Colors.Black));
        }


        private void AssociatedTag(int TagID)
        {
            string[] TagArray = mapService.SelectTagSystem(UTCTime, TagID.ToString());
            TagRecover();
            map.valuePairs.FirstOrDefault(p => p.Key.Equals(TagID)).Value.Background = new SolidColorBrush(Colors.Red);
            Label label = map.valuePairs.FirstOrDefault(p => p.Key.Equals(TagID)).Value;
            SrcCount.ScrollToHorizontalOffset(label.Margin.Left - 600);//滚动条X轴跟随移动
            SrcCount.ScrollToVerticalOffset(label.Margin.Top - 500); ///滚动条Y轴等随移动
            for (int i = 0; i < TagArray.Length; i++)
            {
                map.valuePairs.FirstOrDefault(p => p.Key.Equals(Convert.ToInt32(TagArray[i]))).Value.Background = new SolidColorBrush(Colors.Green);
            }
        }

        private void SerialPortData_BeginningEdit_1(object sender, DataGridBeginningEditEventArgs e)
        {

        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as ToolBar).ToolBar_Formatting();
        }

        public void LoadMap(Ga_Map GetMap)
        {
            UTCTime = UTC.ConvertDateTimeLong(Convert.ToDateTime(GetMap.CreateTime));
            Tags = mapService.TagManagement(UTCTime);
            SerialPortData.AutoGenerateColumns = false;
            SerialPortData.ItemsSource = Tags;
            CanvasWidth = GetMap.Width * 10;
            CanvasHeight = GetMap.Height * 10;
            double CanvasWidths = GetMap.Width * 10 * map.MapSise;
            double CanvasHeights = GetMap.Height * 10 * map.MapSise;
            map.Initial_Canvas(TopX, TopY, mainPanel, CanvasWidths, CanvasHeights);
            map.LoadEditMap(UTCTime, false, true);
        }
    }
}
