using System.Collections.Generic;


namespace AGVSystem.IService.IO_DAL
{
    /// <summary>
    /// AGV数据查询接口
    /// </summary>
    public interface IO_agvDAL
    {
        /// <summary>
        /// 查询所有AGV编号
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>j
        List<string> AGVArray(long MapTime);
    }
}
