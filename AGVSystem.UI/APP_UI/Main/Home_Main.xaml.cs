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
        bool SelectMenu = false;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Setting_Map();
        }

        /// <summary>
        /// 查询默认地图
        /// </summary>
        public void Setting_Map()
        {
            SelectMenu = true;
            MapMenu.ItemsSource = mapService.GetMapRegulate();
            AgvInfo();
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        public void LoadMap()
        {
            try
            {
                DataTable MapData = mapService.defaultMap(MapRegulate.UTCTime);
                if (MapData.Rows.Count > 0)
                {
                    Ga_Map GetMap = new Ga_Map()
                    {
                        Width = Convert.ToDouble(MapData.Rows[0]["Width"].ToString()),
                        Height = Convert.ToDouble(MapData.Rows[0]["Height"].ToString())
                    };
                    string MapNam = MapData.Rows[0]["Name"].ToString();
                    MapMenu.Text = MapNam;
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
                    map.LoadEditMap(MapRegulate.UTCTime, false);
                    TabAgvMoveInfo(MapRegulate.UTCTime);
                    LoadComInfo(MapRegulate.UTCTime);
                }
                else
                {
                    throw new Exception("Maps don't exist！");
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
            SelectionMenu();
        }

        public void SelectionMenu()
        {
            if (SelectMenu)
            {
                DataTable data = mapService.setting();
                if (data.Rows.Count > 0)
                {
                    long DefaultName = long.Parse(data.Rows[0]["Map"].ToString().Trim()); ;
                    MapRegulate.UTCTime = DefaultName;
                    MapRegulate.DefaultMap = DefaultName;
                    SelectMenu = false;
                }
                else
                {
                    throw new Exception("Default map configuration missing!");
                }
            }
            else
            {
                MapRegulate.UTCTime = UTC.ConvertDateTimeLong(Convert.ToDateTime(MapMenu.SelectedValue.ToString()));
            }
            LoadMap();
            Load_agv();
        }








        public void Load_agv()
        {

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
            }
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
            Ga_agvNumArray = Get_AGVmanagement.AgvInfo(Time, ref MapRegulate.selAgv);
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
            setting.GetSettingMap += SelectionMenu;
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
