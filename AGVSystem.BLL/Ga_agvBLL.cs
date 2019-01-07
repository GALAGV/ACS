using AGVSystem.DAL.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.BLL
{
    public class Ga_agvBLL
    {
        Ga_agvDAL GetABB = new Ga_agvDAL();

        /// <summary>
        /// 查询所有AGV
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        public List<int> AGVList(long MapTime)
        {
            return GetABB.AGVNumList(MapTime);
        }
    }
}
