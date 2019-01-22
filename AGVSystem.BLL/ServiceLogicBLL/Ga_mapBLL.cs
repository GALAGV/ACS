using AGVSystem.DAL.DataAccess;
using AGVSystem.IService.IO_BLL;
using AGVSystem.IService.IO_DAL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.BLL.ServiceLogicBLL
{
    public class Ga_mapBLL : IO_MapBLL
    {
        IO_MapDAL map = new Ga_mapDAL();

        /// <summary>
        /// 导出地图信息
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Db"></param>
        /// <returns></returns>
        public string ExportMySqlTables(string TableName, string Db)
        {
            return map.ExportMySqlTable(TableName, Db);
        }

        /// <summary>
        /// 导出地图配置逻辑
        /// </summary>
        /// <param name="MapTime"></param>
        /// <param name="Db"></param>
        /// <returns></returns>
        public string ExportSettings(long MapTime, string Db)
        {
            return map.ExportSetting(MapTime, Db);
        }

        /// <summary>
        /// 导出地图表
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Db"></param>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        public string ExportTableContents(string TableName, string Db, string MapTime)
        {
            return map.ExportTableContent(TableName, Db, MapTime);
        }

        /// <summary>
        /// 查询串口信息
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        public MySqlDataReader ListDevice(long MapTime)
        {
            return map.LoadDevice(MapTime);
        }

        /// <summary>
        /// 查询地图数据
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        public MySqlDataReader MapArray()
        {
            return map.MapList();
        }

        /// <summary>
        /// 删除地图
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        public bool RemoveMap(long MapTime)
        {
            return map.DeleteMap(MapTime);
        }
    }
}
