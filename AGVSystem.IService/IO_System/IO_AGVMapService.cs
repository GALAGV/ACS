using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using AGVSystem.Model;
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


    }
}
