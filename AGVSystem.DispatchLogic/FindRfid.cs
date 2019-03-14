using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using AGVDLL;



namespace AGVSystem.DispatchLogic
{

    public class FindRfid
    {

        /// <summary>
        /// 取出来tag的信息
        /// </summary>
        struct TagInfo
        {
            public string NextTag;
            public string NextLeftTag;
            public string NextRightTag;
            public string PreLeftTag;
            public string PreRightTag;
            public string PreTag;

            public int Speed;
            public int SpeedRev;
            public int StopTime;
            public int Pbs;
            public int PbsRev;
            public string TagTerminal;
        }

        /// <summary>
        /// 生产tag的二维数组
        /// </summary>
        /// <param name="TagNum"></param>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        private int[,] rfidRoute(ref int TagNum, long MapTime)
        {
            string sql = string.Format("SELECT `TagName` FROM agv.`tag{0}`", MapTime.ToString());

            MySqlCommand mc = new MySqlCommand(sql, DAL.DataHelper.MySQLHelper.MyConnCal);

            MySqlDataReader mr = mc.ExecuteReader();
            TagNum = 0;
            while (mr.Read())
            {
                if (TagNum < int.Parse(mr.GetString(0)))
                    TagNum = int.Parse(mr.GetString(0));
            }
            mr.Close();

            int[,] dist = new int[TagNum + 1, TagNum + 1];

            for (int i = 0; i < TagNum + 1; i++)
            {
                for (int j = 0; j < TagNum + 1; j++)
                {
                    if (i == j)
                    {
                        dist[i, j] = 0;
                    }
                    else
                    {
                        dist[i, j] = 99999999;
                    }
                }
            }

            sql = string.Format("SELECT `Tag1`,`Tag2` FROM agv.`line{0}`", MapTime.ToString());
            mc.CommandText = sql;
            mr = mc.ExecuteReader();
            while (mr.Read())
            {
                int i = int.Parse(mr.GetString(0).Substring(2));
                int j = int.Parse(mr.GetString(1).Substring(2));
                dist[i, j] = 1;
                dist[j, i] = 1;
            }
            mr.Close();
            return dist;
        }

        /// <summary>
        /// 输出两点之间的rfid
        /// </summary>
        /// <param name="MapTime"></param>
        public string[,] FindRfidRoute(long MapTime)
        {
            int[,] dist;//两点之间rfid的数量
            string[,] route;//两点之间的rfid
            int tagNum = 0;//rfid数量

            dist = rfidRoute(ref tagNum, MapTime);
            route = new string[tagNum + 1, tagNum + 1];//因为数组是从0开始的，所以0,0悬空。为了一一对应数组大小为，tagNum+1

            for (int l = 0; l < tagNum + 1; l++)
            {
                for (int m = 0; m < tagNum + 1; m++)
                {
                    route[m, l] = m.ToString();
                }
            }

            for (int k = 0; k < tagNum + 1; k++)
            {
                for (int i = 0; i < tagNum + 1; i++)
                {
                    for (int j = 0; j < tagNum + 1; j++)
                    {
                        if (dist[i, j] > dist[i, k] + dist[k, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            route[i, j] = route[i, k] + "," + route[k, j];
                        }
                    }
                }
            }
            return route;
        }

        public static List<RFIDClass> CalRfid(string[,] RouteStr, int StartRfid, int EndRfid, long MapTime)
        {
            List<RFIDClass> RfidList = new List<RFIDClass>();
            string routeTagStr = RouteStr[StartRfid, EndRfid] + ',' + EndRfid.ToString();
            string[] routeTag = routeTagStr.Split(',');

            int tagNum = routeTag.Length;
            int agvDire = 0;//0表示正向,1表示反向
            int preTurn = 0;//之前的RFID是否有转弯指令

            int turnPbs = 2;//pbs属性根据最后是不是需要进料车底部设置
            try
            {
                //if (MainInfo.rfidStation[routeTag[routeTag.Length - 1]].Split('#')[2] == "0")
                //{
                //    turnPbs = 3;//不钻料车
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            for (int i = 0; i < tagNum; i++)
            {
                RFIDClass rfidInfo = new RFIDClass();
                if (i == 0)
                {
                    rfidInfo.rfidID = 0;
                    try
                    {
                        //rfidInfo.hookSwitch = int.Parse(MainInfo.rfidStation[routeTag[i]].Split('#')[2]);//根据站点设置hook属性
                    }
                    catch
                    {
                        Console.WriteLine("没有站点信息");
                    }
                }
                else
                {
                    rfidInfo.rfidID = int.Parse(routeTag[i]);
                }
                //添加RFID信息
                string sql = string.Format("SELECT * FROM agv.`tag{0}` WHERE `TagName` = {1}", MapTime.ToString(), routeTag[i]);

                MySqlCommand mc = new MySqlCommand(sql, DAL.DataHelper.MySQLHelper.MyConnCal);

                MySqlDataReader mr = mc.ExecuteReader();

                TagInfo tagInfo = new TagInfo();

                if (i < tagNum - 1)
                {
                    string nextRfid = routeTag[i + 1];

                    while (mr.Read())
                    {
                        tagInfo.NextTag = mr.GetString("NextTag");
                        tagInfo.NextLeftTag = mr.GetString("NextLeftTag");
                        tagInfo.NextRightTag = mr.GetString("NextRightTag");
                        tagInfo.PreTag = mr.GetString("PreTag");
                        tagInfo.PreLeftTag = mr.GetString("PreLeftTag");
                        tagInfo.PreRightTag = mr.GetString("PreRightTag");
                        tagInfo.Speed = mr.GetInt32("Speed");
                        tagInfo.SpeedRev = mr.GetInt32("SpeedRev");
                        tagInfo.Pbs = mr.GetInt32("Pbs");
                        tagInfo.PbsRev = mr.GetInt32("PbsRev");
                        tagInfo.TagTerminal = mr.GetString("TagTerminal");
                    }

                    if (preTurn == 1)
                    {
                        rfidInfo.trunSwitch = 3;
                        preTurn = 0;
                    }

                    if (nextRfid == tagInfo.NextTag)
                    {
                        rfidInfo.speedNo = tagInfo.Speed;
                        rfidInfo.pbsNo = tagInfo.Pbs;
                        if (i == 0)
                        {
                            rfidInfo.directionSwitch = 0;
                        }
                        else if (agvDire == 1)
                        {
                            rfidInfo.directionSwitch = 1;
                            rfidInfo.startSwitch = 2;
                        }

                        agvDire = 0;
                    }
                    else if (nextRfid == tagInfo.NextLeftTag)
                    {
                        rfidInfo.trunSwitch = 1;
                        preTurn = 1;
                        rfidInfo.speedNo = 2;
                        rfidInfo.pbsNo = turnPbs;
                        if (i == 0)
                        {
                            rfidInfo.directionSwitch = 0;
                        }
                        else if (agvDire == 1)
                        {
                            rfidInfo.directionSwitch = 1;
                            rfidInfo.startSwitch = 2;
                        }
                        agvDire = 0;
                    }
                    else if (nextRfid == tagInfo.NextRightTag)
                    {
                        rfidInfo.trunSwitch = 2;
                        preTurn = 1;
                        rfidInfo.speedNo = 2;
                        rfidInfo.pbsNo = turnPbs;
                        if (i == 0)
                        {
                            rfidInfo.directionSwitch = 0;
                        }
                        else if (agvDire == 1)
                        {
                            rfidInfo.directionSwitch = 1;
                            rfidInfo.startSwitch = 2;
                        }
                        agvDire = 0;
                    }
                    else if (nextRfid == tagInfo.PreTag)
                    {
                        rfidInfo.speedNo = tagInfo.SpeedRev;
                        rfidInfo.pbsNo = tagInfo.PbsRev;
                        if (i == 0)
                        {
                            rfidInfo.directionSwitch = 1;
                        }
                        else if (agvDire == 0)
                        {
                            rfidInfo.directionSwitch = 1;
                            rfidInfo.startSwitch = 2;
                        }
                        agvDire = 1;
                    }
                    else if (nextRfid == tagInfo.PreLeftTag)
                    {
                        preTurn = 1;
                        rfidInfo.trunSwitch = 1;
                        rfidInfo.speedNo = 2;
                        rfidInfo.pbsNo = turnPbs;
                        if (i == 0)
                        {
                            rfidInfo.directionSwitch = 1;
                        }
                        else if (agvDire == 0)
                        {
                            rfidInfo.directionSwitch = 1;
                            rfidInfo.startSwitch = 2;
                        }
                        agvDire = 1;
                    }
                    else if (nextRfid == tagInfo.PreRightTag)
                    {
                        preTurn = 1;
                        rfidInfo.trunSwitch = 2;
                        rfidInfo.speedNo = 2;
                        rfidInfo.pbsNo = turnPbs;
                        if (i == 0)
                        {
                            rfidInfo.directionSwitch = 1;
                        }
                        else if (agvDire == 0)
                        {
                            rfidInfo.directionSwitch = 1;
                            rfidInfo.startSwitch = 2;
                        }
                        agvDire = 1;
                    }
                    if (i == 0)
                    {
                        if (!string.IsNullOrEmpty(tagInfo.TagTerminal))
                        {
                            string[] tagTerminal = tagInfo.TagTerminal.Split('#');
                            if (tagTerminal[2] == "0")
                            {
                                rfidInfo.hookSwitch = 0;
                            }
                            else if (tagTerminal[1] == "1")
                            {
                                rfidInfo.hookSwitch = 1;
                            }
                        }
                    }
                }
                else
                {
                    rfidInfo.trunSwitch = 3;
                    rfidInfo.hookSwitch = 1;
                    rfidInfo.startSwitch = 2;
                }
                mr.Close();
                RfidList.Add(rfidInfo);
                if (rfidInfo.directionSwitch == 1)
                {
                    RFIDClass reStartRc = new RFIDClass();
                    reStartRc.rfidID = int.Parse(routeTag[i]);
                    reStartRc.startSwitch = 1;
                    RfidList.Add(reStartRc);
                }
            }
            return RfidList;
        }
    }
}
