using AGVDLL;
using AGVSystem.Model.Ga_agvModels;
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace AGVSystem.Model.LogicData
{
    public class PortInfo
    {
        public static List<AgvPortInfo> AgvPortsList = new List<AgvPortInfo>();//AGV串口集合

        public static List<DevicePortInfo> DePortsList = new List<DevicePortInfo>(); //按钮，LPC，充电机串口集合

        public static List<NetworkInfo> Networks = new List<NetworkInfo>(); //网络集合
    }

    public class BasePort  //串口基类
    {
        /// <summary>
        /// 串口编号
        /// </summary>
        public int ComNumber { get; set; }

        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; set; }
    }


    public class AgvPortInfo : BasePort
    {
        public IntPtr IntPtr { get; set; }

        /// <summary>
        /// Agv车号
        /// </summary>
        public string AgvStr { get; set; }

        /// <summary>
        /// agvdll 动态调用
        /// </summary>
        public AGVDLL.AGVDLL AgvDll { get; set; }
    }

    public class DevicePortInfo : BasePort
    {
        public SerialPort Port { get; set; }

        public PortType PortType { get; set; }
    }

    public class RFIDLineList
    {
        public List<RFIDClass> GetRFIDs { get; set; }  //RFIDClass集合

        public int Program { get; set; }  //线路好
    }
}
