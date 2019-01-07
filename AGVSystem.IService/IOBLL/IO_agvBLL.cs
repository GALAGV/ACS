using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.IService.IOBLL
{
    public interface IO_agvBLL
    {
        /// <summary>
        /// 查询所有AGV
        /// </summary>
        List<int> AGVNumList(long MapTime);








    }
}
