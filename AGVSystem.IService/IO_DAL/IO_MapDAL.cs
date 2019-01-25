using MySql.Data.MySqlClient;
using System.Data;


namespace AGVSystem.IService.IO_DAL
{
    /// <summary>
    /// 地图信息数据查询接口
    /// </summary>
    public interface IO_MapDAL
    {
        /// <summary>
        /// 查询AGV串口信息
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        MySqlDataReader LoadDevice(long MapTime);

        /// <summary>
        /// 查询所有地图数据
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        MySqlDataReader MapList();


        /// <summary>
        /// 导出地图配置
        /// </summary>
        /// <param name="MapTime"></param>
        /// <param name="Db"></param>
        /// <returns></returns>
        string ExportSetting(long MapTime, string Db);

        /// <summary>
        /// 导出地图信息
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Db"></param>
        /// <returns></returns>
        string ExportMySqlTable(string TableName, string Db);


        /// <summary>
        /// 导出地图
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Db"></param>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        string ExportTableContent(string TableName, string Db, string MapTime);

        /// <summary>
        /// 删除地图
        /// </summary>
        /// <param name="MapTime">UTCTime</param>
        /// <returns></returns>
        bool DeleteMap(long MapTime);

        /// <summary>
        /// 导入地图
        /// </summary>
        /// <param name="MapText"></param>
        /// <returns></returns>
        bool MapTolead(string MapText);

        /// <summary>
        /// 查询所有Tag
        /// </summary>
        /// <param name="exls"></param>
        /// <returns></returns>
        MySqlDataReader GetMapTags(string exls);

        /// <summary>
        /// 查询所有线路数据
        /// </summary>
        /// <param name="Times"></param>
        /// <returns></returns>
        MySqlDataReader LineData(string Times);

        /// <summary>
        /// 查询区域文字数据
        /// </summary>
        /// <param name="Times"></param>
        /// <returns></returns>
        MySqlDataReader widgetArrlist(string Times);

        /// <summary>
        /// 查询默认地图数据
        /// </summary>
        /// <returns></returns>
        DataTable Setting();

        /// <summary>
        /// 查询地图数据
        /// </summary>
        /// <param name="UTCTime"></param>
        /// <returns></returns>
        DataTable defaultMapDAL(long UTCTime);

    }
}
