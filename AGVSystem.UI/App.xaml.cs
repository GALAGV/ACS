using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AGVSystem.Infrastructure.agvCommon;

namespace AGVSystem.UI
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //UI线程未捕获异常处理事件（UI主线程）
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            //非UI线程未捕获异常处理事件(子线程)
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        //UI线程未捕获异常处理事件（UI主线程）
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            OnExceptionHandler(ex);
            e.Handled = true;//表示异常已处理，可以继续运行
        }

        //非UI线程未捕获异常处理事件
        //如果UI线程异常DispatcherUnhandledException未注册，则如果发生了UI线程未处理异常也会触发此异常事件
        //此机制的异常捕获后应用程序会直接终止。没有像DispatcherUnhandledException事件中的Handler=true的处理方式
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                OnExceptionHandler(ex);
            }
        }

        //Task线程内未捕获异常处理事件
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            OnExceptionHandler(ex);
        }

        //异常处理 封装
        private void OnExceptionHandler(Exception ex)
        {
            if (ex != null)
            {
                string errorMsg = "";
                if (ex.InnerException != null)
                {
                    errorMsg += String.Format("【InnerException】{0}\n{1}\n", ex.InnerException.Message, ex.InnerException.StackTrace);
                }
                errorMsg += String.Format("{0}\n{1}", ex.Message, ex.StackTrace);

                WriteLog.writeLogInfo("Error", "Log", errorMsg);//写入日志
            }
        }
    }
}
