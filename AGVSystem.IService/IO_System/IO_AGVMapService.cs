using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using AGVSystem.Model;
using AGVSystem.Model.Ga_agvModels;
using System.Windows.Controls;
using System.Windows.Media;

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
        ///绘制X轴Y轴刻度
        /// </summary>
        /// <param name="mainPanel"></param>
        /// <param name="mainPane2"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="brush"></param>
        /// <param name="width"></param>
        void DrawScale(Canvas mainPanel, Canvas mainPane2, double x, double y, Brush brush, double width);





    }
}
