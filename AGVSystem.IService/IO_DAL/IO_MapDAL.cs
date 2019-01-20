using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


    }
}
