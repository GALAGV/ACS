using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.Model.Ga_agvModels
{
    /// <summary>
    /// 串口实体类
    /// </summary>
    public class Ga_PortInfo
    {
        /// <summary>
        /// 串口编号
        /// </summary>
        public string ComNumber { get; set; }

        /// <summary>
        /// 串口类型
        /// </summary>
        public string ComPortType { get; set; }

        /// <summary>
        /// 串口状态
        /// </summary>
        public string ComStatic { get; set; }

    }
}
