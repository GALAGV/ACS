using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace AGVSystem.Infrastructure.agvCommon
{
   public class WriteLog
    {
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
