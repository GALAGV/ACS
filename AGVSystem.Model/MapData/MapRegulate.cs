using AGVSystem.Model.Ga_agvModels;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Controls;

namespace AGVSystem.Model.MapData
{
    public class MapRegulate
    {
        public static int selAgv = 1; //默认显示AGV

        public static long UTCTime = 0; //当前UTC

        public static long DefaultMap = 0; //默认UTC

        public static string TemplateName = string.Empty; //线路名称规则

        public static int DirectionLine = 0; //线路方向自动计算 0默认 1开启

        public static ConcurrentDictionary<int, Image> GetValuePairs = new ConcurrentDictionary<int, Image>(); //AGV状态Img列表

        public static List<Ga_PortInfo> GetPortInfos = new List<Ga_PortInfo>(); //串口数据源

        public static List<NetworkInfo> networkInfos = new List<NetworkInfo>();

        public static List<Ga_agv> GetAgvs = new List<Ga_agv>(); //所有AGV数据源

        public static List<Ga_agvStatus> Ga_AgvStatuses = new List<Ga_agvStatus>(); //默认AGV初始信息
    }
}
