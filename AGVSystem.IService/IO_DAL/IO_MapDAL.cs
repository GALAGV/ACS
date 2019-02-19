using AGVSystem.Model.DrawMap;
using AGVSystem.Model.Ga_agvModels;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;

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


        /// <summary>
        /// 保存串口数据
        /// </summary>
        /// <param name="mapTime"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool InsertDeviceDAL(long mapTime, DataTable data);

        /// <summary>
        /// 修改启动地图数据
        /// </summary>
        /// <param name="Map"></param>
        /// <param name="Mode"></param>
        /// <returns></returns>
        bool UpdateSetting(long Map, int Mode);

        /// <summary>
        /// 保存地图
        /// </summary>
        /// <param name="Times">UTCTime</param>
        /// <param name="type">是否是新建地图 True是，false否 编辑地图</param>
        /// <param name="Name">地图名称</param>
        /// <param name="Width">宽度</param>
        /// <param name="Height">高度</param>
        /// <param name="AgvStr">默认0</param>
        /// <param name="MapType">地图类型 默认0</param>
        /// <param name="Size">缩放倍数</param>
        /// <param name="keyValues">Tag集合</param>
        /// <param name="Area">区域集合</param>
        /// <param name="TextControl">文字集合</param>
        /// <param name="Line">线路集合</param>
        /// <returns></returns>
        bool SaveAtlas(string Times, bool type, string Name, double Width, double Height, string AgvStr, int MapType, double Size, Dictionary<int, Label> keyValues, Dictionary<int, Label> Area, Dictionary<int, Label> TextControl, List<WirePointArray> Line);

        /// <summary>
        /// 查询线路
        /// </summary>
        /// <param name="MapName"></param>
        /// <returns></returns>
        MySqlDataReader MapRoute(string MapName);

        /// <summary>
        /// 线路保存
        /// </summary>
        /// <param name="route"></param>
        /// <param name="edit">是否编辑</param>
        /// <returns></returns>
        bool SaveRoute(Route route,bool edit, long UTCTime);

        /// <summary>
        /// 判断线路号是否存在
        /// </summary>
        /// <param name="Program"></param>
        /// <param name="MapTime"></param>
        /// <param name="UTCTime"></param>
        /// <returns></returns>
        bool ExistsProgram(string Program, long MapTime, long UTCTime);


        /// <summary>
        /// 查找关联Tag
        /// </summary>
        /// <param name="CreateTime"></param>
        /// <param name="TagNo"></param>
        string[] SelectTag(long CreateTime, string TagNo);

    }
}
