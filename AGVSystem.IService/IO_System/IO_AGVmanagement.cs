﻿using AGVSystem.Model;
using AGVSystem.Model.Ga_agvModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.IService.IO_System
{
    /// <summary>
    /// AGV系统功能接口
    /// </summary>
    public interface IO_AGVmanagement
    {
        /// <summary>
        /// AGV状态信息
        /// </summary>
        List<Ga_agvStatus> AgvInfo();

        /// <summary>
        /// 查寻所有AGV信息
        /// </summary>
        /// <param name="agvNum"></param>
        /// <returns></returns>
        DataTable AgvInfo(long Time, ref int selAgv);








    }
}