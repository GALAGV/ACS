using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using AGVSystem.DAL.DataHelper;
using System.Windows.Controls;
using AGVSystem.Infrastructure.agvCommon;
using AGVSystem.Model.DrawMap;
using AGVSystem.Model.Ga_agvModels;
using System.Collections.ObjectModel;
using AGVSystem.Model.LogicData;
using System.Linq;

namespace AGVSystem.DAL.DataAccess
{
    public class Ga_mapDAL
    {
        List<string> Sql = new List<string>();

        /// <summary>
        /// 查询串口信息
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        public MySqlDataReader LoadDevice(long MapTime)
        {
            string sql = string.Format("SELECT `Com`, `Baud`, `Agv` FROM agv.`device{0}`", MapTime);
            return MySQLHelper.ExecuteReader(sql);
        }

        /// <summary>
        /// 查询所有地图
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        public MySqlDataReader MapList()
        {
            return MySQLHelper.ExecuteReader("SELECT * FROM `agv`.`map`");
        }

        public MySqlDataReader MapArrayDAL()
        {
            return MySQLHelper.ExecuteReader("SELECT  `CreateTime`,`Name` FROM `agv`.`map`");
        }

        /// <summary>
        /// 删除地图数据
        /// </summary>
        /// <param name="MapTime"></param>
        /// <returns></returns>
        public bool DeleteMap(long MapTime)
        {
            string sql = string.Format("DELETE FROM agv.`map` WHERE `CreateTime` = '{0}'; DROP TABLE agv.`tag{0}`; DROP TABLE agv.`widget{0}`; DROP TABLE agv.`line{0}`; DROP TABLE agv.`route{0}`; DROP TABLE agv.`device{0}`;", MapTime);
            return MySQLHelper.ExecuteNonQuery(sql) > 0 ? true : false;
        }

        /// <summary>
        /// 导出配置数据
        /// </summary>
        /// <param name="MapTime"></param>
        /// <param name="Db"></param>
        /// <returns></returns>
        public string ExportSetting(long MapTime, string Db)
        {
            string Delstr = string.Format("DELETE FROM `agv`.`setting`;\r\n");
            string insertSqlText = string.Format("INSERT INTO `agv`.`setting` (`ID`, `Map`, `Mode`) VALUES (1, {0} , {1});\r\n", MapTime, 1);
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
            string dropSql = string.Format("DROP TABLE IF EXISTS {0}.`{1}`;\r\n\r\n", Db, TableName);
            StringBuilder sqlText = new StringBuilder(dropSql);
            string createSql = string.Format("CREATE TABLE IF NOT EXISTS {0}.`{1}` \r\n", Db, TableName);
            sqlText.Append(createSql);
            sqlText.Append(" (");
            string sql = string.Format("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}' AND TABLE_SCHEMA = '{1}'", TableName, Db);
            MySqlDataReader mr = MySQLHelper.ExecuteReader(sql);
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
                sqlText.Append(", \r\n");
                if (mr["COLUMN_KEY"].ToString() == "PRI")
                {
                    keyStr = "PRIMARY KEY(`ID`)";
                }
                colCount++;
            }
            sqlText.Append(keyStr);
            sqlText.Append(") ENGINE = InnoDB AUTO_INCREMENT = 1 DEFAULT CHARSET = utf8 ROW_FORMAT = COMPACT; \r\n\r\n");
            mr.Close();

            sql = string.Format("SELECT * FROM {0}.`{1}`", Db, TableName);

            mr = MySQLHelper.ExecuteReader(sql);

            string Delstr = string.Format("DELETE FROM {0}.`{1}`; \r\n\r\n", Db, TableName);
            StringBuilder insertSqlText = new StringBuilder(Delstr);
            int getName = 0;
            while (mr.Read())
            {
                if (getName == 0)
                {
                    string insertSql = string.Format("INSERT INTO {0}.`{1}` \r\n (", Db, TableName);
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
                            insertSqlText.Append("`) VALUES \r\n");
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
                        insertSqlText.Append("'),\r\n");
                    }
                }
            }
            mr.Close();
            insertSqlText.Remove(insertSqlText.Length - 3, 3);
            insertSqlText.Append(";\r\n\r\n");
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
            MySqlDataReader mr = MySQLHelper.ExecuteReader(sql);
            int colCount = 0;
            while (mr.Read())
            {
                colCount++;
            }
            //sqlText.Append(keyStr);
            //sqlText.Append(") ENGINE = InnoDB AUTO_INCREMENT = 1 DEFAULT CHARSET = utf8 ROW_FORMAT = COMPACT;");
            mr.Close();

            sql = string.Format("SELECT * FROM {0}.`{1}` WHERE `CreateTime` = '{2}'", Db, TableName, MapTime);
            mr = MySQLHelper.ExecuteReader(sql); ;

            string Delstr = string.Format("DELETE FROM {0}.`{1}` WHERE `CreateTime` = '{2}' ;\r\n", Db, TableName, MapTime);
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
            insertSqlText.Append(";\r\n");
            return Delstr + insertSqlText.ToString();
        }

        /// <summary>
        /// 导入地图数据
        /// </summary>
        /// <param name="MapText"></param>
        /// <returns></returns>
        public bool MapTolead(string MapText)
        {
            try
            {
                return MySQLHelper.ExecuteSqlTran(new List<string>() { MapText });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public MySqlDataReader MapTags(string exls)
        {
            return MySQLHelper.ExecuteReader("SELECT * FROM   `agv`.`tag" + exls + "`");
        }


        /// <summary>
        /// 载入所有Tag
        /// </summary>
        /// <param name="exls"></param>
        /// <returns></returns>
        public DataTable GetMapTags(string exls)
        {
            return MySQLHelper.ExecuteDataTable("SELECT `X`,`Y`,`TagName` FROM   `agv`.`tag" + exls + "` order by (TagName+0)");
        }

        /// <summary>
        /// 查询线路数据
        /// </summary>
        /// <param name="Times"></param>
        /// <returns></returns>
        public DataTable LineData(string Times)
        {
            return MySQLHelper.ExecuteDataTable("SELECT * FROM `agv`.`line" + Times + "`");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Times"></param>
        /// <returns></returns>
        public DataTable widgetArrlist(string Times)
        {
            return MySQLHelper.ExecuteDataTable("SELECT * FROM  `agv`.`widget" + Times + "`  order by WidgetNo");
        }

        /// <summary>
        /// 查询默认地图数据
        /// </summary>
        /// <returns></returns>
        public DataTable Setting()
        {
            return MySQLHelper.ExecuteDataTable("SELECT * FROM `agv`.`setting`");
        }

        /// <summary>
        /// 查询地图数据
        /// </summary>
        /// <param name="UTCTime"></param>
        /// <returns></returns>
        public DataTable defaultMapDAL(long UTCTime)
        {
            return MySQLHelper.ExecuteDataTable($"SELECT * FROM `agv`.`map` WHERE CreateTime={UTCTime}");
        }

        /// <summary>
        /// 保存串口数据
        /// </summary>
        /// <param name="mapTime"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool InsertDeviceDAL(long mapTime, DataTable data)
        {
            string desql = string.Format("DELETE FROM agv.`device{0}`;", mapTime.ToString());

            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(desql);
            for (int i = 0; i < data.Rows.Count; i++)
            {
                string sql = string.Format("INSERT INTO agv.`device{0}` (`Com`, `Baud`, `Agv`) VALUES ", mapTime.ToString());
                sbSql.Append(sql);
                sbSql.Append('(');
                sbSql.Append(data.Rows[i][0]);
                sbSql.Append(",'");
                sbSql.Append(data.Rows[i][1]);
                sbSql.Append("','");
                sbSql.Append(data.Rows[i][2]);
                sbSql.Append("')");
                sbSql.Append(";");
            }
            return MySQLHelper.ExecuteNonQuery(sbSql.ToString()) > 0 ? true : false;
        }

        /// <summary>
        /// 修改启动地图配置
        /// </summary>
        /// <param name="Map"></param>
        /// <param name="Mode"></param>
        /// <returns></returns>
        public bool UpdateSetting(long Map, int Mode)
        {
            string sql = string.Format("UPDATE agv.`setting` SET `Map` = {0}, `Mode` = {1} WHERE `ID` = 1;", Map, Mode);
            return MySQLHelper.ExecuteNonQuery(sql) > 0 ? true : false;
        }



        /// <summary>
        /// 保存地图
        /// </summary>
        /// <param name="Times">UTCTime</param>
        /// <param name="type">是否是新建地图 True是，false否 编辑地图</param>
        /// <param name="Name">地图名称</param>
        /// <param name="Width">宽度</param>
        /// <param name="Height">高度</param>
        /// <param name="AgvStr">默认0</param>
        /// <param name="MapType">地图类型 默认0</param>
        /// <param name="Size">缩放倍数</param>
        /// <param name="keyValues">Tag集合</param>
        /// <param name="Area">区域集合</param>
        /// <param name="TextControl">文字集合</param>
        /// <param name="Line">线路集合</param>
        /// <returns></returns>
        public bool SaveAtlas(string Times, bool type, string Name, double Width, double Height, string AgvStr, int MapType, double Size, Dictionary<int, Label> keyValues, Dictionary<int, Label> Area, Dictionary<int, Label> TextControl, List<WirePointArray> Line)
        {
            if (type)
            {
                InsertMapInfo(Name, Times, Width, Height, AgvStr, MapType);
            }
            else
            {
                UpdateMap(Name, AgvStr, Times, MapType);
            }
            SaveTag(Times, type, keyValues, Size);
            InsertLine(Times, Line, Size);
            InsertArea(Times, Area, Size);
            InsertText(Times, TextControl, Size);
            return MySQLHelper.ExecuteSqlTran(Sql);
        }

        /// <summary>
        /// 更新Map信息
        /// </summary>
        /// <param name="MapName"></param>
        /// <param name="AgvStr"></param>
        /// <param name="MapTime"></param>
        /// <param name="MapType"></param>
        public void UpdateMap(string MapName, string AgvStr, string MapTime, int MapType)
        {
            Sql.Add(string.Format("UPDATE agv.`map` SET `Name` = '{0}', `AGV` = '{1}', `Type` = {2} WHERE CreateTime = {3}", MapName, AgvStr, MapType, MapTime));
        }

        /// <summary>
        /// 插入table.map的数据库信息
        /// </summary>
        /// <param name="Conn"></param>
        /// <param name="Name"></param>
        /// <param name="CreateTime">utc时间</param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public void InsertMapInfo(string Name, string CreateTime, double Width, double Height, string AgvStr, int MapType)
        {
            Sql.Add(string.Format("INSERT INTO agv.`map` (`Name`, `CreateTime`, `Width`, `Height`, `AGV`, `Type`) VALUES ('{0}', {1}, {2}, {3}, '{4}', {5})", Name, CreateTime, Width, Height, AgvStr, MapType));
        }

        /// <summary>
        /// Tag保存
        /// </summary>
        /// <param name="CreateTime"></param>
        /// <param name="type"></param>
        private void SaveTag(string CreateTime, bool type, Dictionary<int, Label> keyValues, double Size)
        {
            string sqlTag = $@"DROP TABLE IF EXISTS agv.`tag{CreateTime}`; CREATE TABLE IF NOT EXISTS agv.`tag{CreateTime}` (
                                `ID` INT(10) NOT NULL AUTO_INCREMENT,
                                `TagNo` VARCHAR(50) NULL DEFAULT NULL COMMENT 'tag号（地图记录用）',
                                `TagName` VARCHAR(50) NULL DEFAULT NULL COMMENT 'tag名称',
                                `X` DOUBLE NULL DEFAULT NULL COMMENT 'X（米）',
                                `Y` DOUBLE NULL DEFAULT NULL COMMENT 'Y（米）',
                                `NextTag` VARCHAR(10) NULL DEFAULT NULL COMMENT '后置tag',
                                `NextLeftTag` VARCHAR(10) NULL DEFAULT NULL COMMENT '左后置tag',
                                `NextRightTag` VARCHAR(10) NULL DEFAULT NULL COMMENT '右后置tag',
                                `PreTag` VARCHAR(10) NULL DEFAULT NULL COMMENT '前置tag',
                                `PreLeftTag` VARCHAR(10) NULL DEFAULT NULL COMMENT '左前置tag',
                                `PreRightTag` VARCHAR(10) NULL DEFAULT NULL COMMENT '右前置tag',
                                `Speed` INT(10) NULL DEFAULT NULL COMMENT '正向速度',
                                `SpeedRev` INT(10) NULL DEFAULT NULL COMMENT '反向速度',
                                `PreTurnSpeed` INT(10) NULL DEFAULT NULL COMMENT '（正向）转弯速度',
                                `RevTurnSpeed` INT(10) NULL DEFAULT NULL COMMENT '（反向）转弯速度',
                                `StopTime` INT(10) NULL DEFAULT NULL COMMENT '单位为s',
                                `Pbs` INT(10) NULL DEFAULT NULL COMMENT '障碍物扫描（正向）',
                                `PbsRev` INT(10) NULL DEFAULT NULL COMMENT '障碍物扫描（反向）',
                                `PreTurnPBS` INT(10) NULL DEFAULT NULL COMMENT '（正向）转弯PBS',
                                `RevTurnPBS` INT(10) NULL DEFAULT NULL COMMENT '(反向)转弯PBS',
                                `TagTerminal` VARCHAR(50) NULL DEFAULT NULL COMMENT '0为非终结点，1为终结点，2为辅助点',
                                PRIMARY KEY(`ID`)
                                )
                                COMMENT = '取名为tag+UTC时间，如map1234567890'
                                COLLATE = 'utf8_general_ci'
                                ENGINE = InnoDB
                                AUTO_INCREMENT = 7
                                ; ";
            string sqlWidget = string.Format("DROP TABLE IF EXISTS agv.`widget{0}`;CREATE TABLE IF NOT EXISTS agv.`widget{0}` ( `ID` int(10) NOT NULL AUTO_INCREMENT, `WidgetNo` varchar(50) DEFAULT NULL COMMENT '控件编号', `Name` varchar(50) DEFAULT NULL COMMENT '文字内容', `X` double DEFAULT NULL COMMENT 'X坐标位置（米）', `Y` double DEFAULT NULL COMMENT 'Y坐标位置（米）', `Width` double DEFAULT NULL COMMENT '宽度（米）', `Height` double DEFAULT NULL COMMENT '高度（米）', `FontSize` int(11) DEFAULT NULL COMMENT '字体大小', `FontPosition` varchar(50) DEFAULT NULL COMMENT '字体位置（16进制）', `ForeColor` varchar(50) DEFAULT NULL COMMENT '字体颜色（16进制）', `BackColor` varchar(50) DEFAULT NULL COMMENT '背景颜色（16进制）', `BorderColor` varchar(50) DEFAULT NULL COMMENT '边框颜色（16进制）', PRIMARY KEY(`ID`)) ENGINE = InnoDB DEFAULT CHARSET = utf8; ", CreateTime.ToString());
            string sqlLine = string.Format("DROP TABLE IF EXISTS agv.`line{0}`;CREATE TABLE IF NOT EXISTS agv.`line{0}` ( `ID` int(10) NOT NULL AUTO_INCREMENT, `StartX` double DEFAULT NULL COMMENT '起始X坐标位置（米）', `StartY` double DEFAULT NULL COMMENT '起始Y坐标位置（米）', `EndX` double DEFAULT NULL COMMENT '终点X坐标位置（米）', `EndY` double DEFAULT NULL COMMENT '终点Y坐标位置（米）', `LineStyel` int(11) DEFAULT NULL COMMENT 'Line类型：1为直线，2为折线', `Tag1` varchar(50) DEFAULT NULL, `Tag2` varchar(50) DEFAULT NULL, PRIMARY KEY(`ID`)) ENGINE = InnoDB DEFAULT CHARSET = utf8 ROW_FORMAT = COMPACT; ", CreateTime.ToString());
            string sqlDevice = string.Format("CREATE TABLE IF NOT EXISTS agv.`device{0}` ( `ID` int(10) NOT NULL AUTO_INCREMENT, `Com` int(11) DEFAULT NULL, `Baud` int(11) DEFAULT NULL, `Agv` varchar(50) DEFAULT NULL,  PRIMARY KEY(`ID`)) ENGINE = InnoDB DEFAULT CHARSET = utf8 ROW_FORMAT = COMPACT; ", CreateTime.ToString());
            string networkconfig = $@"CREATE TABLE IF NOT EXISTS agv.`Networkconfig{CreateTime}` (
	                        `ID` INT(255) NOT NULL AUTO_INCREMENT,
	                        `IP_Address` VARCHAR(255) NULL DEFAULT NULL,
	                        `IP_Port` INT(255) NULL DEFAULT NULL,
	                        PRIMARY KEY(`ID`))
                            COLLATE = 'utf8_general_ci'
                            ENGINE = InnoDB; ";
            string sqlRoute = string.Format("CREATE TABLE IF NOT EXISTS agv.`route{0}` ( `ID` int(10) NOT NULL AUTO_INCREMENT, `Program` int(11) DEFAULT NULL COMMENT '线路对应的Program', `Name` varchar(50) DEFAULT NULL COMMENT '线路名称', `CreateTime` bigint(20) NOT NULL DEFAULT '0' COMMENT '线路的创建时间', `Tag` varchar(500) DEFAULT NULL COMMENT 'tag序列', `Speed` varchar(500) DEFAULT NULL COMMENT 'tag速度序列（单位m/min)', `Stop` varchar(500) DEFAULT NULL COMMENT '停止列（单位为s）', `Turn` varchar(500) DEFAULT NULL COMMENT '转弯列，0无动作，1左转，2右转，3取消转弯', `Direction` varchar(500) DEFAULT NULL COMMENT '前进列，后退，0前进，1后退', `Pbs` varchar(500) DEFAULT NULL COMMENT 'Pbs列', `revPbs` varchar(500) DEFAULT NULL COMMENT '反向Pbs列', `Hook` varchar(500) DEFAULT NULL COMMENT 'hook列 0下降，1升起', `ChangeProgram` varchar(500) DEFAULT NULL COMMENT '修改Program', `AGV` varchar(500) DEFAULT NULL COMMENT '线路包括的agv', PRIMARY KEY(`ID`)) ENGINE = InnoDB AUTO_INCREMENT = 7 DEFAULT CHARSET = utf8 ROW_FORMAT = COMPACT; ", CreateTime.ToString());
            MySqlDataReader Dt = null;
            if (!type)
            {
                Dt = MapTags(CreateTime);
            }
            Sql.Add(sqlTag + sqlWidget + sqlLine + sqlDevice + networkconfig + sqlRoute);
            StringBuilder sqlTagd = new StringBuilder();
            foreach (Label lb in keyValues.Values)
            {
                string strTag = string.Format("INSERT INTO agv.`tag{0}` (`TagNo`, `TagName`, `X`, `Y`, `NextTag`, `NextLeftTag`, `NextRightTag`, `PreTag`, `PreLeftTag`, `PreRightTag`, `Speed`, `SpeedRev`, `StopTime`, `Pbs`, `PbsRev`, `TagTerminal`,`PreTurnSpeed`,`RevTurnSpeed`,`PreTurnPBS`,`RevTurnPBS`) VALUES ", CreateTime.ToString());
                sqlTagd.Append(strTag);
                sqlTagd.Append("('");
                sqlTagd.Append("TA" + lb.Tag);
                sqlTagd.Append("','");
                sqlTagd.Append(lb.Tag);
                sqlTagd.Append("',");
                sqlTagd.Append((((lb.Margin.Left + 19) / Size) / 10));
                sqlTagd.Append(",");
                sqlTagd.Append((((lb.Margin.Top + 11.5) / Size) / 10));
                for (int i = 0; i < 6; i++)
                {
                    sqlTagd.Append(",'N/A'");
                }
                for (int i = 0; i < 10; i++)
                {
                    sqlTagd.Append(",0");
                }
                sqlTagd.Append(")");
                sqlTagd.Append(";");
            }
            Sql.Add(sqlTagd.ToString());
            if (Dt != null)
            {
                DataTable data = MySQLHelper.ExecuteDataTable($"select table_name from information_schema.columns where table_schema = 'agv'  and table_name= 'tag{CreateTime}'  and column_name='PreTurnSpeed';");
                if (data.Rows.Count > 0)
                {
                    while (Dt.Read())
                    {
                        string sql = string.Format("UPDATE agv.`tag{0}` SET `NextTag` = '{1}',`NextLeftTag` = '{2}', `NextRightTag` = '{3}',`PreTag` = '{4}',`PreLeftTag` = '{5}',`PreRightTag` = '{6}',`Speed` = '{7}',`SpeedRev` = '{8}',`StopTime` = '{9}',`Pbs` = '{10}',`PbsRev` = '{11}', `TagTerminal` = '{12}',`PreTurnSpeed`={13},`RevTurnSpeed`={14},`PreTurnPBS`={15},`RevTurnPBS`={16}  WHERE TagName = '{17}';\n", CreateTime, Dt["NextTag"].ToString(), Dt["NextLeftTag"].ToString(), Dt["NextRightTag"].ToString(), Dt["PreTag"].ToString(), Dt["PreLeftTag"].ToString(), Dt["PreRightTag"].ToString(), Dt["Speed"].ToString(), Dt["SpeedRev"].ToString(), Dt["StopTime"].ToString(), Dt["Pbs"].ToString(), Dt["PbsRev"].ToString(), Dt["TagTerminal"].ToString(), Dt["PreTurnSpeed"].ToString(), Dt["RevTurnSpeed"].ToString(), Dt["PreTurnPBS"].ToString(), Dt["RevTurnPBS"].ToString(), Dt["TagName"].ToString());
                        Sql.Add(sql);
                    }
                }
                else
                {
                    while (Dt.Read())
                    {
                        string sql = string.Format("UPDATE agv.`tag{0}` SET `NextTag` = '{1}',`NextLeftTag` = '{2}', `NextRightTag` = '{3}',`PreTag` = '{4}',`PreLeftTag` = '{5}',`PreRightTag` = '{6}',`Speed` = '{7}',`SpeedRev` = '{8}',`StopTime` = '{9}',`Pbs` = '{10}',`PbsRev` = '{11}', `TagTerminal` = '{12}' WHERE TagName = '{13}';\n", CreateTime, Dt["NextTag"].ToString(), Dt["NextLeftTag"].ToString(), Dt["NextRightTag"].ToString(), Dt["PreTag"].ToString(), Dt["PreLeftTag"].ToString(), Dt["PreRightTag"].ToString(), Dt["Speed"].ToString(), Dt["SpeedRev"].ToString(), Dt["StopTime"].ToString(), Dt["Pbs"].ToString(), Dt["PbsRev"].ToString(), Dt["TagTerminal"].ToString(), Dt["TagName"].ToString());
                        Sql.Add(sql);
                    }
                }
                Dt.Close();
            }
        }

        /// <summary>
        /// 区域保存
        /// </summary>
        /// <param name="CreateTime"></param>
        public void InsertArea(string CreateTime, Dictionary<int, Label> Area, double Size)
        {
            StringBuilder sqlArea = new StringBuilder();
            foreach (Label lb in Area.Values)
            {
                string strArea = string.Format("INSERT INTO agv.`widget{0}` (`WidgetNo`, `Name`, `FontSize`,`ForeColor`, `X`, `Y`, `Width`, `Height`,`BackColor`,`BorderColor` , `FontPosition`) VALUES ", CreateTime.ToString());
                sqlArea.Append(strArea);
                sqlArea.Append("('");
                sqlArea.Append("AR" + lb.Tag);
                sqlArea.Append("','");
                sqlArea.Append(lb.Content);
                sqlArea.Append("',");
                sqlArea.Append((lb.FontSize) / Size);
                sqlArea.Append(",'");
                sqlArea.Append(lb.Foreground.ToString().Substring(1));
                sqlArea.Append("',");
                sqlArea.Append((lb.Margin.Left / Size) / 10);
                sqlArea.Append(",");
                sqlArea.Append((lb.Margin.Top / Size) / 10);
                sqlArea.Append(",");
                sqlArea.Append((lb.Width / Size) / 10);
                sqlArea.Append(",");
                sqlArea.Append((lb.Height / Size) / 10);
                sqlArea.Append(",'");
                sqlArea.Append(lb.Background.ToString().Substring(1));
                sqlArea.Append("','");
                sqlArea.Append(lb.BorderBrush.ToString().Substring(1));
                sqlArea.Append("','");
                sqlArea.Append(ControlRegulate.aAlignment(lb));
                sqlArea.Append("')");
                sqlArea.Append(";");
            }
            Sql.Add(sqlArea.ToString());
        }


        /// <summary>
        /// 文字保存
        /// </summary>
        /// <param name="CreateTime"></param>
        public void InsertText(string CreateTime, Dictionary<int, Label> TextControl, double Size)
        {
            StringBuilder sqlText = new StringBuilder();
            foreach (Label lb in TextControl.Values)
            {
                string strText = string.Format("INSERT INTO agv.`widget{0}` (`WidgetNo`, `Name`, `FontSize`,`ForeColor`, `X`, `Y`) VALUES ", CreateTime.ToString());
                sqlText.Append(strText);
                sqlText.Append("('");
                sqlText.Append("TE" + lb.Tag);
                sqlText.Append("','");
                sqlText.Append(lb.Content);
                sqlText.Append("'");
                sqlText.Append(",");
                sqlText.Append((lb.FontSize) / Size);
                sqlText.Append(",'");
                sqlText.Append(lb.Foreground.ToString().Substring(1));
                sqlText.Append("',");
                sqlText.Append((lb.Margin.Left / Size) / 10);
                sqlText.Append(",");
                sqlText.Append((lb.Margin.Top / Size) / 10);
                sqlText.Append(")");
                sqlText.Append(";");
            }
            Sql.Add(sqlText.ToString());
        }

        /// <summary>
        /// 线路保存
        /// </summary>
        /// <param name="CreateTime"></param>
        public void InsertLine(string CreateTime, List<WirePointArray> Line, double Size)
        {
            StringBuilder sqlLine = new StringBuilder();
            foreach (WirePointArray point in Line)
            {
                string strLine = string.Format("INSERT INTO agv.`line{0}` (`StartX`, `StartY`, `EndX`, `EndY`, `LineStyel` , `Tag1`, `Tag2`) VALUES ", CreateTime.ToString());
                sqlLine.Append(strLine);
                sqlLine.Append("('");
                sqlLine.Append((point.GetPoint.SetPoint.X / 10) / Size);
                sqlLine.Append("','");
                sqlLine.Append((point.GetPoint.SetPoint.Y / 10) / Size);
                sqlLine.Append("','");
                sqlLine.Append((point.GetWirePoint.SetPoint.X / 10) / Size);
                sqlLine.Append("','");
                sqlLine.Append((point.GetWirePoint.SetPoint.Y / 10) / Size);
                sqlLine.Append("','");
                if (point.circuitType.Equals(CircuitType.Line))
                {
                    sqlLine.Append(1);
                }
                else if (point.circuitType.Equals(CircuitType.Broken))
                {
                    sqlLine.Append(2);
                }
                else
                {
                    sqlLine.Append(3);
                }
                sqlLine.Append("','");
                sqlLine.Append("TA" + point.GetPoint.TagID);
                sqlLine.Append("','");
                sqlLine.Append("TA" + point.GetWirePoint.TagID);
                sqlLine.Append("')");
                sqlLine.Append(";");
            }
            Sql.Add(sqlLine.ToString());
        }

        public MySqlDataReader MapRoute(string MapName)
        {
            return MySQLHelper.ExecuteReader(string.Format("SELECT * FROM `agv`.`route{0}`", MapName));
        }

        public MySqlDataReader MapRouteDAL(string MapName)
        {
            return MySQLHelper.ExecuteReader(string.Format("SELECT `Program`,`Name`,`CreateTime` FROM `agv`.`route{0}`", MapName));
        }

        public bool SaveRoute(Route route, bool edit, long UTCTime)
        {
            if (edit)
            {
                string sql = string.Format("UPDATE agv.`route{0}` SET  `Name` = '{1}', `Tag` = '{2}', `Speed` = '{3}', `Stop` = '{4}', `Turn` = '{5}', `Direction` = '{6}', Pbs = '{7}', Hook = '{8}', `AGV` = '{9}', `ChangeProgram` = '{10}',`Program` = {11}  WHERE CreateTime = {12}", UTCTime, route.Name, route.Tag, route.Speed, route.Stop, route.Turn, route.Direction, route.Pbs, route.Hook, "0", route.ChangeProgram, route.Program, route.CreateTime);
                return MySQLHelper.ExecuteNonQuery(sql) > 0 ? true : false;
            }
            else
            {
                string sql = string.Format("INSERT INTO agv.`route{0}` (`Program`, `Name`, `CreateTime`, `Tag`, `Speed`, `Stop`, `Turn`, `Direction`, `Pbs`, `Hook`, `ChangeProgram`, `AGV`)  VALUES ('{1}', '{2}', {3}, '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}') ", UTCTime, route.Program, route.Name, route.CreateTime, route.Tag, route.Speed, route.Stop, route.Turn, route.Direction, route.Pbs, route.Hook, route.ChangeProgram, "0");
                return MySQLHelper.ExecuteNonQuery(sql) > 0 ? true : false;
            }
        }

        public bool ExistsProgram(string Program, long MapTime, long UTCTime)
        {
            DataTable dt = MySQLHelper.ExecuteDataTable(string.Format("SELECT `CreateTime` FROM agv.`route{0}` WHERE `Program` = {1} AND `CreateTime`!={2}", MapTime, Program, UTCTime));
            return dt.Rows.Count > 0 ? true : false;
        }

        public string[] SelectTag(long CreateTime, string TagNo)
        {
            List<string> listSTag = new List<string>();

            string sql1 = string.Format("SELECT tag1 FROM agv.line{0} WHERE tag2 = 'TA{1}'", CreateTime, TagNo);
            MySqlDataReader mr1 = MySQLHelper.ExecuteReader(sql1);
            while (mr1.Read())
            {
                StringBuilder sb = new StringBuilder(mr1.GetString(0));
                listSTag.Add(sb.Remove(0, 2).ToString());
            }
            mr1.Close();

            string sql2 = string.Format("SELECT tag2 FROM agv. line{0} WHERE tag1 = 'TA{1}'", CreateTime, TagNo);
            MySqlDataReader mr2 = MySQLHelper.ExecuteReader(sql2);
            while (mr2.Read())
            {
                StringBuilder sb = new StringBuilder(mr2.GetString(0));
                listSTag.Add(sb.Remove(0, 2).ToString());
            }
            mr2.Close();
            string[] selectTag = listSTag.ToArray();
            return selectTag;
        }


        /// <summary>
        /// 删除线路
        /// </summary>
        /// <param name="MapTime"></param>
        /// <param name="Program"></param>
        /// <returns></returns>
        public bool DelRoute(long MapTime, int Program)
        {
            string sql = string.Format("DELETE FROM agv.`route{0}` WHERE `Program` = {1}", MapTime, Program);
            return MySQLHelper.ExecuteNonQuery(sql) > 0 ? true : false;
        }



        /// <summary>
        /// 查询所有Tag
        /// </summary>
        /// <param name="exls"></param>
        /// <returns></returns>
        public MySqlDataReader GetMapTags(long exls)
        {
            return MySQLHelper.ExecuteReader("SELECT * FROM agv.tag" + exls + " ORDER BY (TagName+0)");
        }


        public bool UpdateTagInfo(long MapTime, ObservableCollection<MapTag> MapArray)
        {
            DataTable data = MySQLHelper.ExecuteDataTable($"select table_name from information_schema.columns where table_schema = 'agv'  and table_name= 'tag{MapTime}'  and column_name='PreTurnSpeed';");
            var Speed = MainInfo.agvSpeed.ToList();
            var PBS = MainInfo.agvPbs.ToList();
            List<string> sqlText = new List<string>();
            if (data.Rows.Count > 0)
            {
                foreach (MapTag item in MapArray)
                {
                    string sql = string.Format("UPDATE agv.`tag{0}` SET `NextTag` = '{1}',`NextLeftTag` = '{2}', `NextRightTag` = '{3}',`PreTag` = '{4}',`PreLeftTag` = '{5}',`PreRightTag` = '{6}',`Speed` = {7},`SpeedRev` = {8},`StopTime` = {9},`Pbs` = {10},`PbsRev` = {11}, `TagTerminal` = '{12}',`PreTurnSpeed`={13},`RevTurnSpeed`={14},`PreTurnPBS`={15},`RevTurnPBS`={16} WHERE TagName = {17};", MapTime, item.NextTag, item.NextLeftTag, item.NextRightTag, item.PreTag, item.PreLeftTag, item.PreRightTag, Speed.IndexOf(item.Speed).ToString(), Speed.IndexOf(item.SpeedRev).ToString(), item.StopTime, PBS.IndexOf(item.Pbs).ToString(), PBS.IndexOf(item.PbsRev).ToString(), item.TagTerminal, Speed.IndexOf(item.PreTurnSpeed), Speed.IndexOf(item.RevTurnSpeed), PBS.IndexOf(item.PreTurnPBS), PBS.IndexOf(item.RevTurnPBS), item.TagName);
                    sqlText.Add(sql);
                }
                return MySQLHelper.ExecuteSqlTran(sqlText);
            }
            else
            {
                foreach (MapTag item in MapArray)
                {
                    string sql = string.Format("UPDATE agv.`tag{0}` SET `NextTag` = '{1}',`NextLeftTag` = '{2}', `NextRightTag` = '{3}',`PreTag` = '{4}',`PreLeftTag` = '{5}',`PreRightTag` = '{6}',`Speed` = {7},`SpeedRev` = {8},`StopTime` = {9},`Pbs` = {10},`PbsRev` = {11}, `TagTerminal` = '{12}' WHERE TagName = {13};", MapTime, item.NextTag, item.NextLeftTag, item.NextRightTag, item.PreTag, item.PreLeftTag, item.PreRightTag, Speed.IndexOf(item.Speed).ToString(), Speed.IndexOf(item.SpeedRev).ToString(), item.StopTime, PBS.IndexOf(item.Pbs).ToString(), PBS.IndexOf(item.PbsRev).ToString(), item.TagTerminal, item.TagName);
                    sqlText.Add(sql);
                }
                return MySQLHelper.ExecuteSqlTran(sqlText);
            }
        }

        /// <summary>
        /// 初始化数据库连接
        /// </summary>
        public void CreateDB()
        {
            try
            {
                string DB = string.Format("CREATE DATABASE IF NOT EXISTS `agv`; ");
                string userSql = string.Format("CREATE TABLE IF NOT EXISTS agv.`user` ( `ID` int(10) NOT NULL AUTO_INCREMENT, `User` varchar(50) NOT NULL COMMENT '用户名', `Password` varchar(50) DEFAULT NULL COMMENT '密码', `CardNo` varchar(50) DEFAULT NULL COMMENT '工牌号', `Authorization` varchar(50) NOT NULL COMMENT '权限，0管理员，1调试用户，2普通用户', PRIMARY KEY(`ID`)) ENGINE = InnoDB AUTO_INCREMENT = 2 DEFAULT CHARSET = utf8; ");
                string mapSql = string.Format("CREATE TABLE IF NOT EXISTS agv.`map` ( `ID` int(10) NOT NULL AUTO_INCREMENT, `CreateTime` bigint(20) DEFAULT NULL COMMENT '创建时间', `Name` varchar(50) DEFAULT NULL COMMENT '地图名称', `Width` double DEFAULT NULL COMMENT '地图宽度（米）', `Height` double unsigned DEFAULT NULL COMMENT '地图长度（米）',  `AGV` varchar(50) DEFAULT NULL COMMENT '地图上注册的AGV',  `Type` int(11) DEFAULT NULL COMMENT '地图类型，磁标，RFID，激光 0磁标，1RFID，2激光', PRIMARY KEY(`ID`)) ENGINE = InnoDB AUTO_INCREMENT = 42 DEFAULT CHARSET = utf8 ROW_FORMAT = COMPACT COMMENT = '取名为map+UTC时间，如map1234567890'; ");
                string settingSql = string.Format("CREATE TABLE IF NOT EXISTS agv.`setting` (`ID` int(11) NOT NULL AUTO_INCREMENT, `Map` bigint(20) DEFAULT '0' COMMENT '默认地图', `Mode` int(10) DEFAULT '0' COMMENT '启动方式：0.编辑模式，1.自启动模式', PRIMARY KEY(`ID`)) ENGINE = InnoDB AUTO_INCREMENT = 2 DEFAULT CHARSET = utf8; ");
                MySQLHelper.ExecuteSqlTran(new List<string>() { DB, userSql, mapSql, settingSql });
            }
            catch{}
        }

        public bool Savenetwork(long CreateTime, List<string> Ip, List<string> port)
        {
            string networkconfig = $"CREATE TABLE IF NOT EXISTS agv.`Networkconfig{CreateTime}` (`ID` INT(255) NOT NULL AUTO_INCREMENT, `IP_Address` VARCHAR(255) NULL DEFAULT NULL, `IP_Port` INT(255) NULL DEFAULT NULL,PRIMARY KEY(`ID`))COLLATE = 'utf8_general_ci'ENGINE = InnoDB; ";
            string cleartext = $"DELETE FROM agv.`Networkconfig{CreateTime}`";
            List<string> Sql = new List<string>();
            Sql.Add(networkconfig + cleartext);
            for (int i = 0; i < Ip.Count; i++)
            {
                Sql.Add($"INSERT INTO agv.`Networkconfig{CreateTime}`(`IP_Address`,`IP_Port`)Values('{Ip[i]}',{port[i]});");
            }
            return MySQLHelper.ExecuteSqlTran(Sql);
        }

        public MySqlDataReader SelectNetwork(long CreateTime)
        {
            if (TableNotexist($"networkconfig{CreateTime}"))
                return MySQLHelper.ExecuteReader($"SELECT * FROM agv.`networkconfig{CreateTime}`");
            else
                return null;
        }

        public bool TableNotexist(string TableName)
        {
            return MySQLHelper.ExecuteDataTable($"SELECT table_name FROM information_schema.TABLES WHERE table_name ='{TableName}';").Rows.Count > 0;
        }

        public bool ClearLog(string TableName)
        {
            return MySQLHelper.ExecuteNonQuery($"DROP TABLE  `agv`.`{TableName}`") > 0;
        }


        public DataTable PagingSelect(string LogDate, string StartTime, string StopTime, string AgvNum, int Mes, int Inedex, int PageSize, out int PageCount)
        {
            DataTable ds = MySQLHelper.ExecuteDataTable("SELECT table_name FROM information_schema.TABLES WHERE table_name ='loginfo" + LogDate + "';");
            if (ds.Rows.Count > 0)
            {
                string selectList = "`UtcTime`,`AgvNum`,`MasterOn`,`Speed`,`Drive`,`Left`,`Direction`,`Hook`,`Power`,`Pbs`,`Rfid`,`RouteNo`,`ErrorCode`";
                string tableName = $"agv.`loginfo{LogDate}`";
                StringBuilder whereStr = new StringBuilder();
                whereStr.Append($" and UtcTime>={StartTime}");
                whereStr.Append($" and UtcTime<={StopTime}");
                if (AgvNum != "0")
                {
                    whereStr.Append($" and AgvNum={AgvNum}");
                }
                if (Mes == 1)
                {
                    whereStr.Append(" and ErrorCode = 0");
                }
                else if (Mes == 2)
                {
                    whereStr.Append(" and ErrorCode <> 0");
                }
                return MySQLHelper.GetPager(out PageCount, selectList, tableName, whereStr.ToString(), "ID", Inedex, PageSize);
            }
            else
            {
                PageCount = 0;
                return null;
            }
        }
    }
}
