using System;
using AGVDLL;
using OperateIni;

namespace AGVSystem.Infrastructure.agvCommon
{
  public  class SimAgvInfo
    {
        public static string IniPath = AppDomain.CurrentDomain.BaseDirectory + "\\Agv.ini";
        public static CarStatus simInfo(string i)
        {
            string agvNum = "AGV" + i;
            CarStatus cs = new CarStatus();
            int agvNumber = IniFile.ReadIniData(agvNum, "carNum", "", IniPath).TransformInt();
            if (agvNumber != 0 && Convert.ToInt32(i) == agvNumber)
            {
                cs.carNum = agvNumber;
                cs.IsRunning = (IniFile.ReadIniData(agvNum, "run", "", IniPath) == "1" ? true : false);
                cs.speedNo = int.Parse(IniFile.ReadIniData(agvNum, "speed", "", IniPath));
                cs.agvLineRead = (IniFile.ReadIniData(agvNum, "line", "", IniPath) == "1" ? true : false);
                cs.agvDriverDown = (IniFile.ReadIniData(agvNum, "drive", "", IniPath) == "1" ? true : false);
                cs.agvRunDirection = (IniFile.ReadIniData(agvNum, "dire", "", IniPath) == "0" ? true : false);
                cs.agvHookUP = (IniFile.ReadIniData(agvNum, "hook", "", IniPath) == "1" ? true : false);
                cs.errorSwitch = (IniFile.ReadIniData(agvNum, "errorSwitch", "", IniPath) == "1" ? true : false);
                cs.errorCode = int.Parse(IniFile.ReadIniData(agvNum, "errorCode", "", IniPath));
                cs.rfidStatus = IniFile.ReadIniData(agvNum, "rfid", "", IniPath);
                cs.programNo = int.Parse(IniFile.ReadIniData(agvNum, "program", "", IniPath));
                cs.powerCurrentF = float.Parse(IniFile.ReadIniData(agvNum, "power", "", IniPath));
                cs.agvRouteNo = int.Parse(IniFile.ReadIniData(agvNum, "routeNo", "", IniPath));
            }
            return cs;
        }

        public static void writeRouteNo(string i, string RouteNo)
        {
            string agvNum = "AGV" + i;
            IniFile.WriteIniData(agvNum, "RouteNo", RouteNo, IniPath);
        }

        public static void writeRunning(string i, int run)
        {
            string agvNum = "AGV" + i;
            IniFile.WriteIniData(agvNum, "run", run.ToString(), IniPath);
        }

        public static void writeDire(string i, int dire)
        {
            string agvNum = "AGV" + i;
            IniFile.WriteIniData(agvNum, "dire", dire.ToString(), IniPath);
        }

        public static void writeHook(string i, int hook)
        {
            string agvNum = "AGV" + i;
            IniFile.WriteIniData(agvNum, "hook", hook.ToString(), IniPath);
        }

        public static void writeStatus(string i, int status)
        {
            string agvNum = "AGV" + i;
            IniFile.WriteIniData(agvNum, "Status", status.ToString(), IniPath);
        }

    }
}
