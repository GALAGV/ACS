using System.Collections.Generic;
using AGVDLL;

namespace AGVSystem.Model.LogicData
{
    public class AgvInfo
    {
        #region 构造
        public AgvInfo()
        {
            this.agvSendResult = new int[] { -1, -1, -1 };
            this.agvStart = true;
            this.sendRfid = 1;
            this.agvRouteName = string.Empty;
            this.sendRouteName = string.Empty;
            this.agvTask = string.Empty;
            this.sendTask = string.Empty;
        }
        #endregion

        #region 属性

        
        /// <summary>
        /// AGV编号
        /// </summary>
        public int agvNumber { get; set; }

        /// <summary>
        /// 网络状态
        /// </summary>
        public agvRun agvStatic { get; set; }

        /// <summary>
        /// 运准
        /// </summary>
        public bool masterOn { get; set; }

        /// <summary>
        ///启停
        /// </summary>
        public bool isRunning { get; set; }

        /// <summary>
        /// 在磁条上
        /// </summary>
        public bool isLine { get; set; }

        /// <summary>
        /// 驱动状态
        /// </summary>
        public bool driveState { get; set; }

        /// <summary>
        /// 电压
        /// </summary>
        public float voltage { get; set; }

        /// <summary>
        /// 速度
        /// </summary>
        public int speedNo { get; set; }

        /// <summary>
        /// 左转
        /// </summary>
        public bool turnLeft { get; set; }

        /// <summary>
        /// 右转
        /// </summary>
        public bool turnRight { get; set; }

        /// <summary>
        /// 方向
        /// </summary>
        public bool direction { get; set; }

        /// <summary>
        /// 挂钩
        /// </summary>
        public bool hook { get; set; }

        /// <summary>
        /// RFID编号
        /// </summary>
        public string rfid { get; set; }

        /// <summary>
        /// 线路号
        /// </summary>
        public int routeNo { get; set; }

        /// <summary>
        /// PBS
        /// </summary>
        public int pbs { get; set; }

        /// <summary>
        /// 步骤
        /// </summary>
        public int step { get; set; }

        /// <summary>
        /// 程序号
        /// </summary>
        public int programNo { get; set; }

        /// <summary>
        /// 是否有错误
        /// </summary>
        public bool errorSwitch { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        public int errorCode { get; set; }

        /// <summary>
        /// agv准备下发的路径名称。   
        /// </summary>
        public string agvRouteName { get; set; }

        /// <summary>
        /// agv已接受的路径名称
        /// </summary>                                                              
        public string sendRouteName { get; set; }

        /// <summary>
        /// agv的准备下发的运行任务。
        /// </summary>                                
        public string agvTask { get; set; }

        /// <summary>
        /// agv已接受的运行任务。
        /// </summary>                                      
        public string sendTask { get; set; }

        /// <summary>
        /// agv的准备下发的运行任务编号
        /// </summary>                                            
        public int agvRouteNo { get; set; }

        /// <summary>
        /// agv已接受的运行任务
        /// </summary>                                        
        public int sendRouteNo { get; set; }

        /// <summary>
        /// agv是否需要动作下发指令
        /// </summary>                                
        public bool agvActive { get; set; }

        /// <summary>
        /// agv接受完指令后，是否启动过
        /// </summary>                                       
        public bool agvStart { get; set; }

        /// <summary>
        /// agv定时启动标志位,0表示未启动，1表示需要启动，2表示启动完成 
        /// </summary>                                                   
        public int agvTimerStart { get; set; }

        /// <summary>
        /// pc发送启动的标志位，会直接把agv状态固定两秒为运行状态    
        /// </summary>                                                     
        public int pcAgvStart { get; set; }

        /// <summary>
        /// agv的动作是否发送成功。     
        /// </summary>                                                
        public int[] agvSendResult { get; set; }

        /// <summary>
        /// 该次rfid是整条下发还是调路径  1代表调路径，2代表逐个rfid下发，3代表计算路径下发
        /// </summary>
        public int sendRfid { get; set; }

        /// <summary>
        /// agv缓存点是否需要启动,0表示未启动，1表示需要启动，2表示启动完成
        /// </summary>                                      
        public int agvBufferStart { get; set; }

        /// <summary>
        /// agv充电继电器闭合 1.闭合 2.断开 
        /// </summary>
        public int agvRelayClose { get; set; }

        /// <summary>
        /// agv充电继电器状态 1.闭合 2.断开      
        /// </summary>                                                                                        
        public int agvRelayStatus { get; set; }

        /// <summary>
        /// agv下发rfid的List
        /// </summary>
        public List<RFIDClass> agvRfidList { get; set; }

        #endregion
    }

        #region 枚举

    /// <summary>
    ///  设备类型
    /// </summary>
    public enum Facility
    {
        /// <summary>
        /// 串口
        /// </summary>
        ComPort = 1,

        /// <summary>
        /// 网口
        /// </summary>
        Internet = 2
    }

    public enum PortType
    {


        AGV = 1,

        /// <summary>
        /// 按钮
        /// </summary>
        Button = 2,

        /// <summary>
        /// 充电机
        /// </summary>
        Charge = 3,

        /// <summary>
        /// PLC
        /// </summary>
        PLC = 4
    }


    public enum agvRun
    {
        /// <summary>
        /// 运行
        /// </summary>
        ON=1,

        /// <summary>
        /// 停止
        /// </summary>
        OFF=2
    }

    #endregion
}
