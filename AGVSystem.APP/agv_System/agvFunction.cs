using AGVSystem.IService.IO_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using AGVSystem.Model;

namespace AGVSystem.APP.agv_System
{
    public class agvFunction : IO_AGVmanagement
    {
        /// <summary>
        /// AGV信息
        /// </summary>
        /// <returns></returns>
        public DataTable AgvInfo()
        {
            DataTable AgvData = new DataTable("AgvInfo");
            AgvData.Columns.Add(new DataColumn("Agv"));
            AgvData.Columns.Add(new DataColumn("Info"));
            AgvData.Rows.Add(new object[] { "AGV", "" });

            AgvData.Rows.Add(new object[] { "报警信息", "" });
            AgvData.Rows.Add(new object[] { "Program", "" });
            AgvData.Rows.Add(new object[] { "Step", "" });
            AgvData.Rows.Add(new object[] { "出发地", "" });
            AgvData.Rows.Add(new object[] { "目的地", "" });
            AgvData.Rows.Add(new object[] { "任务名", "" });
            return AgvData;
        }

        /// <summary>
        /// 显示所有AGV信息
        /// </summary>
        /// <param name="Agvlist"></param>
        /// <param name="selAgv"></param>
        /// <returns></returns>
        public DataTable AgvInfo(List<int> Agvlist, ref int selAgv)
        {
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
