﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AGVDLL;

namespace AGVSystem.Model.LogicData
{
   public class MainInfo
    {


        public static int prity = 1;//校验位： 0.无校验 1.奇校验 2.偶校验
        public static int stopBits = 0;//停止位： 0.1位 1.1.5位 2.2位

        public static int buttonPrity = 0;//校验位： 0.无校验 1.奇校验 2.偶校验
        public static int buttonStopBits = 0;//停止位： 0.1位 1.1.5位 2.2位

        public static List<AGVDLL.AGVDLL> listAgvDll = new List<AGVDLL.AGVDLL>();
        public static List<Thread> GetThreads = new List<Thread>();
        public static List<IntPtr> listPtr = new List<IntPtr>();


        public static Dictionary<int,CarStatus> carStatusList = new Dictionary<int, CarStatus>();
        public static List<int> agvNo = new List<int>();
    }
}