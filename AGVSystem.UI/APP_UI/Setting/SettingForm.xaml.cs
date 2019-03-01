using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MySql.Data.MySqlClient;
using AGVSystem.BLL.ServiceLogicBLL;
using AGVSystem.Model.MapData;
using System.ComponentModel;
using System.Text.RegularExpressions;
using AGVSystem.Model.LogicData;
using System.Data;
using AGVSystem.APP.agv_Map;
using OperateIni;
using System.IO;
using AGVSystem.Infrastructure.agvCommon;
using System.Linq;

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
        private Menutype GetMenutype = Menutype.MapSetting;
        private agvMapRegulate mapService = new agvMapRegulate();
        private Ga_mapBLL mapMessage = new Ga_mapBLL();
        private Grid gridItem = new Grid();  //端口绘制容器
        private Grid gridNetwork = new Grid(); //网络绘制容器
        private int Index = 0;
        private int NetworkIndex = 0;
        public delegate void SettingMap();
        public SettingMap GetSettingMap;
        private string ServePath { get; set; }
        private string ServeName { get; set; }
        private int isRun { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (OperateIniTool.Exist)
            {
                Map_Size.Text = OperateIniTool.OperateIniRead("AGV", "MapSise");
                interval.Text = OperateIniTool.OperateIniRead("Table", "interval");
                ServePath = $"{AppDomain.CurrentDomain.BaseDirectory}{OperateIniTool.OperateIniRead("Table", "ServeNameEXE")}";
                ServeName = OperateIniTool.OperateIniRead("Table", "ServeName");
                isRun = OperateIniTool.OperateIniRead("AGV", "IsStart").TransformInt();
            }
            ConfigDataBase();
            PortLoad();
            MapList.ItemsSource = mapService.GetMapRegulate();
            DataTable MapData = mapService.defaultMap(MapRegulate.DefaultMap);
            if (MapData.Rows.Count > 0)
            {
                MapList.Text = MapData.Rows[0]["Name"].ToString();
            }
            else
            {
                MapList.SelectedIndex = 0;
            }
            RunStart();
        }

        #region 开机启动

        private void RunStart()
        {
            if (isRun.Equals(0))
            {
                Run.IsChecked = false;
            }
            else
            {
                Run.IsChecked = true;
            }
        }

        private void SaveRun()
        {
            if (Run.IsChecked == true)
            {
                if (!AppSelfStarting.IsExistKey("AGVSystem"))
                {
                    AppSelfStarting.SelfRunning(true, "AGVSystem", $"{AppDomain.CurrentDomain.BaseDirectory}AGVSystem.exe");
                }
                IniFile.WriteIniData("AGV", "IsStart", "1", AppDomain.CurrentDomain.BaseDirectory + "\\setting.ini");
            }
            else
            {
                if (AppSelfStarting.IsExistKey("AGVSystem"))
                {
                    AppSelfStarting.SelfRunning(false, "AGVSystem", $"{AppDomain.CurrentDomain.BaseDirectory}AGVSystem.exe");
                }
                IniFile.WriteIniData("AGV", "IsStart", "0", AppDomain.CurrentDomain.BaseDirectory + "\\setting.ini");
            }
        }
        #endregion

        #region 菜单

        private void Com_Selected(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(Port, 3);
            Panel.SetZIndex(agv_system, 2);
            Panel.SetZIndex(agv_map, 1);
            Panel.SetZIndex(network, 0);
            GetMenutype = Menutype.ProntSetting;
        }
        private void Map_Selected(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(Port, 1);
            Panel.SetZIndex(agv_system, 2);
            Panel.SetZIndex(agv_map, 3);
            Panel.SetZIndex(network, 0);
            GetMenutype = Menutype.MapSetting;
        }

        private void System_Selected(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(Port, 1);
            Panel.SetZIndex(agv_system, 3);
            Panel.SetZIndex(agv_map, 2);
            Panel.SetZIndex(network, 0);
            GetMenutype = Menutype.SystemSetting;
        }

        private void IP_Selected(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(Port, 1);
            Panel.SetZIndex(agv_system, 2);
            Panel.SetZIndex(agv_map, 3);
            Panel.SetZIndex(network, 4);
            GetMenutype = Menutype.Network;
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 获取数据库参数

        string[] DataBaseStr;
        private void ConfigDataBase()
        {
            string connectionString = IniFile.ReadIniData("DB", "MySql", "", AppDomain.CurrentDomain.BaseDirectory + "\\setting.ini");
            DataBaseStr = connectionString.Split(';');
            if (DataBaseStr.Length == 3)
            {
                string BaseSite = DataBaseStr[0].Split('=')[1];
                string BaseName = DataBaseStr[1].Split('=')[1];
                string BasePassword = DataBaseStr[2].Split('=')[1];
                Site.Text = BaseSite;
                Number.Text = BaseName;
                Password.Text = BasePassword;
            }
        }
        private string DataBaseConfig()
        {
            string DataBaseConfig = $"{DataBaseStr[0].Split('=')[0]}={Site.Text.Trim()}";
            string DataUserConfig = $"{DataBaseStr[1].Split('=')[0]}={Number.Text.Trim()}";
            string DataPassword = $"{DataBaseStr[2].Split('=')[0]}={Password.Text.Trim()}";
            return string.Join(";", new string[] { DataBaseConfig, DataUserConfig, DataPassword });
        }
        #endregion

        #region 加载串口/网络


        public void PortLoad()
        {
            GetMenutype = Menutype.ProntSetting;
            gridItem.VerticalAlignment = VerticalAlignment.Top;
            gridItem.HorizontalAlignment = HorizontalAlignment.Center;
            CountMap.Content = gridItem;
            gridNetwork.VerticalAlignment = VerticalAlignment.Top;
            gridNetwork.HorizontalAlignment = HorizontalAlignment.Left;
            networkCount.Content = gridNetwork;
            MySqlDataReader PortData = mapMessage.ListDevice(MapRegulate.UTCTime);
            int i = 0;
            while (PortData.Read())
            {
                AddRows(i, PortData["Com"].ToString(), PortData["Baud"].ToString(), PortData["Agv"].ToString());
                Index = i;
                i++;
            }
            PortData.Close();
            if (gridItem.RowDefinitions.Count.Equals(1) || gridItem.RowDefinitions.Count.Equals(0))
            {
                DeletePort.IsEnabled = false;
            }
            MySqlDataReader reader = mapMessage.SelectNetworkBLL(MapRegulate.UTCTime);
            if (reader != null)
            {
                int id = 0;
                while (reader.Read())
                {
                    AddNetwork(id, reader["IP_Address"].ToString(), reader["IP_Port"].ToString());
                    NetworkIndex = id;
                    id++;
                }
            }
            if (gridNetwork.RowDefinitions.Count.Equals(1) || gridNetwork.RowDefinitions.Count.Equals(0))
            {
                Network_Delete.IsEnabled = false;
            }
        }
        #endregion

        #region 添加串口

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
            combo.VerticalContentAlignment = VerticalAlignment.Center;
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
            combo2.VerticalContentAlignment = VerticalAlignment.Center;
            Grid.SetColumn(combo2, 3);
            Grid.SetRow(combo2, i);
            gridItem.Children.Add(combo2);
            TextBoxes.Add(combo2);

            Label labe3 = new Label();
            labe3.Content = "AGV/其他：";
            labe3.Margin = new Thickness(0, 2, 10, 2);
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
            else if (Agv.Equals("Plc"))
            {
                combo3.Text = "PLC";
            }
            else
            {
                combo3.Text = Agv;
            }
            combo3.VerticalContentAlignment = VerticalAlignment.Center;
            combo3.Margin = new Thickness(0, 2, 10, 2);
            ComboBoxItem itel = new ComboBoxItem();
            itel.Content = "PLC";
            combo3.Items.Add(itel);
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

        #endregion

        #region 添加网络


        List<Label> IpName = new List<Label>();
        List<TextBox> Ipsite = new List<TextBox>();
        List<Label> PortName = new List<Label>();
        List<TextBox> PortData = new List<TextBox>();
        List<RowDefinition> NetworkRows = new List<RowDefinition>();
        private void AddNetwork(int Index, string IP, string Port)
        {
            //创建行
            RowDefinition row = new RowDefinition();
            gridNetwork.RowDefinitions.Add(row);
            for (int s = 0; s < 4; s++)
            {
                //创建列
                ColumnDefinition column = new ColumnDefinition();
                gridNetwork.ColumnDefinitions.Add(column);
            }
            NetworkRows.Add(row);
            Label label = new Label();
            label.Content = "IP地址：";
            label.Foreground = new SolidColorBrush(Colors.Black);
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            label.Margin = new Thickness(10, 2, 10, 2);
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, Index);
            gridNetwork.Children.Add(label);
            IpName.Add(label);


            TextBox combo = new TextBox();
            combo.Text = IP;
            combo.Width = 170;
            combo.VerticalContentAlignment = VerticalAlignment.Center;
            combo.Margin = new Thickness(0, 2, 10, 2);
            Grid.SetColumn(combo, 1);
            Grid.SetRow(combo, Index);
            gridNetwork.Children.Add(combo);
            Ipsite.Add(combo);

            Label labe2 = new Label();
            labe2.Content = "端口号：";
            labe2.HorizontalAlignment = HorizontalAlignment.Center;
            labe2.VerticalAlignment = VerticalAlignment.Center;
            labe2.Margin = new Thickness(20, 2, 10, 2);

            Grid.SetColumn(labe2, 2);
            Grid.SetRow(labe2, Index);
            gridNetwork.Children.Add(labe2);
            PortName.Add(labe2);

            TextBox combo2 = new TextBox();
            combo2.Text = Port;
            combo2.Margin = new Thickness(0, 2, 10, 2);
            combo2.Width = 170;
            combo2.VerticalContentAlignment = VerticalAlignment.Center;
            Grid.SetColumn(combo2, 3);
            Grid.SetRow(combo2, Index);
            gridNetwork.Children.Add(combo2);
            PortData.Add(combo2);
        }

        #endregion

        #region 删除网络

        private void Network_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (gridNetwork.RowDefinitions.Count.Equals(1) || gridNetwork.RowDefinitions.Count.Equals(0))
            {
                return;
            }
            gridNetwork.Children.Remove(IpName[NetworkIndex]);
            gridNetwork.Children.Remove(Ipsite[NetworkIndex]);
            gridNetwork.Children.Remove(PortName[NetworkIndex]);
            gridNetwork.Children.Remove(PortData[NetworkIndex]);

            IpName.Remove(IpName[NetworkIndex]);
            Ipsite.Remove(Ipsite[NetworkIndex]);
            PortName.Remove(PortName[NetworkIndex]);
            PortData.Remove(PortData[NetworkIndex]);

            gridNetwork.RowDefinitions.Remove(NetworkRows[NetworkIndex]);
            NetworkRows.Remove(NetworkRows[NetworkIndex]);
            for (int i = 0; i < IpName.Count; i++)
            {
                Grid.SetRow(IpName[i], i);
                Grid.SetRow(Ipsite[i], i);
                Grid.SetRow(PortName[i], i);
                Grid.SetRow(PortData[i], i);
            }
            NetworkIndex--;
            if (gridNetwork.RowDefinitions.Count.Equals(1))
            {
                Network_Delete.IsEnabled = false;
            }
        }
        #endregion

        #region 串口选项更改


        private void Combo_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            string str = box.Text;
            box.Items.Clear();
            box.Text = str;
            string[] polist = SerialPort.GetPortNames();
            for (int i = 0; i < polist.Length; i++)
            {
                ComboBoxItem ite = new ComboBoxItem();
                ite.Content = polist[i];
                box.Items.Add(ite);
            }
        }
        #endregion

        #region 内容过滤


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
        #endregion

        #region 添加串口

        private void AddPort_Click(object sender, RoutedEventArgs e)
        {
            if (gridItem.RowDefinitions.Count.Equals(0))
            { Index = 0; }
            else
            { Index++; DeletePort.IsEnabled = true; }
            AddRows(Index, "", "", "");
        }
        #endregion

        #region 删除串口


        private void DeletePort_Click(object sender, RoutedEventArgs e)
        {
            if (gridItem.RowDefinitions.Count.Equals(1) || gridItem.RowDefinitions.Count.Equals(0))
            {
                return;
            }
            gridItem.Children.Remove(GetTs[Index]);
            gridItem.Children.Remove(combos[Index]);
            gridItem.Children.Remove(Labels[Index]);
            gridItem.Children.Remove(TextBoxes[Index]);
            gridItem.Children.Remove(labels[Index]);
            gridItem.Children.Remove(Combos[Index]);

            GetTs.Remove(GetTs[Index]);
            combos.Remove(combos[Index]);
            Labels.Remove(Labels[Index]);
            TextBoxes.Remove(TextBoxes[Index]);
            labels.Remove(labels[Index]);
            Combos.Remove(Combos[Index]);

            gridItem.RowDefinitions.Remove(definitions[Index]);
            definitions.Remove(definitions[Index]);
            for (int i = 0; i < GetTs.Count; i++)
            {
                Grid.SetRow(GetTs[i], i);
                Grid.SetRow(combos[i], i);
                Grid.SetRow(Labels[i], i);
                Grid.SetRow(TextBoxes[i], i);
                Grid.SetRow(labels[i], i);
                Grid.SetRow(Combos[i], i);
            }
            Index--;
            if (gridItem.RowDefinitions.Count.Equals(1))
            {
                DeletePort.IsEnabled = false;
            }
        }
        #endregion

        #region 设置保存

        private void SaveSetting_Click(object sender, RoutedEventArgs e)
        {
            switch (GetMenutype)
            {
                case Menutype.ProntSetting:
                    SavePront();
                    GetSettingMap();
                    break;
                case Menutype.MapSetting:
                    MapSave();
                    GetSettingMap();
                    break;
                case Menutype.SystemSetting:
                    SystemSave();
                    break;
                case Menutype.Network:
                    NetworkSave();
                    GetSettingMap();
                    break;
                    //default:
                    //    break;
            }

        }
        #endregion

        #region 网络保存

        private void NetworkSave()
        {
            bool verify = true;
            for (int i = 0; i < Ipsite.Count; i++)
            {
                if (!FormatVerification.HasIP(Ipsite[i].Text.Trim()) || !FormatVerification.IsFloat(PortData[i].Text.Trim()))
                {
                    verify = false;
                }
            }
            if (verify)
            {
                PortInfo.IP.Clear();
                PortInfo.Port.Clear();
                if (mapMessage.SavenetworkBLL(MapRegulate.UTCTime, Ipsite.Select(p => p.Text.Trim()).ToList(), PortData.Select(p => p.Text.Trim()).ToList()))
                {
                    MessageBox.Show("保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("保存失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("格式输入错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        #endregion

        #region 串口保存

        /// <summary>
        /// 串口保存
        /// </summary>
        private void SavePront()
        {
            bool Textnull = true;
            for (int i = 0; i < combos.Count; i++)
            {
                if (combos[i].Text.Equals(""))
                {
                    Textnull = false;
                }
                if (TextBoxes[i].Text.Equals(""))
                {
                    Textnull = false;
                }
                if (Combos[i].Text.Equals(""))
                {
                    Textnull = false;
                }
                else
                {
                    string[] arr = Combos[i].Text.Replace('，', ',').Split(',');
                    if (arr.Length > 0)
                    {
                        if (IsRepeat2(arr))
                        {
                            MessageBox.Show("AGV编号重复！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }
                string[] ar = Combos[i].Text.Replace('，', ',').Split(',');
                for (int s = 0; s < ar.Length; s++)
                {
                    if (ar[s].Equals(""))
                    {
                        Textnull = false;
                    }
                    if (ar[s].ToString().Trim().Equals("按钮") || ar[s].ToString().Trim().Equals("充电机"))
                    {
                        continue;
                    }
                    string pattern = "[\u4e00-\u9fbb]";
                    if (Regex.IsMatch(ar[s], pattern))
                    {
                        MessageBox.Show("AGV编号不能为中文！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        Textnull = false;
                        return;
                    }
                }

            }
            if (Textnull)
            {
                //PortInfo.AGVCom.Clear();
                //PortInfo.Baud.Clear();
                //PortInfo.agv.Clear();


                //PortInfo.buttonPort.Clear();
                //PortInfo.buttonCom.Clear();
                //PortInfo.buttonBaud.Clear();
                //PortInfo.buttonStr.Clear();

                //PortInfo.plcPort.Clear();
                //PortInfo.plcCom.Clear();
                //PortInfo.plcBaud.Clear();
                //PortInfo.plcStr.Clear();

                //PortInfo.chargePort.Clear();
                //PortInfo.chargeCom.Clear();
                //PortInfo.chargeBaud.Clear();
                //PortInfo.chargeStr.Clear();
                DataTable dr = new DataTable();
                for (int i = 0; i < 3; i++)
                {
                    DataColumn dc = new DataColumn();
                    dr.Columns.Add(dc);
                }
                for (int i = 0; i < combos.Count; i++)
                {
                    DataRow dt = dr.NewRow();
                    string type = Combos[i].Text.ToString().Trim();
                    if (type.Equals("按钮"))
                    {
                        dt[0] = combos[i].Text.ToString().Trim().Substring(3);
                        dt[1] = TextBoxes[i].Text.ToString().Trim();
                        dt[2] = "Button";
                        //PortInfo.buttonCom.Add(Convert.ToInt32(combos[i].Text.ToString().Trim().Substring(3)));
                        //PortInfo.buttonBaud.Add(Convert.ToInt32(TextBoxes[i].Text.ToString().Trim()));
                        //PortInfo.buttonStr.Add("Button");
                    }
                    else if (type.Equals("充电机"))
                    {
                        dt[0] = combos[i].Text.ToString().Trim().Substring(3);
                        dt[1] = TextBoxes[i].Text.ToString().Trim();
                        dt[2] = "Charge";
                        //PortInfo.buttonCom.Add(Convert.ToInt32(combos[i].Text.ToString().Trim().Substring(3)));
                        //PortInfo.buttonBaud.Add(Convert.ToInt32(TextBoxes[i].Text.ToString().Trim()));
                        //PortInfo.chargeStr.Add("Charge");
                    }
                    else if (type.Equals("PLC"))
                    {
                        dt[0] = combos[i].Text.ToString().Trim().Substring(3);
                        dt[1] = TextBoxes[i].Text.ToString().Trim();
                        dt[2] = "Plc";
                        //PortInfo.plcCom.Add(Convert.ToInt32(combos[i].Text.ToString().Trim().Substring(3)));
                        //PortInfo.plcBaud.Add(Convert.ToInt32(combos[i].Text.ToString().Trim().Substring(3)));
                        //PortInfo.plcStr.Add("Plc");
                    }
                    else
                    {
                        dt[0] = combos[i].Text.ToString().Trim().Substring(3);
                        dt[1] = TextBoxes[i].Text.ToString().Trim();
                        dt[2] = Combos[i].Text.Replace('，', ',').ToString().Trim();
                        //PortInfo.buttonCom.Add(Convert.ToInt32(combos[i].Text.ToString().Trim().Substring(3)));
                        //PortInfo.buttonBaud.Add(Convert.ToInt32(TextBoxes[i].Text.ToString().Trim()));
                        //PortInfo.agv.Add(Combos[i].Text.Replace('，', ',').ToString().Trim());
                    }
                    dr.Rows.Add(dt);
                }
                if (mapMessage.InsertDeviceBLL(MapRegulate.UTCTime, dr))
                {
                    MessageBox.Show("设置保存成功!", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    //GetMap.Maplist_SelectionChanged(null, null);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("保存失败!", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                MessageBox.Show("串口号，波特率，AGV/其他不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region 地图保存

        /// <summary>
        /// 地图保存
        /// </summary>
        private void MapSave()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\setting.ini"))
            {
                IniFile.WriteIniData("AGV", "MapSise", Map_Size.Text, AppDomain.CurrentDomain.BaseDirectory + "\\setting.ini");
            }
            if (mapMessage.UpdateSettingMap(UTC.ConvertDateTimeLong(Convert.ToDateTime(MapList.SelectedValue.ToString())), 1))
            {
                MessageBox.Show("设置保存成功!", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        #endregion

        #region 系统设置保存

        /// <summary>
        /// 系统设置保存
        /// </summary>
        private void SystemSave()
        {
            if (FormatVerification.IsFloat(interval.Text.Trim()))
            {
                if (OperateIniTool.Exist)
                {
                    DataBaseConfig().OperateIniWrite("DB", "MySql");
                    interval.Text.Trim().OperateIniWrite("Table", "interval");
                    ConnectionStringsConfig.UpdateConnectionStringsConfig("DataBaseText", DataBaseConfig());
                    SaveRun();
                    this.Close();
                }
                else
                {
                    MessageBoxTool.Error("配置文件丢失！");
                }
            }
            else
            {
                MessageBoxTool.Error("格式错误！");
            }
        }
        #endregion

        #region 重复判断

        /// <summary>
        /// 重复
        /// </summary>
        /// <param name="yourValue"></param>
        /// <returns></returns>
        public bool IsRepeat2(string[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    if (j <= i)
                    {
                        continue;
                    }
                    if (array[i] == array[j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region 测试连接

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            string Config = DataBaseConfig();

            using (MySqlConnection connection = new MySqlConnection(Config))
            {
                using (MySqlCommand cmd = new MySqlCommand("", connection))
                {
                    try
                    {
                        connection.Open();
                        MessageBox.Show("连接成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        MessageBox.Show($"连接失败，错误信息：\r\n{ex.Message}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        connection.Close();
                        this.Cursor = Cursors.Arrow;
                    }
                }
            }

        }
        #endregion

        #region 添加网络设置

        private void Network_Add_Click(object sender, RoutedEventArgs e)
        {
            if (gridNetwork.RowDefinitions.Count.Equals(0))
            { NetworkIndex = 0; }
            else
            { NetworkIndex++; Network_Delete.IsEnabled = true; }
            AddNetwork(NetworkIndex, "", "");
        }
        #endregion

        #region 服务安装卸载

        private void Start_Service_Click(object sender, RoutedEventArgs e)
        {
            if (!ServiceFactory.IsServiceExisted(ServeName))
            {
                this.Cursor = Cursors.Wait;
                ServiceFactory.InstallService(ServePath);
                ServiceFactory.ServiceStart(ServeName);
                this.Cursor = Cursors.Arrow;
                MessageBoxTool.Succeed("服务安装成功！");
            }
            else
            {
                MessageBoxTool.Error("服务已存在！");
            }
        }

        private void Stop_Service_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceFactory.IsServiceExisted(ServeName))
            {
                this.Cursor = Cursors.Wait;
                ServiceFactory.ServiceStop(ServeName);
                ServiceFactory.UninstallService(ServePath);
                this.Cursor = Cursors.Arrow;
                MessageBoxTool.Succeed("卸载成功！");
            }
            else
            {
                MessageBoxTool.Error("服务不存在！");
            }
        }
        #endregion
    }

    /// <summary>
    /// 菜单项
    /// </summary>
    public enum Menutype
    {
        /// <summary>
        /// 串口设置
        /// </summary>
        [Description("串口设置")]
        ProntSetting,

        /// <summary>
        /// 地图设置
        /// </summary>
        [Description("地图设置")]
        MapSetting,

        /// <summary>
        /// 网络设置
        /// </summary>
        [Description("网络设置")]
        Network,

        /// <summary>
        /// 系统设置
        /// </summary>
        [Description("系统设置")]
        SystemSetting
    }
}
