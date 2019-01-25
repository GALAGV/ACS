namespace AGVSystem.Model.Ga_agvModels
{
    /// <summary>
    /// AGV信息实体类
    /// </summary>
    public class Ga_agv
    {
        /// <summary>
        /// AGV编号
        /// </summary>
        public int agvNumber { get; set; }

        /// <summary>
        /// 网络状态
        /// </summary>
        public string agvStatic { get; set; }

        /// <summary>
        /// 电压
        /// </summary>
        public string Voltage { get; set; }

        /// <summary>
        /// 速度
        /// </summary>
        public string Speed { get; set; }

        /// <summary>
        /// 转弯
        /// </summary>
        public string turn { get; set; }

        /// <summary>
        /// 方向
        /// </summary>
        public string Dir { get; set; }

        /// <summary>
        /// 挂钩
        /// </summary>
        public string Hook { get; set; }

        /// <summary>
        /// RFID编号
        /// </summary>
        public string RFID { get; set; }

        /// <summary>
        /// 线路号
        /// </summary>
        public string RouteNo { get; set; }

        /// <summary>
        /// PBS
        /// </summary>
        public string PBS { get; set; }

        /// <summary>
        /// 步骤
        /// </summary>
        public string Step { get; set; }

        /// <summary>
        /// 程序号
        /// </summary>
        public string Program { get; set; }
    }
}
