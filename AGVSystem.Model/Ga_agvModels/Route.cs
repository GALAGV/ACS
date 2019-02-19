using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AGVSystem.Model.Ga_agvModels
{
    public class Route :  INotifyPropertyChanged
    {
        public int ID { get; set; }

        public int Program { get; set; }

        public string Name { get; set; }

        public long CreateTime { get; set; }

        public string Tag { get; set; }

        private Brush tagColor;
        public Brush TagColor
        {
            get { return tagColor; }
            set
            {
                tagColor = value;
                GetChanged("TagColor");
            }
        }

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

        public string Stop { get; set; }


        public string Turn { get; set; }

        private Brush turnColor;
        public Brush TurnColor
        {
            get { return turnColor; }
            set
            {
                turnColor = value;
                GetChanged("TurnColor");
            }
        }

        public string Direction { get; set; }


        private Brush directionColor;
        public Brush DirectionColor
        {
            get { return directionColor; }
            set
            {
                directionColor = value;
                GetChanged("DirectionColor");
            }
        }

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

        public string revPbs { get; set; }

        public string Hook { get; set; }

        private Brush hookColor;
        public Brush HookColor
        {
            get { return hookColor; }
            set
            {
                hookColor = value;
                GetChanged("HookColor");
            }
        }

        public string ChangeProgram { get; set; }


        public List<Ga_agvStatus> agvSetSpeed { get; set; }
        public List<Ga_agvStatus> agvSetPBS { get; set; }

        public List<Ga_agvStatus> agvSetTurn { get; set; }

        public List<Ga_agvStatus> agvSetDirection { get; set; }


        public List<Ga_agvStatus> agvSetHook { get; set; }

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
