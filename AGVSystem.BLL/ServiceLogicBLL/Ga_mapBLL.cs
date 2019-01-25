using AGVSystem.DAL.DataAccess;
using AGVSystem.IService.IO_BLL;
using AGVSystem.IService.IO_DAL;
using MySql.Data.MySqlClient;
using System.Data;

namespace AGVSystem.BLL.ServiceLogicBLL
{
    public class Ga_mapBLL : IO_MapBLL
    {
        IO_MapDAL map = new Ga_mapDAL();

        /// <summary>
        /// 导入地图
        /// </summary>
        /// <param name="MapText"></param>
        /// <returns></returns>
        public bool agvMap_Tolead(string MapText)
        {
           return map.MapTolead(MapText);
        }

        /// <summary>
        /// 查询地图信息
        /// </summary>
        /// <param name="UTCTime"></param>
        /// <returns></returns>
        public DataTable defaultMapBLL(long UTCTime)
        {
            return map.defaultMapDAL(UTCTime);
        }

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

        public MySqlDataReader GetWidget(string Times)
        {
            return map.widgetArrlist(Times);
        }

        /// <summary>
        /// 查询线路信息
        /// </summary>
        /// <param name="Times"></param>
        /// <returns></returns>
        public MySqlDataReader LinelistArrer(string Times)
        {
            return map.LineData(Times);
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

        public DataTable Map_Setting()
        {
            return map.Setting();
        }

        /// <summary>
        /// 查询所有信标数据
        /// </summary>
        /// <param name="exls"></param>
        /// <returns></returns>
        public MySqlDataReader RataTable(string exls)
        {
           return map.GetMapTags(exls);
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
