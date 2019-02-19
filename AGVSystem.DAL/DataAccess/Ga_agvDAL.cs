using MySql.Data.MySqlClient;
using System.Collections.Generic;
using AGVSystem.DAL.DataHelper;

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

        #endregion




    }
}
