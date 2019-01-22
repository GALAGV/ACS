using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
