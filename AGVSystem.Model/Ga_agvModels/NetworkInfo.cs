using System.ComponentModel;
using System.Windows.Media;

namespace AGVSystem.Model.Ga_agvModels
{
    public class NetworkInfo : INotifyPropertyChanged
    {
        public string IP_Address { get; set; }

        public int IP_Port { get; set; }

        public string IP_Static { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private Brush iP_StaticColor;
        public Brush IP_StaticColor
        {
            get { return iP_StaticColor; }
            set
            {
                iP_StaticColor = value;
                GetChanged("IP_StaticColor");
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
    }
}
