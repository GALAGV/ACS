using System.Windows;
using System.Windows.Controls;

namespace AGVSystem.Infrastructure.agvCommon
{
    public static class ToolBarStyle
    {
        /// <summary>
        /// 格式化ToolBar样式 去除箭头
        /// </summary>
        /// <param name="toolBar"></param>
        public static void ToolBar_Formatting(this ToolBar toolBar)
        {
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

    }
}
