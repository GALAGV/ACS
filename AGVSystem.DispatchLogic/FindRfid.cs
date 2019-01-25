using AGVSystem.DAL.DataHelper;
using MySql.Data.MySqlClient;

namespace AGVSystem.DispatchLogic
{

    public class FindRfid
    {
        /// <summary>
        /// 生产tag的二维数组
        /// </summary>
        /// <param name="TagNum"></param>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        private int[,] rfidRoute(ref int TagNum, long MapTime)
        {
            string sql = string.Format("SELECT `TagName` FROM agv.`tag{0}`", MapTime.ToString());

            MySqlDataReader mr = MySQLHelper.ExecuteReader(sql);

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
            mr = MySQLHelper.ExecuteReader(sql);
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
    }
}
