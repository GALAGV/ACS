using AGVSystem.BLL.ServiceLogicBLL;
using AGVSystem.IService.IO_BLL;
using AGVSystem.IService.IO_System;
using AGVSystem.Model.Ga_agvModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using AGVSystem.Infrastructure.agvCommon;
using System.Collections.ObjectModel;

namespace AGVSystem.APP.agv_Map
{
    public class agvMapRegulate : IO_AGVMapService
    {
        IO_MapBLL IO_AGVMapService = new Ga_mapBLL();


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
            ObservableCollection<Route> routes = new ObservableCollection<Route>();
            MySqlDataReader mySql = IO_AGVMapService.BLLMapRoute(MapName);
            while (mySql.Read())
            {
                routes.Add(new Route() {
                    ID= Convert.ToInt32(mySql["ID"].ToString().Trim()),
                    Name= mySql["Name"].ToString().Trim(),
                    CreateTime= long.Parse(mySql["CreateTime"].ToString().Trim()),
                    Tag= mySql["Tag"].ToString().Trim(),
                    Direction = mySql["Direction"].ToString().Trim(),
                    ChangeProgram= mySql["ChangeProgram"].ToString().Trim(),
                    Hook = mySql["Hook"].ToString().Trim(),
                    Pbs= mySql["Pbs"].ToString().Trim(),
                    Program= Convert.ToInt32(mySql["Program"].ToString().Trim()),
                    revPbs= mySql["revPbs"].ToString().Trim(),
                    Speed= mySql["Speed"].ToString().Trim(),
                    Stop= mySql["Stop"].ToString().Trim(),
                    Turn = mySql["Turn"].ToString().Trim()
                });
            }
            mySql.Close();
            return routes;
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
                    IO_AGVMapService.RemoveMap(UTC.ConvertDateTimeLong(Convert.ToDateTime(item.CreateTime)));
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
    }
}
