using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;

namespace AGVSystem.Model.DrawMap
{
    /// <summary>
    /// 线路实体类
    /// </summary>
    public class WirePointArray
    {
        /// <summary>
        /// 线路起始位置信息
        /// </summary>
        public WirePoint GetPoint { get; set; }

        /// <summary>
        /// 线路结束点位置信息
        /// </summary>
        public WirePoint GetWirePoint { get; set; }

        /// <summary>
        /// 线路类型
        /// </summary>
        public CircuitType circuitType { get; set; }

        /// <summary>
        /// 绘制方向(顺时针/逆时针)
        /// </summary>
        public int Direction { get; set; }//0,顺时针,1,逆时针

    }

    public class WirePointLine : WirePointArray
    {
        /// <summary>
        /// 直线对象
        /// </summary>
        public Path GetPath { get; set; }

    }
    public class WirePointBroken : WirePointArray
    {
        /// <summary>
        /// 折线对象
        /// </summary>
        public List<Path> Paths { get; set; }//三部分（两直线，一半圆）
    }

    /// <summary>
    /// 绘制点实体类
    /// </summary>
    public class WirePoint
    {
        /// <summary>
        /// Tag编号
        /// </summary>
        public int TagID { get; set; }

        /// <summary>
        /// 对应中心点
        /// </summary>
        public Point SetPoint { get; set; }
    }

    /// <summary>
    /// 操作类型枚举
    /// </summary>
    public enum CircuitType
    {
        /// <summary>
        /// 直线
        /// </summary>
        Line,

        /// <summary>
        /// 折线
        /// </summary>
        Broken,

        /// <summary>
        /// 半圆
        /// </summary>
        Semicircle,


    /// <summary>
    /// 清除线路
    /// </summary>
        Clear,

        /// <summary>
        /// 水平对齐
        /// </summary>
        Align,


        /// <summary>
        /// 垂直对齐
        /// </summary>
        vertical
    }

    public enum Colortype
    {
        FontColor,//字体颜色

        BgColor, //背景颜色

        BrColor, //边框颜色
    }
}
