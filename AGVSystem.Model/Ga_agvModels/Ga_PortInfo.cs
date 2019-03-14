using AGVSystem.Model.LogicData;
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
        public PortType ComPortType { get; set; }

        /// <summary>
        /// 串口状态
        /// </summary>
        private string comStatic { get; set; }
        public string ComStatic
        {
            get { return comStatic; }
            set
            {
                comStatic = value;
                GetChanged("ComStatic");
            }
        }



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
