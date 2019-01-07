using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.IService.IOSystem
{
    public interface IO_AGVmanagement
    {
        /// <summary>
        /// AGV状态信息
        /// </summary>
        DataTable AgvInfo();

        /// <summary>
        /// 查寻所有AGV信息
        /// </summary>
        /// <param name="agvNum"></param>
        /// <returns></returns>
        DataTable AgvInfo(List<int> agvNum, ref int selAgv);








    }
}
