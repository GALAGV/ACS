using AGVDLL;
using AGVSystem.Model.LogicData;
using AGVSystem.Model.MapData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AGVSystem.DispatchLogic
{
    public class CalRoute
    {
        public ConcurrentDictionary<int, AgvInfo> agvInfoDic = new ConcurrentDictionary<int, AgvInfo>(); //AGV信息集合
        DataTable routeDt = new DataTable();
        public List<int[]> avoidStop = new List<int[]>();//停止避让
        public List<int> avoidClear = new List<int>();//停止避让的阻碍路径是否已经清空的标志位
        public void sendRouteDt(DataTable RouteDt)
        {
            routeDt = RouteDt;
            agvInfoDic.Clear();
            avoidStop.Clear();
            avoidClear.Clear();
            foreach (string i in MainInfo.agvNo)
            {
                int agvNum = int.Parse(i);
                agvInfoDic.GetOrAdd(agvNum, new AgvInfo());
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
            foreach (int Key in agvInfoDic.Keys)
            {
                if (agvInfoDic[Key].agvStart && !agvInfoDic[Key].isRunning)
                {
                    agvTaskRoute(agvInfoDic[Key].agvNumber, "51-63","测试",0,0);
                }
            }



            //int tsAgvCount = 0;//ts区域agv数量
            //for (int i = 0; i < agvCount; i++)
            //{
            //    if (agvStatusList[i].carNum == 0)
            //    {
            //        //agvStatusList[i].carNum = int.Parse(MainInfo.agvNo[i]);
            //    }
            //    if (string.IsNullOrEmpty(agvStatusList[i].rfidStatus))
            //    {
            //        agvStatusList[i].rfidStatus = "-53328";
            //    }

            //    //ts区域agv数量计数
            //    //if (int.Parse(agvStatusList[i].rfidStatus) > 86 && int.Parse(agvStatusList[i].rfidStatus) < 100)
            //    //{
            //    //    tsAgvCount++;
            //    //}
            //}

            //for (int i = 0; i < avoidStop.Count; i++)
            //{
            //    for (int j = 0; j < agvCount; j++)
            //    {
            //        if (agvStatusList[j].carNum == avoidStop[i][1] && agvStatusList[j].IsRunning)
            //        {
            //            lock (avoidStop)
            //            {
            //                //avoidStop[i][1] = 0;
            //                avoidStop[i][2] = 0;
            //            }
            //        }
            //        if (agvStatusList[j].carNum == avoidStop[i][1] && avoidStop[i][0].ToString() != agvStatusList[j].rfidStatus)
            //        {
            //            lock (avoidStop)
            //            {
            //                avoidStop[i][2] = 2;
            //            }
            //        }
            //    }
            //    avoidClear[i] = 0;
            //}

            //for (int i = 0; i < agvCount; i++)
            //{
            //    int avoidNum = 0;

            //    //6tag需要避让的tag
            //    if (agvStatusList[i].rfidStatus.Equals("7"))
            //    {
            //        for (int j = 0; j < agvCount; j++)
            //        {
            //            if (agvStatusList[j].rfidStatus.Equals(avoidStop[avoidNum][0].ToString()) && avoidStop[avoidNum][1] == 0)//车号位不为零而且发送位为零，便会发送停止allcommand，并把发送位置为1
            //            {
            //                lock (avoidStop)
            //                {
            //                    avoidStop[avoidNum][1] = agvStatusList[j].carNum;
            //                    avoidStop[avoidNum][2] = 0;
            //                }
            //                break;
            //            }
            //        }
            //        avoidClear[avoidNum] = agvStatusList[i].carNum;
            //    }
            //    avoidNum++;

            //    //54tag需要避让的tag
            //    if (agvStatusList[i].rfidStatus.Equals("24"))
            //    {
            //        for (int j = 0; j < agvCount; j++)
            //        {
            //            if (agvStatusList[j].rfidStatus.Equals(avoidStop[avoidNum][0].ToString()) && avoidStop[avoidNum][1] == 0)//车号位不为零而且发送位为零，便会发送停止allcommand，并把发送位置为1
            //            {
            //                lock (avoidStop)
            //                {
            //                    avoidStop[avoidNum][1] = agvStatusList[j].carNum;
            //                    avoidStop[avoidNum][2] = 0;
            //                }
            //                break;
            //            }
            //        }
            //        avoidClear[avoidNum] = agvStatusList[i].carNum;
            //    }
            //    avoidNum++;
            //}

            //for (int i = 0; i < avoidStop.Count; i++)
            //{
            //    if ((avoidStop[i][1] == avoidClear[i] || avoidClear[i] == 0) && avoidStop[i][1] != 0)
            //    {
            //        avoidStop[i][2] = 2;
            //    }
            //}

            //for (int i = 0; i < agvCount; i++)
            //{
            //if (!agvStart[agvStatusList[i].carNum] && agvStatusList[i].IsRunning)
            //{
            //    lock (agvStart)
            //    {
            //        agvStart[agvStatusList[i].carNum] = true;
            //    }
            //}
            //if (agvStatusList[i].rfidStatus != "1" && agvStatusList[i].rfidStatus != "2" && agvStatusList[i].rfidStatus != "3")
            //{
            //    //agvChargeStop[agvStatusList[i].carNum] = 0;

            //}
            //if (MainInfo.rfidStation.ContainsKey(agvStatusList[i].rfidStatus))
            //{

            //    string tagTerminal = MainInfo.rfidStation[agvStatusList[i].rfidStatus];
            //    string[] tagSt = tagTerminal.Split('#');

            //    //A等待点
            //    if (tagSt[0] == "A" && tagSt[1] == "2" && tagSt[2] != "2")
            //    {
            //        stationA++;
            //    }
            //    if (tagSt[0] == "A" && tagSt[1] == "1")
            //    {
            //        stationA++;
            //    }

            //    if (tagSt[0] == "B" && tagSt[1] == "2" && tagSt[2] != "2")
            //    {
            //        stationB++;
            //    }

            //    if (tagSt[0] == "AB")
            //    {
            //        if (agvStatusList[i].agvRouteNo == aRoute)
            //        {
            //            stationA++;
            //        }
            //        else if (tagSt[1] == "1" && agvStatusList[i].agvRouteNo == bRoute)
            //        {
            //            stationB++;
            //        }
            //    }

            //    if (tagSt[0] == "CR" && agvStatusList[i].IsRunning)
            //    {
            //        if (agvStatusList[i].agvRouteNo == bRoute)
            //        {
            //            stationB++;
            //        }
            //        else if (agvStatusList[i].agvRouteNo == aRoute)
            //        {
            //            stationA++;
            //        }
            //    }

            //    if (tagTerminal == "A#2#1")
            //    {
            //        convyerA = agvStatusList[i].carNum;
            //    }

            //    if (tagTerminal == "DR#1#2")
            //    {
            //        drOut = agvStatusList[i].carNum;
            //    }

            //    if (tagTerminal == "B#2#1")
            //    {
            //        convyerB = agvStatusList[i].carNum;
            //    }

            //    if (tagSt[0] == "A" && tagSt[1] == "1" && tagSt[2] == "1" && (agvStatusList[i].IsRunning || agvBufferStart[agvStatusList[i].carNum] == 1))
            //    {
            //        stationA1Start = agvStatusList[i].carNum;
            //    }
            //    if (tagSt[0] == "A" && tagSt[1] == "1" && tagSt[2] == "2")
            //    {
            //        stationA1Start = agvStatusList[i].carNum;
            //    }

            //    if (tagSt[0] == "B" && tagSt[1] == "1" && tagSt[2] == "1" && (agvStatusList[i].IsRunning || agvBufferStart[agvStatusList[i].carNum] == 1))
            //    {
            //        stationB1Start = agvStatusList[i].carNum;
            //    }
            //    if (tagSt[0] == "B" && tagSt[1] == "1" && tagSt[2] == "2")
            //    {
            //        stationB1Start = agvStatusList[i].carNum;
            //    }

            //    if (tagSt[0] == "A" && tagSt[1] == "2" && tagSt[2] == "1" && (agvStatusList[i].IsRunning || agvBufferStart[agvStatusList[i].carNum] == 1))
            //    {
            //        stationA2Start = agvStatusList[i].carNum;
            //    }
            //    if (tagSt[0] == "A" && tagSt[1] == "2" && tagSt[2] == "2")
            //    {
            //        stationA2Start = agvStatusList[i].carNum;
            //    }

            //    if (tagSt[0] == "B" && tagSt[1] == "2" && tagSt[2] == "1" && (agvStatusList[i].IsRunning || agvBufferStart[agvStatusList[i].carNum] == 1))
            //    {
            //        stationB2Start = agvStatusList[i].carNum;
            //    }
            //    if (tagSt[0] == "B" && tagSt[1] == "2" && tagSt[2] == "2")
            //    {
            //        stationB2Start = agvStatusList[i].carNum;
            //    }

            //    if (tagSt.Length > 1 && tagSt[0] == "CH")
            //    {
            //        if (tagSt[1] == "1")
            //        {
            //            chargeSt1 = agvStatusList[i].carNum;
            //        }
            //        else if (tagSt[1] == "2")
            //        {
            //            chargeSt2 = agvStatusList[i].carNum;
            //        }
            //        else if (tagSt[1] == "3")
            //        {
            //            chargeSt3 = agvStatusList[i].carNum;
            //        }
            //    }
            //    if (tagSt[0] == "LD" || (tagTerminal == "CH#3#0" && agvStatusList[i].IsRunning))
            //    {
            //        loadSt++;
            //    }
            //    if (tagTerminal == "DR#1#1")
            //    {
            //        DrGet = 1;
            //    }
            //    if (tagTerminal == "DR#1#2")
            //    {
            //        ReqOut = 1;
            //    }

            //}
            //if (agvStatusList[i].rfidStatus == "33" && !agvStatusList[i].IsRunning)
            //{
            //    AagvGet = 1;
            //    if (agvStatusList[i].errorCode != 0)
            //    {
            //        AagvEm = 1;
            //    }
            //}
            //else if (agvStatusList[i].rfidStatus == "32" && !agvStatusList[i].IsRunning)
            //{
            //    BagvGet = 1;
            //    if (agvStatusList[i].errorCode != 0)
            //    {
            //        AagvEm = 1;
            //    }
            //}
            //}

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

            //#region 调取默认路径


            //#endregion

            //#region buffer启动点


            //#endregion

            //#region 充电点


            //#endregion

            //#region 站点调取路径

            //foreach (AgvInfo agvInfo in agvInfoDic.Values)
            //{
            //    //在rfid表里标注站点名了
            //    if (MainInfo.rfidStation.ContainsKey(agvInfo.rfid))
            //    {

            //    }

            //    //没在rfid表里标注站点名
            //    if (agvInfo.rfid.Equals("1") && !agvInfo.isRunning && agvInfo.errorCode != 205)
            //    {
            //        agvTaskRoute(agvInfo.agvNumber, "", "", 0, 1);
            //    }

            //}



            //#endregion

            ////=========================================2017-3-12-END==============================================

            ////用于判断是不是在定时时间段内
            //long dtNowLong = long.Parse(DateTime.Now.ToString("HHmmss"));
            //bool dtNowBool = false;
            //for (int i = 0; i < 6; i++)
            //{
            //    //long stLong = long.Parse(TimerInfo.startTimeList[i].ToString("HHmmss"));
            //    //long etLong = long.Parse(TimerInfo.endTimeList[i].ToString("HHmmss"));
            //    //if (dtNowLong > stLong && dtNowLong < etLong && TimerInfo.useList[i] == 1)
            //    //{
            //    //    dtNowBool = true;
            //    //    break;
            //    //}
            //}

            //if (dtNowBool && MainInfo.fixedTime != 0)
            //{
            //    if (MainInfo.allStart == 1)
            //    {
            //        MainInfo.allStart = 0;
            //        if (MainInfo.fixedTime > 0)
            //        {
            //            MainInfo.agvRunTime = UTC.ConvertDateTimeLong(DateTime.Now) + MainInfo.fixedTime;

            //        }
            //        //for (int i = 0; i < agvCount; i++)
            //        //{
            //        //    if (int.Parse(agvStatusList[i].rfidStatus) >= 1 && int.Parse(agvStatusList[i].rfidStatus) <= 13)
            //        //        agvTimerStart[agvStatusList[i].carNum] = 1;
            //        //}
            //    }
            //    if (MainInfo.fixedTime > 0)
            //    {
            //        if (MainInfo.agvRunTime == 0)
            //        {
            //            MainInfo.agvRunTime = UTC.ConvertDateTimeLong(DateTime.Now) + MainInfo.fixedTime;
            //        }
            //        else if (MainInfo.agvRunTime <= UTC.ConvertDateTimeLong(DateTime.Now))
            //        {
            //            MainInfo.allStart = 1;
            //        }
            //    }
            //    pauseInt = 0;
            //}
            //else if (MainInfo.fixedTime != 0)
            //{
            //    if (pauseInt == 0)
            //    {
            //        pauseInt = 1;

            //        long pauseTime = 0;
            //        long timeNow = UTC.ConvertDateTimeLong(DateTime.Now);
            //        pauseTime = MainInfo.agvRunTime - timeNow;


            //        long minInterval = 9999999;

            //        DateTime RecentStTime = DateTime.Now;//最近的开始时间
            //        DateTime dt = DateTime.Now;//现在的时间
            //        int resetInt = 1;//用来判断现在的时间是不是已经超过最晚的定时启动结束时间，1为是，0为否
            //        for (int i = 0; i < 6; i++)
            //        {
            //            //long stLong = long.Parse(TimerInfo.startTimeList[i].ToString("HHmmss"));
            //            //long interval = stLong - dtNowLong;
            //            //long etLong = long.Parse(TimerInfo.endTimeList[i].ToString("HHmmss"));
            //            //if (dtNowLong < etLong && TimerInfo.useList[i] == 1)
            //            //{
            //            //    resetInt = 0;
            //            //}
            //            //if (TimerInfo.useList[i] == 1 && interval > 0 && interval < minInterval)
            //            //{
            //            //    minInterval = interval;
            //            //    RecentStTime = TimerInfo.startTimeList[i];
            //            //}

            //        }

            //        if (resetInt == 1)
            //        {
            //            MainInfo.agvRunTime = 0;
            //        }
            //        else
            //        {

            //            string recentStTime = dt.ToShortDateString() + " " + RecentStTime.ToShortTimeString();
            //            MainInfo.agvRunTime = UTC.ConvertDateTimeLong(DateTime.Parse(recentStTime)) + pauseTime;
            //        }


            //    }
            //}

        }

        /// <summary>
        /// 发送agv指令
        /// </summary>
        /// <param name="AgvStatus"></param>
        /// <param name="AgvRoute"></param>
        /// <param name="AgvTask"></param>
        /// <param name="HasButton">0表示该点没有按钮，1表示有按钮, 2表示按车上按钮,3表示运行中调路径</param>
        /// <param name="SelRfidType">选择rfid找路类型</param>
        private void agvTaskRoute(int AgvNum, string AgvRoute, string AgvTask, int HasButton, int SelRfidType)
        {
            lock (agvInfoDic)
            {
                agvInfoDic[AgvNum].agvRouteName = AgvRoute;
                agvInfoDic[AgvNum].agvTask = AgvTask;
                agvInfoDic[AgvNum].agvRouteNo = SearchAgvTaskNo(AgvRoute);
                SelRoute(AgvRoute, AgvNum, SelRfidType);
                agvInfoDic[AgvNum].agvActive = true;
                agvInfoDic[AgvNum].agvStart = false;
                switch (HasButton)
                {
                    case 0:
                        agvInfoDic[AgvNum].agvSendResult[0] = 0;
                        agvInfoDic[AgvNum].agvSendResult[1] = 0;
                        agvInfoDic[AgvNum].agvSendResult[2] = 0;
                        break;
                    case 1:
                        agvInfoDic[AgvNum].agvSendResult[0] = 0;
                        agvInfoDic[AgvNum].agvSendResult[1] = 0;
                        agvInfoDic[AgvNum].agvSendResult[2] = 2;
                        break;
                    case 2:
                        agvInfoDic[AgvNum].agvSendResult[0] = 0;
                        agvInfoDic[AgvNum].agvSendResult[1] = 0;
                        agvInfoDic[AgvNum].agvSendResult[2] = -1;
                        break;
                    case 3:
                        agvInfoDic[AgvNum].agvSendResult[0] = 0;
                        agvInfoDic[AgvNum].agvSendResult[1] = -1;
                        agvInfoDic[AgvNum].agvSendResult[2] = -1;
                        break;
                    default:
                        break;
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="RouteName"></param>
        /// <param name="AgvNo"></param>
        /// <param name="SelRfidType">rfid轨迹方式，0代表默认路径，1代表自动寻路</param>
        public void SelRoute(string RouteName, int AgvNo, int SelRfidType)
        {
            List<RFIDClass> rc = new List<RFIDClass>();
            string[] routeNameGp = RouteName.Split('#');
            int routeCount = routeNameGp.Length;
            if (SelRfidType == 0)
            {
                DataRow[] routeDr = new DataRow[routeCount];
                for (int i = 0; i < routeCount; i++)
                {
                    foreach (DataRow dr in routeDt.Rows)
                    {
                        if (dr[2].ToString() == routeNameGp[i])
                        {
                            routeDr[i] = dr;
                            break;
                        }
                    }
                }
                rc = DrToRfidList(routeDr);
                agvInfoDic[AgvNo].agvRfidList = rc;
            }
            else if (SelRfidType == 1)
            {
                string[] rfidPoint = routeNameGp[0].Split('-');
                agvInfoDic[AgvNo].agvRfidList = FindRfid.CalRfid(MainInfo.rfidRoute, int.Parse(rfidPoint[0]), int.Parse(rfidPoint[1]), MapRegulate.UTCTime);
            }
        }

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
            //for (int i = 0; i < MainInfo.agvNo.Count; i++)//两个任务同为上料或ts以agvtask为准，两个任务不是相同的以sendtasklist为准
            //{
            //    if (MainInfo.agvNo[i] != CarNum.ToString())
            //    {
            //        if (sendTaskList[int.Parse(MainInfo.agvNo[i])] == TaskName || sendTaskList[int.Parse(MainInfo.agvNo[i])] == TaskName)
            //        {
            //            agvList.Add(int.Parse(MainInfo.agvNo[i]));
            //        }


            //        //if (!string.IsNullOrEmpty(sendTaskList[int.Parse(MainInfo.agvNo[i])]) && sendTaskList[int.Parse(MainInfo.agvNo[i])].Substring(0, 2).Equals("sl") && (agvTaskList[int.Parse(MainInfo.agvNo[i])].Substring(0, 2).Equals("ts") || agvTaskList[int.Parse(MainInfo.agvNo[i])].Substring(0, 2).Equals("pk")))
            //        //{
            //        //    if (sendTaskList[int.Parse(MainInfo.agvNo[i])] == TaskName)
            //        //    {
            //        //        agvList.Add(int.Parse(MainInfo.agvNo[i]));
            //        //    }
            //        //}
            //        //else
            //        //{
            //        //    if (agvTaskList[int.Parse(MainInfo.agvNo[i])] == TaskName)
            //        //    {
            //        //        agvList.Add(int.Parse(MainInfo.agvNo[i]));
            //        //    }
            //        //}
            //    }

            //}
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
