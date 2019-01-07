using AGVSystem.IService.IO_DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.DAL.DataAccess
{
    public class Ga_mapDAL : IO_MapDAL
    {
        /// <summary>
        /// 查询串口信息
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        public DataTable LoadDevice(long MapTime)
        {
            string sql = string.Format("SELECT `Com`, `Baud`, `Agv` FROM agv.`device{0}`", MapTime);
            return MySqlHelper.ExecuteDataTable(sql);
        }



    }
}
