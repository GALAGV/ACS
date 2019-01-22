using AGVSystem.BLL;
using AGVSystem.BLL.ServiceLogicBLL;
using AGVSystem.IService.IO_BLL;
using AGVSystem.IService.IO_System;
using AGVSystem.Model;
using AGVSystem.Model.Ga_agvModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGVSystem.Infrastructure.agvCommon;
using System.Windows.Controls;
using System.Windows.Media;

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
        public List<Ga_Map> GetMapRegulate()
        {
            List<Ga_Map> ga_s = new List<Ga_Map>();
            MySqlDataReader mySql = IO_AGVMapService.MapArray();
            while (mySql.Read())
            {
                ga_s.Add(
                    new Ga_Map()
                    {
                        Name = mySql["Name"].ToString(),
                        Width = Convert.ToDouble(mySql["Width"].ToString()),
                        Height = Convert.ToDouble(mySql["Height"].ToString()),

                        CreateTime = UTC.ConvertLongDateTime(long.Parse(mySql["CreateTime"].ToString())).ToString("yyyy-MM-dd HH:mm:ss")
                    });
            }
            mySql.Close();
            return ga_s;
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
        /// 绘制刻度
        /// </summary>
        /// <param name="mainPanel"></param>
        /// <param name="mainPane2"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="brush"></param>
        /// <param name="width"></param>
        public void DrawScale(Canvas mainPanel, Canvas mainPane2, double x, double y, Brush brush, double width)
        {
           
        }
    }
}
