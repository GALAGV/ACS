using MySql.Data.MySqlClient;
using System.Data;

namespace AGVSystem.IService.IO_BLL
{
    /// <summary>
    /// 地图信息业务逻辑接口
    /// </summary>
    public interface IO_MapBLL
    {
        MySqlDataReader ListDevice(long MapTime);

        /// <summary>
        /// 查询所有地图信息
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        MySqlDataReader MapArray();

        /// <summary>
        /// 删除地图
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        bool RemoveMap(long MapTime);


        /// <summary>
        /// 导入地图
        /// </summary>
        /// <param name="MapText"></param>
        /// <returns></returns>
        bool agvMap_Tolead(string MapText);

        /// <summary>
        /// 导出地图配置逻辑
        /// </summary>
        /// <param name="MapTime"></param>
        /// <param name="Db"></param>
        /// <returns></returns>
        string ExportSettings(long MapTime, string Db);

        /// <summary>
        /// 导出地图信息逻辑
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Db"></param>
        /// <returns></returns>
        string ExportMySqlTables(string TableName, string Db);


        /// <summary>
        /// 导出地图逻辑
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Db"></param>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        string ExportTableContents(string TableName, string Db, string MapTime);


        /// <summary>
        ///加载所有Tag
        /// </summary>
        /// <param name="exls"></param>
        /// <returns></returns>
        MySqlDataReader RataTable(string exls);

        /// <summary>
        /// 查询线路信息
        /// </summary>
        /// <param name="Times"></param>
        /// <returns></returns>
        MySqlDataReader LinelistArrer(string Times);

        /// <summary>
        /// 查询区域文字信息
        /// </summary>
        /// <param name="Times"></param>
        /// <returns></returns>
        MySqlDataReader  GetWidget(string Times);


        /// <summary>
        /// 查询默认地图数据
        /// </summary>
        /// <returns></returns>
        DataTable Map_Setting();


        /// <summary>
        /// 查询地图数据
        /// </summary>
        /// <param name="UTCTime"></param>
        /// <returns></returns>
        DataTable defaultMapBLL(long UTCTime);


        /// <summary>
        /// 保存串口数据
        /// </summary>
        /// <param name="mapTime"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool InsertDeviceBLL(long mapTime, DataTable data);

        /// <summary>
        /// 修改启动地图
        /// </summary>
        /// <param name="Map"></param>
        /// <param name="Mode"></param>
        /// <returns></returns>
        bool UpdateSettingMap(long Map, int Mode);
    }
}
