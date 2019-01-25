using AGVSystem.APP.agv_Map;
using AGVSystem.Infrastructure.agvCommon;
using AGVSystem.Model.Ga_agvModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AGVSystem.UI.APP_UI.Map
{
    /// <summary>
    /// MapEdit.xaml 的交互逻辑
    /// </summary>
    public partial class MapEdit : Window
    {
        Ga_Map GetMap = new Ga_Map();
        Painting GetPainting = new Painting();
        Point jos = new Point();

        MapInstrument instrument = new MapInstrument();
        double CanvasWidth, CanvasHeight;

        public MapEdit(Ga_Map ga_Map)
        {
            InitializeComponent();
            GetMap = ga_Map;
            LoadMap();
            instrument.MapSise = 2;
            instrument.LoadEditMap(UTC.ConvertDateTimeLong(Convert.ToDateTime(GetMap.CreateTime)), CanvasWidth, CanvasHeight,true);
        }

        /// <summary>
        /// 载入数据
        /// </summary>
        public void LoadMap()
        {
            this.Title = "地图编辑-" + GetMap.Name;
            //CanvasWidth = GetMap.Width * 10 > this.Width * 2 ? GetMap.Width *10 : this.Width * 2;
            //CanvasHeight = GetMap.Height * 10 > this.Height * 2 ? GetMap.Height *10: this.Height * 2;
            CanvasWidth = GetMap.Width * 10;
            CanvasHeight = GetMap.Height * 10 ;
            TopX.Width = CanvasWidth;
            TopY.Height = CanvasHeight;
            mainPanel.Width = CanvasWidth;
            mainPanel.Height = CanvasHeight;
            instrument.GetCanvas = mainPanel;
            //GetPainting.CoordinateX(TopX, TopY); //绘制X轴Y轴刻度
            GetPainting.Coordinate(mainPanel); //绘制画布
        }

        /// <summary>
        /// 生成Tag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sign_Click(object sender, RoutedEventArgs e)
        {
          
            instrument.TagNew();
        }

        /// <summary>
        /// 滚动条滚动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SrcCount_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            instrument.point.X = e.HorizontalOffset;
            instrument.point.Y = e.VerticalOffset;
        }

        /// <summary>
        /// 直线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Map_Line_Click(object sender, RoutedEventArgs e)
        {
            instrument.DrawLine();
        }

        /// <summary>
        /// 鼠标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mouse_Click(object sender, RoutedEventArgs e)
        {
            instrument.MousePointer();
        }

        /// <summary>
        /// 折线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Broken_Click(object sender, RoutedEventArgs e)
        {
            instrument.BrokenLine();
        }

        private void Semicircle_Map_Click(object sender, RoutedEventArgs e)
        {
            instrument.Semicircle();
        }

        private void Area_Click(object sender, RoutedEventArgs e)
        {
            instrument.AreaNew();
        }

        private void Shrink_Click(object sender, RoutedEventArgs e)
        {
            instrument.MapSise -= 0.1;
            if (instrument.MapSise < 2)
            {
                instrument.MapSise = 1;
                return;
            }
            //mainPanel.Width = CanvasWidth * instrument.MapSise;
            //mainPanel.Height = CanvasHeight * instrument.MapSise;
            instrument.Mapmagnify(instrument.MapSise, CanvasWidth, CanvasHeight);
        }



        private void Tongs_Tool_Click(object sender, RoutedEventArgs e)
        {
            instrument.ToolBarMap();
        }

        private void Recover_Click(object sender, RoutedEventArgs e)
        {
            instrument.MapSise = 2;
            instrument.Mapmagnify(instrument.MapSise, CanvasWidth, CanvasHeight);
        }

        private void Magnify_Click(object sender, RoutedEventArgs e)
        {
            instrument.MapSise += 0.1;
            if (instrument.MapSise > 5)
            {
                return;
            }
            //mainPanel.Width = CanvasWidth * instrument.MapSise;
            //mainPanel.Height = CanvasHeight * instrument.MapSise;
            instrument.Mapmagnify(instrument.MapSise, CanvasWidth, CanvasHeight);
        }
    }
}
