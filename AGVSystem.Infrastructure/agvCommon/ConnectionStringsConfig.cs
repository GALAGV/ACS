using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.Infrastructure.agvCommon
{
   public  static class ConnectionStringsConfig
    {
        public static void UpdateConnectionStringsConfig(string key, string conString)
        {
            bool isModified = false;    //记录该连接串是否已经存在 
            if (ConfigurationManager.ConnectionStrings[key] != null)
            {
                isModified = true;
            }
            //新建一个连接字符串实例 
            ConnectionStringSettings mySettings = new ConnectionStringSettings(key, conString);

            // 打开可执行的配置文件*.exe.config 
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // 如果连接串已存在，首先删除它 
            if (isModified)
            {
                config.ConnectionStrings.ConnectionStrings.Remove(key);
            }
            // 将新的连接串添加到配置文件中. 
            config.ConnectionStrings.ConnectionStrings.Add(mySettings);
            // 保存对配置文件所作的更改 
            config.Save(ConfigurationSaveMode.Modified);
            // 强制重新载入配置文件的ConnectionStrings配置节  
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        /// <summary>
        /// 获取连接节点值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConnectionValue(string key)
        {
            if (ConfigurationManager.ConnectionStrings[key] != null)
                return ConfigurationManager.ConnectionStrings[key].ConnectionString;
            return string.Empty;
        }
    }
}
