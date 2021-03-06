﻿using AGVSystem.BLL.ServiceLogicBLL;
using AGVSystem.Model.Ga_agvModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using AGVSystem.Infrastructure.agvCommon;
using System.Collections.ObjectModel;
using AGVSystem.Model.LogicData;
using System.Windows.Media;

namespace AGVSystem.APP.agv_Map
{
    public class agvMapRegulate 
    {
        Ga_mapBLL IO_AGVMapService = new Ga_mapBLL();


        /// <summary>
        ///  地图导出
        /// </summary>
        /// <param name="MapTime">UTCTime</param>
        /// <param name="FileName">保存路径</param>
        /// <returns></returns>
        public bool Export_Map(long MapTime, string FileName)
        {
            try
            {
                string sql = IO_AGVMapService.ExportSettings(MapTime, "agv") + IO_AGVMapService.ExportMySqlTables("tag" + MapTime, "agv") + IO_AGVMapService.ExportMySqlTables("line" + MapTime, "agv") + IO_AGVMapService.ExportMySqlTables("device" + MapTime, "agv") + IO_AGVMapService.ExportMySqlTables("widget" + MapTime, "agv") + IO_AGVMapService.ExportMySqlTables("route" + MapTime, "agv");
                sql = sql + IO_AGVMapService.ExportTableContents("map", "agv", MapTime.ToString());
                File.WriteAllText(FileName, sql);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 加载所有地图数据
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Ga_Map> GetMapRegulate()
        {
            ObservableCollection<Ga_Map> ga_s = new ObservableCollection<Ga_Map>();
            MySqlDataReader mySql = IO_AGVMapService.MapArray();
            while (mySql.Read())
            {
                ga_s.Add(new Ga_Map()
                {
                    Name = mySql["Name"].ToString(),
                    Width = Convert.ToDouble(mySql["Width"].ToString()),
                    Height = Convert.ToDouble(mySql["Height"].ToString()),
                    ID = Convert.ToInt32(mySql["ID"].ToString()),
                    CreateTime = UTC.ConvertLongDateTime(long.Parse(mySql["CreateTime"].ToString())).ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            mySql.Close();
            return ga_s;
        }

        /// <summary>
        /// 线路集合
        /// </summary>
        /// <param name="MapName"></param>
        /// <returns></returns>
        public ObservableCollection<Route> GetrouteList(string MapName)
        {
            return Ergodic_Read(IO_AGVMapService.BLLMapRoute(MapName));
        }


        public ObservableCollection<Route> GetrouteList(string MapName, string LineName, string Program)
        {
            if (string.IsNullOrWhiteSpace(Program) || !FormatVerification.IsFloat(Program))
                return Ergodic_Read(IO_AGVMapService.BLLMapRoute(MapName, LineName, "N/A"));
            else
                return Ergodic_Read(IO_AGVMapService.BLLMapRoute(MapName, LineName, Program));
        }

        private ObservableCollection<Route> Ergodic_Read(MySqlDataReader read)
        {
            ObservableCollection<Route> routes = new ObservableCollection<Route>();
            while (read.Read())
            {
                routes.Add(new Route()
                {
                    ID = Convert.ToInt32(read["ID"].ToString().Trim()),
                    Name = read["Name"].ToString().Trim(),
                    CreateTime = long.Parse(read["CreateTime"].ToString().Trim()),
                    Tag = read["Tag"].ToString().Trim(),
                    Direction = read["Direction"].ToString().Trim(),
                    ChangeProgram = read["ChangeProgram"].ToString().Trim(),
                    Hook = read["Hook"].ToString().Trim(),
                    Pbs = read["Pbs"].ToString().Trim(),
                    Program = Convert.ToInt32(read["Program"].ToString().Trim()),
                    revPbs = read["revPbs"].ToString().Trim(),
                    Speed = read["Speed"].ToString().Trim(),
                    Stop = read["Stop"].ToString().Trim(),
                    Turn = read["Turn"].ToString().Trim()
                });
            }
            read.Close();
            return routes;
        }




        /// <summary>
        /// 显示所有信标数据
        /// </summary>
        /// <param name="Beacon"></param>
        /// <param name="arr"></param>
        public ObservableCollection<MapTag> TagManagement(long UTC)
        {
            MySqlDataReader data = IO_AGVMapService.RataTableBLL(UTC);
            ObservableCollection<MapTag> tags = new ObservableCollection<MapTag>();
            while (data.Read())
            {
                MapTag tag = new MapTag();
                tag.TagName = data["TagName"].ToString();
                tag.NextTag = data["NextTag"].ToString();
                tag.NextTagColor = data["NextTag"].ToString() != "N/A" ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Black);
                tag.PreLeftTag = data["PreLeftTag"].ToString();
                tag.PreLeftTagColor = data["PreLeftTag"].ToString() != "N/A" ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Black);
                tag.PreRightTag = data["PreRightTag"].ToString();
                tag.PreRightTagColor = data["PreRightTag"].ToString() != "N/A" ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Black);
                tag.PreTag = data["PreTag"].ToString();
                tag.PreTagColor = data["PreTag"].ToString() != "N/A" ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Black);
                tag.NextLeftTag = data["NextLeftTag"].ToString();
                tag.NextLeftTagColor = data["NextLeftTag"].ToString() != "N/A" ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Black);
                tag.NextRightTag = data["NextRightTag"].ToString();
                tag.NextRightTagColor = data["NextRightTag"].ToString() != "N/A" ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Black);
                tag.Speed = MainInfo.agvSpeed[Convert.ToInt32(data["Speed"].ToString())];
                tag.SpeedColor = Convert.ToInt32(data["Speed"].ToString()) != 0 && Convert.ToInt32(data["Speed"].ToString()) != 10 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black);
                tag.SpeedRev = MainInfo.agvSpeed[Convert.ToInt32(data["SpeedRev"].ToString())];
                tag.SpeedRevColor= Convert.ToInt32(data["SpeedRev"].ToString()) != 0 && Convert.ToInt32(data["SpeedRev"].ToString()) != 10 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black);
                tag.StopTime = data["StopTime"].ToString();
                tag.Pbs = MainInfo.agvPbs[Convert.ToInt32(data["Pbs"].ToString())];
                tag.PbsColor= Convert.ToInt32(data["Pbs"].ToString()) !=16 && Convert.ToInt32(data["Pbs"].ToString()) != 0 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black);
                tag.PbsRev = MainInfo.agvPbs[Convert.ToInt32(data["PbsRev"].ToString())];
                tag.PbsRevColor= Convert.ToInt32(data["PbsRev"].ToString()) != 16 && Convert.ToInt32(data["PbsRev"].ToString()) != 0 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black);
                tag.TagTerminal = data["TagTerminal"] is DBNull ? "" : data["TagTerminal"].ToString();

                if (CheckColumnName(data, "PreTurnSpeed"))
                {
                    tag.PreTurnSpeed = MainInfo.agvSpeed[data.GetInt32("PreTurnSpeed")];
                    tag.PreTurnSpeedColor = (data.GetInt32("PreTurnSpeed") != 0 && data.GetInt32("PreTurnSpeed") != 10 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black));
                    tag.RevTurnSpeed =  MainInfo.agvSpeed[data.GetInt32("RevTurnSpeed")] ;
                    tag.RevTurnSpeedColor = (data.GetInt32("RevTurnSpeed") != 0 && data.GetInt32("RevTurnSpeed") != 10 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black)) ;
                    tag.PreTurnPBS = MainInfo.agvPbs[data.GetInt32("PreTurnPBS")];
                    tag.PreTurnPBSColor = (data.GetInt32("PreTurnPBS") != 0 && data.GetInt32("PreTurnPBS") != 16 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black)) ;
                    tag.RevTurnPBS =MainInfo.agvPbs[data.GetInt32("RevTurnPBS")] ;
                    tag.RevTurnPBSColor =  (data.GetInt32("RevTurnPBS") != 0 && data.GetInt32("RevTurnPBS") != 16 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black))  ;
                }
                else
                {
                    tag.PreTurnSpeed = MainInfo.agvSpeed[0];
                    tag.PreTurnSpeedColor =new SolidColorBrush(Colors.Black);
                    tag.RevTurnSpeed =  MainInfo.agvSpeed[0];
                    tag.RevTurnSpeedColor =  new SolidColorBrush(Colors.Black);
                    tag.PreTurnPBS =  MainInfo.agvPbs[0];
                    tag.PreTurnPBSColor =  new SolidColorBrush(Colors.Black);
                    tag.RevTurnPBS =MainInfo.agvPbs[0];
                    tag.RevTurnPBSColor = new SolidColorBrush(Colors.Black);
                }
                tags.Add(tag);
            }
            data.Close();
            return tags;
        }

        /// <summary>
        /// 检查 SqlDataReader 实例中是否包含数据表的列
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public bool CheckColumnName(MySqlDataReader reader, string columnName)
        {
            bool result = false;
            DataTable dt = reader.GetSchemaTable();
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["ColumnName"].ToString() == columnName)
                {
                    result = true;
                }
            }
            return result; //result 为 true则 列名存在
        }



        /// <summary>
        /// 删除地图
        /// </summary>
        /// <param name="gs"></param>
        /// <returns></returns>
        public bool Delete_Map(List<Ga_Map> gs)
        {
            try
            {
                foreach (Ga_Map item in gs)
                {
                    long UTCTime = UTC.ConvertDateTimeLong(Convert.ToDateTime(item.CreateTime));
                    IO_AGVMapService.RemoveMap(UTCTime);
                    CachePlant.Remove(UTCTime.ToString()); //移除缓存
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="map_sql"></param>
        /// <returns></returns>
        public bool AGV_MapTolead(string map_sql)
        {
            return IO_AGVMapService.agvMap_Tolead(map_sql);
        }

        /// <summary>
        /// 查询配置参数
        /// </summary>
        /// <returns></returns>
        public DataTable setting()
        {
            return IO_AGVMapService.Map_Setting();
        }

        public DataTable defaultMap(long Time)
        {
            return IO_AGVMapService.defaultMapBLL(Time);
        }

        /// <summary>
        /// 保存串口
        /// </summary>
        /// <param name="mapTime"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool InsertDeviceSetting(long mapTime, DataTable data)
        {
            return IO_AGVMapService.InsertDeviceBLL(mapTime, data);
        }

        public bool SaveRouteSystem(Route route, bool edit, long UTCTime)
        {
            return IO_AGVMapService.SaveRouteBLL(route, edit, UTCTime);
        }

        public bool ExistsSystem(string Program, long MapTime, long UTCTime)
        {
            return IO_AGVMapService.ExistsProgramBLL(Program, MapTime, UTCTime);
        }

        public string[] SelectTagSystem(long CreateTime, string TagNo)
        {
            return IO_AGVMapService.SelectTagBLL(CreateTime, TagNo);
        }

        public bool DelRouteMapSystem(long MapTime, int Program)
        {
            return IO_AGVMapService.DelRouteMap(MapTime, Program);
        }

        public bool UpdateTagInfoSystem(long MapTime, ObservableCollection<MapTag> MapArray)
        {
            return IO_AGVMapService.UpdateTagInfoBLL(MapTime, MapArray);
        }


        public void DataBase()
        {
            IO_AGVMapService.CreationBase();
        }
    }
}
