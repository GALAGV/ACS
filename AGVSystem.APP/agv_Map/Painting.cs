
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AGVSystem.APP.agv_Map
{
    public class Painting
    {

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
        public Path DrawingSemicircle(Point startPt, Point endPt, Canvas mainPanel, int Sise, Brush brush, double width)
        {
            return GetPath(startPt, endPt, mainPanel, false, Sise, brush, width);
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
        public List<Path> DrawingBroken(Point startPt, Point endPt, Canvas mainPanel, int Sise, Brush brush, double width, double X, double Y)
        {
            List<Path> GetPatjs = new List<Path>();
            double drn = startPt.X - endPt.X;
            double hrn = startPt.Y - endPt.Y;

            if ((drn > 0 && hrn < 0) || (drn < 0 && hrn > 0))
            {
                double diff = startPt.Y - endPt.Y;
                if (diff < 0)
                {
                    Point TX = new Point() { X = endPt.X + X, Y = startPt.Y };
                    Point TY = new Point() { X = endPt.X, Y = startPt.Y + Y };
                    GetPatjs.Add(DrawingLine(endPt, TY, mainPanel, brush, width));
                    GetPatjs.Add(DrawingLine(startPt, TX, mainPanel, brush, width));
                    GetPatjs.Add(GetPath(TX, TY, mainPanel, true, Sise, brush, width));
                }
                else
                {
                    Point TX = new Point() { X = endPt.X - X, Y = startPt.Y };
                    Point TY = new Point() { X = endPt.X, Y = startPt.Y - Y };
                    GetPatjs.Add(DrawingLine(startPt, TX, mainPanel, brush, width));
                    GetPatjs.Add(DrawingLine(endPt, TY, mainPanel, brush, width));
                    GetPatjs.Add(GetPath(TX, TY, mainPanel, true, Sise, brush, width));
                }
            }
            else
            {
                double diff = startPt.Y - endPt.Y;
                if (diff < 0)
                {
                    Point TX = new Point() { X = endPt.X - X, Y = startPt.Y };
                    Point TY = new Point() { X = endPt.X, Y = startPt.Y + Y };
                    GetPatjs.Add(DrawingLine(startPt, TX, mainPanel, brush, width));
                    GetPatjs.Add(DrawingLine(endPt, TY, mainPanel, brush, width));
                    GetPatjs.Add(GetPath(TX, TY, mainPanel, false, Sise, brush, width));
                }
                else
                {
                    Point TX = new Point() { X = endPt.X + X, Y = startPt.Y };
                    Point TY = new Point() { X = endPt.X, Y = startPt.Y - Y };
                    GetPatjs.Add(DrawingLine(endPt, TY, mainPanel, brush, width));
                    GetPatjs.Add(DrawingLine(startPt, TX, mainPanel, brush, width));
                    GetPatjs.Add(GetPath(TX, TY, mainPanel, false, Sise, brush, width));
                }
            }
            return GetPatjs;
        }


        /// <summary>
        /// 绘制地图坐标系
        /// </summary>
        /// <param name="mainPanel"></param>
        public void Coordinate(Canvas mainPanel, double x, double y, Brush brush, double width)
        {
            for (int i = 0; i <= (mainPanel.Height / x); i++)
            {
                DrawingLine(new Point(0, x * i), new Point(mainPanel.Width, y * i), mainPanel, brush, width);
            }
            for (int i = 0; i <= (mainPanel.Width / x); i++)
            {
                DrawingLine(new Point(x * i, 0), new Point(x * i, mainPanel.Height), mainPanel, brush, width);
            }
        }




        /// <summary>
        /// 绘制X轴Y轴刻度
        /// </summary>
        /// <param name="mainPanel"></param>
        /// <param name="mainPane2"></param>
        public void CoordinateX(Canvas mainPanel, Canvas mainPane2, double x, double y, Brush brush, double width)
        {

            for (int i = 0; i <= (mainPanel.Width / x); i++)
            {
                if (i.Equals(0))
                {
                    continue;
                }
                if (i % 10 == 0)
                {
                    DrawingLine(new Point(x * i, 3), new Point(x * i, mainPanel.Height), mainPanel, brush, width);
                    continue;
                }
                if (i % 5 == 0)
                {
                    DrawingLine(new Point(x * i, 8), new Point(x * i, mainPanel.Height), mainPanel, brush, width);
                    continue;
                }
                else
                {
                    DrawingLine(new Point(x * i, 12), new Point(x * i, mainPanel.Height), mainPanel, brush, width);
                }

            }
            DrawingLine(new Point(0, mainPanel.Height - 1), new Point(mainPanel.Width, mainPanel.Height - 1), mainPanel, Brushes.Black, 2);
            for (int i = 0; i <= (mainPane2.Height / x); i++)
            {
                if (i.Equals(0))
                {
                    continue;
                }
                if (i % 10 == 0)
                {
                    DrawingLine(new Point(3, x * i), new Point(mainPane2.Width, y * i), mainPane2, brush, width);
                    continue;
                }
                if (i % 5 == 0)
                {
                    DrawingLine(new Point(8, x * i), new Point(mainPane2.Width, y * i), mainPane2, brush, width);
                    continue;
                }
                else
                {
                    DrawingLine(new Point(12, x * i), new Point(mainPane2.Width, y * i), mainPane2, brush, width);
                }
            }
            DrawingLine(new Point(mainPane2.Width - 1, 0), new Point(mainPane2.Width - 1, mainPane2.Height), mainPane2, Brushes.Black, 2);
        }
    }
}
