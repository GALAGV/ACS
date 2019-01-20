using AGVSystem.DAL.DataAccess;
using AGVSystem.IService.IO_BLL;
using AGVSystem.IService.IO_DAL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.BLL.ServiceLogicBLL
{
    public class Ga_mapBLL : IO_MapBLL
    {
        IO_MapDAL map = new Ga_mapDAL();

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
    }
}
