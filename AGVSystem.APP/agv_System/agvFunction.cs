using System;
using System.Collections.Generic;
using AGVSystem.Model.Ga_agvModels;
using MySql.Data.MySqlClient;
using AGVSystem.BLL.ServiceLogicBLL;
using AGVSystem.Model.LogicData;
using System.IO.Ports;
using System.Windows.Media;

namespace AGVSystem.APP.agv_System
{
    public class agvFunction
    {
        private readonly Ga_agvBLL GetAgvBLL = new Ga_agvBLL();
        private readonly Ga_mapBLL MapBLL = new Ga_mapBLL();

        /// <summary>
        /// AGV信息
        /// </summary>
        /// <returns></returns>
        public List<Ga_agvStatus> AgvInfo()
        {
            return new List<Ga_agvStatus>()
            {
                new Ga_agvStatus { StatusName = "AGV",  StatusValue = "1" , ColorBg = new SolidColorBrush(Colors.Black)},
                new Ga_agvStatus { StatusName = "网络状态", StatusValue = "在线" , ColorBg = new SolidColorBrush(Colors.Green)},
                new Ga_agvStatus { StatusName = "运行准备", StatusValue = "ON" , ColorBg = new SolidColorBrush(Colors.Green) },
                new Ga_agvStatus { StatusName = "驱动状态", StatusValue = "上升"  , ColorBg = new SolidColorBrush(Colors.Red)},
                new Ga_agvStatus { StatusName = "脱轨状态", StatusValue = "正常"  , ColorBg = new SolidColorBrush(Colors.Green)},
                new Ga_agvStatus { StatusName = "出发位置", StatusValue = "5" , ColorBg = new SolidColorBrush(Colors.Black)},
                new Ga_agvStatus { StatusName = "目的位置", StatusValue = "4" , ColorBg = new SolidColorBrush(Colors.Black)},
                new Ga_agvStatus { StatusName = "任务名称", StatusValue = "运输", ColorBg = new SolidColorBrush(Colors.Black) },
                new Ga_agvStatus { StatusName = "报警信息", StatusValue = "正常" , ColorBg = new SolidColorBrush(Colors.Green)},
            };
        }

        /// <summary>
        /// 显示所有AGV信息
        /// </summary>
        /// <param name="Agvlist"></param>
        /// <param name="selAgv"></param>
        /// <returns></returns>
        public List<Ga_agv> AgvInfo(long Time, ref int selAgv)
        {
            MainInfo.agvNo.Clear();
            List<string> Agvlist = GetAgvBLL.AGVNumList(Time);
            List<Ga_agv> Ga_agvNum = new List<Ga_agv>();
            for (int i = 0; i < Agvlist.Count; i++)
            {
                Ga_agvNum.Add(
                       new Ga_agv()
                       {
                           agvNumber = Convert.ToInt32(Agvlist[i]),
                           agvStatic = "离线",
                           Dir = "",
                           Hook = "",
                           PBS = "",
                           Program = "",
                           RFID = "",
                           RouteNo = "",
                           Speed = "",
                           Step = "",
                           turn = "",
                           Voltage = "",
                           ColorBg = new SolidColorBrush(Colors.Red)
                       });
                MainInfo.agvNo.Add(Agvlist[i]);
                if (i.Equals(0))
                    selAgv = Convert.ToInt32(Agvlist[0]);
            }
            return Ga_agvNum;
        }

        /// <summary>
        /// 显示所有串口信息
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>
        public List<Ga_PortInfo> agvGather(long Time)
        {
            List<Ga_PortInfo> portInfos = new List<Ga_PortInfo>();
            MySqlDataReader Read = MapBLL.ListDevice(Time);
            while (Read.Read())
            {
                string Port_type = string.Empty;
                if (Read["Agv"].ToString() == "Button")
                {
                    PortInfo.buttonPort.Add(new SerialPort());
                    PortInfo.buttonCom.Add(Convert.ToInt32(Read["Com"].ToString()));
                    PortInfo.buttonBaud.Add(Convert.ToInt32(Read["Baud"].ToString()));
                    PortInfo.buttonStr.Add("Button");
                    Port_type = "按钮";
                }
                else if (Read["Agv"].ToString() == "Charge")
                {
                    PortInfo.chargePort.Add(new SerialPort());
                    PortInfo.chargeCom.Add(Convert.ToInt32(Read["Com"].ToString()));
                    PortInfo.chargeBaud.Add(Convert.ToInt32(Read["Baud"].ToString()));
                    PortInfo.chargeStr.Add("Charge");
                    Port_type = "充电机";
                }
                else if (Read["Agv"].ToString() == "Plc")
                {
                    PortInfo.chargePort.Add(new SerialPort());
                    PortInfo.plcCom.Add(Convert.ToInt32(Read["Com"].ToString()));
                    PortInfo.plcBaud.Add(Convert.ToInt32(Read["Baud"].ToString()));
                    PortInfo.plcStr.Add("Plc");
                    Port_type = "PLC";
                }
                else
                {
                    PortInfo.AGVCom.Add(Convert.ToInt32(Read["Com"].ToString()));
                    PortInfo.Baud.Add(Convert.ToInt32(Read["Baud"].ToString()));
                    PortInfo.agv.Add((Read["Agv"].ToString()));
                    Port_type = "AGV";
                }
                portInfos.Add(
                    new Ga_PortInfo()
                    {
                        ComNumber = "COM" + Read["Com"].ToString().Trim(),
                        ComPortType = Port_type,
                        ComStatic = "关闭"
                    });
            }
            Read.Close();
            return portInfos;
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        public void OpenPort()
        {


        }
    }
}
