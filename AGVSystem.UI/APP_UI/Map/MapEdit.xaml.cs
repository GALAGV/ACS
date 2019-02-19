using AGVSystem.APP.agv_Map;
using AGVSystem.Infrastructure.agvCommon;
using AGVSystem.Model.Ga_agvModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AGVSystem.UI.APP_UI.Map
{
    /// <summary>
    /// MapEdit.xaml 的交互逻辑
    /// </summary>
    public partial class MapEdit : Window
    {
        Ga_Map GetMap = new Ga_Map();
        Painting GetPainting = new Painting();
        MapInstrument instrument = new MapInstrument();
        double CanvasWidth, CanvasHeight; //初始宽高
        bool editStatic = true;

        public MapEdit(Ga_Map ga_Map,bool edit)
        {
            InitializeComponent();
            GetMap = ga_Map;
            editStatic = edit;
            LoadMap(edit);

        }

        /// <summary>
        /// 载入数据
        /// </summary>
        public void LoadMap(bool edit)
        {
            this.Title = "地图编辑-" + GetMap.Name;
            //CanvasWidth = GetMap.Width * 10 > this.Width * 2 ? GetMap.Width *10 : this.Width * 2;
            //CanvasHeight = GetMap.Height * 10 > this.Height * 2 ? GetMap.Height *10: this.Height * 2;
            CanvasWidth = GetMap.Width * 10;
            CanvasHeight = GetMap.Height * 10;
            TopX.Width = CanvasWidth * instrument.MapSise;
            TopY.Height = CanvasHeight * instrument.MapSise;
            mainPanel.Width = TopX.Width;
            mainPanel.Height = TopY.Height;
            instrument.GetCanvas = mainPanel;
            TopX.Children.Clear();
            TopY.Children.Clear();
            GetPainting.CoordinateX(TopX, TopY); //绘制X轴Y轴刻度
            if (edit)
                instrument.LoadEditMap(UTC.ConvertDateTimeLong(Convert.ToDateTime(GetMap.CreateTime)), CanvasWidth, CanvasHeight, true);
            else
                GetPainting.Coordinate(mainPanel);
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


            SrcX.ScrollToHorizontalOffset(e.HorizontalOffset);//X轴标尺跟随移动
            SrcY.ScrollToVerticalOffset(e.VerticalOffset); //Y轴标尺等随移动


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

        /// <summary>
        /// 区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Area_Click(object sender, RoutedEventArgs e)
        {
            instrument.AreaNew();
        }

        private void Shrink_Click(object sender, RoutedEventArgs e)
        {
            instrument.MapSise -= 0.2;
            if (instrument.MapSise < 1)
            {
                instrument.MapSise = 1;
                return;
            }
            CanvasMapZoom();
        }

        private void Tongs_Tool_Click(object sender, RoutedEventArgs e)
        {
            instrument.ToolBarMap();
        }

        private void Recover_Click(object sender, RoutedEventArgs e)
        {
            instrument.MapSise = 1;
            CanvasMapZoom();
        }

        private void Magnify_Click(object sender, RoutedEventArgs e)
        {
            instrument.MapSise += 0.2;
            if (instrument.MapSise > 5)
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
            TopX.Children.Clear();
            TopY.Children.Clear();
            TopX.Width = CanvasWidth * instrument.MapSise;
            TopY.Height = CanvasHeight * instrument.MapSise;
            GetPainting.Scale_X = 10 * instrument.MapSise;
            GetPainting.Scale_Y = 10 * instrument.MapSise;
            GetPainting.CoordinateX(TopX, TopY); //绘制X轴Y轴刻度
            instrument.Mapmagnify(instrument.MapSise, CanvasWidth, CanvasHeight);
        }

        /// <summary>
        /// 保存地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveMap_Click(object sender, RoutedEventArgs e)
        {

            if (instrument.MapPreserve(UTC.ConvertDateTimeLong(Convert.ToDateTime(GetMap.CreateTime)).ToString(), !editStatic, GetMap.Name, CanvasWidth, CanvasHeight))
            {
                MessageBox.Show("保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("保存失败", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintImg_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
            {
                dialog.PrintVisual(mainPanel, "Print");
            }
        }

        /// <summary>
        /// 保存为图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Saveimg_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "图片|*.png";
            sfd.FileName = GetMap.Name;
            if (sfd.ShowDialog() == true)
            {
                SaveCanvas(this, this.mainPanel, 96, sfd.FileName);
            }
        }

        public static void SaveCanvas(Window window, Canvas canvas, int dpi, string filename)
        {
            Size size = new Size(canvas.Width, canvas.Height);
            canvas.Measure(size);
            //canvas.Arrange(new Rect(size));

            var rtb = new RenderTargetBitmap(
                (int)canvas.Width, //width
                (int)canvas.Height, //height
                dpi, //dpi x
                dpi, //dpi y
                PixelFormats.Pbgra32 // pixelformat
                );
            rtb.Render(canvas);

            SaveRTBAsPNG(rtb, filename);
        }

        /// <summary>
        /// 水平对齐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Level_Click(object sender, RoutedEventArgs e)
        {
            instrument.align();
        }

        /// <summary>
        /// 垂直对齐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Vertical_Click(object sender, RoutedEventArgs e)
        {
            instrument.alignVe();
        }

        /// <summary>
        /// 清楚线路
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearLine_Click(object sender, RoutedEventArgs e)
        {
            instrument.ClearTen();
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

        private static void SaveRTBAsPNG(RenderTargetBitmap bmp, string filename)
        {
            var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
            enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));

            using (var stm = System.IO.File.Create(filename))
            {
                enc.Save(stm);
            }
        }

    }
}
