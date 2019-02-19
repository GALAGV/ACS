using AGVSystem.DAL.DataAccess;
using System.Collections.Generic;

namespace AGVSystem.BLL.ServiceLogicBLL
{
    public class Ga_agvBLL 
    {
        private Ga_agvDAL GetABB = new Ga_agvDAL();

        /// <summary>
        /// 查询所有AGV
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        public List<string> AGVNumList(long MapTime)
        {
            return GetABB.AGVArray(MapTime);
        }
    }
}
