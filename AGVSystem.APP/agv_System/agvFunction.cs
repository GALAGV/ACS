using AGVSystem.IService.IO_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using AGVSystem.Model;
using AGVSystem.IService.IO_BLL;
using AGVSystem.BLL;
using AGVSystem.Model.Ga_agvModels;

namespace AGVSystem.APP.agv_System
{
    public class agvFunction : IO_AGVmanagement
    {
        IO_agvBLL GetAgvBLL = new Ga_agvBLL();

        /// <summary>
        /// AGV信息
        /// </summary>
        /// <returns></returns>
        public List<Ga_agvStatus> AgvInfo()
        {
            return new List<Ga_agvStatus>()
            {
                new Ga_agvStatus { StatusName = "AGV",  StatusValue = "1" },
                new Ga_agvStatus { StatusName = "网络状态", StatusValue = "在线" },
                new Ga_agvStatus { StatusName = "运行准备", StatusValue = "ON" },
                new Ga_agvStatus { StatusName = "驱动状态", StatusValue = "上升" },
                new Ga_agvStatus { StatusName = "脱轨状态", StatusValue = "正常" },
                new Ga_agvStatus { StatusName = "出发位置", StatusValue = "5" },
                new Ga_agvStatus { StatusName = "目的位置", StatusValue = "4" },
                new Ga_agvStatus { StatusName = "任务名称", StatusValue = "运输" },
                new Ga_agvStatus { StatusName = "报警信息", StatusValue = "正常" },
            };
        }

        /// <summary>
        /// 显示所有AGV信息
        /// </summary>
        /// <param name="Agvlist"></param>
        /// <param name="selAgv"></param>
        /// <returns></returns>
        public DataTable AgvInfo(long Time,ref int selAgv)
        {
            List<int> Agvlist = GetAgvBLL.AGVNumList(Time);
            DataTable dt = new DataTable("TabAgvMoveInfo");
            dt.Columns.Add(new DataColumn("type"));
            dt.Columns.Add(new DataColumn("TagName"));
            dt.Columns.Add(new DataColumn("Speed"));
            dt.Columns.Add(new DataColumn("turn"));
            dt.Columns.Add(new DataColumn("Dir"));
            dt.Columns.Add(new DataColumn("Hook"));
            dt.Columns.Add(new DataColumn("Rfid"));
            dt.Columns.Add(new DataColumn("Program"));
            dt.Columns.Add(new DataColumn("Step"));

            for (int i = 0; i < Agvlist.Count; i++)
            {
                dt.Rows.Add(new object[] { "离线", Agvlist[i], "", "", "", "", "", "" });
                MainInfo.agvNo.Add(Agvlist[i]);
            }
            if (Agvlist.Count > 0)
            {
                selAgv = Agvlist[0];
            }
            return dt;
        }






    }
}
