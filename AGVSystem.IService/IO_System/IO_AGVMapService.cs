using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using AGVSystem.Model.Ga_agvModels;

namespace AGVSystem.IService.IO_System
{
    public interface IO_AGVMapService
    {
        /// <summary>
        /// 查询地图数据
        /// </summary>
        /// <returns></returns>
        ObservableCollection<Ga_Map> GetMapRegulate();

        /// <summary>
        /// 导出agv地图
        /// </summary>
        /// <returns></returns>
        bool Export_Map(long UTCTime, string FileName);

        /// <summary>
        /// 删除地图
        /// </summary>
        /// <param name="gs"></param>
        /// <returns></returns>
        bool Delete_Map(List<Ga_Map> gs);

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="map_sql"></param>
        /// <returns></returns>
       bool AGV_MapTolead(string map_sql);


        /// <summary>
        /// 查询配置参数
        /// </summary>
        /// <returns></returns>
        DataTable setting();


        DataTable defaultMap(long Time);


        /// <summary>
        /// 保存串口
        /// </summary>
        /// <param name="mapTime"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool InsertDeviceSetting(long mapTime, DataTable data);

        /// <summary>
        /// 线路集合
        /// </summary>
        /// <param name="MapName"></param>
        /// <returns></returns>
        ObservableCollection<Route> GetrouteList(string MapName);


        /// <summary>
        /// 线路保存
        /// </summary>
        /// <param name="route"></param>
        /// <param name="edit">是否编辑</param>
        /// <returns></returns>
        bool SaveRouteSystem(Route route, bool edit, long UTCTime);



        /// <summary>
        /// 判断线路号是否存在
        /// </summary>
        /// <param name="Program"></param>
        /// <param name="MapTime"></param>
        /// <param name="UTCTime"></param>
        /// <returns></returns>
        bool ExistsSystem(string Program, long MapTime, long UTCTime);


        /// <summary>
        /// 查找关联Tag
        /// </summary>
        /// <param name="CreateTime"></param>
        /// <param name="TagNo"></param>
        string[] SelectTagSystem(long CreateTime, string TagNo);

    }
}
