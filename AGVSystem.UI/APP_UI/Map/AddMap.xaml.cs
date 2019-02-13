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
    }
}
