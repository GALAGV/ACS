using AGVSystem.Model.DrawMap;
using AGVSystem.Model.Ga_agvModels;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;

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

        bool SaveMapInfo(string Times, bool type, string Name, double Width, double Height, string AgvStr, int MapType, double Size, Dictionary<int, Label> keyValues, Dictionary<int, Label> Area, Dictionary<int, Label> TextControl, List<WirePointArray> Line);


        MySqlDataReader BLLMapRoute(string MapName);


        /// <summary>
        /// 线路保存
        /// </summary>
        /// <param name="route"></param>
        /// <param name="edit">是否编辑</param>
        /// <returns></returns>
        bool SaveRouteBLL(Route route, bool edit, long UTCTime);



        /// <summary>
        /// 判断线路号是否存在
        /// </summary>
        /// <param name="Program"></param>
        /// <param name="MapTime"></param>
        /// <param name="UTCTime"></param>
        /// <returns></returns>
        bool ExistsProgramBLL(string Program, long MapTime, long UTCTime);


        /// <summary>
        /// 查找关联Tag
        /// </summary>
        /// <param name="CreateTime"></param>
        /// <param name="TagNo"></param>
        string[] SelectTagBLL(long CreateTime, string TagNo);
    }
}
