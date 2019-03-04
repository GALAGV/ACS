using AGVSystem.Infrastructure.agvCommon;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AGVSystem.BLL.ServiceLogicBLL;

namespace AGVSystem.APP.LogClearService
{
    /// <summary>
    /// 日志清理
    /// </summary>
    public class LogServe
    {
        private CancellationTokenSource cancellationTokenSource { get; set; } //Task的控制，如取消、定时取消
        private List<string> Table { get; set; } //待清理日志表名称集合
        private int interval { get; set; }  //日期间隔 单位一个月 ，如上个月：-1 
        private Ga_mapBLL _Serve { get; set; } 

        #region 构造函数

        public LogServe()
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            this.Table = new List<string>();
            this.interval = 1;
            this._Serve = new Ga_mapBLL();
        }
        #endregion

        #region 开启服务

        public void LogOnStart()
        {
            try
            {
                if (OperateIniTool.Exist)
                {
                    interval = OperateIniTool.OperateIniRead("Table", "interval").TransformInt();
                    string Tableresult = OperateIniTool.OperateIniRead("Table", "Name");
                    Table = !string.IsNullOrEmpty(Tableresult) ? Tableresult.Split(',').ToList() : new List<string>();
                    WriteLog.writeLogInfo("LogClear", "Log", "配置读取成功");
                }
                else
                {
                    WriteLog.writeLogInfo("LogClear", "Log", "配置文件丢失");
                }
                Task.Factory.StartNew(() =>
                {
                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        ClearLog();
                    }
                }, cancellationTokenSource.Token);
                WriteLog.writeLogInfo("LogClear", "Log", "日志清理服务启动成功");
            }
            catch (Exception ex)
            {
                cancellationTokenSource.Cancel();
                WriteLog.writeLogInfo("LogClear", "Log", "服务启动出错,错误信息:" + ex.ToString() + "");
            }
        }
        #endregion

        #region 结束服务

        /// <summary>
        /// 服务结束
        /// </summary>
        public void LogOnStop()
        {
            try
            {
                cancellationTokenSource.Cancel();
                WriteLog.writeLogInfo("LogClear", "Log", "服务已停止！");
            }
            catch (Exception ex)
            {
                WriteLog.writeLogInfo("LogClear", "Log", $"服务停止异常，异常信息：{ex.Message.ToString()}");
            }
        }
        #endregion

        #region 计算日期范围

        /// <summary>
        ///计算日期范围
        /// </summary>
        /// <param name="Table"></param>
        public void ClearLog()
        {
            try
            {
                int ThisDate = Convert.ToInt32(DateTime.Now.ToString("dd")); //当前日期
                int LastDate = Convert.ToInt32(DataTimeTool.LastDayOfMonth(DateTime.Now.AddMonths(-interval)).ToString("dd")); //本月最后一天
                if (ThisDate.Equals(LastDate))
                {
                    //本月最后一天清理上个月所有日志
                    ClearTable(DataTimeTool.FirstDayOfMonth(DateTime.Now.AddMonths(-interval)), DataTimeTool.LastDayOfMonth(DateTime.Now.AddMonths(-interval)));
                }
                else
                {
                    //清理上个月一号到上个月相同日期所有日志
                    ClearTable(DataTimeTool.FirstDayOfMonth(DateTime.Now.AddMonths(-interval)), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")).AddMonths(-1));
                }
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                WriteLog.writeLogInfo("LogClear", "Log", "服务运行中出错，错误信息：" + ex.ToString() + "");
            }
        }
        #endregion

        #region 删除日志表

        /// <summary>
        ///  删除日志表
        /// </summary>
        /// <param name="start"></param>
        /// <param name="lastTime"></param>
        /// <param name="Table"></param>
        public void ClearTable(DateTime start, DateTime lastTime)
        {
            for (DateTime dt = start; dt < lastTime; dt = dt.AddDays(1))
            {
                string Time = dt.ToString("yyyyMMdd");
                foreach (var item in Table)
                {
                    if (_Serve.TableNotexistBLL($"{item}{Time}"))
                    {
                        _Serve.ClearLogBLL($"{item}{Time}");
                        WriteLog.writeLogInfo("LogClear", "Log", $"ClearTable:{item}{Time}");
                    }
                }
            }
        }

        #endregion
    }
}
