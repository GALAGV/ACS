using AGVSystem.Model.MapData;
using OperateIni;
using System;
using System.Windows;

namespace AGVSystem.UI.APP_UI.Setting
{
    /// <summary>
    /// LineConfig.xaml 的交互逻辑
    /// </summary>
    public partial class LineConfig : Window
    {
        public LineConfig()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveLineConfig();
            if (MapRegulate.DirectionLine.Equals(1))
                Direction.IsChecked = true;
            else
                Direction.IsChecked = false;
        }

        private void SaveLineConfig()
        {
            string template = Template.Text.Trim();
            if (template.Contains("[StartTag]") && template.Contains("[StopTag]"))
            {
                MapRegulate.TemplateName = Template.Text.Trim();
                IniFile.WriteIniData("AGV", "TemplateName", MapRegulate.TemplateName, AppDomain.CurrentDomain.BaseDirectory + "\\setting.ini");
                if (Direction.IsChecked == true)
                {
                    MapRegulate.DirectionLine = 1;
                }
                else
                {
                    MapRegulate.DirectionLine = 0;
                }
                IniFile.WriteIniData("AGV", "DirectionLine", MapRegulate.DirectionLine.ToString(), AppDomain.CurrentDomain.BaseDirectory + "\\setting.ini");
                MessageBox.Show("保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("格式输入错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Template.Text = MapRegulate.TemplateName;
            if (MapRegulate.DirectionLine.Equals(1))
                Direction.IsChecked = true;
            else
                Direction.IsChecked = false;
        }
    }
}
