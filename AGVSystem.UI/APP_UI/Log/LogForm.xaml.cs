using AGVSystem.APP.agv_Map;
using AGVSystem.APP.agv_System;
using AGVSystem.BLL.ServiceLogicBLL;
using AGVSystem.Infrastructure.agvCommon;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AGVSystem.UI.APP_UI.Log
{
    /// <summary>
    /// LogForm.xaml 的交互逻辑
    /// </summary>
    public partial class LogForm : Window
    {
        private Ga_mapBLL Ga_Map = new Ga_mapBLL();
        private agvFunction Function = new agvFunction();
        private agvMapRegulate regulate = new agvMapRegulate();
        private int PageCount = 0; //总记录条数
        private int Index = 1;
        public LogForm()
        {
            InitializeComponent();
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            Index = 1;
            LoadLogInfo();
        }

        private void LoadLogInfo()
        {
            if (!Verify())
                return;

            string Times = (Convert.ToDateTime(DateTimes.Text)).ToString("yyyyMMdd");
            string StartTime = (Convert.ToDateTime($"{StartHour.Text.Trim()}:{Startminute.Text.Trim()}:{Startsecond.Text.Trim()}")).ToString("HHmmss");
            string StopTime = (Convert.ToDateTime($"{StopHour.Text.Trim()}:{Stopminute.Text.Trim()}:{Stopsecond.Text.Trim()}")).ToString("HHmmss");
            string AGVNum = string.IsNullOrWhiteSpace(AgvNum.Text) ? "0" : AgvNum.Text;

            int Mes = MsgType.SelectedIndex;

            DataTable data = Ga_Map.PagingSelectBLL(Times, StartTime, StopTime, AGVNum, Mes, Index, Convert.ToInt32(PagSize.Text), out PageCount);
            if (data == null)
            {
                Panel.SetZIndex(Panel2, 1);
            }
            else
            {
                if (data.Rows.Count == 0)
                {
                    Panel.SetZIndex(Panel2, 1);
                }
                else
                {
                    Panel.SetZIndex(Panel2, -1);
                }
            }
            Table_Data.DataContext = data;
            Table_Data.AutoGenerateColumns = false;

            CountData.Content = PageCount % Convert.ToInt32(PagSize.Text) == 0 ? PageCount / Convert.ToInt32(PagSize.Text) : (PageCount / Convert.ToInt32(PagSize.Text)) + 1;
            Counts.Content = PageCount;
            PagIndex.Content = Index;
        }


        #region 表单验证

        public bool Verify()
        {
            if (string.IsNullOrEmpty(DateTimes.Text))
            {
                MessageBoxTool.Error("请选择查询日期！");
                return false;
            }
            if (!string.IsNullOrEmpty(AgvNum.Text))
            {
                if (!FormatVerification.IsFloat(AgvNum.Text.Trim()))
                {
                    MessageBoxTool.Error("AGV编号格式输入错误！");
                    return false;
                }
            }
            if (FormatVerification.IsFloat(StartHour.Text))
            {
                if (FormatVerification.Strfloat(StartHour.Text))
                {
                    MessageBoxTool.Error("开始时间小时格式输入错误！");
                    return false;
                }
                else
                {
                    if (Convert.ToInt32(StartHour.Text) > 23 || Convert.ToInt32(StartHour.Text) < 0)
                    {
                        MessageBoxTool.Error("开始时间小时格式输入错误！");
                        return false;
                    }
                }
            }
            else
            {
                MessageBoxTool.Error("开始时间格小时式输入错误！");
                return false;
            }
            if (FormatVerification.IsFloat(Startminute.Text))
            {
                if (FormatVerification.Strfloat(Startminute.Text))
                {
                    MessageBoxTool.Error("开始时间分钟格式输入错误！");
                    return false;
                }
                else
                {
                    if (Convert.ToInt32(Startminute.Text) > 60 || Convert.ToInt32(Startminute.Text) < 0)
                    {
                        MessageBoxTool.Error("开始时间分钟格式输入错误！");
                        return false;
                    }
                }
            }
            else
            {
                MessageBoxTool.Error("开始时间分钟格式输入错误！");
                return false;
            }

            if (FormatVerification.IsFloat(Startsecond.Text))
            {
                if (FormatVerification.Strfloat(Startsecond.Text))
                {
                    MessageBoxTool.Error("开始时间秒格式输入错误！");
                    return false;
                }
                else
                {
                    if (Convert.ToInt32(Startsecond.Text) > 60 || Convert.ToInt32(Startsecond.Text) < 0)
                    {
                        MessageBoxTool.Error("开始时间秒格式输入错误！");
                        return false;
                    }
                }
            }
            else
            {
                MessageBoxTool.Error("开始时间秒格式输入错误！");
                return false;
            }
            if (FormatVerification.IsFloat(StopHour.Text))
            {
                if (FormatVerification.Strfloat(StopHour.Text))
                {
                    MessageBoxTool.Error("结束时间小时格式输入错误！");
                    return false;
                }
                else
                {
                    if (Convert.ToInt32(StopHour.Text) > 23 || Convert.ToInt32(StopHour.Text) < 0)
                    {
                        MessageBoxTool.Error("结束时间小时格式输入错误！");
                        return false;
                    }
                }
            }
            else
            {
                MessageBoxTool.Error("结束时间格小时式输入错误！");
                return false;
            }
            if (FormatVerification.IsFloat(Stopminute.Text))
            {
                if (FormatVerification.Strfloat(Stopminute.Text))
                {
                    MessageBoxTool.Error("结束时间分钟格式输入错误！");
                    return false;
                }
                else
                {
                    if (Convert.ToInt32(Stopminute.Text) > 60 || Convert.ToInt32(Stopminute.Text) < 0)
                    {
                        MessageBoxTool.Error("结束时间分钟格式输入错误！");
                        return false;
                    }
                }
            }
            else
            {
                MessageBoxTool.Error("结束时间分钟格式输入错误！");
                return false;
            }
            if (FormatVerification.IsFloat(Stopsecond.Text))
            {
                if (FormatVerification.Strfloat(Stopsecond.Text))
                {
                    MessageBoxTool.Error("结束时间秒格式输入错误！");
                    return false;
                }
                else
                {
                    if (Convert.ToInt32(Stopsecond.Text) > 60 || Convert.ToInt32(Stopsecond.Text) < 0)
                    {
                        MessageBoxTool.Error("结束时间秒格式输入错误！");
                        return false;
                    }
                }
            }
            else
            {
                MessageBoxTool.Error("结束时间秒格式输入错误！");
                return false;
            }
            return true;
        }


        #endregion

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            if (Table_Data.Items.Count > 0)
            {
                if (Index == 1)
                    return;

                Index--;
                LoadLogInfo();
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (Table_Data.Items.Count > 0)
            {
                if (Convert.ToInt32(CountData.Content) <= Index)
                    return;

                Index++;
                LoadLogInfo();
            }
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            if (Table_Data.Items.Count > 0)
            {
                if (FormatVerification.IsFloat(IndexPs.Text.Trim()))
                {
                    if (!FormatVerification.Strfloat(IndexPs.Text.Trim()))
                    {
                        int Ind = Convert.ToInt32(IndexPs.Text.Trim());
                        if (Ind > 0 && Ind <= Convert.ToInt32(CountData.Content))
                        {
                            Index = Ind;
                        }
                        LoadLogInfo();
                    }
                }
                else
                {
                    MessageBoxTool.Error("格式输入错误！");
                }
            }
        }

        /// <summary>
        /// 导出日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Write_Log_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DateTimes.Text))
            {
                MessageBoxTool.Error("请选择查询日期！");
                return;
            }
            string Times = (Convert.ToDateTime(DateTimes.Text)).ToString("yyyyMMdd");
            if (Ga_Map.TableNotexistBLL($"loginfo{Times}"))
            {
                Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                sfd.Filter = "日志文件|*.log";
                sfd.FileName = $"log_{Times}";
                if (sfd.ShowDialog() == true)
                {
                    Function.LogWrite(sfd.FileName, Times);
                }
            }
            else
            {
                MessageBoxTool.Error("暂无日志！");
            }
        }

        /// <summary>
        /// 导入log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Read_log_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".log";
            dlg.Filter = "日志文件|*.log";
            if (dlg.ShowDialog() == true)
            {
                string sqlText = File.ReadAllText(dlg.FileName);
                if (regulate.AGV_MapTolead(sqlText) == true)
                {
                    MessageBox.Show("导入成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("导入失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
