using AGVSystem.APP.agv_Map;
using AGVSystem.Model.Ga_agvModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AGVSystem.UI.APP_UI.Map
{
    /// <summary>
    /// MapEdit.xaml 的交互逻辑
    /// </summary>
    public partial class MapEdit : Window
    {
        Ga_Map GetMap = new Ga_Map();
        Painting GetPainting = new Painting();

        public MapEdit(Ga_Map ga_Map)
        {
            InitializeComponent();
            GetMap = ga_Map;
            LoadMap();
        }

        public void LoadMap()
        {
            this.Title = "地图编辑-" + GetMap.Name;
            TopX.Width = this.Width * 10;
            TopY.Height = this.Height * 10;
            mainPanel.Width = this.Width * 2;
            mainPanel.Height = this.Height * 2;
            GetPainting.CoordinateX(TopX, TopY, 10, 10, Brushes.Black, 1);

            GetPainting.Coordinate(mainPanel, 10, 10, new SolidColorBrush(Color.FromRgb(208, 208, 208)), 1);
        }
    }
}
