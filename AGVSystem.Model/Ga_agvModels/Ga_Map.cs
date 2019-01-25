using System.ComponentModel;

namespace AGVSystem.Model.Ga_agvModels
{
    /// <summary>
    /// 地图实体类
    /// </summary>
    public class Ga_Map : INotifyPropertyChanged
    {

        public int ID { get; set; }

        private bool isSelected = false;

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; GetChanged("IsSelected"); }
        }

        /// <summary>
        /// 地图名称
        /// </summary>
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                GetChanged("Name");
            }
        }

        /// <summary>
        /// 宽度
        /// </summary>
        private double width;
        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                GetChanged("Width");
            }
        }

        /// <summary>
        /// 高度
        /// </summary>
        private double height;
        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                GetChanged("Height");
            }
        }

        /// <summary>
        /// 地图类型 0磁标，1RFID，2激光
        /// </summary>
        private int type;
        public int Type
        {
            get { return type; }
            set
            {
                type = value;
                GetChanged("Type");
            }
        }


        /// <summary>
        /// 创建时间
        /// </summary>
        private string createTime;
        public string CreateTime
        {
            get { return createTime; }
            set
            {
                createTime = value;
                GetChanged("CreateTime");
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
    }
}
