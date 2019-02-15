using System.Windows;
using System.Windows.Controls;

namespace AGVSystem.Infrastructure.agvCommon
{
    public class ControlRegulate
    {
        /// <summary>
        /// 设置文字对齐方式
        /// </summary>
        /// <param name="alignment">对齐方式</param>
        /// <param name="label">控件</param>
        public static void aAlignment(string alignment, Label label)
        {
            if (alignment.Equals("居中对齐") || alignment.Equals("水平居中"))
            {
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.VerticalContentAlignment = VerticalAlignment.Center;
            }
            else if (alignment.Equals("靠左对齐"))
            {
                label.HorizontalContentAlignment = HorizontalAlignment.Left;
                label.VerticalContentAlignment = VerticalAlignment.Center;
            }
            else if (alignment.Equals("靠右对齐"))
            {
                label.HorizontalContentAlignment = HorizontalAlignment.Right;
                label.VerticalContentAlignment = VerticalAlignment.Center;
            }
            else if (alignment.Equals("顶部对齐"))
            {
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.VerticalContentAlignment = VerticalAlignment.Top;
            }
            else if (alignment.Equals("底部对齐"))
            {
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.VerticalContentAlignment = VerticalAlignment.Bottom;
            }
        }


        /// <summary>
        /// 区域文字对齐方式
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static string aAlignment(Label label)
        {
            if (label.HorizontalContentAlignment.Equals(HorizontalAlignment.Center) && label.VerticalContentAlignment.Equals(VerticalAlignment.Center))
            {
                return "居中对齐";
            }
            else if (label.HorizontalContentAlignment.Equals(HorizontalAlignment.Left) && label.VerticalContentAlignment.Equals(VerticalAlignment.Center))
            {
                return "靠左对齐";
            }
            else if (label.HorizontalContentAlignment.Equals(HorizontalAlignment.Right) && label.VerticalContentAlignment.Equals(VerticalAlignment.Center))
            {
                return "靠右对齐";
            }
            else if (label.HorizontalContentAlignment.Equals(HorizontalAlignment.Center) && label.VerticalContentAlignment.Equals(VerticalAlignment.Top))
            {
                return "顶部对齐";
            }
            else if (label.HorizontalContentAlignment.Equals(HorizontalAlignment.Center) && label.VerticalContentAlignment.Equals(VerticalAlignment.Bottom))
            {
                return "底部对齐";
            }
            else
            {
                return "";
            }
        }


    }
}
