using AGVSystem.DAL.DataAccess;
using AGVSystem.IService.IO_BLL;
using AGVSystem.IService.IO_DAL;
using System.Collections.Generic;

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
        List<string> IO_agvBLL.AGVNumList(long MapTime)
        {
            return GetABB.AGVArray(MapTime);
        }



    }
}
