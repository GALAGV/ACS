using AGVSystem.BLL;
using AGVSystem.IService.IO_BLL;
using AGVSystem.IService.IO_System;
using AGVSystem.Model;
using AGVSystem.Model.Ga_agvModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.APP.agv_Map
{
    public class agvMapRegulate : IO_AGVMapService
    {
        IO_MapBLL IO_AGVMapService = new Ga_mapBLL();

        public List<Ga_Map> GetMapRegulate()
        {
            List<Ga_Map> ga_s = new List<Ga_Map>();
            MySqlDataReader mySql = IO_AGVMapService.MapArray();
            while (mySql.Read())
            {
                ga_s.Add(
                    new Ga_Map()
                    {
                        ID = Convert.ToInt32(mySql["ID"].ToString()),
                        Name = mySql["Name"].ToString(),
                        Width = Convert.ToDouble(mySql["Width"].ToString()),
                        Height = Convert.ToDouble(mySql["Height"].ToString()),
                        CreateTime = long.Parse(mySql["CreateTime"].ToString())
                    });
            }
            mySql.Close();
            return ga_s;
        }
    }
}
