using System;
using System.Collections.Generic;
using AGVSystem.Model.Ga_agvModels;
using MySql.Data.MySqlClient;
using AGVSystem.BLL.ServiceLogicBLL;
using AGVSystem.Model.LogicData;
using System.IO.Ports;
using System.Windows.Media;
using System.Linq;
using AGVSystem.Infrastructure.agvCommon;
using System.Text;
using System.IO;
using System.Collections.Concurrent;
using AGVDLL;
using System.Collections.ObjectModel;
using AGVSystem.Model.MapData;
using System.Windows.Controls;
using System.Data;

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
        public ObservableCollection<Ga_agvStatus> AgvInfo()
        {
            return new ObservableCollection<Ga_agvStatus>()
            {
                new Ga_agvStatus { StatusName = "AGV",  StatusValue = "" , ColorBg = new SolidColorBrush(Colors.Black)},
                new Ga_agvStatus { StatusName = "网络状态", StatusValue = "" , ColorBg = new SolidColorBrush(Colors.Black)},
                new Ga_agvStatus { StatusName = "运行准备", StatusValue = "" , ColorBg = new SolidColorBrush(Colors.Black) },
                new Ga_agvStatus { StatusName = "驱动状态", StatusValue = ""  , ColorBg = new SolidColorBrush(Colors.Black)},
                new Ga_agvStatus { StatusName = "脱轨状态", StatusValue = ""  , ColorBg = new SolidColorBrush(Colors.Black)},
                new Ga_agvStatus { StatusName = "出发位置", StatusValue = "" , ColorBg = new SolidColorBrush(Colors.Black)},
                new Ga_agvStatus { StatusName = "目的位置", StatusValue = "" , ColorBg = new SolidColorBrush(Colors.Black)},
                new Ga_agvStatus { StatusName = "任务名称", StatusValue = "", ColorBg = new SolidColorBrush(Colors.Black) },
                new Ga_agvStatus { StatusName = "报警信息", StatusValue = "" , ColorBg = new SolidColorBrush(Colors.Black)},
            };
        }

        /// <summary>
        /// 显示所有AGV信息
        /// </summary>
        /// <param name="Agvlist"></param>
        /// <param name="selAgv"></param>
        /// <returns></returns>
        public ObservableCollection<Ga_agv> AgvInfo(long Time, ref int selAgv, ref ConcurrentDictionary<int, CarStatus> pairs, ref ConcurrentDictionary<int, AgvInfo> valuePairs)
        {
            MainInfo.agvNo.Clear();
            MapRegulate.Site.Clear();
            valuePairs.Clear();
            pairs.Clear();
            List<string> Agvlist = GetAgvBLL.AGVNumList(Time);
            ObservableCollection<Ga_agv> Ga_agvNum = new ObservableCollection<Ga_agv>();
            Random random = new Random();
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
                           Operation = "",
                           ColorOperation = new SolidColorBrush(Colors.Black),
                           ColorBg = new SolidColorBrush(Colors.Red),
                           Turncolor = new SolidColorBrush(Colors.Black),
                           Dircolor = new SolidColorBrush(Colors.Black)
                       });
                MainInfo.agvNo.Add(Agvlist[i]);
                MapRegulate.Site.GetOrAdd(Convert.ToInt32(Agvlist[i]), new SiteInfo() { agvSite = new Label(), agvSiteColor = MainInfo.Brushes[random.Next(0, 17)] });
                valuePairs.GetOrAdd(Convert.ToInt32(Agvlist[i]), new AgvInfo());
                pairs.GetOrAdd(Convert.ToInt32(Agvlist[i]), new CarStatus() { errorCode = 205, carNum = Convert.ToInt32(Agvlist[i]) });
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
        public ObservableCollection<Ga_PortInfo> agvGather(long Time)
        {
            ObservableCollection<Ga_PortInfo> portInfos = new ObservableCollection<Ga_PortInfo>();
            MySqlDataReader Read = MapBLL.ListDevice(Time);
            PortInfo.AgvPortsList.Clear();
            PortInfo.DePortsList.Clear();
            SerialPort SP = new SerialPort();
            SP.StopBits = StopBits.One;
            SP.Parity = Parity.Odd;
            SP.ReadTimeout = 1000;
            SP.WriteTimeout = 1000;
            while (Read.Read())
            {
                PortType Port_type;
                SP.PortName = "COM" + Read["Com"].ToString();
                if (Read["Agv"].ToString() == "Button")
                {
                    //PortInfo.buttonPort.Add(new SerialPort());
                    //PortInfo.buttonCom.Add(Convert.ToInt32(Read["Com"].ToString()));
                    //PortInfo.buttonBaud.Add(Convert.ToInt32(Read["Baud"].ToString()));
                    //PortInfo.buttonStr.Add("Button");
                    Port_type = PortType.Button;
                    PortInfo.DePortsList.Add(new DevicePortInfo()
                    {
                        ComNumber = Convert.ToInt32(Read["Com"].ToString()),
                        BaudRate = Convert.ToInt32(Read["Baud"].ToString()),
                        Port = SP,
                        PortType = PortType.Button
                    });
                }
                else if (Read["Agv"].ToString() == "Charge")
                {
                    //PortInfo.chargePort.Add(new SerialPort());
                    //PortInfo.chargeCom.Add(Convert.ToInt32(Read["Com"].ToString()));
                    //PortInfo.chargeBaud.Add(Convert.ToInt32(Read["Baud"].ToString()));
                    //PortInfo.chargeStr.Add("Charge");
                    Port_type = PortType.Charge;
                    PortInfo.DePortsList.Add(new DevicePortInfo()
                    {
                        ComNumber = Convert.ToInt32(Read["Com"].ToString()),
                        BaudRate = Convert.ToInt32(Read["Baud"].ToString()),
                        Port = SP,
                        PortType = PortType.Charge
                    });
                }
                else if (Read["Agv"].ToString() == "Plc")
                {
                    //PortInfo.chargePort.Add(new SerialPort());
                    //PortInfo.plcCom.Add(Convert.ToInt32(Read["Com"].ToString()));
                    //PortInfo.plcBaud.Add(Convert.ToInt32(Read["Baud"].ToString()));
                    //PortInfo.plcStr.Add("Plc");
                    Port_type = PortType.PLC;
                    PortInfo.DePortsList.Add(new DevicePortInfo()
                    {
                        ComNumber = Convert.ToInt32(Read["Com"].ToString()),
                        BaudRate = Convert.ToInt32(Read["Baud"].ToString()),
                        Port = SP,
                        PortType = PortType.PLC
                    });
                }
                else
                {
                    //PortInfo.AGVCom.Add(Convert.ToInt32(Read["Com"].ToString()));
                    //PortInfo.Baud.Add(Convert.ToInt32(Read["Baud"].ToString()));
                    //PortInfo.agv.Add((Read["Agv"].ToString()));
                    Port_type = PortType.AGV;

                    PortInfo.AgvPortsList.Add(new AgvPortInfo()
                    {
                        AgvDll = new AGVDLL.AGVDLL(),
                        AgvStr = Read["Agv"].ToString(),
                        BaudRate = Convert.ToInt32(Read["Baud"].ToString()),
                        ComNumber = Convert.ToInt32(Read["Com"].ToString())
                    });
                }
                portInfos.Add(
                    new Ga_PortInfo()
                    {
                        ComNumber = "COM" + Read["Com"].ToString().Trim(),
                        ComPortType = Port_type,
                        ComStatic = "关闭",
                        ComStaticColor = new SolidColorBrush(Colors.Red)
                    });
            }
            Read.Close();
            return portInfos;
        }

        /// <summary>
        /// 加载网络数据
        /// </summary>
        /// <param name="UTCTime"></param>
        /// <returns></returns>
        public ObservableCollection<NetworkInfo> LoadNetwork(long UTCTime)
        {
            PortInfo.Networks.Clear();
            MySqlDataReader reader = MapBLL.SelectNetworkBLL(UTCTime);
            ObservableCollection<NetworkInfo> Net = new ObservableCollection<NetworkInfo>();
            if (reader != null)
            {
                while (reader.Read())
                {
                    Net.Add(new NetworkInfo()
                    {
                        IP_Address = reader.GetString("IP_Address"),
                        IP_Port = reader.GetInt32("IP_Port"),
                        IP_Static = "断开",
                        IP_StaticColor = new SolidColorBrush(Colors.Red),
                    });
                    PortInfo.Networks.Add(new NetworkInfo()
                    {
                        IP_Address = reader.GetString("IP_Address"),
                        IP_Port = reader.GetInt32("IP_Port")
                    });
                }
                reader.Close();
            }
            return Net;
        }



        /// <summary>
        /// 打开串口
        /// </summary>
        public void OpenPort()
        {


        }

        /// <summary>
        /// 导出日志
        /// </summary>
        /// <param name="path"></param>
        /// <param name="DateTime"></param>
        public void LogWrite(string path, string DateTime)
        {
            try
            {
                string Tableresult = OperateIniTool.OperateIniRead("Table", "Name");
                List<string> Table = !string.IsNullOrEmpty(Tableresult) ? Tableresult.Split(',').ToList() : new List<string>();
                StringBuilder builder = new StringBuilder();
                foreach (var item in Table)
                {
                    if (MapBLL.TableNotexistBLL($"{item}{DateTime}"))
                    {
                        builder.Append(MapBLL.ExportMySqlTables($"{item}{DateTime}", "agv"));
                    }
                }
                File.WriteAllText(path, builder.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }

}
