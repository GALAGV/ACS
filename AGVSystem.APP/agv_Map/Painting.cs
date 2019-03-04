using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AGVSystem.APP.agv_Map
{
    public class Painting
    {

        #region 画布参数

        /// <summary>
        /// 画布间隔长度
        /// </summary>
        public double Canvas_X { get; set; }

        /// <summary>
        /// 画布间隔高度
        /// </summary>
        public double Canvas_Y { get; set; }

        /// <summary>
        /// 画布背景颜色
        /// </summary>
        public Brush CanvasBackground_Color { get; set; }


        #endregion

        #region 刻度参数

        /// <summary>
        /// 画布绘制宽度
        /// </summary>
        public int CanvasLine_Width { get; set; }

        /// <summary>
        /// 刻度间隔长度
        /// </summary>
        public double Scale_X { get; set; }

        /// <summary>
        /// 刻度间隔高度
        /// </summary>
        public double Scale_Y { get; set; }

        /// <summary>
        /// 刻度背景颜色
        /// </summary>
        public Brush ScaleBackground_Color { get; set; }

        /// <summary>
        /// 刻度绘制宽度
        /// </summary>
        public int ScaleLine_Width { get; set; }


        #endregion

        #region 线路参数

        /// <summary>
        /// 线路颜色
        /// </summary>
        public Brush LineBackground_Color { get; set; }

        /// <summary>
        /// 线路宽度
        /// </summary>
        public double Line_Width { get; set; }

        /// <summary>
        /// 半圆角度
        /// </summary>
        public int AngleLine { get; set; }

        /// <summary>
        ///偏移X
        /// </summary>
        public int Excursion_X { get; set; }

        /// <summary>
        /// 偏移Y
        /// </summary>
        public int Excursion_Y { get; set; }


        #endregion

        #region 刻度容器

       /// <summary>
       /// X轴刻度容器
       /// </summary>
        private Canvas mainPaneX { get; set; }

        /// <summary>
        /// Y轴刻度容器
        /// </summary>
        private Canvas mainPaneY { get; set; }

        /// <summary>
        /// 主容器
        /// </summary>
        private Canvas CanvasMain { get; set; }

        #endregion

        #region 初始化容器

        public void InitializeCanvas(Canvas mainPane_X, Canvas mainPane_Y, Canvas Canvas_Main, double Width, double Height)
        {
            this.mainPaneX = mainPane_X;
            this.mainPaneY = mainPane_Y;
            this.CanvasMain = Canvas_Main;
            this.Change_Size(Width, Height);
        }

        /// <summary>
        /// 改变容器大小
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public void Change_Size(double Width, double Height)
        {
            this.mainPaneX.Width = Width;
            this.mainPaneY.Height = Height;
            this.CanvasMain.Width = Width;
            this.CanvasMain.Height = Height;
        }

        #endregion

        #region 构造函数
        public Painting()
        {
            this.Scale_X = 10;
            this.Scale_Y = 10;
            this.ScaleBackground_Color = Brushes.Black;
            this.ScaleLine_Width = 1;


            this.Canvas_X = 10;
            this.Canvas_Y = 10;
            this.CanvasLine_Width = 1;
            this.CanvasBackground_Color = new SolidColorBrush(Color.FromRgb(208, 208, 208));


            this.Line_Width = 1.7;
            this.AngleLine = 18;
            this.Excursion_X = 19;
            this.Excursion_Y = 18;
            this.LineBackground_Color = Brushes.Black;
        }
        #endregion

        #region 画直线

        /// <summary>
        /// 画直线
        /// </summary>
        /// <param name="startPt"></param>
        /// <param name="endPt"></param>
        /// <param name="mainPanel"></param>
        /// <returns></returns>
        public Path Line(Point startPt, Point endPt)
        {
            return DrawingLine(startPt, endPt, LineBackground_Color, Line_Width, CanvasMain);
        }

        /// <summary>
        /// 绘制直线
        /// </summary>
        /// <param name="startPt">起始点</param>
        /// <param name="endPt">结束点</param>
        /// <param name="mainPanel">绘制容器</param>
        /// <param name="brush">绘制颜色</param>
        /// <param name="width">绘制宽度</param>
        /// <returns></returns>
        public Path DrawingLine(Point startPt, Point endPt, Brush brush, double width, Canvas canvas)
        {
            LineGeometry myLineGeometry = new LineGeometry();//实例化一条直线
            myLineGeometry.StartPoint = startPt;//设置起点
            myLineGeometry.EndPoint = endPt;//设置终点

            Path myPath = new Path();
            myPath.Stroke = brush;//设置颜色
            myPath.StrokeThickness = width; //设置宽度
            myPath.Data = myLineGeometry;//设置绘制形状
            canvas.Children.Add(myPath);//绑定数据
            return myPath;
        }
        #endregion

        #region 画半圆

      
        /// <summary>
        /// 绘制半圆线路
        /// </summary>
        /// <param name="startPt">起始点</param>
        /// <param name="endPt">结束点</param>
        /// <param name="mainPanel">绘制容器</param>
        /// <param name="Sise">角度比例</param>
        /// <param name="brush">绘制颜色</param>
        /// <param name="width">宽度</param>
        /// <returns></returns>
        public Path DrawingSemicircle(Point startPt, Point endPt)
        {
            return GetPath(startPt, endPt, false, AngleLine, LineBackground_Color, Line_Width);
        }

        /// <summary>
        /// 半圆
        /// </summary>
        /// <param name="startPt"></param>
        /// <param name="endPt"></param>
        /// <param name="mainPanel"></param>
        /// <param name="Static"></param>
        /// <param name="Sise"></param>
        /// <param name="brush">绘制颜色</param>
        /// <param name="width">宽度</param>
        /// <returns></returns>
        public Path GetPath(Point startPt, Point endPt, bool Static, int Sise, Brush brush, double width)
        {
            Path path = new Path();
            PathGeometry pathGeometry = new PathGeometry();
            ArcSegment arc = new ArcSegment(startPt, new Size(Sise, Sise), 0, false, Static ? SweepDirection.Clockwise : SweepDirection.Counterclockwise, true);
            PathFigure figure = new PathFigure();
            figure.StartPoint = endPt;
            figure.Segments.Add(arc);
            pathGeometry.Figures.Add(figure);
            path.Data = pathGeometry;
            path.Stroke = brush;
            path.StrokeThickness = width; //设置宽度
            CanvasMain.Children.Add(path);
            return path;
        }
        #endregion

        #region 画折线

        /// <summary>
        /// 绘制折线线路
        /// </summary>
        /// <param name="startPt"></param>
        /// <param name="endPt"></param>
        /// <param name="mainPanel"></param>
        /// <param name="Static"></param>
        /// <param name="Sise"></param>
        /// <param name="brush">绘制颜色</param>
        /// <param name="width">宽度</param>
        /// <param name="X">偏移X</param>
        /// <param name="Y">偏移Y</param>
        /// <returns></returns>
        public List<Path> DrawingBroken(Point startPt, Point endPt)
        {
            List<Path> GetPatjs = new List<Path>();
            double drn = startPt.X - endPt.X;
            double hrn = startPt.Y - endPt.Y;

            if ((drn > 0 && hrn < 0) || (drn < 0 && hrn > 0))
            {
                double diff = startPt.Y - endPt.Y;
                if (diff < 0)
                {
                    Point TX = new Point() { X = endPt.X + Excursion_X, Y = startPt.Y };
                    Point TY = new Point() { X = endPt.X, Y = startPt.Y + Excursion_Y };
                    GetPatjs.Add(Line(endPt, TY));
                    GetPatjs.Add(Line(startPt, TX));
                    GetPatjs.Add(GetPath(TX, TY, true, AngleLine, LineBackground_Color, Line_Width));
                }
                else
                {
                    Point TX = new Point() { X = endPt.X - Excursion_X, Y = startPt.Y };
                    Point TY = new Point() { X = endPt.X, Y = startPt.Y - Excursion_Y };
                    GetPatjs.Add(Line(startPt, TX));
                    GetPatjs.Add(Line(endPt, TY));
                    GetPatjs.Add(GetPath(TX, TY, true, AngleLine, LineBackground_Color, Line_Width));
                }
            }
            else
            {
                double diff = startPt.Y - endPt.Y;
                if (diff < 0)
                {
                    Point TX = new Point() { X = endPt.X - Excursion_X, Y = startPt.Y };
                    Point TY = new Point() { X = endPt.X, Y = startPt.Y + Excursion_Y };
                    GetPatjs.Add(Line(startPt, TX));
                    GetPatjs.Add(Line(endPt, TY));
                    GetPatjs.Add(GetPath(TX, TY, false, AngleLine, LineBackground_Color, Line_Width));
                }
                else
                {
                    Point TX = new Point() { X = endPt.X + Excursion_X, Y = startPt.Y };
                    Point TY = new Point() { X = endPt.X, Y = startPt.Y - Excursion_Y };
                    GetPatjs.Add(Line(endPt, TY));
                    GetPatjs.Add(Line(startPt, TX));
                    GetPatjs.Add(GetPath(TX, TY, false, AngleLine, LineBackground_Color, Line_Width));
                }
            }
            return GetPatjs;
        }

        #endregion

        #region 绘制画布

        /// <summary>
        /// 绘制地图坐标系
        /// </summary>
        /// <param name="mainPanel"></param>
        public void Coordinate()
        {
            for (int i = 0; i <= (CanvasMain.Height / Canvas_X); i++)
            {
                DrawingLine(new Point(0, Canvas_X * i), new Point(CanvasMain.Width, Canvas_Y * i), CanvasBackground_Color, CanvasLine_Width, CanvasMain);
            }
            for (int i = 0; i <= (CanvasMain.Width / Canvas_X); i++)
            {
                DrawingLine(new Point(Canvas_X * i, 0), new Point(Canvas_X * i, CanvasMain.Height), CanvasBackground_Color, CanvasLine_Width, CanvasMain);
            }
        }
        #endregion

        #region 绘制刻度

        /// <summary>
        /// 绘制X轴Y轴刻度
        /// </summary>
        /// <param name="mainPanel"></param>
        /// <param name="mainPane2"></param>
        public void CoordinateX()
        {
            this.mainPaneX.Children.Clear();
            this.mainPaneY.Children.Clear();
            for (int i = 0; i <= (mainPaneX.Width / Scale_X); i++)
            {
                if (i.Equals(0))
                {
                    continue;
                }
                if (i % 10 == 0)
                {
                    DrawingLine(new Point(Scale_X * i, 3), new Point(Scale_X * i, mainPaneX.Height), ScaleBackground_Color, ScaleLine_Width, mainPaneX);
                    continue;
                }
                if (i % 5 == 0)
                {
                    DrawingLine(new Point(Scale_X * i, 8), new Point(Scale_X * i, mainPaneX.Height), ScaleBackground_Color, ScaleLine_Width, mainPaneX);
                    continue;
                }
                else
                {
                    DrawingLine(new Point(Scale_X * i, 12), new Point(Scale_X * i, mainPaneX.Height), ScaleBackground_Color, ScaleLine_Width, mainPaneX);
                }

            }
            DrawingLine(new Point(0, mainPaneX.Height - 1), new Point(mainPaneX.Width, mainPaneX.Height - 1), ScaleBackground_Color, 2, mainPaneX);
            for (int i = 0; i <= (mainPaneY.Height / Scale_X); i++)
            {
                if (i.Equals(0))
                {
                    continue;
                }
                if (i % 10 == 0)
                {
                    DrawingLine(new Point(3, Scale_X * i), new Point(mainPaneY.Width, Scale_Y * i), ScaleBackground_Color, ScaleLine_Width, mainPaneY);
                    continue;
                }
                if (i % 5 == 0)
                {
                    DrawingLine(new Point(8, Scale_X * i), new Point(mainPaneY.Width, Scale_Y * i), ScaleBackground_Color, ScaleLine_Width, mainPaneY);
                    continue;
                }
                else
                {
                    DrawingLine(new Point(12, Scale_X * i), new Point(mainPaneY.Width, Scale_Y * i), ScaleBackground_Color, ScaleLine_Width, mainPaneY);
                }
            }
            DrawingLine(new Point(mainPaneY.Width - 1, 0), new Point(mainPaneY.Width - 1, mainPaneY.Height), ScaleBackground_Color, 2, mainPaneY);
        }
        #endregion

    }
}
