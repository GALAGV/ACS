using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.Model.Ga_agvModels
{
    /// <summary>
    /// 地图实体类
    /// </summary>
    public class Ga_Map
    {
        public int ID { get; set; }

        /// <summary>
        /// 地图名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 宽带
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 地图类型 0磁标，1RFID，2激光
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreateTime { get; set; }


    }
}
