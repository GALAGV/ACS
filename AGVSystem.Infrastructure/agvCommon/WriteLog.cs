using System;
using System.IO;

namespace AGVSystem.Infrastructure.agvCommon
{
    public class WriteLog
    {
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="DireName">文件夹名称</param>
        /// <param name="LogName">文件名称</param>
        /// <param name="LogInfo">日志内容</param>
        public static void writeLogInfo(string DireName, string LogName, string LogInfo)
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\" + DireName))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\" + DireName);
            }
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\" + DireName + "\\" + LogName + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
            FileStream fs = new FileStream(filePath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);

            sw.Write(DateTime.Now.ToString() + " " + LogInfo + "\r\n");
            sw.Close();
            fs.Close();
        }


    }
}
