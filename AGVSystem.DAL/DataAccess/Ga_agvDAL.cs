using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<int> AGVNumList(long MapTime)
        {
            List<int> agvNumList = new List<int>();
            string sql = string.Format("SELECT `AGV` FROM agv.device{0}", MapTime);
            MySqlDataReader mr = MySqlHelper.ExecuteReader(sql);
            while (mr.Read())
            {
                string agvStr = (mr.IsDBNull(0) ? "" : mr.GetString(0));
                if (!string.IsNullOrEmpty(agvStr) && agvStr != "Charge" && agvStr != "Button")
                {
                    string[] agvArray = agvStr.Split(',');
                    for (int i = 0; i < agvArray.Length; i++)
                    {
                        agvNumList.Add(Convert.ToInt32(agvArray[i]));
                    }
                }
            }
            mr.Close();
            return agvNumList;
        }

        #endregion

    }
}
