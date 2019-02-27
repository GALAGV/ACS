using System.ComponentModel;
using System.Windows.Media;

namespace AGVSystem.Model.Ga_agvModels
{
    /// <summary>
    /// 串口实体类
    /// </summary>
    public class Ga_PortInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// 串口编号
        /// </summary>
        public string ComNumber { get; set; }

        /// <summary>
        /// 串口类型
        /// </summary>
        public string ComPortType { get; set; }

        /// <summary>
        /// 串口状态
        /// </summary>
        public string ComStatic { get; set; }

        private Brush comStaticColor;
        public Brush ComStaticColor
        {
            get { return comStaticColor; }
            set
            {
                comStaticColor = value;
                GetChanged("ComStaticColor");
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
