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

    }
}
