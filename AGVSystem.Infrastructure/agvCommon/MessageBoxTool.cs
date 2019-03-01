using System.Windows;

namespace AGVSystem.Infrastructure.agvCommon
{
    public class MessageBoxTool
    {
        public static void Succeed(string Info)
        {
            MessageBox.Show(Info, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void Error(string Info)
        {
            MessageBox.Show(Info, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
