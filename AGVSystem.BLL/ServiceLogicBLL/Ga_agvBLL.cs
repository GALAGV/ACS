using AGVSystem.DAL.DataAccess;
using System.Collections.Generic;

namespace AGVSystem.BLL.ServiceLogicBLL
{
    public class Ga_agvBLL 
    {
        private Ga_agvDAL GetAgvDAL = new Ga_agvDAL();

        /// <summary>
        /// 查询所有AGV
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        public List<string> AGVNumList(long MapTime)
        {
            return GetAgvDAL.AGVArray(MapTime);
        }

        /// <summary>
        /// 记录已下发线路
        /// </summary>
        /// <param name="CarNum"></param>
        /// <param name="RouteName"></param>
        /// <param name="Task"></param>
        public void LogRouteInfoBLL(int CarNum, string[] RouteName, string Task)
        {
            GetAgvDAL.LogRouteInfoDAL(CarNum, RouteName, Task);
        }

    }
}
