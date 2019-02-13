using System.ComponentModel;
using System.Windows.Media;

namespace AGVSystem.Model.Ga_agvModels
{
    /// <summary>
    /// 状态信息实体类
    /// </summary>
    public class Ga_agvStatus : INotifyPropertyChanged
    {

        /// <summary>
        /// 状态名称
        /// </summary>
        private string statusName;
        public string StatusName
        {
            get { return statusName; }
            set
            {
                statusName = value;
                GetChanged("StatusName");
            }
        }


        /// <summary>
        /// 状态值
        /// </summary>
        public string statusValue;
        public string StatusValue
        {
            get { return statusValue; }
            set
            {
                statusValue = value;
                GetChanged("StatusValue");
            }
        }


        private Brush colorBg;
        public Brush ColorBg
        {
            get { return colorBg; }
            set
            {
                colorBg = value;
                GetChanged("Brush");
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
