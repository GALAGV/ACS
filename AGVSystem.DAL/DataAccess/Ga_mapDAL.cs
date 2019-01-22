using AGVSystem.IService.IO_DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AGVSystem.DAL.DataAccess
{
    public class Ga_mapDAL : IO_MapDAL
    {
        /// <summary>
        /// 查询串口信息
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        public MySqlDataReader LoadDevice(long MapTime)
        {
            string sql = string.Format("SELECT `Com`, `Baud`, `Agv` FROM agv.`device{0}`", MapTime);
            return MySqlHelper.ExecuteReader(sql);
        }

        /// <summary>
        /// 查询所有地图
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        public MySqlDataReader MapList()
        {
            return MySqlHelper.ExecuteReader("SELECT * FROM `agv`.`map`");
        }

        /// <summary>
        /// 删除地图数据
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        public bool DeleteMap(long MapTime)
        {
            string sql = string.Format("DELETE FROM agv.`map` WHERE `CreateTime` = '{0}'; DROP TABLE agv.`tag{0}`; DROP TABLE agv.`widget{0}`; DROP TABLE agv.`line{0}`; DROP TABLE agv.`route{0}`; DROP TABLE agv.`device{0}`;", MapTime);
            return MySqlHelper.ExecuteNonQuery(sql) > 0 ? true : false;
        }

        /// <summary>
        /// 导出配置数据
        /// </summary>
        /// <param name="MapTime"></param>
        /// <param name="Db"></param>
        /// <returns></returns>
        public string ExportSetting(long MapTime, string Db)
        {
            string Delstr = string.Format("DELETE FROM `agv`.`setting`;");
            string insertSqlText = string.Format("INSERT INTO `agv`.`setting` (`ID`, `Map`, `Mode`) VALUES (1, {0} , {1});", MapTime, 1);
            return Delstr + insertSqlText;
        }

        /// <summary>
        /// 导出地图信息数据
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Db"></param>
        /// <returns></returns>
        public string ExportMySqlTable(string TableName, string Db)
        {
            string dropSql = string.Format("DROP TABLE IF EXISTS {0}.`{1}`;", Db, TableName);
            StringBuilder sqlText = new StringBuilder(dropSql);
            string createSql = string.Format("CREATE TABLE IF NOT EXISTS {0}.`{1}`", Db, TableName);
            sqlText.Append(createSql);
            sqlText.Append(" (");
            string sql = string.Format("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}' AND TABLE_SCHEMA = '{1}'", TableName, Db);
            MySqlDataReader mr = MySqlHelper.ExecuteReader(sql);
            string keyStr = "";
            int colCount = 0;
            while (mr.Read())
            {
                sqlText.Append(" `");
                sqlText.Append(mr["COLUMN_NAME"].ToString());
                sqlText.Append("` ");
                sqlText.Append(mr["COLUMN_TYPE"].ToString().ToUpper());
                if (mr["IS_NULLABLE"].ToString() == "NO")
                {
                    sqlText.Append(" NOT NULL ");
                }
                else
                {
                    sqlText.Append(" DEFAULT NULL ");
                }
                sqlText.Append(mr["EXTRA"].ToString().ToUpper());
                if (!string.IsNullOrEmpty(mr["COLUMN_COMMENT"].ToString()))
                {
                    sqlText.Append(" COMMENT ");
                    sqlText.Append("'");
                    sqlText.Append(mr["COLUMN_COMMENT"].ToString());
                    sqlText.Append("'");
                }
                sqlText.Append(",");
                if (mr["COLUMN_KEY"].ToString() == "PRI")
                {
                    keyStr = "PRIMARY KEY(`ID`)";
                }
                colCount++;
            }
            sqlText.Append(keyStr);
            sqlText.Append(") ENGINE = InnoDB AUTO_INCREMENT = 1 DEFAULT CHARSET = utf8 ROW_FORMAT = COMPACT;");
            mr.Close();

            sql = string.Format("SELECT * FROM {0}.`{1}`", Db, TableName);

            mr = MySqlHelper.ExecuteReader(sql);

            string Delstr = string.Format("DELETE FROM {0}.`{1}`;", Db, TableName);
            StringBuilder insertSqlText = new StringBuilder(Delstr);
            int getName = 0;
            while (mr.Read())
            {
                if (getName == 0)
                {
                    string insertSql = string.Format("INSERT INTO {0}.`{1}` (", Db, TableName);
                    insertSqlText.Append(insertSql);
                    for (int i = 0; i < colCount; i++)
                    {
                        insertSqlText.Append(" `");
                        insertSqlText.Append(mr.GetName(i));
                        if (i < colCount - 1)
                        {
                            insertSqlText.Append("`, ");
                        }
                        else
                        {
                            insertSqlText.Append("`) VALUES");
                        }
                    }
                    getName = 1;
                }

                for (int i = 0; i < colCount; i++)
                {
                    if (i == 0)
                    {
                        insertSqlText.Append("('");
                    }
                    else
                    {
                        insertSqlText.Append(" '");
                    }

                    insertSqlText.Append(mr[i].ToString());

                    if (i < colCount - 1)
                    {
                        insertSqlText.Append("', ");
                    }
                    else
                    {
                        insertSqlText.Append("'),");
                    }
                }
            }
            mr.Close();
            insertSqlText.Remove(insertSqlText.Length - 1, 1);
            insertSqlText.Append(";");
            return sqlText.ToString() + insertSqlText.ToString();
        }

        /// <summary>
        /// 导出地图表数据
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Db"></param>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        public string ExportTableContent(string TableName, string Db, string MapTime)
        {
            StringBuilder sqlText = new StringBuilder();
            string sql = string.Format("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}' AND TABLE_SCHEMA = '{1}'", TableName, Db);
            MySqlDataReader mr = MySqlHelper.ExecuteReader(sql);
            int colCount = 0;
            while (mr.Read())
            {
                colCount++;
            }
            //sqlText.Append(keyStr);
            //sqlText.Append(") ENGINE = InnoDB AUTO_INCREMENT = 1 DEFAULT CHARSET = utf8 ROW_FORMAT = COMPACT;");
            mr.Close();

            sql = string.Format("SELECT * FROM {0}.`{1}` WHERE `CreateTime` = '{2}'", Db, TableName, MapTime);
            mr = MySqlHelper.ExecuteReader(sql); ;

            string Delstr = string.Format("DELETE FROM {0}.`{1}` WHERE `CreateTime` = '{2}' ;", Db, TableName, MapTime);
            StringBuilder insertSqlText = new StringBuilder();
            int getName = 0;
            while (mr.Read())
            {
                if (getName == 0)
                {
                    string insertSql = string.Format("INSERT INTO {0}.`{1}` (", Db, TableName);
                    insertSqlText.Append(insertSql);
                    for (int i = 0; i < colCount; i++)
                    {
                        insertSqlText.Append(" `");
                        insertSqlText.Append(mr.GetName(i));
                        if (i < colCount - 1)
                        {
                            insertSqlText.Append("`, ");
                        }
                        else
                        {
                            insertSqlText.Append("`) VALUES");
                        }
                    }
                    getName = 1;
                }

                for (int i = 0; i < colCount; i++)
                {
                    if (i == 0)
                    {
                        insertSqlText.Append("('");
                    }
                    else
                    {
                        insertSqlText.Append(" '");
                    }

                    insertSqlText.Append(mr[i].ToString());

                    if (i < colCount - 1)
                    {
                        insertSqlText.Append("', ");
                    }
                    else
                    {
                        insertSqlText.Append("'),");
                    }
                }
            }
            mr.Close();
            insertSqlText.Remove(insertSqlText.Length - 1, 1);
            insertSqlText.Append(";");
            return Delstr + insertSqlText.ToString();
        }


    }
}
