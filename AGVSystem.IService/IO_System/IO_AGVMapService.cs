using System.Collections.Generic;
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
        List<Ga_Map> GetMapRegulate();

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


    }
}
