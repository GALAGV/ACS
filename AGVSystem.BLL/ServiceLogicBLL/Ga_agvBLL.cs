using AGVSystem.DAL.DataAccess;
using AGVSystem.IService.IO_BLL;
using AGVSystem.IService.IO_DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.BLL.ServiceLogicBLL
{
    public class Ga_agvBLL: IO_agvBLL
    {
        IO_agvDAL GetABB = new Ga_agvDAL();

        /// <summary>
        /// 查询所有AGV
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        List<int> IO_agvBLL.AGVNumList(long MapTime)
        {
            return GetABB.AGVArray(MapTime);
        }



    }
}
