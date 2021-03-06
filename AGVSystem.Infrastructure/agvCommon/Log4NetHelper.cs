﻿using System;
using System.Collections.Generic;
using System.Xml;
using log4net;
using log4net.Config;

namespace AGVSystem.Infrastructure.agvCommon
{
    /// <summary>
    /// 日志
    /// </summary>
    public class Log4NetHelper
    {
        private static string m_logFile;

        private static Dictionary<string, ILog> m_lstLog = new Dictionary<string, ILog>();

        static Log4NetHelper()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "log4net.config";
            XmlConfigurator.Configure(new System.IO.FileInfo(path));
            m_logFile = path;
            m_lstLog["command_log"] = LogManager.GetLogger("command_log");
            m_lstLog["task_log"] = LogManager.GetLogger("task_log");
            m_lstLog["debug_log"] = LogManager.GetLogger("debug_log");
            m_lstLog["error_log"] = LogManager.GetLogger("error_log");
        }

        /// <summary>
        /// 写入Command日志
        /// </summary>
        /// <param name="strInfoLog">strInfoLog</param>
        public static void WriteCommandLog(string strInfoLog)
        {

            if (m_lstLog["command_log"].IsInfoEnabled)
            {
                m_lstLog["command_log"].Info(strInfoLog);
            }
        }


        /// <summary>
        /// 写入Task日志
        /// </summary>
        /// <param name="strInfoLog">strInfoLog</param>
        public static void WriteTaskLog(string strInfoLog, Exception ex = null)
        {
            if (m_lstLog["task_log"].IsInfoEnabled)
            {
                m_lstLog["task_log"].Error(strInfoLog, ex);
            }
        }

        /// <summary>
        /// 写入Debug日志
        /// </summary>
        /// <param name="strErrLog">strErrLog</param>
        /// <param name="ex">Exception</param>
        public static void WriteDebugLog(string strErrLog, Exception ex = null)
        {
            if (m_lstLog["debug_log"].IsErrorEnabled)
            {
                m_lstLog["debug_log"].Debug(strErrLog, ex);
            }
        }

        /// <summary>
        /// 写入Error日志
        /// </summary>
        /// <param name="strErrLog">strErrLog</param>
        /// <param name="ex">Exception</param>
        public static void WriteErrorLog(string strErrLog, Exception ex = null)
        {
            if (m_lstLog["error_log"].IsErrorEnabled)
            {
                m_lstLog["error_log"].Error(strErrLog, ex);
            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="strType">日志类型</param>
        /// <param name="strLog">strLog</param>
        public static void WriteByLogType(string strType, string strLog)
        {
            if (!m_lstLog.ContainsKey(strType))
            {
                //判断是否存在节点
                if (!HasLogNode(strType))
                {
                    WriteErrorLog("log4net配置文件不存在【" + strType + "】配置");
                    return;
                }
                m_lstLog[strType] = log4net.LogManager.GetLogger(strType);
            }
            m_lstLog[strType].Error(strLog);
        }

        /// <summary>
        /// 是否存在指定的配置
        /// </summary>
        /// <param name="strNodeName">strNodeName</param>
        /// <returns>返回值</returns>
        private static bool HasLogNode(string strNodeName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(m_logFile);
            var lstNodes = doc.SelectNodes("//configuration/log4net/logger");
            foreach (XmlNode item in lstNodes)
            {
                if (item.Attributes["name"].Value.ToLower() == strNodeName)
                    return true;
            }
            return false;
        }
    }
}