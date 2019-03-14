using MySql.Data.MySqlClient;
using System.Collections.Generic;
using AGVSystem.DAL.DataHelper;
using System;
using System.Linq;
namespace AGVSystem.DAL.DataAccess
{
    public class Ga_agvDAL
    {

        #region 查询AGV信息

        /// <summary>
        /// 查询所有AGV
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        public List<string> AGVArray(long MapTime)
        {
            List<string> agvNumList = new List<string>();
            string sql = string.Format("SELECT `AGV` FROM agv.device{0}", MapTime);
            MySqlDataReader mr = MySQLHelper.ExecuteReader(sql);
            while (mr.Read())
            {
                string agvStr = (mr.IsDBNull(0) ? "" : mr.GetString(0));
                if (!string.IsNullOrEmpty(agvStr) && agvStr != "Charge" && agvStr != "Button" && agvStr != "Plc")
                {
                    string[] agvArray = agvStr.Split(',');
                    for (int i = 0; i < agvArray.Length; i++)
                    {
                        agvNumList.Add(agvArray[i]);
                    }
                }
            }
            mr.Close();
            return agvNumList;
        }

        /// <summary>
        /// 记录已下发路径到数据库
        /// </summary>
        /// <param name="CarNum"></param>
        /// <param name="RouteName"></param>
        /// <param name="Task"></param>
        public void LogRouteInfoDAL(int CarNum, string[] RouteName, string Task)
        {
            string routeName = "";
            //for (int i = 0; i < RouteName.Length; i++)
            //{
            //    routeName += RouteName[i] + ", ";
            //}
            ////routeName = routeName.Substring(0, routeName.Length - 2);
            routeName = string.Join(",", RouteName.ToList().Select(p => p));
            DateTime dt = DateTime.Now;
            //long utcTime = UTC.ConvertDateTimeLong(dt);
            long utcTime = int.Parse(dt.ToString("HHmmss"));
            string sqlCtTb = string.Format("CREATE TABLE IF NOT EXISTS agv.`logRouteinfo{0}` ( `ID` int(10) NOT NULL AUTO_INCREMENT, `UtcTime` int(20) NOT NULL DEFAULT '0', `AgvNum` int(10) NOT NULL DEFAULT '0', `RouteName` varchar(100) NOT NULL DEFAULT '0', `TaskName` varchar(100) NOT NULL DEFAULT '0', PRIMARY KEY(`ID`)) ENGINE = InnoDB AUTO_INCREMENT = 3 DEFAULT CHARSET = utf8; ", DateTime.Now.ToString("yyyyMMdd"));
            string sql = string.Format("INSERT INTO agv.`logRouteinfo{0}`(`UtcTime`, `AgvNum`, `RouteName`, `TaskName`) VALUES ({1}, {2}, '{3}', '{4}');", dt.ToString("yyyyMMdd"), utcTime, CarNum, routeName, Task);
            //MySqlCommand mc = new MySqlCommand(sqlCtTb + sql, DataBase.ConnRouteInfo);
            //mc.ExecuteNonQuery();
            //mc.Dispose();
            MySQLHelper.ExecuteNonQuery(sqlCtTb + sql);
        }

        #endregion




    }
}
