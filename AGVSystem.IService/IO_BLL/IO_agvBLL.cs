using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.IService.IO_BLL
{
    /// <summary>
    /// AGV信息业务逻辑接口
    /// </summary>
    public interface IO_agvBLL
    {
        /// <summary>
        /// 查询所有AGV
        /// </summary>
        List<string> AGVNumList(long MapTime);








    }
}
