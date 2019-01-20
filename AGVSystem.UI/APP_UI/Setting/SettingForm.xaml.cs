using AGVSystem.BLL;
using AGVSystem.IService.IO_BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Ports;
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

namespace AGVSystem.UI.APP_UI.Setting
{
    /// <summary>
    /// SettingForm.xaml 的交互逻辑
    /// </summary>
    public partial class SettingForm : Window
    {
        public SettingForm()
        {
            InitializeComponent();
        }

        private void Com_Selected(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(Port,3);
            Panel.SetZIndex(agv_system, 2);
            Panel.SetZIndex(agv_map, 1);
        }

        private void Map_Selected(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(Port, 1);
            Panel.SetZIndex(agv_system, 2);
            Panel.SetZIndex(agv_map, 3);
        }

        private void System_Selected(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(Port, 1);
            Panel.SetZIndex(agv_system, 3);
            Panel.SetZIndex(agv_map, 2);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        IO_MapBLL mapMessage = new Ga_mapBLL();
        Grid gridItem = new Grid();
        int Index = 0;
        long Time= 1535037182;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PortLoad();
        }
        public void PortLoad()
        {
            gridItem.VerticalAlignment = VerticalAlignment.Top;
            gridItem.HorizontalAlignment = HorizontalAlignment.Center;
            CountMap.Content = gridItem;
            DataTable PortData = mapMessage.ListDevice(Time);
            if (PortData.Rows.Count > 0)
            {
                for (int i = 0; i < PortData.Rows.Count; i++)
                {
                    AddRows(i, PortData.Rows[i]["Com"].ToString(), PortData.Rows[i]["Baud"].ToString(), PortData.Rows[i]["Agv"].ToString());
                    Index = i;
                }
            }
            if (gridItem.RowDefinitions.Count.Equals(1) || gridItem.RowDefinitions.Count.Equals(0))
            {
                //DeleteRows.IsEnabled = false;
            }
        }

        List<Label> GetTs = new List<Label>();
        List<ComboBox> combos = new List<ComboBox>();
        List<Label> Labels = new List<Label>();
        List<TextBox> TextBoxes = new List<TextBox>();
        List<Label> labels = new List<Label>();
        List<ComboBox> Combos = new List<ComboBox>();
        List<RowDefinition> definitions = new List<RowDefinition>();
        public void AddRows(int i, string port, string Baud, string Agv)
        {
            //创建行
            RowDefinition row = new RowDefinition();
            gridItem.RowDefinitions.Add(row);
            for (int s = 0; s < 6; s++)
            {
                //创建列
                ColumnDefinition column = new ColumnDefinition();
                gridItem.ColumnDefinitions.Add(column);
            }
            definitions.Add(row);
            Label label = new Label();
            label.Content = "串口号：";
            label.Foreground = new SolidColorBrush(Colors.Black);
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            label.Margin = new Thickness(10, 2, 10, 2);
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, i);
            gridItem.Children.Add(label);
            GetTs.Add(label);


            ComboBox combo = new ComboBox();
            combo.Text = port;
            combo.DropDownOpened += Combo_DropDownOpened;
            combo.Width = 75;
            combo.Margin = new Thickness(0, 2, 10, 2);
            ComboBoxItem ite = new ComboBoxItem();
            ite.Content = port.Equals("") ? "" : "COM" + port;
            combo.Items.Add(ite);
            combo.SelectedIndex = 0;
            Grid.SetColumn(combo, 1);
            Grid.SetRow(combo, i);
            gridItem.Children.Add(combo);
            combos.Add(combo);

            Label labe2 = new Label();
            labe2.Content = "波特率：";
            labe2.HorizontalAlignment = HorizontalAlignment.Center;
            labe2.VerticalAlignment = VerticalAlignment.Center;
            labe2.Margin = new Thickness(0, 2, 10, 2);

            Grid.SetColumn(labe2, 2);
            Grid.SetRow(labe2, i);
            gridItem.Children.Add(labe2);
            Labels.Add(labe2);

            TextBox combo2 = new TextBox();
            combo2.Text = Baud;
            combo2.Margin = new Thickness(0, 2, 10, 2);
            combo2.Width = 60;
            Grid.SetColumn(combo2, 3);
            Grid.SetRow(combo2, i);
            gridItem.Children.Add(combo2);
            TextBoxes.Add(combo2);

            Label labe3 = new Label();
            labe3.Content = "AGV/其他：";
            labe3.Margin = new Thickness(0,2, 10, 2);
            labe3.HorizontalAlignment = HorizontalAlignment.Center;
            labe3.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(labe3, 4);
            Grid.SetRow(labe3, i);
            gridItem.Children.Add(labe3);
            labels.Add(labe3);


            ComboBox combo3 = new ComboBox();
            if (Agv.Equals("Button"))
            {
                combo3.Text = "按钮";
            }
            else if (Agv.Equals("Charge"))
            {
                combo3.Text = "充电机";
            }
            else
            {
                combo3.Text = Agv;
            }
            combo3.Margin = new Thickness(0, 2, 10, 2);
            ComboBoxItem iter = new ComboBoxItem();
            iter.Content = "按钮";
            combo3.Items.Add(iter);
            ComboBoxItem ites = new ComboBoxItem();
            ites.Content = "充电机";
            combo3.Items.Add(ites);
            combo3.IsEditable = true;
            combo3.Width = 150;
            Grid.SetColumn(combo3, 5);
            Grid.SetRow(combo3, i);
            gridItem.Children.Add(combo3);
            combo3.KeyDown += Combo3_KeyDown;
            Combos.Add(combo3);
        }

        private void Combo_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            box.Items.Clear();
            string[] polist = SerialPort.GetPortNames();
            for (int i = 0; i < polist.Length; i++)
            {
                ComboBoxItem ite = new ComboBoxItem();
                ite.Content = polist[i];
                box.Items.Add(ite);
            }
        }

        private void Combo3_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            bool shiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;//判断shifu键是否按下
            if (shiftKey == true)                  //当按下shift
            {
                e.Handled = true;//不可输入
            }
            else  //未按shift
            {
                if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Enter))
                {
                    if (e.Key != Key.OemComma)
                    {
                        e.Handled = true;//不可输入
                    }

                }
            }
        }
    }
}
