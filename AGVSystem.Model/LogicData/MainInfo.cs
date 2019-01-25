﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Data;
using System.Collections.Concurrent;
using System.IO.Ports;

namespace AGVSystem.Model.LogicData
{
    public class MainInfo
    {


        public static long selMember = 0;
        public static int mode = 0;
        public static int mapType = 0;
        public static int fixedTime = 0;
        public static int allStart = 0;
        public static long agvRunTime = 0;
        public static long mapTime;
        public static List<string> agvNo = new List<string>();
        public static List<string[]> listLine = new List<string[]>();
        public static DataTable routeDt = new DataTable();

        public static int support300 = 0;

        public static string passWord = "";

        public static int resetButtonTime = 0;

        public static int prity = 1;//校验位： 0.无校验 1.奇校验 2.偶校验
        public static int stopBits = 0;//停止位： 0.1位 1.1.5位 2.2位

        public static int buttonPrity = 0;//校验位： 0.无校验 1.奇校验 2.偶校验
        public static int buttonStopBits = 0;//停止位： 0.1位 1.1.5位 2.2位

        public static string[,] rfidRoute;//rfid路径数组


        public static List<Thread> listTh = new List<Thread>();
        public static List<AGVDLL.AGVDLL> listAgvDll = new List<AGVDLL.AGVDLL>();
        public static List<SerialPort> PlcPortList = new List<SerialPort>();
        //public static List<TIPSWirelessButton.TIPSWirelessButton> listButtonDll = new List<TIPSWirelessButton.TIPSWirelessButton>();
        //public static List<Charge> listChargeDll = new List<Charge>();
        //public static List<TIPSButtonClass> listTipsButton = new List<TIPSButtonClass>();
        //public static List<TIPSButtonClass> listTipsButtonPre = new List<TIPSButtonClass>();
        public static List<IntPtr> listPtr = new List<IntPtr>();


        public static double width;
        public static double height;
        //public static List<Label> listTag = new List<Label>();
        //public static List<Label> listWidget = new List<Label>();
        public static int mul = 10;
        public static double times = 1.5;

        public static ConcurrentDictionary<string, string> stationRfid = new ConcurrentDictionary<string, string>();//站点对应的RFID
        public static ConcurrentDictionary<string, string> rfidStation = new ConcurrentDictionary<string, string>();//RFID对应站点
        //public static List<StationConn> StationAList = new List<StationConn>();//A号站点
        //public static List<StationConn> StationBList = new List<StationConn>();//B号站点
        //public static List<BufferConn> BufferConnList = new List<BufferConn>();//气缸站点
        //public static ConcurrentDictionary<string, long> buttonStation = new ConcurrentDictionary<string, long>();//轮询查询站点信息


        public static int[] A = new int[3];
        public static int A3WOut = 0;
        public static int A3WIn = 0;
        public static int A3WRun = 0;
        public static int A3WGet = 0;
        public static int A3WComp = 0;

        public static int A3EOut = 0;
        public static int A3EIn = 0;
        public static int A3ERun = 0;
        public static int A3EGet = 0;
        public static int A3EComp = 0;

        public static int B1NOut = 0;//轮询查询站点信息
        public static int B1NIn = 0;//轮询查询站点信息
        public static int B1NRun = 0;
        public static int B1NGet = 0;
        public static int B1NComp = 0;

        public static int B1SOut = 0;//轮询查询站点信息
        public static int B1SIn = 0;//轮询查询站点信息
        public static int B1SRun = 0;
        public static int B1SGet = 0;
        public static int B1SComp = 0;

        public static long paintCall = 0;

        public static int warningVoltage = 24;
    }
}
