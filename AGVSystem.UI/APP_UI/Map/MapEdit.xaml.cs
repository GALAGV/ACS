using AGVSystem.APP.agv_Map;
using AGVSystem.Infrastructure.agvCommon;
using AGVSystem.Model.Ga_agvModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using AGVSystem.UI.APP_UI.Edit;

namespace AGVSystem.UI.APP_UI.Map
{
    /// <summary>
    /// MapEdit.xaml 的交互逻辑
    /// </summary>
    public partial class MapEdit : Window
    {
        private Ga_Map GetMap = new Ga_Map();
        private Painting GetPainting = new Painting();
        private MapInstrument instrument = new MapInstrument();
        private double CanvasWidth, CanvasHeight; //初始宽高
        private bool editStatic = true;

        public MapEdit(Ga_Map ga_Map, bool edit)
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
            CanvasWidth = GetMap.Width * 10;
            CanvasHeight = GetMap.Height * 10;
            double CanvasWidths = CanvasWidth * instrument.MapSise;
            double CanvasHeights = CanvasHeight * instrument.MapSise;
            instrument.action = EditTag;
            instrument.AreaAction = EditArea;
            instrument.Initial_Canvas(TopX, TopY, mainPanel, CanvasWidths, CanvasHeights);
            if (edit)
                instrument.LoadEditMap(UTC.ConvertDateTimeLong(Convert.ToDateTime(GetMap.CreateTime)), CanvasWidth, CanvasHeight, true);
            else
                instrument.Canvas_Draw();
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
            //TopX.Children.Clear();
            //TopY.Children.Clear();
            //TopX.Width = CanvasWidth * instrument.MapSise;
            //TopY.Height = CanvasHeight * instrument.MapSise;
            //GetPainting.Scale_X = 10 * instrument.MapSise;
            //GetPainting.Scale_Y = 10 * instrument.MapSise;
            //GetPainting.CoordinateX(); //绘制X轴Y轴刻度
            instrument.Mapmagnify(instrument.MapSise, CanvasWidth, CanvasHeight);
        }

        /// <summary>
        /// 保存地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveMap_Click(object sender, RoutedEventArgs e)
        {
            string UTCTime = UTC.ConvertDateTimeLong(Convert.ToDateTime(GetMap.CreateTime)).ToString();
            if (instrument.MapPreserve(UTCTime, !editStatic, GetMap.Name, CanvasWidth, CanvasHeight))
            {
                if (!editStatic)
                    editStatic = true;

                CachePlant.Remove(UTCTime); //移除缓存
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

        public void SaveCanvas(Window window, Canvas canvas, int dpi, string filename)
        {
            Size size = new Size(canvas.Width, canvas.Height);
            canvas.Measure(size);

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
            (sender as ToolBar).ToolBar_Formatting();

        }

        private void SaveRTBAsPNG(RenderTargetBitmap bmp, string filename)
        {
            var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
            enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));

            using (var stm = System.IO.File.Create(filename))
            {
                enc.Save(stm);
            }
        }

        private void EditTag(Label Tagobj)
        {
            Edit.Tag tag = new Edit.Tag(Tagobj, instrument.MapSise);
            tag.action = instrument.MouseMove;
            tag.exist = Tagexist;
            tag.ClearTag = DeleteTag;
            tag.ClearLine = ClearAssociated;
            tag.ShowDialog();
        }

        private bool Tagexist(int TagPrimary, int TagNum)
        {
            var a = instrument.valuePairs.Where(p => p.Key.Equals(TagNum) && !p.Key.Equals(TagPrimary)).ToList();
            if (instrument.valuePairs.Where(p => p.Key.Equals(TagNum) && !p.Key.Equals(TagPrimary)).ToList().Count() >= 1)
            {
                return true;
            }
            else
            {
                var Tagobj = instrument.valuePairs.FirstOrDefault(x => x.Key.Equals(TagPrimary)).Value;
                Tagobj.Content = TagNum;
                Tagobj.Tag = TagNum;
                instrument.valuePairs.Remove(TagPrimary);
                instrument.valuePairs.Add(TagNum, Tagobj);
                instrument.wirePointArrays.Where(p => p.GetPoint.TagID.Equals(TagPrimary)).ToList().ForEach(x => { x.GetPoint.TagID = TagNum; });
                instrument.wirePointArrays.Where(p => p.GetWirePoint.TagID.Equals(TagPrimary)).ToList().ForEach(x => { x.GetWirePoint.TagID = TagNum; });
                return false;
            }
        }

        private void ClearAssociated(int TagID)
        {
            instrument.wirePointArrays.Where(x => x.GetPoint.TagID.Equals(TagID) || x.GetWirePoint.TagID.Equals(TagID)).ToList().ForEach(x => instrument.CrearS(x, false));
        }


        private void DeleteTag(Label label)
        {
            int Id = Convert.ToInt32(label.Content.ToString());
            var TagArray = instrument.valuePairs.FirstOrDefault(x => x.Key.Equals(Id));
            if (TagArray.Value != null)
            {
                instrument.GetCanvas.Children.Remove(TagArray.Value);
                instrument.valuePairs.Remove(Id);
                instrument.wirePointArrays.Where(p => p.GetPoint.TagID.Equals(Id) || p.GetWirePoint.TagID.Equals(Id)).ToList().ForEach(x => instrument.CrearS(x, true));
            }
        }


        private void EditArea(Label label)
        {
            Area area = new Area(label, instrument.MapSise);
            area.ArDelete = DeleteArea;
            area.ShowDialog();
        }

        private void DeleteArea(int ID)
        {
            var AreaArray = instrument.keyValuePairs.FirstOrDefault(x => x.Key.Equals(ID));
            if (AreaArray.Value != null)
            {
                instrument.keyValuePairs.Remove(AreaArray.Key);
                instrument.GetCanvas.Children.Remove(AreaArray.Value);
            }
        }


    }
}
