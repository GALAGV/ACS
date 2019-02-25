using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace AGVSystem.Model.Ga_agvModels
{
    /// <summary>
    /// Tag实体类
    /// </summary>
    public class MapTag : INotifyPropertyChanged
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tag号
        /// </summary>
        public string TagNo { get; set; }
        
        /// <summary>
        /// Tag名称
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// X（米）
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y（米）
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 后置tag
        /// </summary>
        public string NextTag { get; set; }

        private Brush nextTagColor;
        public Brush NextTagColor
        {
            get { return nextTagColor; }
            set
            {
                nextTagColor = value;
                GetChanged("NextTagColor");
            }
        }


        public List<Ga_agvStatus> NextTagList { get; set; }

        /// <summary>
        /// 左后置tag
        /// </summary>
        public string NextLeftTag { get; set; }

        private Brush nextLeftTagColor;
        public Brush NextLeftTagColor
        {
            get { return nextLeftTagColor; }
            set
            {
                nextLeftTagColor = value;
                GetChanged("NextLeftTagColor");
            }
        }

        public List<Ga_agvStatus> NextLeftTagList { get; set; }

        /// <summary>
        /// 右后置tag
        /// </summary>
        public string NextRightTag { get; set; }

        private Brush nextRightTagColor;
        public Brush NextRightTagColor
        {
            get { return nextRightTagColor; }
            set
            {
                nextRightTagColor = value;
                GetChanged("NextRightTagColor");
            }
        }

        public List<Ga_agvStatus> NextRightTagList { get; set; }

        /// <summary>
        /// 前置tag
        /// </summary>
        public string PreTag { get; set; }

        private Brush preTagColor;
        public Brush PreTagColor
        {
            get { return preTagColor; }
            set
            {
                preTagColor = value;
                GetChanged("PreTagColor");
            }
        }

        public List<Ga_agvStatus> PreTagList { get; set; }

        /// <summary>
        /// 左前置tag
        /// </summary>
        public string PreLeftTag { get; set; }

        private Brush preLeftTagColor;
        public Brush PreLeftTagColor
        {
            get { return preLeftTagColor; }
            set
            {
                preLeftTagColor = value;
                GetChanged("PreLeftTagColor");
            }
        }

        public List<Ga_agvStatus> PreLeftTagList { get; set; }

        /// <summary>
        /// 右前置tag
        /// </summary>
        public string PreRightTag { get; set; }

        private Brush preRightTagColor;
        public Brush PreRightTagColor
        {
            get { return preRightTagColor; }
            set
            {
                preRightTagColor = value;
                GetChanged("PreRightTagColor");
            }
        }

        public List<Ga_agvStatus> PreRightTagList { get; set; }

        /// <summary>
        /// 正向速度
        /// </summary>
        public string Speed { get; set; }

        private Brush speedColor;
        public Brush SpeedColor
        {
            get { return speedColor; }
            set
            {
                speedColor = value;
                GetChanged("SpeedColor");
            }
        }

        public List<Ga_agvStatus> SpeedList { get; set; }

        /// <summary>
        /// 反向速度
        /// </summary>
        public string SpeedRev { get; set; }

        private Brush speedRevColor;
        public Brush SpeedRevColor
        {
            get { return speedRevColor; }
            set
            {
                speedRevColor = value;
                GetChanged("SpeedRevColor");
            }
        }

        public List<Ga_agvStatus> SpeedRevList { get; set; }

        /// <summary>
        /// 单位为s
        /// </summary>
        public string StopTime { get; set; }

        /// <summary>
        /// 障碍物扫描（正向）
        /// </summary>
        public string Pbs { get; set; }

        private Brush pbsColor;
        public Brush PbsColor
        {
            get { return pbsColor; }
            set
            {
                pbsColor = value;
                GetChanged("PbsColor");
            }
        }

        public List<Ga_agvStatus> PbsList { get; set; }

        /// <summary>
        /// 障碍物扫描（反向）
        /// </summary>
        public string PbsRev { get; set; }

        private Brush pbsRevColor;
        public Brush PbsRevColor
        {
            get { return pbsRevColor; }
            set
            {
                pbsRevColor = value;
                GetChanged("PbsRevColor");
            }
        }

        public List<Ga_agvStatus> PbsRevList { get; set; }


        /// <summary>
        /// (正向)转弯速度
        /// </summary>
        public string PreTurnSpeed { get; set; }

        private Brush preTurnSpeedColor;
        public Brush PreTurnSpeedColor
        {
            get { return preTurnSpeedColor; }
            set
            {
                preTurnSpeedColor = value;
                GetChanged("PreTurnSpeedColor");
            }
        }


        public List<Ga_agvStatus> PreTurnSpeedList { get; set; }

        /// <summary>
        /// （反向）转弯速度
        /// </summary>
        public string RevTurnSpeed { get; set; }

        private Brush revTurnSpeedColor;
        public Brush RevTurnSpeedColor
        {
            get { return revTurnSpeedColor; }
            set
            {
                revTurnSpeedColor = value;
                GetChanged("RevTurnSpeedColor");
            }
        }


        public List<Ga_agvStatus> RevTurnSpeedList { get; set; }

        /// <summary>
        /// （正向）转弯PBS
        /// </summary>
        public string PreTurnPBS { get; set; }

        private Brush preTurnPBSColor;
        public Brush PreTurnPBSColor
        {
            get { return preTurnPBSColor; }
            set
            {
                preTurnPBSColor = value;
                GetChanged("PreTurnPBSColor");
            }
        }

        public List<Ga_agvStatus> PreTurnPBSList { get; set; }

        /// <summary>
        /// (反向)转弯PBS
        /// </summary>
        public string RevTurnPBS { get; set; }

        private Brush revTurnPBSColor;
        public Brush RevTurnPBSColor
        {
            get { return revTurnPBSColor; }
            set
            {
                revTurnPBSColor = value;
                GetChanged("RevTurnPBSColor");
            }
        }

        public List<Ga_agvStatus> RevTurnPBSList { get; set; }

        /// <summary>
        /// 0为非终结点，1为终结点，2为辅助点
        /// </summary>
        public string TagTerminal { get; set; }

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
    }
}
