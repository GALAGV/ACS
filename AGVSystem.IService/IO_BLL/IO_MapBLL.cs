using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystem.IService.IO_BLL
{
    /// <summary>
    /// 地图信息业务逻辑接口
    /// </summary>
    public interface IO_MapBLL
    {
        DataTable ListDevice(long MapTime);
    }
}
