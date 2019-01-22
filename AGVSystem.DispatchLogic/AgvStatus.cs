using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.DispatchLogic
{
    class AgvStatus
    {
        string[] agvRouteName { get; set; }//每辆agv准备下发的路径名称。                                                 
        string[] sendRouteNameList { get; set; }//每辆agv已接受的路径名称。                                                
        string agvTask { get; set; }//每辆agv的准备下发的运行任务。 分为tz300,tz3012,tz303,ts300,ts3012,ts303,s141,s142。                                    
        string sendTask { get; set; }//每辆agv已接受的运行任务。 分为tz300,tz3012,tz303,ts300,ts3012,ts303,s141,s142。                                     
        int agvRouteNo { get; set; } //每辆agv的准备下发的运行任务。 分为tz300,tz3012,tz303,ts300,ts3012,ts303,s141,s142。                                    
        int sendRouteNo { get; set; } //每辆agv已接受的运行任务。 分为tz300,tz3012,tz303,ts300,ts3012,ts303,s141,s142。                                     
        bool agvActive { get; set; } //每辆agv是否需要动作下发指令。                                    
        bool agvStart { get; set; } //每辆agv接受完指令后，是否启动过。                                    
        int agvTimerStart { get; set; }//每辆agv定时启动标志位,0表示未启动，1表示需要启动，2表示启动完成                                           
        int pcAgvStart { get; set; }//pc发送启动的标志位，会直接把agv状态固定两秒为运行状态                                    
        int agvSendResult { get; set; }//每辆agv的动作是否发送成功。                                       
        int sendRfid { get; set; }// 该次rfid是整条下发还是调路径 1代表调路径，2代表逐个rfid下发                                 
        int agvBufferStart { get; set; }// agv缓存点是否需要启动
        int agvRelayClose { get; set; }//agv充电继电器闭合 1.闭合 2.断开                                                                           
        int agvRelayStatus { get; set; }// agv充电点是否要停止        
    }
}
