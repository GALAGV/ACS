using AGVSystem.APP.agv_System;
using AGVSystem.IService.IO_System;
using AGVSystem.UI.APP_UI.Map;
using AGVSystem.UI.APP_UI.Setting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using AGVSystem.Model.Ga_agvModels;
using AGVSystem.APP.agv_Map;
using AGVSystem.Model.MapData;
using AGVSystem.Infrastructure.agvCommon;
using AGVSystem.Model.LogicData;
using System.Windows.Media.Imaging;
using System.IO;
using OperateIni;
using System.Linq;
using System.Collections.ObjectModel;

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
        IO_AGVMapService mapService = new agvMapRegulate(); //业务逻辑接口
        MapInstrument map = new MapInstrument();
        List<Ga_agv> Ga_agvNumArray; //数据源
        Painting GetPainting = new Painting();
        bool OpenPort = false;
        bool StartNoopsyche = false;
        ObservableCollection<Ga_Map> maps = new ObservableCollection<Ga_Map>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Setting_Map();
        }

        /// <summary>
        /// 查询默认地图
        /// </summary>
        public void Setting_Map()
        {
            maps = mapService.GetMapRegulate();
            MapMenu.ItemsSource = maps;
            DataTable data = mapService.setting();
            if (data.Rows.Count > 0)
            {
                long DefaultName = long.Parse(data.Rows[0]["Map"].ToString().Trim());
                MapRegulate.DefaultMap = DefaultName;
                Ga_Map ga = maps.FirstOrDefault(x => x.CreateTime.Equals(UTC.ConvertLongDateTime(DefaultName).ToString("yyyy-MM-dd HH:mm:ss")));
                if (ga != null)
                {
                    MapMenu.Text = ga.Name;
                }
            }
            else
            {
                throw new Exception("默认地图配置丢失！");
            }
            AgvInfo();
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        public void LoadMap()
        {
            try
            {
                Ga_Map GetMap = maps.FirstOrDefault(x => x.CreateTime.Equals(MapMenu.SelectedValue.ToString()));
                if (GetMap != null)
                {
                    TopX.Children.Clear();
                    TopY.Children.Clear();
                    GetPainting.CoordinateX(TopX, TopY);
                    map.GetCanvas = mainPanel;
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\setting.ini"))
                    {
                        string Size = IniFile.ReadIniData("AGV", "MapSise", "", AppDomain.CurrentDomain.BaseDirectory + "\\setting.ini");
                        map.MapSise = Convert.ToDouble(Size);
                    }
                    double CanvasWidth = GetMap.Width * 10 * map.MapSise > this.Width * 1.2 ? GetMap.Width * 10 * map.MapSise : this.Width * 1.2;
                    double CanvasHeight = GetMap.Height * 10 * map.MapSise > this.Height * 1.2 ? GetMap.Height * 10 * map.MapSise : this.Height * 1.2;
                    TopX.Width = CanvasWidth;
                    TopY.Height = CanvasHeight;
                    mainPanel.Width = CanvasWidth;
                    mainPanel.Height = CanvasHeight;
                    map.LoadEditMap(MapRegulate.UTCTime, false,false);
                    TabAgvMoveInfo(MapRegulate.UTCTime);
                    LoadComInfo(MapRegulate.UTCTime);
                }
                else
                {
                    throw new Exception("地图数据不存在！");
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 地图选项更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MapMenu.Items.Count > 0 && MapMenu.SelectedValue != null)
            {
                SelectionMenu();
            }
        }

        public void SelectionMenu()
        {
            MapRegulate.UTCTime = UTC.ConvertDateTimeLong(Convert.ToDateTime(MapMenu.SelectedValue.ToString()));
            LoadMap();
            Load_agv();
        }


        public void Load_agv()
        {
            MapRegulate.GetValuePairs.Clear();
            Grid gridItem = new Grid();
            gridItem.Children.Clear();
            gridItem.VerticalAlignment = VerticalAlignment.Top;
            gridItem.HorizontalAlignment = HorizontalAlignment.Center;
            agvlist.Content = gridItem;
            //创建行
            for (int s = 0; s < MainInfo.agvNo.Count; s++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(40);
                gridItem.RowDefinitions.Add(row);
            }
            for (int s = 0; s < 2; s++)
            {
                //创建列
                ColumnDefinition column = new ColumnDefinition();
                gridItem.ColumnDefinitions.Add(column);
            }

            for (int i = 0; i < MainInfo.agvNo.Count; i++)
            {
                Label labe2 = new Label();
                labe2.Content = $"AGV{MainInfo.agvNo[i]}";
                labe2.HorizontalAlignment = HorizontalAlignment.Center;
                labe2.VerticalAlignment = VerticalAlignment.Center;
                labe2.Margin = new Thickness(10, 5, 35, 5);
                labe2.FontSize = 14;
                labe2.FontWeight = FontWeights.Black;
                Grid.SetColumn(labe2, 0);
                Grid.SetRow(labe2, i);
                gridItem.Children.Add(labe2);

                Image image = new Image();
                image.Source = new BitmapImage(new Uri("pack://application:,,,/image/Gray_s.png"));
                image.HorizontalAlignment = HorizontalAlignment.Center;
                image.VerticalAlignment = VerticalAlignment.Center;
                image.Margin = new Thickness(20, 5, 0, 5);
                Grid.SetColumn(image, 1);
                Grid.SetRow(image, i);
                gridItem.Children.Add(image);
                MapRegulate.GetValuePairs.GetOrAdd(Convert.ToInt32(MainInfo.agvNo[i]), image);
            }
        }



        /// <summary>
        /// 加载串口信息
        /// </summary>
        public void LoadComInfo(long Time)
        {
            MapRegulate.GetPortInfos = Get_AGVmanagement.agvGather(Time);
            SerialPortData.DataContext = MapRegulate.GetPortInfos;
            SerialPortData.AutoGenerateColumns = false;
        }

        /// <summary>
        /// 显示所有AGV初始信息
        /// </summary>
        /// <param name="Time"></param>
        public void TabAgvMoveInfo(long Time)
        {
            MapRegulate.GetAgvs = Get_AGVmanagement.AgvInfo(Time, ref MapRegulate.selAgv);
            Ga_agvNumArray = MapRegulate.GetAgvs;
            TabAgvMoveData.DataContext = Ga_agvNumArray;
            TabAgvMoveData.AutoGenerateColumns = false;
        }

        /// <summary>
        ///加载默认agv初始信息
        /// </summary>
        public void AgvInfo()
        {
            MapRegulate.Ga_AgvStatuses = Get_AGVmanagement.AgvInfo();
            TabAgvData.DataContext = MapRegulate.Ga_AgvStatuses;
            TabAgvData.HeadersVisibility = DataGridHeadersVisibility.None;
            TabAgvData.AutoGenerateColumns = false;
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            SettingForm setting = new SettingForm();
            setting.GetSettingMap += SelectionMenu;
            setting.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!OpenPort)
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
            else
            {
                e.Cancel = true;
                MessageBox.Show("请先关闭串口！", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }

        private void Com_Map_Click(object sender, RoutedEventArgs e)
        {
            CompileMap compile = new CompileMap();
            compile.Show();
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Port_Click(object sender, RoutedEventArgs e)
        {
            if (!OpenPort)
            {
                PortImg.Source = new BitmapImage(new Uri("pack://application:,,,/image/Stop.png", UriKind.Absolute));
                PortText.Text = "关闭串口";
                OpenPort = true;
                StartBrainpower.IsEnabled = true;
                MapMenu.IsEnabled = false;
                MessageBox.Show("打开成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                PortImg.Source = new BitmapImage(new Uri("pack://application:,,,/image/start.png", UriKind.Absolute));
                PortText.Text = "打开串口";
                OpenPort = false;
                MapMenu.IsEnabled = true;
                StartBrainpower.IsEnabled = false;
                MessageBox.Show("关闭成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        /// <summary>
        /// 智能运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartBrainpower_Click(object sender, RoutedEventArgs e)
        {
            if (!StartNoopsyche)
            {
                StartImg.Source = new BitmapImage(new Uri("pack://application:,,,/image/Stopagv.png", UriKind.Absolute));
                StartText.Text = "停止运行";
                StartNoopsyche = true;
                Open_Port.IsEnabled = false;
            }
            else
            {
                StartImg.Source = new BitmapImage(new Uri("pack://application:,,,/image/noopsyche.png", UriKind.Absolute));
                StartText.Text = "智能运行";
                StartNoopsyche = false;
                Open_Port.IsEnabled = true;
            }

        }

        private void AddMapInfo_Click(object sender, RoutedEventArgs e)
        {
            AddMap map = new AddMap();
            map.ShowDialog();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EditLine edit = new EditLine(maps);
            edit.Show();
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
        }
    }
}
