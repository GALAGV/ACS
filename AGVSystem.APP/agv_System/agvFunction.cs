using AGVSystem.IService.IO_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using AGVSystem.Model;
using AGVSystem.IService.IO_BLL;
using AGVSystem.BLL;
using AGVSystem.Model.Ga_agvModels;
using MySql.Data.MySqlClient;
using AGVSystem.BLL.ServiceLogicBLL;
using AGVSystem.Model.LogicData;

namespace AGVSystem.APP.agv_System
{
    public class agvFunction : IO_AGVmanagement
    {
        IO_agvBLL GetAgvBLL = new Ga_agvBLL();
        IO_MapBLL MapBLL = new Ga_mapBLL();

        /// <summary>
        /// AGV信息
        /// </summary>
        /// <returns></returns>
        public List<Ga_agvStatus> AgvInfo()
        {
            return new List<Ga_agvStatus>()
            {
                new Ga_agvStatus { StatusName = "AGV",  StatusValue = "1" },
                new Ga_agvStatus { StatusName = "网络状态", StatusValue = "在线" },
                new Ga_agvStatus { StatusName = "运行准备", StatusValue = "ON" },
                new Ga_agvStatus { StatusName = "驱动状态", StatusValue = "上升" },
                new Ga_agvStatus { StatusName = "脱轨状态", StatusValue = "正常" },
                new Ga_agvStatus { StatusName = "出发位置", StatusValue = "5" },
                new Ga_agvStatus { StatusName = "目的位置", StatusValue = "4" },
                new Ga_agvStatus { StatusName = "任务名称", StatusValue = "运输" },
                new Ga_agvStatus { StatusName = "报警信息", StatusValue = "正常" },
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
            List<int> Agvlist = GetAgvBLL.AGVNumList(Time);
            List<Ga_agv> Ga_agvNum = new List<Ga_agv>();
            for (int i = 0; i < Agvlist.Count; i++)
            {
                Ga_agvNum.Add(
                       new Ga_agv()
                       {
                           agvNumber = Agvlist[i],
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
                       });
                MainInfo.agvNo.Add(Agvlist[i]);
                if (i.Equals(0))
                    selAgv = Agvlist[0];
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
                portInfos.Add(
                    new Ga_PortInfo()
                    {
                        ComNumber = Convert.ToInt32(Read["Com"].ToString().Trim()),
                        ComStatic = "离线"
                    });
                if (Read["Agv"].ToString() == "Button")
                {
                    //PortInfo.buttonPort.Add(new SerialPort());
                    PortInfo.buttonCom.Add(Convert.ToInt32(Read["Com"].ToString()));
                    PortInfo.buttonBaud.Add(Convert.ToInt32(Read["Baud"].ToString()));
                    PortInfo.buttonStr.Add("Button");
                }
                else if (Read["Agv"].ToString() == "Charge")
                {
                    //PortInfo.chargePort.Add(new SerialPort());
                    PortInfo.chargeCom.Add(Convert.ToInt32(Read["Com"].ToString()));
                    PortInfo.chargeBaud.Add(Convert.ToInt32(Read["Baud"].ToString()));
                    PortInfo.chargeStr.Add("Charge");
                }
                else
                {
                    PortInfo.AGVCom.Add(Convert.ToInt32(Read["Com"].ToString()));
                    PortInfo.Baud.Add(Convert.ToInt32(Read["Baud"].ToString()));
                    PortInfo.agv.Add((Read["Agv"].ToString()));
                }
            }
            Read.Close();
            return portInfos;
        }

        



    }
}
