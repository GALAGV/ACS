using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Windows.Media;

namespace AGVSystem.Model.LogicData
{
    public class MainInfo
    {
        public static List<string> agvNo = new List<string>();

        public static int prity = 1;//校验位： 0.无校验 1.奇校验 2.偶校验
        public static int stopBits = 0;//停止位： 0.1位 1.1.5位 2.2位

        public static int buttonPrity = 0;//校验位： 0.无校验 1.奇校验 2.偶校验
        public static int buttonStopBits = 0;//停止位： 0.1位 1.1.5位 2.2位

        public static string[] agvSpeed = { "0: 5", "1: 10", "2: 15", "3: 20", "4: 25", "5: 30", "6: 35", "7: 40", "8: 50", "9: 30", "无变化" };
        public static string[] agvPbs = { "区域0", "区域1", "区域2", "区域3", "区域4", "区域5", "区域6", "区域7", "区域8", "区域9", "区域10", "区域11", "区域12", "区域13", "区域14", "区域15", "无变化" };
        public static string[] agvDire = { "前进", "后退", "保持" };
        public static string[] agvTurn = { "无转弯", "左转", "右转", "取消转弯" };
        public static string[] agvHook = { "下降", "升起", "保持" };
        public static string[] agvTime = { "0", "-99" };

        public static string[,] rfidRoute;//rfid路径数组

        public static ConcurrentDictionary<string, string> stationRfid = new ConcurrentDictionary<string, string>();//站点对应的RFID
        public static ConcurrentDictionary<string, string> rfidStation = new ConcurrentDictionary<string, string>();//RFID对应站点

        public static Brush[] Brushes = {
            new SolidColorBrush(Colors.Red),
            new SolidColorBrush(Colors.Orange),
            new SolidColorBrush(Color.FromRgb(141, 24, 198)),
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Blue),
            new SolidColorBrush(Colors.Purple),
            new SolidColorBrush(Color.FromRgb(186, 80, 80)),
            new SolidColorBrush(Color.FromRgb(68, 95, 50)),
            new SolidColorBrush(Color.FromRgb(163, 115, 133)),
            new SolidColorBrush(Color.FromRgb(186, 80, 80)),

            new SolidColorBrush(Color.FromRgb(146, 80, 80)),
            new SolidColorBrush(Color.FromRgb(136, 52, 50)),
            new SolidColorBrush(Color.FromRgb(173, 125, 133)),
            new SolidColorBrush(Color.FromRgb(136, 81, 85)),
            new SolidColorBrush(Color.FromRgb(126, 83, 88)),
            new SolidColorBrush(Color.FromRgb(69, 97, 50)),
            new SolidColorBrush(Color.FromRgb(61, 53, 53)),
        };


    }
}
