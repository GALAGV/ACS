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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Setting_Map();
        }


        /// <summary>
        /// 查询默认地图
        /// </summary>
        public void Setting_Map()
        {
            DataTable data = mapService.setting();
            if (data.Rows.Count > 0)
            {
                MapRegulate.UTCTime = long.Parse(data.Rows[0]["Map"].ToString());

                DataTable MapData = mapService.defaultMap(MapRegulate.UTCTime);
                if (MapData.Rows.Count > 0)
                {
                    Ga_Map GetMap = new Ga_Map()
                    {
                        Width = Convert.ToDouble(MapData.Rows[0]["Width"].ToString()),
                        Height = Convert.ToDouble(MapData.Rows[0]["Height"].ToString())
                    };
                    GetPainting.CoordinateX(TopX, TopY);
                    GetPainting.Coordinate(mainPanel);
                    map.GetCanvas = mainPanel;
                    map.MapSise = 2;
                    double CanvasWidth = GetMap.Width * 10 * map.MapSise > this.Width * 1.5 ? GetMap.Width * 10 * map.MapSise : this.Width * 1.5;
                    double CanvasHeight = GetMap.Height * 10 * map.MapSise > this.Height * 1.2 ? GetMap.Height * 10 * map.MapSise : this.Height * 1.2;
                    TopX.Width = CanvasWidth;
                    TopY.Height = CanvasHeight;
                    mainPanel.Width = CanvasWidth;
                    mainPanel.Height = CanvasHeight;
                    map.LoadEditMap(MapRegulate.UTCTime);
                    TabAgvMoveInfo(MapRegulate.UTCTime);
                    AgvInfo();
                    LoadComInfo(MapRegulate.UTCTime);
                }
                else
                {
                    throw new Exception("Maps don't exist！");
                }
            }
            else
            {
                throw new Exception("Default map configuration missing!");
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
