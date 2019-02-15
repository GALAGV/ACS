using AGVSystem.Infrastructure.agvCommon;
using AGVSystem.Model.Ga_agvModels;
using System;
using System.Windows;

namespace AGVSystem.UI.APP_UI.Map
{
    /// <summary>
    /// AddMap.xaml 的交互逻辑
    /// </summary>
    public partial class AddMap : Window
    {
        public AddMap()
        {
            InitializeComponent();
        }

        private void FormClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MapAdd_Click(object sender, RoutedEventArgs e)
        {
            if (FormatVerification.IsFloat(mapHeight.Text.Trim()) && FormatVerification.IsFloat(mapWidth.Text.Trim()) && !string.IsNullOrEmpty(mapHeight.Text.Trim()) && !string.IsNullOrEmpty(mapWidth.Text.Trim()) && !string.IsNullOrEmpty(mapName.Text.Trim()))
            {
                MapEdit map = new MapEdit(
                        new Ga_Map()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Height = Convert.ToDouble(mapHeight.Text),
                            Width = Convert.ToDouble(mapWidth.Text),
                            Name = mapName.Text,
                        }, false);
                map.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("格式输入错误！！！", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
