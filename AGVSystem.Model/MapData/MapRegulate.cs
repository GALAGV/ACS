using AGVSystem.Model.Ga_agvModels;
using AGVSystem.Model.LogicData;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public static ObservableCollection<Ga_PortInfo> GetPortInfos = new ObservableCollection<Ga_PortInfo>(); //串口数据源

        public static ObservableCollection<NetworkInfo> networkInfos = new ObservableCollection<NetworkInfo>();

        public static ObservableCollection<Ga_agv> GetAgvs = new ObservableCollection<Ga_agv>(); //所有AGV数据源

        public static ObservableCollection<Ga_agvStatus> Ga_AgvStatuses = new ObservableCollection<Ga_agvStatus>(); //默认AGV初始信息

        public static ObservableCollection<Ga_Map> maps = new ObservableCollection<Ga_Map>(); //地图选项列表数据源

        public static ConcurrentDictionary<int, SiteInfo> Site = new ConcurrentDictionary<int, SiteInfo>(); //所有AGV位置


        public static List<Ga_agvStatus> speed = MainInfo.agvSpeed.Select(p => new Ga_agvStatus() { StatusName = p, statusValue = MainInfo.agvSpeed.ToList().IndexOf(p).ToString() }).ToList();

        public static List<Ga_agvStatus> pbs = MainInfo.agvPbs.Select(p => new Ga_agvStatus() { StatusName = p, statusValue = MainInfo.agvPbs.ToList().IndexOf(p).ToString() }).ToList();

        public static List<Ga_agvStatus> turn = MainInfo.agvTurn.Select(p => new Ga_agvStatus() { StatusName = p, statusValue = MainInfo.agvTurn.ToList().IndexOf(p).ToString() }).ToList();

        public static List<Ga_agvStatus> direction = MainInfo.agvDire.Select(p => new Ga_agvStatus() { StatusName = p, statusValue = MainInfo.agvDire.ToList().IndexOf(p).ToString() }).ToList();

        public static List<Ga_agvStatus> hook = MainInfo.agvHook.Select(p => new Ga_agvStatus() { StatusName = p, statusValue = MainInfo.agvHook.ToList().IndexOf(p).ToString() }).ToList();
    }

    public class SiteInfo {

        public Label agvSite { get; set; }

        public System.Windows.Media.Brush agvSiteColor { get; set; }

    }

}
