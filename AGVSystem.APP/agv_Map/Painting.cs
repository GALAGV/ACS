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
        public int Line_Width { get; set; }

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

        #region 初始化参数
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


            this.Line_Width = 2;
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
        public Path Line(Point startPt, Point endPt, Canvas mainPanel)
        {
            return DrawingLine(startPt, endPt, mainPanel, LineBackground_Color, Line_Width);
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
        public Path DrawingLine(Point startPt, Point endPt, Canvas mainPanel, Brush brush, double width)
        {
            LineGeometry myLineGeometry = new LineGeometry();//实例化一条直线
            myLineGeometry.StartPoint = startPt;//设置起点
            myLineGeometry.EndPoint = endPt;//设置终点

            Path myPath = new Path();
            myPath.Stroke = brush;//设置颜色
            myPath.StrokeThickness = width; //设置宽度
            myPath.Data = myLineGeometry;//设置绘制形状
            mainPanel.Children.Add(myPath);//绑定数据
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
        public Path DrawingSemicircle(Point startPt, Point endPt, Canvas mainPanel)
        {
            return GetPath(startPt, endPt, mainPanel, false, AngleLine, LineBackground_Color, Line_Width);
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
        public Path GetPath(Point startPt, Point endPt, Canvas mainPanel, bool Static, int Sise, Brush brush, double width)
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
            mainPanel.Children.Add(path);
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
        public List<Path> DrawingBroken(Point startPt, Point endPt, Canvas mainPanel)
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
                    GetPatjs.Add(Line(endPt, TY, mainPanel));
                    GetPatjs.Add(Line(startPt, TX, mainPanel));
                    GetPatjs.Add(GetPath(TX, TY, mainPanel, true, AngleLine, LineBackground_Color, Line_Width));
                }
                else
                {
                    Point TX = new Point() { X = endPt.X - Excursion_X, Y = startPt.Y };
                    Point TY = new Point() { X = endPt.X, Y = startPt.Y - Excursion_Y };
                    GetPatjs.Add(Line(startPt, TX, mainPanel));
                    GetPatjs.Add(Line(endPt, TY, mainPanel));
                    GetPatjs.Add(GetPath(TX, TY, mainPanel, true, AngleLine, LineBackground_Color, Line_Width));
                }
            }
            else
            {
                double diff = startPt.Y - endPt.Y;
                if (diff < 0)
                {
                    Point TX = new Point() { X = endPt.X - Excursion_X, Y = startPt.Y };
                    Point TY = new Point() { X = endPt.X, Y = startPt.Y + Excursion_Y };
                    GetPatjs.Add(Line(startPt, TX, mainPanel));
                    GetPatjs.Add(Line(endPt, TY, mainPanel));
                    GetPatjs.Add(GetPath(TX, TY, mainPanel, false, AngleLine, LineBackground_Color, Line_Width));
                }
                else
                {
                    Point TX = new Point() { X = endPt.X + Excursion_X, Y = startPt.Y };
                    Point TY = new Point() { X = endPt.X, Y = startPt.Y - Excursion_Y };
                    GetPatjs.Add(Line(endPt, TY, mainPanel));
                    GetPatjs.Add(Line(startPt, TX, mainPanel));
                    GetPatjs.Add(GetPath(TX, TY, mainPanel, false, AngleLine, LineBackground_Color, Line_Width));
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
        public void Coordinate(Canvas mainPanel)
        {
            for (int i = 0; i <= (mainPanel.Height / Canvas_X); i++)
            {
                DrawingLine(new Point(0, Canvas_X * i), new Point(mainPanel.Width, Canvas_Y * i), mainPanel, CanvasBackground_Color, CanvasLine_Width);
            }
            for (int i = 0; i <= (mainPanel.Width / Canvas_X); i++)
            {
                DrawingLine(new Point(Canvas_X * i, 0), new Point(Canvas_X * i, mainPanel.Height), mainPanel, CanvasBackground_Color, CanvasLine_Width);
            }
        }
        #endregion

        #region 绘制刻度

        /// <summary>
        /// 绘制X轴Y轴刻度
        /// </summary>
        /// <param name="mainPanel"></param>
        /// <param name="mainPane2"></param>
        public void CoordinateX(Canvas mainPanel, Canvas mainPane2)
        {
            for (int i = 0; i <= (mainPanel.Width / Scale_X); i++)
            {
                if (i.Equals(0))
                {
                    continue;
                }
                if (i % 10 == 0)
                {
                    DrawingLine(new Point(Scale_X * i, 3), new Point(Scale_X * i, mainPanel.Height), mainPanel, ScaleBackground_Color, ScaleLine_Width);
                    continue;
                }
                if (i % 5 == 0)
                {
                    DrawingLine(new Point(Scale_X * i, 8), new Point(Scale_X * i, mainPanel.Height), mainPanel, ScaleBackground_Color, ScaleLine_Width);
                    continue;
                }
                else
                {
                    DrawingLine(new Point(Scale_X * i, 12), new Point(Scale_X * i, mainPanel.Height), mainPanel, ScaleBackground_Color, ScaleLine_Width);
                }

            }
            DrawingLine(new Point(0, mainPanel.Height - 1), new Point(mainPanel.Width, mainPanel.Height - 1), mainPanel, ScaleBackground_Color, 2);
            for (int i = 0; i <= (mainPane2.Height / Scale_X); i++)
            {
                if (i.Equals(0))
                {
                    continue;
                }
                if (i % 10 == 0)
                {
                    DrawingLine(new Point(3, Scale_X * i), new Point(mainPane2.Width, Scale_Y * i), mainPane2, ScaleBackground_Color, ScaleLine_Width);
                    continue;
                }
                if (i % 5 == 0)
                {
                    DrawingLine(new Point(8, Scale_X * i), new Point(mainPane2.Width, Scale_Y * i), mainPane2, ScaleBackground_Color, ScaleLine_Width);
                    continue;
                }
                else
                {
                    DrawingLine(new Point(12, Scale_X * i), new Point(mainPane2.Width, Scale_Y * i), mainPane2, ScaleBackground_Color, ScaleLine_Width);
                }
            }
            DrawingLine(new Point(mainPane2.Width - 1, 0), new Point(mainPane2.Width - 1, mainPane2.Height), mainPane2, ScaleBackground_Color, 2);
        }

        #endregion
    }
}
