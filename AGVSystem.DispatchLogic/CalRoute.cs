using AGVDLL;
using AGVSystem.Infrastructure.agvCommon;
using AGVSystem.Model.LogicData;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.DispatchLogic
{
    class CalRoute
    {
        int aRoute = 31;
        int bRoute = 32;
        public List<CarStatus> agvStatusList = new List<CarStatus>();

        public ConcurrentDictionary<int, string[]> agvRouteNameList = new ConcurrentDictionary<int, string[]>();//每辆agv准备下发的路径名称。
        public ConcurrentDictionary<int, string[]> sendRouteNameList = new ConcurrentDictionary<int, string[]>();//每辆agv已接受的路径名称。
        public ConcurrentDictionary<int, string> agvTaskList = new ConcurrentDictionary<int, string>();//每辆agv的准备下发的运行任务。 分为tz300,tz3012,tz303,ts300,ts3012,ts303,s141,s142。
        public ConcurrentDictionary<int, string> sendTaskList = new ConcurrentDictionary<int, string>();//每辆agv已接受的运行任务。 分为tz300,tz3012,tz303,ts300,ts3012,ts303,s141,s142。
        public ConcurrentDictionary<int, int> agvRouteNoList = new ConcurrentDictionary<int, int>();//每辆agv的准备下发的运行任务。 分为tz300,tz3012,tz303,ts300,ts3012,ts303,s141,s142。
        public ConcurrentDictionary<int, int> sendRouteNoList = new ConcurrentDictionary<int, int>();//每辆agv已接受的运行任务。 分为tz300,tz3012,tz303,ts300,ts3012,ts303,s141,s142。
        public ConcurrentDictionary<int, bool> agvActive = new ConcurrentDictionary<int, bool>();//每辆agv是否需要动作下发指令。
        public ConcurrentDictionary<int, bool> agvStart = new ConcurrentDictionary<int, bool>();//每辆agv接受完指令后，是否启动过。
        public ConcurrentDictionary<int, int> agvTimerStart = new ConcurrentDictionary<int, int>();//每辆agv定时启动标志位,0表示未启动，1表示需要启动，2表示启动完成
        public ConcurrentDictionary<int, int> pcAgvStart = new ConcurrentDictionary<int, int>();//pc发送启动的标志位，会直接把agv状态固定两秒为运行状态
        public ConcurrentDictionary<int, int[]> agvSendResult = new ConcurrentDictionary<int, int[]>();//每辆agv的动作是否发送成功。
        public ConcurrentDictionary<int, int> sendRfid = new ConcurrentDictionary<int, int>();// 该次rfid是整条下发还是调路径 1代表调路径，2代表逐个rfid下发
        public ConcurrentDictionary<int, int> agvBufferStart = new ConcurrentDictionary<int, int>();// agv缓存点是否需要启动
        public ConcurrentDictionary<int, int> agvRelayClose = new ConcurrentDictionary<int, int>();//agv充电继电器闭合 1.闭合 2.断开
        public ConcurrentDictionary<int, int> agvRelayStatus = new ConcurrentDictionary<int, int>();//agv充电继电器状态 1.闭合 2.断开
        //public ConcurrentDictionary<int, int> agvChargeStop = new ConcurrentDictionary<int, int>();// agv充电点是否要停止

        DataTable routeDt = new DataTable();

        public int warningVoltage = 24;
        public int ChargeAgv = 0;
        public int agvRePos = 0;//agv归位标志位

        public int emergencyButton = 0;//紧急按钮当前状态
        public int emergencySwith = 0;//0表示急停按钮没有使用过，1表示急停按钮按下， 2表示急停按钮复原

        int pauseInt = 0; //是否是暂停状态

        public List<int[]> avoidStop = new List<int[]>();//停止避让
        public List<int> avoidClear = new List<int>();//停止避让的阻碍路径是否已经清空的标志位

        private Hashtable agvRfidListHash = new Hashtable();

        public void sendRouteDt(DataTable RouteDt)
        {
            routeDt = RouteDt;
            agvTaskList.Clear();
            agvRouteNoList.Clear();
            sendTaskList.Clear();
            agvActive.Clear();
            agvSendResult.Clear();
            agvStart.Clear();
            agvTimerStart.Clear();
            agvBufferStart.Clear();
            agvRelayClose.Clear();
            agvRelayStatus.Clear();
            pcAgvStart.Clear();
            agvRouteNameList.Clear();
            sendRouteNameList.Clear();
            avoidStop.Clear();
            avoidClear.Clear();
            agvRfidListHash.Clear();

            foreach (string i in MainInfo.agvNo)
            {
                int agvNum = int.Parse(i);
                agvTaskList.GetOrAdd(agvNum, "");
                sendTaskList.GetOrAdd(agvNum, "");
                agvRouteNoList.GetOrAdd(agvNum, 0);
                sendRouteNoList.GetOrAdd(agvNum, 0);
                agvActive.GetOrAdd(agvNum, false);
                agvStart.GetOrAdd(agvNum, true);
                agvTimerStart.GetOrAdd(agvNum, 0);
                agvBufferStart.GetOrAdd(agvNum, 0);
                agvRelayClose.GetOrAdd(agvNum, 0);
                agvRelayStatus.GetOrAdd(agvNum, 0);
                agvSendResult.GetOrAdd(agvNum, new int[] { -1, -1, -1 });//第一位表示rfidlist 数字表示发送到第几位，第二位allcommand，第三位start -1为成功，0为未发送或者发送失败，2表示按钮
                agvRouteNameList.GetOrAdd(agvNum, new string[] { "" });
                sendRouteNameList.GetOrAdd(agvNum, new string[] { "" });
                RFIDClass rf = new RFIDClass();
                pcAgvStart.GetOrAdd(agvNum, 0);
            }
            avoidStop.Add(new int[] { 6, 0, 0 });
            avoidStop.Add(new int[] { 54, 0, 0 });

            for (int i = 0; i < avoidStop.Count; i++)
            {
                avoidClear.Add(0);
            }
        }

        public void agvSch()
        {
            //初期agvNum, rfid没有回应值写入默认值
            int agvCount = agvStatusList.Count;

            //int tsAgvCount = 0;//ts区域agv数量
            for (int i = 0; i < agvCount; i++)
            {
                if (agvStatusList[i].carNum == 0)
                {
                    agvStatusList[i].carNum = int.Parse(MainInfo.agvNo[i]);
                }
                if (string.IsNullOrEmpty(agvStatusList[i].rfidStatus))
                {
                    agvStatusList[i].rfidStatus = "-53328";
                }

                //ts区域agv数量计数
                //if (int.Parse(agvStatusList[i].rfidStatus) > 86 && int.Parse(agvStatusList[i].rfidStatus) < 100)
                //{
                //    tsAgvCount++;
                //}
            }

            //ConnForm.dkst = dkSt;
            //ConnForm.stInfoList = StList;
            //ChargeForm.ciList = chargeStList;

            for (int i = 0; i < avoidStop.Count; i++)
            {
                for (int j = 0; j < agvCount; j++)
                {
                    if (agvStatusList[j].carNum == avoidStop[i][1] && agvStatusList[j].IsRunning)
                    {
                        lock (avoidStop)
                        {
                            //avoidStop[i][1] = 0;
                            avoidStop[i][2] = 0;
                        }
                    }
                    if (agvStatusList[j].carNum == avoidStop[i][1] && avoidStop[i][0].ToString() != agvStatusList[j].rfidStatus)
                    {
                        lock (avoidStop)
                        {
                            avoidStop[i][2] = 2;
                        }
                    }
                }
                avoidClear[i] = 0;
            }

            for (int i = 0; i < agvCount; i++)
            {
                int avoidNum = 0;

                //6tag需要避让的tag
                if (agvStatusList[i].rfidStatus.Equals("7"))
                {
                    for (int j = 0; j < agvCount; j++)
                    {
                        if (agvStatusList[j].rfidStatus.Equals(avoidStop[avoidNum][0].ToString()) && avoidStop[avoidNum][1] == 0)//车号位不为零而且发送位为零，便会发送停止allcommand，并把发送位置为1
                        {
                            lock (avoidStop)
                            {
                                avoidStop[avoidNum][1] = agvStatusList[j].carNum;
                                avoidStop[avoidNum][2] = 0;
                            }
                            break;
                        }
                    }
                    avoidClear[avoidNum] = agvStatusList[i].carNum;
                }
                avoidNum++;

                //54tag需要避让的tag
                if (agvStatusList[i].rfidStatus.Equals("24"))
                {
                    for (int j = 0; j < agvCount; j++)
                    {
                        if (agvStatusList[j].rfidStatus.Equals(avoidStop[avoidNum][0].ToString()) && avoidStop[avoidNum][1] == 0)//车号位不为零而且发送位为零，便会发送停止allcommand，并把发送位置为1
                        {
                            lock (avoidStop)
                            {
                                avoidStop[avoidNum][1] = agvStatusList[j].carNum;
                                avoidStop[avoidNum][2] = 0;
                            }
                            break;
                        }
                    }
                    avoidClear[avoidNum] = agvStatusList[i].carNum;
                }
                avoidNum++;
            }



            for (int i = 0; i < avoidStop.Count; i++)
            {
                if ((avoidStop[i][1] == avoidClear[i] || avoidClear[i] == 0) && avoidStop[i][1] != 0)
                {
                    avoidStop[i][2] = 2;
                }

            }

            int stationA = 0;//A区AGV数量
            int stationB = 0;//B区AGV数量
            int convyerA = 0;//A区辊床是否有AGV
            int convyerB = 0;//B区辊床是否有AGV
            int stationA1Start = 0;// A1区是否有启动AGV
            int stationB1Start = 0;// B1区是否有启动AGV
            int stationA2Start = 0;// A2辊床区是否有启动AGV
            int stationB2Start = 0;// B2辊床区是否有启动AGV
            int chargeSt1 = 0;// 充电站1是否有AGV
            int chargeSt2 = 0;// 充电站2是否有AGV
            int chargeSt3 = 0;// 充电站3是否有AGV
            int loadSt = 0;//上料点是否有agv
            int drOut = 0;//暗室外缓存点是否有AGV

            int AagvGet = 0;
            int BagvGet = 0;
            int AagvEm = 0;
            int BagvEm = 0;
            int DrGet = 0;
            int ReqOut = 0;


            for (int i = 0; i < agvCount; i++)
            {
                if (!agvStart[agvStatusList[i].carNum] && agvStatusList[i].IsRunning)
                {
                    lock (agvStart)
                    {
                        agvStart[agvStatusList[i].carNum] = true;
                    }
                }
                if (agvStatusList[i].rfidStatus != "1" && agvStatusList[i].rfidStatus != "2" && agvStatusList[i].rfidStatus != "3")
                {
                    //agvChargeStop[agvStatusList[i].carNum] = 0;

                }
                if (MainInfo.rfidStation.ContainsKey(agvStatusList[i].rfidStatus))
                {

                    string tagTerminal = MainInfo.rfidStation[agvStatusList[i].rfidStatus];
                    string[] tagSt = tagTerminal.Split('#');

                    //A等待点
                    if (tagSt[0] == "A" && tagSt[1] == "2" && tagSt[2] != "2")
                    {
                        stationA++;
                    }
                    if (tagSt[0] == "A" && tagSt[1] == "1")
                    {
                        stationA++;
                    }



                    if (tagSt[0] == "B" && tagSt[1] == "2" && tagSt[2] != "2")
                    {
                        stationB++;
                    }

                    if (tagSt[0] == "AB")
                    {
                        if (agvStatusList[i].agvRouteNo == aRoute)
                        {
                            stationA++;
                        }
                        else if (tagSt[1] == "1" && agvStatusList[i].agvRouteNo == bRoute)
                        {
                            stationB++;
                        }
                    }

                    if (tagSt[0] == "CR" && agvStatusList[i].IsRunning)
                    {
                        if (agvStatusList[i].agvRouteNo == bRoute)
                        {
                            stationB++;
                        }
                        else if (agvStatusList[i].agvRouteNo == aRoute)
                        {
                            stationA++;
                        }
                    }

                    if (tagTerminal == "A#2#1")
                    {
                        convyerA = agvStatusList[i].carNum;
                    }

                    if (tagTerminal == "DR#1#2")
                    {
                        drOut = agvStatusList[i].carNum;
                    }

                    if (tagTerminal == "B#2#1")
                    {
                        convyerB = agvStatusList[i].carNum;
                    }

                    if (tagSt[0] == "A" && tagSt[1] == "1" && tagSt[2] == "1" && (agvStatusList[i].IsRunning || agvBufferStart[agvStatusList[i].carNum] == 1))
                    {
                        stationA1Start = agvStatusList[i].carNum;
                    }
                    if (tagSt[0] == "A" && tagSt[1] == "1" && tagSt[2] == "2")
                    {
                        stationA1Start = agvStatusList[i].carNum;
                    }

                    if (tagSt[0] == "B" && tagSt[1] == "1" && tagSt[2] == "1" && (agvStatusList[i].IsRunning || agvBufferStart[agvStatusList[i].carNum] == 1))
                    {
                        stationB1Start = agvStatusList[i].carNum;
                    }
                    if (tagSt[0] == "B" && tagSt[1] == "1" && tagSt[2] == "2")
                    {
                        stationB1Start = agvStatusList[i].carNum;
                    }

                    if (tagSt[0] == "A" && tagSt[1] == "2" && tagSt[2] == "1" && (agvStatusList[i].IsRunning || agvBufferStart[agvStatusList[i].carNum] == 1))
                    {
                        stationA2Start = agvStatusList[i].carNum;
                    }
                    if (tagSt[0] == "A" && tagSt[1] == "2" && tagSt[2] == "2")
                    {
                        stationA2Start = agvStatusList[i].carNum;
                    }

                    if (tagSt[0] == "B" && tagSt[1] == "2" && tagSt[2] == "1" && (agvStatusList[i].IsRunning || agvBufferStart[agvStatusList[i].carNum] == 1))
                    {
                        stationB2Start = agvStatusList[i].carNum;
                    }
                    if (tagSt[0] == "B" && tagSt[1] == "2" && tagSt[2] == "2")
                    {
                        stationB2Start = agvStatusList[i].carNum;
                    }

                    if (tagSt.Length > 1 && tagSt[0] == "CH")
                    {
                        if (tagSt[1] == "1")
                        {
                            chargeSt1 = agvStatusList[i].carNum;
                        }
                        else if (tagSt[1] == "2")
                        {
                            chargeSt2 = agvStatusList[i].carNum;
                        }
                        else if (tagSt[1] == "3")
                        {
                            chargeSt3 = agvStatusList[i].carNum;
                        }
                    }
                    if (tagSt[0] == "LD" || (tagTerminal == "CH#3#0" && agvStatusList[i].IsRunning))
                    {
                        loadSt++;
                    }
                    if (tagTerminal == "DR#1#1")
                    {
                        DrGet = 1;
                    }
                    if (tagTerminal == "DR#1#2")
                    {
                        ReqOut = 1;
                    }

                }
                if (agvStatusList[i].rfidStatus == "33" && !agvStatusList[i].IsRunning)
                {
                    AagvGet = 1;
                    if (agvStatusList[i].errorCode != 0)
                    {
                        AagvEm = 1;
                    }
                }
                else if (agvStatusList[i].rfidStatus == "32" && !agvStatusList[i].IsRunning)
                {
                    BagvGet = 1;
                    if (agvStatusList[i].errorCode != 0)
                    {
                        AagvEm = 1;
                    }
                }
            }

            //dkSt.agvGet = DrGet;
            //dkSt.agvReqOut = ReqOut;
            //StList[0].agvGet = AagvGet;
            //StList[1].agvGet = BagvGet;
            //StList[0].agvEm = AagvEm;
            //StList[1].agvEm = BagvEm;
            //===========================2017-3-12-调试301、302做的注释，将来从flash调路径要使用的===============
            //if (chargeSt1 == 0)
            //{
            //    chargeStList[0].ChargeCommand = "0";
            //}
            //if (chargeSt2 == 0)
            //{
            //    chargeStList[1].ChargeCommand = "0";
            //}
            //if (chargeSt3 == 0)
            //{
            //    chargeStList[2].ChargeCommand = "0";
            //}


            for (int i = 0; i < agvCount; i++)
            {
                if (MainInfo.rfidStation.ContainsKey(agvStatusList[i].rfidStatus) && MainInfo.rfidStation[agvStatusList[i].rfidStatus] == "CR#1#0" && !agvStatusList[i].IsRunning && agvStatusList[i].errorCode != 205)
                {
                    ///==============================TANG=========================================///
                    if (stationA < 1 && agvStatusList[i].agvRouteNo != aRoute)
                    {
                        string[] routeStr = new string[1];
                        string taskStr = "A区域";
                        routeStr[0] = aRoute.ToString();
                        lock (sendRfid)
                        {
                            sendRfid[agvStatusList[i].carNum] = 1;
                        }
                        agvTaskRoute(agvStatusList[i], routeStr, taskStr, 0);
                    }
                    else if (stationA < 1 && agvStatusList[i].agvRouteNo == aRoute)
                    {
                        lock (agvBufferStart)
                        {
                            agvBufferStart[agvStatusList[i].carNum] = 1;
                        }
                        stationA++;
                    }
                    else if (stationB < 1 && agvStatusList[i].agvRouteNo != bRoute)
                    {
                        string[] routeStr = new string[1];
                        string taskStr = "B区域";
                        routeStr[0] = bRoute.ToString();
                        lock (sendRfid)
                        {
                            sendRfid[agvStatusList[i].carNum] = 1;
                        }
                        agvTaskRoute(agvStatusList[i], routeStr, taskStr, 0);
                    }
                    else if (stationB < 1 && agvStatusList[i].agvRouteNo == bRoute)
                    {
                        lock (agvBufferStart)
                        {
                            agvBufferStart[agvStatusList[i].carNum] = 1;
                        }
                        stationB++;
                    }
                    else if (stationA < 2 && agvStatusList[i].agvRouteNo != aRoute)
                    {
                        string[] routeStr = new string[1];
                        string taskStr = "A区域";
                        routeStr[0] = aRoute.ToString();
                        lock (sendRfid)
                        {
                            sendRfid[agvStatusList[i].carNum] = 1;
                        }
                        agvTaskRoute(agvStatusList[i], routeStr, taskStr, 0);
                    }
                    else if (stationA < 2 && agvStatusList[i].agvRouteNo == aRoute)
                    {
                        lock (agvBufferStart)
                        {
                            agvBufferStart[agvStatusList[i].carNum] = 1;
                        }
                        stationA++;
                    }
                    else if (stationB < 2 && agvStatusList[i].agvRouteNo != bRoute)
                    {
                        string[] routeStr = new string[1];
                        string taskStr = "B区域";
                        routeStr[0] = bRoute.ToString();
                        lock (sendRfid)
                        {
                            sendRfid[agvStatusList[i].carNum] = 1;
                        }
                        agvTaskRoute(agvStatusList[i], routeStr, taskStr, 0);
                    }
                    else if (stationB < 2 && agvStatusList[i].agvRouteNo == bRoute)
                    {
                        lock (agvBufferStart)
                        {
                            agvBufferStart[agvStatusList[i].carNum] = 1;
                        }
                        stationB++;
                    }
                }
                else if (agvStatusList[i].agvRouteNo != aRoute && agvStatusList[i].agvRouteNo != bRoute)
                {
                    string[] routeStr = new string[1];
                    string taskStr = "A区域";
                    routeStr[0] = aRoute.ToString();
                    lock (sendRfid)
                    {
                        sendRfid[agvStatusList[i].carNum] = 1;
                    }
                    agvTaskRoute(agvStatusList[i], routeStr, taskStr, 2);
                }
                ///============================================TANG========================================///   

                if (agvStatusList[i].rfidStatus == "33" && stationB1Start == 0 && !agvStatusList[i].IsRunning && agvStatusList[i].errorCode != 205)
                {
                    //StList[0].agvGet = 1;
                    //if (StList[0].outCmd == 1)//A区是否可以出发
                    //{
                    //    lock (agvBufferStart)
                    //    {
                    //        agvBufferStart[agvStatusList[i].carNum] = 1;
                    //    }
                    //    stationA2Start = agvStatusList[i].carNum;
                    //    stationA++;
                    //}

                }

                if (agvStatusList[i].rfidStatus == "35" && stationA2Start == 0 && !agvStatusList[i].IsRunning && agvStatusList[i].errorCode != 205)
                {
                    lock (agvBufferStart)
                    {
                        agvBufferStart[agvStatusList[i].carNum] = 1;
                    }
                    stationB1Start = agvStatusList[i].carNum;
                }

                if (agvStatusList[i].rfidStatus == "32" && stationA1Start == 0 && !agvStatusList[i].IsRunning && agvStatusList[i].errorCode != 205)
                {
                    //StList[1].agvGet = 1;
                    //if (StList[1].outCmd == 1)//B区是否可以出发
                    //{
                    //    lock (agvBufferStart)
                    //    {
                    //        agvBufferStart[agvStatusList[i].carNum] = 1;
                    //    }
                    //    stationB2Start = agvStatusList[i].carNum;
                    //    stationB++;
                    //}

                }

                if (agvStatusList[i].rfidStatus == "17" && stationB2Start == 0 && !agvStatusList[i].IsRunning && agvStatusList[i].errorCode != 205)
                {
                    lock (agvBufferStart)
                    {
                        agvBufferStart[agvStatusList[i].carNum] = 1;
                    }
                    stationA1Start = agvStatusList[i].carNum;
                }

                if (agvStatusList[i].rfidStatus == "46" && convyerB == 0 && !agvStatusList[i].IsRunning && agvStatusList[i].errorCode != 205)
                {
                    lock (agvBufferStart)
                    {
                        agvBufferStart[agvStatusList[i].carNum] = 1;
                    }
                }

                if (agvStatusList[i].rfidStatus == "47" && convyerA == 0 && !agvStatusList[i].IsRunning && agvStatusList[i].errorCode != 205)
                {
                    lock (agvBufferStart)
                    {
                        agvBufferStart[agvStatusList[i].carNum] = 1;
                    }
                }

                //if (MainInfo.rfidStation.ContainsKey(agvStatusList[i].rfidStatus) && MainInfo.rfidStation[agvStatusList[i].rfidStatus] == "DR#1#0" && dkSt.allowIn == 1 && dkSt.agvGet == 0 && !agvStatusList[i].IsRunning && agvStatusList[i].errorCode != 205)
                //{
                //    lock (agvBufferStart)
                //    {
                //        agvBufferStart[agvStatusList[i].carNum] = 1;
                //    }
                //}

                //if (MainInfo.rfidStation.ContainsKey(agvStatusList[i].rfidStatus) && MainInfo.rfidStation[agvStatusList[i].rfidStatus] == "DR#1#1" && dkSt.agvPass == 1 && dkSt.allowOut == 1 && drOut == 0 && !agvStatusList[i].IsRunning && agvStatusList[i].errorCode != 205)
                //{
                //    lock (agvBufferStart)
                //    {
                //        agvBufferStart[agvStatusList[i].carNum] = 1;
                //    }
                //}

                if (MainInfo.rfidStation.ContainsKey(agvStatusList[i].rfidStatus) && MainInfo.rfidStation[agvStatusList[i].rfidStatus] == "CH#0#0" && chargeSt1 == 0 && !agvStatusList[i].IsRunning && agvStatusList[i].errorCode != 205)
                {
                    //if ((chargeStList[0].InOutStation == "1" && chargeStList[0].Auto == "1") || !ChargeForm.chSt1Use)
                    //{
                    //    lock (agvBufferStart)
                    //    {
                    //        agvBufferStart[agvStatusList[i].carNum] = 1;
                    //    }
                    //}
                }

                //if (MainInfo.rfidStation.ContainsKey(agvStatusList[i].rfidStatus) && MainInfo.rfidStation[agvStatusList[i].rfidStatus] == "CH#1#0" && agvStatusList[i].errorCode != 205)
                //{
                //    if (chargeSt2 == 0 && !agvStatusList[i].IsRunning)
                //    {
                //        if (((chargeStList[0].InOutStation == "1" && chargeStList[0].Auto == "1") || !ChargeForm.chSt1Use) && ((chargeStList[1].InOutStation == "1" && chargeStList[1].Auto == "1") || !ChargeForm.chSt2Use))
                //        {
                //            lock (agvBufferStart)
                //            {
                //                agvBufferStart[agvStatusList[i].carNum] = 1;
                //            }
                //            chargeSt2 = agvStatusList[i].carNum;
                //        }

                //        lock (agvRelayClose)
                //        {
                //            agvRelayClose[agvStatusList[i].carNum] = 2;
                //        }

                //        if (agvRelayStatus[agvStatusList[i].carNum] == 0)//继电器断开
                //        {
                //            chargeStList[0].ChargeCommand = "0";
                //        }
                //    }
                //    else if (chargeSt2 != 0 && !agvStatusList[i].IsRunning)
                //    {
                //        chargeStList[0].ChargeCommand = "1";
                //        if (chargeStList[0].ChargeStatus != "1" && chargeStList[0].InOutStation == "2")
                //        {
                //            lock (agvRelayClose)
                //            {
                //                agvRelayClose[agvStatusList[i].carNum] = 1;
                //            }
                //        }
                //        else if (chargeStList[0].ChargeStatus == "1")
                //        {
                //            lock (agvRelayClose)
                //            {
                //                agvRelayClose[agvStatusList[i].carNum] = 0;
                //            }
                //        }
                //    }
                //}

                //if (MainInfo.rfidStation.ContainsKey(agvStatusList[i].rfidStatus) && MainInfo.rfidStation[agvStatusList[i].rfidStatus] == "CH#2#0" && agvStatusList[i].errorCode != 205)
                //{
                //    if (chargeSt3 == 0 && !agvStatusList[i].IsRunning)
                //    {

                //        if (((chargeStList[1].InOutStation == "1" && chargeStList[1].Auto == "1") || !ChargeForm.chSt2Use) && ((chargeStList[2].InOutStation == "1" && chargeStList[2].Auto == "1") || !ChargeForm.chSt3Use))
                //        {
                //            lock (agvBufferStart)
                //            {
                //                agvBufferStart[agvStatusList[i].carNum] = 1;
                //            }
                //            chargeSt3 = agvStatusList[i].carNum;
                //        }

                //        lock (agvRelayClose)
                //        {
                //            agvRelayClose[agvStatusList[i].carNum] = 2;
                //        }

                //        if (agvRelayStatus[agvStatusList[i].carNum] == 0)//继电器断开
                //        {
                //            chargeStList[1].ChargeCommand = "0";
                //        }
                //    }
                //    else if (chargeSt3 != 0 && !agvStatusList[i].IsRunning)
                //    {
                //        chargeStList[1].ChargeCommand = "1";
                //        if (chargeStList[1].ChargeStatus != "1" && chargeStList[1].InOutStation == "2")
                //        {
                //            lock (agvRelayClose)
                //            {
                //                agvRelayClose[agvStatusList[i].carNum] = 1;
                //            }
                //        }
                //        else if (chargeStList[1].ChargeStatus == "1")
                //        {
                //            lock (agvRelayClose)
                //            {
                //                agvRelayClose[agvStatusList[i].carNum] = 0;
                //            }
                //        }

                //    }
                //}

                //if (MainInfo.rfidStation.ContainsKey(agvStatusList[i].rfidStatus) && MainInfo.rfidStation[agvStatusList[i].rfidStatus] == "CH#3#0" && agvStatusList[i].errorCode != 205)
                //{
                //    if (loadSt < 2 && !agvStatusList[i].IsRunning)
                //    {
                //        if ((chargeStList[2].InOutStation == "1" && chargeStList[2].Auto == "1") || !ChargeForm.chSt3Use)
                //        {
                //            lock (agvBufferStart)
                //            {
                //                agvBufferStart[agvStatusList[i].carNum] = 1;
                //            }
                //            loadSt = agvStatusList[i].carNum;
                //        }

                //        lock (agvRelayClose)
                //        {
                //            agvRelayClose[agvStatusList[i].carNum] = 2;
                //        }

                //        if (agvRelayStatus[agvStatusList[i].carNum] == 0)//继电器断开
                //        {
                //            chargeStList[2].ChargeCommand = "0";
                //        }
                //    }
                //    else if (loadSt != 0 && !agvStatusList[i].IsRunning)
                //    {
                //        chargeStList[2].ChargeCommand = "1";
                //        if (chargeStList[2].ChargeStatus != "1" && chargeStList[2].InOutStation == "2")
                //        {
                //            lock (agvRelayClose)
                //            {
                //                agvRelayClose[agvStatusList[i].carNum] = 1;
                //            }
                //        }
                //        else if (chargeStList[2].ChargeStatus == "1")
                //        {
                //            lock (agvRelayClose)
                //            {
                //                agvRelayClose[agvStatusList[i].carNum] = 0;
                //            }
                //        }
                //    }
                //}

                if (agvRouteNameList[agvStatusList[i].carNum][0] == "" && agvStatusList[i].agvRouteNo != 0)
                {
                    for (int j = 0; j < routeDt.Rows.Count; j++)
                    {
                        if (routeDt.Rows[j][1].ToString() == agvStatusList[i].agvRouteNo.ToString())
                        {
                            agvRouteNameList[agvStatusList[i].carNum][0] = routeDt.Rows[j][2].ToString();
                            break;
                        }
                    }
                }

                if (agvRouteNameList[agvStatusList[i].carNum][0] == "" && agvStatusList[i].agvRouteNo != 0)
                {

                }
            }


            //=========================================2017-3-12-END==============================================

            //用于判断是不是在定时时间段内
            long dtNowLong = long.Parse(DateTime.Now.ToString("HHmmss"));
            bool dtNowBool = false;
            for (int i = 0; i < 6; i++)
            {
                //long stLong = long.Parse(TimerInfo.startTimeList[i].ToString("HHmmss"));
                //long etLong = long.Parse(TimerInfo.endTimeList[i].ToString("HHmmss"));
                //if (dtNowLong > stLong && dtNowLong < etLong && TimerInfo.useList[i] == 1)
                //{
                //    dtNowBool = true;
                //    break;
                //}
            }

            if (dtNowBool && MainInfo.fixedTime != 0)
            {
                if (MainInfo.allStart == 1)
                {
                    MainInfo.allStart = 0;
                    if (MainInfo.fixedTime > 0)
                    {
                        MainInfo.agvRunTime = UTC.ConvertDateTimeLong(DateTime.Now) + MainInfo.fixedTime;

                    }
                    for (int i = 0; i < agvCount; i++)
                    {
                        if (int.Parse(agvStatusList[i].rfidStatus) >= 1 && int.Parse(agvStatusList[i].rfidStatus) <= 13)
                            agvTimerStart[agvStatusList[i].carNum] = 1;
                    }
                }
                if (MainInfo.fixedTime > 0)
                {
                    if (MainInfo.agvRunTime == 0)
                    {
                        MainInfo.agvRunTime = UTC.ConvertDateTimeLong(DateTime.Now) + MainInfo.fixedTime;
                    }
                    else if (MainInfo.agvRunTime <= UTC.ConvertDateTimeLong(DateTime.Now))
                    {
                        MainInfo.allStart = 1;
                    }
                }
                pauseInt = 0;
            }
            else if (MainInfo.fixedTime != 0)
            {
                if (pauseInt == 0)
                {
                    pauseInt = 1;

                    long pauseTime = 0;
                    long timeNow = UTC.ConvertDateTimeLong(DateTime.Now);
                    pauseTime = MainInfo.agvRunTime - timeNow;


                    long minInterval = 9999999;

                    DateTime RecentStTime = DateTime.Now;//最近的开始时间
                    DateTime dt = DateTime.Now;//现在的时间
                    int resetInt = 1;//用来判断现在的时间是不是已经超过最晚的定时启动结束时间，1为是，0为否
                    for (int i = 0; i < 6; i++)
                    {
                        //long stLong = long.Parse(TimerInfo.startTimeList[i].ToString("HHmmss"));
                        //long interval = stLong - dtNowLong;
                        //long etLong = long.Parse(TimerInfo.endTimeList[i].ToString("HHmmss"));
                        //if (dtNowLong < etLong && TimerInfo.useList[i] == 1)
                        //{
                        //    resetInt = 0;
                        //}
                        //if (TimerInfo.useList[i] == 1 && interval > 0 && interval < minInterval)
                        //{
                        //    minInterval = interval;
                        //    RecentStTime = TimerInfo.startTimeList[i];
                        //}

                    }

                    if (resetInt == 1)
                    {
                        MainInfo.agvRunTime = 0;
                    }
                    else
                    {

                        string recentStTime = dt.ToShortDateString() + " " + RecentStTime.ToShortTimeString();
                        MainInfo.agvRunTime = UTC.ConvertDateTimeLong(DateTime.Parse(recentStTime)) + pauseTime;
                    }


                }
            }
        }

        /// <summary>
        /// 发送agv指令
        /// </summary>
        /// <param name="AgvStatus"></param>
        /// <param name="AgvRoute"></param>
        /// <param name="AgvTask"></param>
        /// <param name="HasButton">0表示该点没有按钮，1表示有按钮, 2表示按车上按钮,3表示运行中调路径</param>
        private void agvTaskRoute(CarStatus AgvStatus, string[] AgvRoute, string AgvTask, int HasButton)
        {
            bool result = true;

            //bool result = false;
            //for (int i = 0; i < AgvRoute.Length; i++)
            //{
            //    if (AgvRoute[i] != agvRouteNameList[AgvStatus.carNum][i])
            //    {
            //        result = true;
            //        break;
            //    }
            //}


            if (result)
            {
                lock (agvRouteNameList)
                {
                    agvRouteNameList[AgvStatus.carNum] = AgvRoute;
                }

                SelRoute(AgvRoute, AgvStatus.carNum);
                lock (agvTaskList)
                {
                    agvTaskList[AgvStatus.carNum] = AgvTask;
                }

                lock (agvRouteNoList)
                {
                    agvRouteNoList[AgvStatus.carNum] = SearchAgvTaskNo(AgvRoute[0]);
                }

                lock (agvActive)
                {
                    agvActive[AgvStatus.carNum] = true;
                }

                lock (agvStart)
                {
                    agvStart[AgvStatus.carNum] = false;
                }

                lock (agvSendResult)
                {
                    switch (HasButton)
                    {
                        case 0:
                            agvSendResult[AgvStatus.carNum][0] = 0;
                            agvSendResult[AgvStatus.carNum][1] = 0;
                            agvSendResult[AgvStatus.carNum][2] = 0;
                            break;
                        case 1:
                            agvSendResult[AgvStatus.carNum][0] = 0;
                            agvSendResult[AgvStatus.carNum][1] = 0;
                            agvSendResult[AgvStatus.carNum][2] = 2;
                            break;
                        case 2:
                            agvSendResult[AgvStatus.carNum][0] = 0;
                            agvSendResult[AgvStatus.carNum][1] = 0;
                            agvSendResult[AgvStatus.carNum][2] = -1;
                            break;
                        case 3:
                            agvSendResult[AgvStatus.carNum][0] = 0;
                            agvSendResult[AgvStatus.carNum][1] = -1;
                            agvSendResult[AgvStatus.carNum][2] = -1;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private int SearchAgvTaskNo(string AgvTask)
        {
            int agvTaskNo = 1;
            foreach (DataRow dr in routeDt.Rows)
            {
                if (Convert.ToString(dr[2]) == AgvTask)
                {
                    agvTaskNo = Convert.ToInt32(dr[1]);
                    break;
                }
            }
            return agvTaskNo;

        }


        public List<RFIDClass> GetAGVroute(int i)
        {
            List<RFIDClass> rc = new List<RFIDClass>();
            rc = (List<RFIDClass>)agvRfidListHash[i];
            return rc;
        }

        public void SelRoute(string[] RouteName, int AgvNo)
        {
            List<RFIDClass> rc = new List<RFIDClass>();
            int routeCount = RouteName.Length;
            DataRow[] routeDr = new DataRow[routeCount];
            for (int i = 0; i < routeCount; i++)
            {
                foreach (DataRow dr in routeDt.Rows)
                {
                    if (dr[2].ToString() == RouteName[i])
                    {
                        routeDr[i] = dr;
                        break;
                    }
                }
            }
            rc = DrToRfidList(routeDr);

            agvRfidListHash[AgvNo] = rc;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="CarNum"></param>
        ///// <param name="SpeedNo"></param>
        ///// <param name="Model">0为slow,1为stop，2为slow启动，3为stop启动</param>
        //private void SetAllCommand(int CarNum, int SpeedNo, int Model)
        //{
        //    agvAllCommand[CarNum].directionSwitch = 0;
        //    agvAllCommand[CarNum].hookSwitch = 0;
        //    agvAllCommand[CarNum].masterSwitch = 0;
        //    agvAllCommand[CarNum].pbsNo = 999;
        //    agvAllCommand[CarNum].programNo = 999;
        //    agvAllCommand[CarNum].restSwitch = 0;
        //    agvAllCommand[CarNum].rfidID = 999;
        //    agvAllCommand[CarNum].stopMode = 0;
        //    agvAllCommand[CarNum].trunSwitch = 0;
        //    switch (Model)
        //    {
        //        case 0:
        //            agvAllCommand[CarNum].speedNo = 1;
        //            agvAllCommand[CarNum].startSwitch = 0;
        //            break;
        //        case 1:
        //            agvAllCommand[CarNum].speedNo = 999;
        //            agvAllCommand[CarNum].startSwitch = 2;
        //            break;
        //        case 2:
        //            agvAllCommand[CarNum].speedNo = SpeedNo;
        //            agvAllCommand[CarNum].startSwitch = 0;
        //            break;
        //        case 3:
        //            agvAllCommand[CarNum].speedNo = 999;
        //            agvAllCommand[CarNum].startSwitch = 1;
        //            break;
        //        default:
        //            agvAllCommand[CarNum].speedNo = 999;
        //            agvAllCommand[CarNum].startSwitch = 0;
        //            break;
        //    }
        //}


        public List<RFIDClass> DrToRfidList(DataRow[] dr)
        {
            List<RFIDClass> rfidList = new List<RFIDClass>();
            for (int i = 0; i < dr.Length; i++)
            {
                List<string> rfid = dr[i][4].ToString().Split(',').ToList<string>();
                List<string> speed = dr[i][5].ToString().Split(',').ToList<string>();
                List<string> stop = dr[i][6].ToString().Split(',').ToList<string>();
                List<string> turn = dr[i][7].ToString().Split(',').ToList<string>();
                List<string> direction = dr[i][8].ToString().Split(',').ToList<string>();
                List<string> pbs = dr[i][9].ToString().Split(',').ToList<string>();
                List<string> hook = dr[i][11].ToString().Split(',').ToList<string>();
                List<string> ChangeProgram = dr[i][12].ToString().Split(',').ToList<string>();

                if (i == 0)
                {
                    RFIDClass rf = new RFIDClass();
                    rf.rfidID = 0;
                    rf.startSwitch = 0;
                    rf.restSwitch = 0;
                    rf.masterSwitch = 0;
                    rf.hookSwitch = int.Parse(hook[0]);
                    rf.directionSwitch = int.Parse(direction[0]);
                    rf.programNo = int.Parse(ChangeProgram[0]);
                    rf.speedNo = (speed[0] == "10" ? 999 : int.Parse(speed[0]));
                    rf.pbsNo = (pbs[0] == "16" ? 999 : int.Parse(pbs[0]));
                    rf.stopMode = 0;
                    rf.trunSwitch = int.Parse(turn[0]);
                    rfidList.Add(rf);
                }

                for (int j = 1; j < rfid.Count; j++)
                {
                    if (stop[j] == "-99")
                    {
                        RFIDClass rf = new RFIDClass();
                        rf.rfidID = int.Parse(rfid[j]);
                        rf.startSwitch = 2;
                        rf.restSwitch = 0;
                        rf.masterSwitch = 0;
                        rf.hookSwitch = (hook[j] == "2" ? 0 : 1);
                        rf.directionSwitch = (direction[j] == "2" ? 0 : 1);
                        rf.programNo = 999;
                        rf.speedNo = (speed[j] == "10" ? 999 : int.Parse(speed[j]));
                        rf.pbsNo = (pbs[j] == "16" ? 999 : int.Parse(pbs[j]));
                        rf.stopMode = 0;
                        rf.trunSwitch = int.Parse(turn[j]);
                        rfidList.Add(rf);
                    }
                    else if (hook[j] == "2" && direction[j] == "2" && ChangeProgram[j] == "999")
                    {
                        RFIDClass rf = new RFIDClass();
                        rf.rfidID = int.Parse(rfid[j]);
                        rf.startSwitch = 0;
                        rf.restSwitch = 0;
                        rf.masterSwitch = 0;
                        rf.hookSwitch = 0;
                        rf.directionSwitch = 0;
                        rf.programNo = 999;
                        rf.speedNo = (speed[j] == "10" ? 999 : int.Parse(speed[j]));
                        rf.pbsNo = (pbs[j] == "16" ? 999 : int.Parse(pbs[j]));
                        rf.stopMode = 0;
                        rf.trunSwitch = int.Parse(turn[j]);
                        rfidList.Add(rf);
                    }
                    else
                    {
                        //有升hook或者反向 停止动作 
                        RFIDClass rf = new RFIDClass();
                        rf.rfidID = int.Parse(rfid[j]);

                        rf.startSwitch = 2;
                        rf.restSwitch = 0;
                        rf.masterSwitch = 0;
                        rf.hookSwitch = (hook[j] == "2" ? 0 : 1);
                        rf.directionSwitch = (direction[j] == "2" ? 0 : 1);
                        rf.programNo = 999;
                        rf.speedNo = 999;//(speed[j] == "10" ? 999 : int.Parse(speed[j]));
                        rf.pbsNo = (pbs[j] == "16" ? 999 : int.Parse(pbs[j]));
                        rf.stopMode = 0;
                        rf.trunSwitch = 0;
                        rfidList.Add(rf);

                        //转弯 启动动作 不是最后指令
                        if (!(j == rfid.Count - 1 && i == dr.Length - 1))
                        {
                            RFIDClass rf1 = new RFIDClass();
                            rf1.rfidID = int.Parse(rfid[j]);
                            rf1.startSwitch = 1;
                            rf1.restSwitch = 0;
                            rf1.masterSwitch = 0;
                            rf1.hookSwitch = 0;
                            rf1.directionSwitch = 0;
                            rf1.programNo = int.Parse(ChangeProgram[j]);
                            rf1.speedNo = (speed[j] == "10" ? 999 : int.Parse(speed[j]));
                            rf1.pbsNo = (pbs[j] == "16" ? 999 : int.Parse(pbs[j]));
                            rf1.stopMode = 0;
                            rf1.trunSwitch = int.Parse(turn[j]);
                            rfidList.Add(rf1);
                        }
                    }
                }
            }
            rfidList[rfidList.Count - 1].startSwitch = 2;
            rfidList[0].rfidID = 0;
            return rfidList;
        }

        //private List<int> AgvTask(string TaskName)
        //{
        //    List<int> agvList = new List<int>();
        //    int agvCount = agvTaskList.Count;
        //    for (int i = 0; i < MainInfo.agvNo.Count; i++)//两个任务同为上料或ts以agvtask为准，两个任务不是相同的以sendtasklist为准
        //    {
        //        if (!string.IsNullOrEmpty(sendTaskList[int.Parse(MainInfo.agvNo[i])]) && sendTaskList[int.Parse(MainInfo.agvNo[i])].Substring(0,2).Equals("sl") && (agvTaskList[int.Parse(MainInfo.agvNo[i])].Substring(0,2).Equals("ts") || agvTaskList[int.Parse(MainInfo.agvNo[i])].Substring(0, 2).Equals("pk")))
        //        {
        //            if (sendTaskList[int.Parse(MainInfo.agvNo[i])] == TaskName)
        //            {
        //                agvList.Add(int.Parse(MainInfo.agvNo[i]));
        //            }
        //        }
        //        else
        //        {
        //            if (agvTaskList[int.Parse(MainInfo.agvNo[i])] == TaskName)
        //            {
        //                agvList.Add(int.Parse(MainInfo.agvNo[i]));
        //            }
        //        }
        //    }
        //    return agvList;
        //}


        private List<int> AgvTask(string TaskName, int CarNum)
        {
            List<int> agvList = new List<int>();
            //int agvCount = agvTaskList.Count;
            for (int i = 0; i < MainInfo.agvNo.Count; i++)//两个任务同为上料或ts以agvtask为准，两个任务不是相同的以sendtasklist为准
            {
                if (MainInfo.agvNo[i] != CarNum.ToString())
                {
                    if (sendTaskList[int.Parse(MainInfo.agvNo[i])] == TaskName || sendTaskList[int.Parse(MainInfo.agvNo[i])] == TaskName)
                    {
                        agvList.Add(int.Parse(MainInfo.agvNo[i]));
                    }


                    //if (!string.IsNullOrEmpty(sendTaskList[int.Parse(MainInfo.agvNo[i])]) && sendTaskList[int.Parse(MainInfo.agvNo[i])].Substring(0, 2).Equals("sl") && (agvTaskList[int.Parse(MainInfo.agvNo[i])].Substring(0, 2).Equals("ts") || agvTaskList[int.Parse(MainInfo.agvNo[i])].Substring(0, 2).Equals("pk")))
                    //{
                    //    if (sendTaskList[int.Parse(MainInfo.agvNo[i])] == TaskName)
                    //    {
                    //        agvList.Add(int.Parse(MainInfo.agvNo[i]));
                    //    }
                    //}
                    //else
                    //{
                    //    if (agvTaskList[int.Parse(MainInfo.agvNo[i])] == TaskName)
                    //    {
                    //        agvList.Add(int.Parse(MainInfo.agvNo[i]));
                    //    }
                    //}
                }

            }
            return agvList;
        }


        ///// <summary>
        ///// 写入agv任务和线路到ini文件
        ///// </summary>
        ///// <param name="AgvNum">agv小车的序号</param>
        ///// <param name="RT">0表示route，1表示task</param>
        ///// <param name="RTName">route或task名称</param>
        //public void writeRouteTaskIni(string AgvNum, int RT, string RTName)
        //{
        //    if (!File.Exists(taskRouteIniPath))
        //    {
        //        File.Create(taskRouteIniPath);
        //    }
        //    if (RT == 0)
        //    {
        //        IniFile.WriteIniData("AGV" + AgvNum, "route", RTName, taskRouteIniPath);
        //    }
        //    else if (RT == 1)
        //    {
        //        IniFile.WriteIniData("AGV" + AgvNum, "task", RTName, taskRouteIniPath);
        //    }
        //}
    }
}
