﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.IService.IODAL
{
    public interface IO_agvDAL
    {

        /// <summary>
        /// 查询所有AGV编号
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>j
        List<int> AGVArray(long MapTime);








    }
}
