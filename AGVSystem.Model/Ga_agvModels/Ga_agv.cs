using System.ComponentModel;
using System.Windows.Media;

namespace AGVSystem.Model.Ga_agvModels
{
    /// <summary>
    /// AGV信息实体类
    /// </summary>
    public class Ga_agv : INotifyPropertyChanged
    {
        /// <summary>
        /// AGV编号
        /// </summary>
        public int agvNumber { get; set; }

        /// <summary>
        /// 网络状态
        /// </summary>
        private string AgvStatic;

        public string agvStatic
        {
            get { return AgvStatic; }
            set
            {
                AgvStatic = value;
                GetChanged("agvStatic");
            }
        }

        /// <summary>
        /// 电压
        /// </summary>
        private string voltage;

        public string Voltage
        {
            get { return voltage; }
            set
            {
                voltage = value;
                GetChanged("Voltage");
            }
        }

        /// <summary>
        /// 速度
        /// </summary>
        private string speed;

        public string Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                GetChanged("Speed");
            }
        }

        /// <summary>
        /// 转弯
        /// </summary>
        private string Turn;

        public string turn
        {
            get { return Turn; }
            set
            {
                Turn = value;
                GetChanged("turn");
            }
        }

        private Brush turncolor;
        public Brush Turncolor
        {
            get { return turncolor; }
            set
            {
                turncolor = value;
                GetChanged("Turncolor");
            }
        }

        /// <summary>
        /// 运行状态 运行/停止
        /// </summary>
        private string operation;

        public string Operation
        {
            get { return operation; }
            set
            {
                operation = value;
                GetChanged("Operation");
            }
        }
        private Brush coloroperation;
        public Brush ColorOperation
        {
            get { return coloroperation; }
            set
            {
                coloroperation = value;
                GetChanged("ColorOperation");
            }
        }

        /// <summary>
        /// 方向
        /// </summary>
        private string dir;

        public string Dir
       {
            get { return dir; }
            set
            {
                dir = value;
                GetChanged("Dir");
            }
        }

        private Brush dircolor;
        public Brush Dircolor
        {
            get { return dircolor; }
            set
            {
                dircolor = value;
                GetChanged("Dircolor");
            }
        }

        /// <summary>
        /// 挂钩
        /// </summary>
        private string hook;

        public string Hook
        {
            get { return hook; }
            set
            {
                hook = value;
                GetChanged("Hook");
            }
        }

        private Brush hookcolor;
        public Brush Hookcolor
        {
            get { return hookcolor; }
            set
            {
                hookcolor = value;
                GetChanged("Hookcolor");
            }
        }

        /// <summary>
        /// RFID编号
        /// </summary>
        private string rFID;

        public string RFID
        {
            get { return rFID; }
            set
            {
                rFID = value;
                GetChanged("RFID");
            }
        }

        /// <summary>
        /// 线路号
        /// </summary>
        private string routeNo;

        public string RouteNo
        {
            get { return routeNo; }
            set
            {
                routeNo = value;
                GetChanged("RouteNo");
            }
        }

        /// <summary>
        /// PBS
        /// </summary>
        private string pBS;
        public string PBS
        {
            get { return pBS; }
            set
            {
                pBS = value;
                GetChanged("PBS");
            }
        }

        /// <summary>
        /// 步骤
        /// </summary>
        private string step;

        public string Step
        {
            get { return step; }
            set
            {
                step = value;
                GetChanged("Step");
            }
        }

        /// <summary>
        /// 程序号
        /// </summary>
        private string program;

        public string Program
        {
            get { return program; }
            set
            {
                program = value;
                GetChanged("Program");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// 属性更改通知客户端事件
        /// </summary>
        private void GetChanged(string Name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(Name));
            }
        }

        private Brush colorBg;
        public Brush ColorBg
        {
            get { return colorBg; }
            set
            {
                colorBg = value;
                GetChanged("ColorBg");
            }
        }
    }
}
