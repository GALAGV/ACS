using AGVSystem.DAL.DataAccess;
using AGVSystem.Model.DrawMap;
using AGVSystem.Model.Ga_agvModels;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Controls;

namespace AGVSystem.BLL.ServiceLogicBLL
{
    public class Ga_mapBLL 
    {
       private readonly Ga_mapDAL map = new Ga_mapDAL();

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
        /// 保存配置
        /// </summary>
        /// <param name="mapTime"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool InsertDeviceBLL(long mapTime, DataTable data)
        {
            return map.InsertDeviceDAL(mapTime, data);
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

        public bool UpdateSettingMap(long Map, int Mode)
        {
            return map.UpdateSetting(Map, Mode);
        }

        public bool SaveMapInfo(string Times, bool type, string Name, double Width, double Height, string AgvStr, int MapType, double Size, Dictionary<int, Label> keyValues, Dictionary<int, Label> Area, Dictionary<int, Label> TextControl, List<WirePointArray> Line)
        {
            return map.SaveAtlas(Times, type, Name, Width, Height, AgvStr, MapType, Size, keyValues, Area, TextControl, Line);
        }

        public MySqlDataReader BLLMapRoute(string MapName)
        {
            return map.MapRoute(MapName);
        }

        public MySqlDataReader MapArrayBLL()
        {
            return map.MapArrayDAL();
        }

        public bool SaveRouteBLL(Route route, bool edit, long UTCTime)
        {
            return map.SaveRoute(route, edit, UTCTime);
        }

        public bool ExistsProgramBLL(string Program, long MapTime, long UTCTime)
        {
            return map.ExistsProgram(Program, MapTime, UTCTime);
        }

        public string[] SelectTagBLL(long CreateTime, string TagNo)
        {
            return map.SelectTag(CreateTime, TagNo);
        }

        /// <summary>
        /// 删除线路
        /// </summary>
        /// <param name="MapTime"></param>
        /// <param name="Program"></param>
        /// <returns></returns>
        public bool DelRouteMap(long MapTime, int Program)
        {
            return map.DelRoute(MapTime, Program);
        }


        public MySqlDataReader RataTableBLL(long exls)
        {
            return map.GetMapTags(exls);
        }

        public bool UpdateTagInfoBLL(long MapTime, ObservableCollection<MapTag> MapArray)
        {
            return map.UpdateTagInfo(MapTime, MapArray);
        }
    }
}
