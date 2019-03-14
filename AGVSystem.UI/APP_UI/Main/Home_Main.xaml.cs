using AGVSystem.APP.agv_System;
using AGVSystem.UI.APP_UI.Map;
using AGVSystem.UI.APP_UI.Setting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using AGVSystem.Model.Ga_agvModels;
using AGVSystem.APP.agv_Map;
using AGVSystem.Model.MapData;
using AGVSystem.Infrastructure.agvCommon;
using AGVSystem.Model.LogicData;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using AGVDLL;
using System.Threading;
using System.Collections.Concurrent;
using AGVSystem.DispatchLogic;
using System.Windows.Media;
using AGVSystem.Model.DrawMap;
using System.Linq;
using System.Windows.Shapes;
using AGVSystem.UI.APP_UI.Log;
using AGVSystem.BLL.ServiceLogicBLL;
using System.Windows.Input;
using System.Net;

namespace AGVSystem.UI.APP_UI.Main
{
    /// <summary>
    /// Home_Main.xaml 的交互逻辑
    /// </summary>
    public partial class Home_Main : Window
    {
        public Home_Main()
        {
            InitializeComponent();
        }

        private bool realBool = true;//是否模拟标志位
        private bool runSwitch = true;//用于计算AGV是否运行的标志位
        private bool IsConnect = false;//是否连接标志位(是否打开串口)
        private bool IsRun = false;//是否运行标志位
        private bool StartNoopsyche = false; //是否智能运行
        private agvFunction Get_AGVmanagement = new agvFunction(); //业务逻辑处理
        private agvMapRegulate mapService = new agvMapRegulate();  //业务逻辑处理
        private Ga_agvBLL GetBLL = new Ga_agvBLL();  //业务逻辑处理
        private Ga_mapBLL MapBLL = new Ga_mapBLL();
        private MapInstrument map = new MapInstrument();  //绘图处理
        private ConcurrentDictionary<int, CarStatus> CarStatusDic = new ConcurrentDictionary<int, CarStatus>();//AGV信息
        private List<Task> AgvRunTaskList = new List<Task>();//AGV运行线程List
        private List<Task> agvNoopsyche = new List<Task>(); //AGV路径计算线程List
        private List<Task> PathTask = new List<Task>();
        private CancellationTokenSource Cts = new CancellationTokenSource();//用于取消串口任务
        private CancellationTokenSource CtsCal = new CancellationTokenSource();//用于取消计算路径任务
        private CancellationTokenSource Interface = new CancellationTokenSource();  //用于取消界面刷新任务
        private CancellationTokenSource PathLine = new CancellationTokenSource(); //用于取消路径下发任务
        private CalRoute calR = new CalRoute(); //任务计算
        private DataTable routeTable = new DataTable(); //线路集合
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => { mapService.DataBase(); });//检查Database是否存在
            Setting_Map();
        }

        #region 加载地图数据

        /// <summary>
        /// 加载地图数据
        /// </summary>
        public void LoadMap()
        {
            try
            {
                Ga_Map GetMap = MapRegulate.maps.FirstOrDefault(x => x.CreateTime.Equals(MapMenu.SelectedValue.ToString()));
                if (GetMap != null)
                {
                    if (OperateIniTool.Exist)
                    {
                        string SiseResult = OperateIniTool.OperateIniRead("AGV", "MapSise");
                        map.MapSise = !string.IsNullOrEmpty(SiseResult) ? Convert.ToDouble(SiseResult) : 2;
                        MapRegulate.TemplateName = OperateIniTool.OperateIniRead("AGV", "TemplateName");
                        string LineResult = OperateIniTool.OperateIniRead("AGV", "DirectionLine");
                        MapRegulate.DirectionLine = LineResult.TransformInt();
                    }
                    double CanvasWidth = GetMap.Width * 10 * map.MapSise > this.Width * 1.2 ? GetMap.Width * 10 * map.MapSise : this.Width * 1.2;
                    double CanvasHeight = GetMap.Height * 10 * map.MapSise > this.Height * 1.2 ? GetMap.Height * 10 * map.MapSise : this.Height * 1.2;
                    map.Initial_Canvas(TopX, TopY, mainPanel, CanvasWidth, CanvasHeight);
                    map.LoadEditMap(MapRegulate.UTCTime, false, false);
                    TabAgvMoveInfo(MapRegulate.UTCTime);
                    LoadComInfo(MapRegulate.UTCTime);
                    InitializeNetwork(MapRegulate.UTCTime);
                }
                else
                {
                    throw new Exception("地图数据不存在！");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 地图选项更改

        /// <summary>
        /// 地图选项更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MapMenu.Items.Count > 0 && MapMenu.SelectedValue != null)
            {
                SelectionMenu();
            }
        }

        public void SelectionMenu()
        {
            MapRegulate.UTCTime = UTC.ConvertDateTimeLong(Convert.ToDateTime(MapMenu.SelectedValue.ToString()));
            LoadMap();
            Load_agv();
        }
        #endregion

        #region 界面数据绑定

        #region 界面绑定

        /// <summary>
        /// 加载串口信息
        /// </summary>
        public void LoadComInfo(long Time)
        {
            MapRegulate.GetPortInfos = Get_AGVmanagement.agvGather(Time);
            SerialPortData.DataContext = MapRegulate.GetPortInfos;
            SerialPortData.AutoGenerateColumns = false;
        }

        /// <summary>
        /// 加载网络信息
        /// </summary>
        /// <param name="Time"></param>
        public void InitializeNetwork(long Time)
        {
            MapRegulate.networkInfos = Get_AGVmanagement.LoadNetwork(Time);
            TabSerialPortDatas.DataContext = MapRegulate.networkInfos;
            TabSerialPortDatas.AutoGenerateColumns = false;
        }

        /// <summary>
        /// 显示所有AGV初始信息
        /// </summary>
        /// <param name="Time"></param>
        public void TabAgvMoveInfo(long Time)
        {
            MapRegulate.GetAgvs = Get_AGVmanagement.AgvInfo(Time, ref MapRegulate.selAgv, ref CarStatusDic, ref calR.agvInfoDic);
            TabAgvMoveData.DataContext = MapRegulate.GetAgvs;
            TabAgvMoveData.AutoGenerateColumns = false;
        }

        /// <summary>
        ///加载默认agv初始信息
        /// </summary>
        public void AgvInfo()
        {
            MapRegulate.Ga_AgvStatuses = Get_AGVmanagement.AgvInfo();
            TabAgvData.DataContext = MapRegulate.Ga_AgvStatuses;
            TabAgvData.HeadersVisibility = DataGridHeadersVisibility.None;
            TabAgvData.AutoGenerateColumns = false;
        }
        #endregion

        #region 加载AGV状态图标

        public void Load_agv()
        {
            MapRegulate.GetValuePairs.Clear();
            Grid gridItem = new Grid();
            gridItem.Children.Clear();
            gridItem.VerticalAlignment = VerticalAlignment.Top;
            gridItem.HorizontalAlignment = HorizontalAlignment.Center;
            agvlist.Content = gridItem;
            //创建行
            for (int s = 0; s < MainInfo.agvNo.Count; s++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(40);
                gridItem.RowDefinitions.Add(row);
            }
            for (int s = 0; s < 2; s++)
            {
                //创建列
                ColumnDefinition column = new ColumnDefinition();
                gridItem.ColumnDefinitions.Add(column);
            }

            for (int i = 0; i < MainInfo.agvNo.Count; i++)
            {
                Label labe2 = new Label();
                labe2.Content = $"AGV{MainInfo.agvNo[i]}";
                labe2.HorizontalAlignment = HorizontalAlignment.Center;
                labe2.VerticalAlignment = VerticalAlignment.Center;
                labe2.Margin = new Thickness(10, 5, 20, 5);
                labe2.FontSize = 14;
                labe2.Tag = MainInfo.agvNo[i];
                labe2.FontWeight = FontWeights.Black;
                labe2.Cursor = Cursors.Hand;
                labe2.MouseDown += Labe2_MouseDown;
                Grid.SetColumn(labe2, 0);
                Grid.SetRow(labe2, i);
                gridItem.Children.Add(labe2);

                Image image = new Image();
                image.Source = new BitmapImage(new Uri("pack://application:,,,/image/Gray_s.png"));
                image.HorizontalAlignment = HorizontalAlignment.Center;
                image.VerticalAlignment = VerticalAlignment.Center;
                image.Margin = new Thickness(10, 5, 0, 5);
                Grid.SetColumn(image, 1);
                Grid.SetRow(image, i);
                gridItem.Children.Add(image);
                MapRegulate.GetValuePairs.GetOrAdd(Convert.ToInt32(MainInfo.agvNo[i]), image);
            }
        }

        private void Labe2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Label label = sender as Label;
            MapRegulate.selAgv = Convert.ToInt32(label.Tag);
            //MessageBox.Show(label.Tag.ToString());
        }
        #endregion

        #endregion

        #region 查询默认地图

        /// <summary>
        /// 查询默认地图
        /// </summary>
        public void Setting_Map()
        {
            MapRegulate.maps = mapService.GetMapRegulate();
            MapMenu.ItemsSource = MapRegulate.maps;
            DataTable data = mapService.setting();
            if (data.Rows.Count > 0)
            {
                long DefaultName = long.Parse(data.Rows[0]["Map"].ToString().Trim());
                MapRegulate.DefaultMap = DefaultName;
                Ga_Map ga = MapRegulate.maps.FirstOrDefault(x => x.CreateTime.Equals(UTC.ConvertLongDateTime(DefaultName).ToString("yyyy-MM-dd HH:mm:ss")));
                if (ga != null)
                {
                    MapMenu.Text = ga.Name;
                }
            }
            else
            {
                throw new Exception("默认地图配置丢失！");
            }
            AgvInfo();
        }
        #endregion

        #region 打开串口

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Port_Click(object sender, RoutedEventArgs e)
        {
            if (!IsConnect)
            {
                ClearOrder(); //清空所有正在执行的任务
                Cts = new CancellationTokenSource();
                bool Open = true; //所有设备是否打开成功
                if (!realBool) //不为模拟
                {
                    {
                        //网口
                        for (int i = 0; i < PortInfo.Networks.Count; i++)
                        {
                            AsyncTcpClient client = OpenNetwork(PortInfo.Networks[i]); //建立网络连接
                            Thread.Sleep(200);
                            if (!client.Connected)
                            {
                                Open = false;
                                MessageBoxTool.Error($"连接IP ：{PortInfo.Networks[i].IP_Address}端口：{PortInfo.Networks[i].IP_Port}，失败！");
                                break;
                            }
                            else
                            {
                                PortInfo.Networks[i].TcpClient = client;
                            }
                        }
                    }
                    {
                        if (Open)
                        {
                            //其他外设
                            for (int i = 0; i < PortInfo.DePortsList.Count; i++)
                            {
                                if (!OpenPeripheral(i, PortInfo.DePortsList[i]))
                                {
                                    Open = false;
                                    Cts.Cancel();
                                    break;
                                }
                            }
                        }
                    }
                    {
                        if (Open)
                        {   //agv串口
                            for (int i = 0; i < PortInfo.AgvPortsList.Count; i++)
                            {
                                if (!OpenPort(i, PortInfo.AgvPortsList[i]))
                                {
                                    Open = false;
                                    Cts.Cancel();
                                    break;
                                }
                            }
                        }
                    }
                }
                else //模拟
                {
                    for (int i = 0; i < PortInfo.AgvPortsList.Count; i++)
                    {
                        int Index = i;
                        Task tk = new Task(() => AgvRunTh(Index), Cts.Token);
                        AgvRunTaskList.Add(tk);
                    }
                }
                if (Open)
                {
                    if (AgvRunTaskList.Count != 0)
                    {
                        routeTable = MapBLL.BLLMapRouteTable(MapRegulate.UTCTime.ToString());
                        calR.sendRouteDt(routeTable);
                        for (int i = 0; i < AgvRunTaskList.Count; i++)
                        {
                            AgvRunTaskList[i].Start();
                        }
                        Interface = new CancellationTokenSource();
                        AgvRunTaskList.Add(Task.Factory.StartNew(() => InterfaceUpdate(), Interface.Token));//界面刷新
                        TxtLogShowInfo($"OpenPort", false);
                        PortImg.Source = new BitmapImage(new Uri("pack://application:,,,/image/Stop.png", UriKind.Absolute));
                        PortText.Text = "关闭串口";
                        StartBrainpower.IsEnabled = true;
                        MapMenu.IsEnabled = false;
                        IsConnect = true;
                        Menu.IsEnabled = false;
                        Issueoff.IsEnabled = true;
                        Issue_Line.IsEnabled = true;
                    }
                }
            }
            else   //关闭串口
            {
                Interface.Cancel();
                Cts.Cancel();
                Thread.Sleep(100);
                for (int j = 0; j < 3; j++)
                {
                    bool IsComp = true;
                    for (int i = 0; i < AgvRunTaskList.Count; i++)
                    {
                        if (AgvRunTaskList[i].Status == TaskStatus.Running)
                        {
                            Interface.Cancel();
                            Cts.Cancel();
                            IsComp = false;
                        }
                    }
                    if (IsComp)
                    {
                        break;
                    }
                }
                if (!realBool) //不为模拟
                {
                    //关闭AGV串口
                    for (int i = 0; i < PortInfo.AgvPortsList.Count; i++)
                    {
                        PortInfo.AgvPortsList[i].IntPtr = new IntPtr();
                        PortInfo.AgvPortsList[i].AgvDll.closePort(i + 1);
                    }
                    //关闭其他外设
                    for (int i = 0; i < PortInfo.DePortsList.Count; i++)
                    {
                        if (PortInfo.DePortsList[i].Port.IsOpen)
                        {
                            PortInfo.DePortsList[i].Port.Close();
                        }
                    }
                    //关闭网络
                    for (int i = 0; i < PortInfo.Networks.Count; i++)
                    {
                        if (PortInfo.Networks[i].TcpClient != null)
                        {
                            if (PortInfo.Networks[i].TcpClient.Connected)
                                PortInfo.Networks[i].TcpClient.Close();
                        }
                    }
                }
                MapRegulate.GetValuePairs.Values.ToList().ForEach(p => p.Source = new BitmapImage(new Uri("pack://application:,,,/image/Gray_s.png", UriKind.Absolute)));
                MapRegulate.Ga_AgvStatuses.ToList().ForEach(p => p.StatusValue = "");
                MapRegulate.GetAgvs.ToList().ForEach(p => { p.agvStatic = "离线"; p.Dir = ""; p.Hook = ""; p.PBS = ""; p.Operation = ""; p.RFID = ""; p.Speed = ""; p.Step = ""; p.turn = ""; p.Voltage = ""; p.RouteNo = ""; p.Program = ""; p.ColorBg = new SolidColorBrush(Colors.Red); });
                MapRegulate.Site.Values.ToList().ForEach(p => mainPanel.Children.Remove(p.agvSite));
                UpdatePort(); UpdateNetwork();
                TxtLogShowInfo("ClosePort", false);
                PortImg.Source = new BitmapImage(new Uri("pack://application:,,,/image/start.png", UriKind.Absolute));
                PortText.Text = "打开串口";
                IsConnect = false;
                MapMenu.IsEnabled = true;
                StartBrainpower.IsEnabled = false;
                Menu.IsEnabled = true;
                Issueoff.IsEnabled = false;
                Issue_Line.IsEnabled = false;
            }

        }



        #region TCPClient

        /// <summary>
        /// 打开网络
        /// </summary>
        /// <param name="i"></param>
        /// <param name="Net"></param>
        /// <returns></returns>
        public AsyncTcpClient OpenNetwork(NetworkInfo Net)
        {
            AsyncTcpClient tcpClient = new AsyncTcpClient(IPAddress.Parse(Net.IP_Address), Net.IP_Port);//new一个异步tcp客户端用于和IO模块交互         
            tcpClient.ServerConnected += TcpClient_ServerConnected; ;
            tcpClient.ServerDisconnected += TcpClient_ServerDisconnected;
            tcpClient.ServerExceptionOccurred += TcpClient_ServerExceptionOccurred; ;
            tcpClient.DatagramReceived += TcpClient_DatagramReceived; ;
            tcpClient.Connect();
            return tcpClient;
        }

        /// <summary>
        /// 与服务器连接发送异常事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TcpClient_ServerExceptionOccurred(object sender, TcpServerExceptionOccurredEventArgs e)
        {
            (sender as AsyncTcpClient).Close();
            TxtLogShowInfo(e.Exception.Message, true);
        }

        /// <summary>
        /// 与服务器端口断开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TcpClient_ServerDisconnected(object sender, TcpServerDisconnectedEventArgs e)
        {
            TxtLogShowInfo($"IP：{e.Addresses[0].ToString()}，端口：{e.Port}，断开", true);
        }

        /// <summary>
        /// 与服务器建立连接事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TcpClient_ServerConnected(object sender, TcpServerConnectedEventArgs e)
        {
            TxtLogShowInfo($"IP：{e.Addresses[0].ToString()}，端口：{e.Port}，连接成功", true);
        }

        /// <summary>
        /// 接收到数据报文事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TcpClient_DatagramReceived(object sender, TcpDatagramReceivedEventArgs<byte[]> e)
        {
           

        }
        #endregion


        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="i">AGV控制串口List序号，不是串口号</param>
        /// <param name="PortInfo"></param>
        public bool OpenPort(int i, AgvPortInfo PortInfo)
        {
            int groupNo = i + 1;//groupNo从1开始
            PortInfo.AgvDll.dllName = PortInfo.ComNumber.ToString();
            IntPtr ptr = PortInfo.AgvDll.openPort(groupNo, PortInfo.ComNumber, PortInfo.BaudRate, MainInfo.prity, MainInfo.stopBits);
            AGVSystem.Model.LogicData.PortInfo.AgvPortsList[i].IntPtr = ptr;
            if (ptr == (IntPtr)0)
            {
                //打开失败         
                MessageBox.Show("打开串口COM" + PortInfo.ComNumber.ToString() + "失败！", "提示", MessageBoxButton.OK);
                return false;
            }
            else
            {
                //打开串口成功
                Task tk = new Task(() => AgvRunTh(i), Cts.Token);
                AgvRunTaskList.Add(tk);
                return true;
            }
        }

        /// <summary>
        /// 清空任务
        /// </summary>
        private void ClearOrder()
        {
            //清空所有正在执行的任务
            while (true)
            {
                bool compBool = true;
                for (int i = 0; i < AgvRunTaskList.Count; i++)
                {
                    if (AgvRunTaskList[i].Status == TaskStatus.Running)
                    {
                        Interface.Cancel();
                        Cts.Cancel();
                        compBool = false;
                    }
                }
                if (compBool)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(200);
                }
            }
            foreach (NetworkInfo item in PortInfo.Networks)
            {
                if (item.TcpClient != null)
                {
                    AsyncTcpClient client = item.TcpClient;
                    if (client.Connected)
                    {
                        client.Close();
                        client.Dispose();
                    }
                }
            }
            AgvRunTaskList.Clear();
        }


        /// <summary>
        /// 打开其他串口（按钮，充电机，PLC）
        /// </summary>
        /// <param name="device"></param>
        private bool OpenPeripheral(int i,DevicePortInfo device)
        {
            try
            {
                if (!device.Port.IsOpen)
                {
                    device.Port.Open();
                }
                //打开串口成功
                Task tk = new Task(() => RunPeripheral(i), Cts.Token);
                AgvRunTaskList.Add(tk);
                return true;
            }
            catch (Exception ex)
            {
                MessageBoxTool.Error($"打开串口COM{ device.ComNumber} 失败，错误信息：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 其他串口数据处理（按钮/充电机/PLC）
        /// </summary>
        /// <param name="i"></param>
        private void RunPeripheral(int i)
        {
            while (!Cts.Token.IsCancellationRequested)
            {
                if(!PortInfo.DePortsList[i].Port.IsOpen)
                {
                    PortInfo.DePortsList[i].Port.Open();
                }
                if (PortInfo.DePortsList[i].PortType == PortType.Button) //按钮
                {


                }
                else if (PortInfo.DePortsList[i].PortType == PortType.Charge) //充电机
                {


                }
                else if (PortInfo.DePortsList[i].PortType == PortType.PLC) //PLC
                {


                }
            }
        }

        #endregion

        #region 智能运行

        /// <summary>
        /// 智能运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartBrainpower_Click(object sender, RoutedEventArgs e)
        {
            if (!StartNoopsyche)
            {
                CancelTask();//清除所有任务
                agvNoopsyche.Clear();
                CtsCal = new CancellationTokenSource();
                Task tk = new Task(() =>
                {
                    while (!CtsCal.IsCancellationRequested)
                    {
                        calR.agvSch();
                        Thread.Sleep(500);
                    }
                }, CtsCal.Token);
                agvNoopsyche.Add(tk);
                agvNoopsyche.ForEach(p => p.Start());
                StartImg.Source = new BitmapImage(new Uri("pack://application:,,,/image/Stopagv.png", UriKind.Absolute));
                StartText.Text = "停止运行";
                StartNoopsyche = true;
                Open_Port.IsEnabled = false;
                Issueoff.IsEnabled = false;
                Issue_Line.IsEnabled = false;
            }
            else
            {
                CancelTask(); //清除所有任务
                StartImg.Source = new BitmapImage(new Uri("pack://application:,,,/image/noopsyche.png", UriKind.Absolute));
                StartText.Text = "智能运行";
                StartNoopsyche = false;
                Open_Port.IsEnabled = true;
                Issueoff.IsEnabled = true;
                Issue_Line.IsEnabled = true;
            }
        }
        //取消所有任务
        private void CancelTask()
        {
            while (true)
            {
                bool IsRunagv = true;
                for (int i = 0; i < agvNoopsyche.Count; i++)
                {
                    if (agvNoopsyche[i].Status == TaskStatus.Running)
                    {
                        CtsCal.Cancel();
                        IsRunagv = false;
                    }
                }
                if (IsRunagv)
                {
                    break;
                }
            }
        }
        #endregion

        #region 程序退出

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsConnect)
            {
                MessageBoxResult result = MessageBox.Show("确定退出程序？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                e.Cancel = true;
                MessageBox.Show("请先关闭串口！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #endregion

        #region 功能菜单

        /// <summary>
        /// 系统设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            SettingForm setting = new SettingForm();
            setting.GetSettingMap += SelectionMenu;
            setting.ShowDialog();
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 编辑地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Com_Map_Click(object sender, RoutedEventArgs e)
        {
            CompileMap compile = new CompileMap();
            compile.Map_action += Setting_Map;
            compile.Show();
        }


        /// <summary>
        /// ToolBar样式初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as ToolBar).ToolBar_Formatting();
        }

        /// <summary>
        /// 编辑线路
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EditLine edit = new EditLine(MapRegulate.maps);
            edit.Show();
        }

        /// <summary>
        /// 编辑信标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TagEdit_Click(object sender, RoutedEventArgs e)
        {
            EditTag edit = new EditTag();
            edit.Show();
        }

        /// <summary>
        /// 历史信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Log_AGV_Click(object sender, RoutedEventArgs e)
        {
            LogForm log = new LogForm();
            log.Show();
        }

        #endregion

        #region 线路绘制模拟

        private List<Path> Path = new List<Path>();
        private Image GetPath = new Image();

        public void Line()
        {
            var routes = mapService.GetrouteList(MapRegulate.UTCTime.ToString());
            map.SignLine(map.wirePointArrays, Brushes.Black, 2, 1);
            Task.Factory.StartNew(() =>
            {


                foreach (var item in routes)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        map.SignLine(map.wirePointArrays, Brushes.Black, 1.7, 888);
                        Path.ForEach(p => mainPanel.Children.Remove(p));
                        mainPanel.Children.Remove(GetPath);
                        string[] TagArray = item.Tag.Split(',');
                        for (int i = 0; i < TagArray.Length; i++)
                        {
                            if (!i.Equals(TagArray.Length - 1))
                            {
                                string aa = TagArray[i];
                                string bb = TagArray[i + 1];
                                List<WirePointArray> wires = map.wirePointArrays.Where(p => (p.GetPoint.TagID.Equals(Convert.ToInt32(TagArray[i])) && p.GetWirePoint.TagID.Equals(Convert.ToInt32(TagArray[i + 1]))) || (p.GetPoint.TagID.Equals(Convert.ToInt32(TagArray[i + 1])) && p.GetWirePoint.TagID.Equals(Convert.ToInt32(TagArray[i])))).ToList();
                                map.SignLine(wires, Brushes.Red, 3, 999);

                                if (true)
                                {
                                    int LineTag1 = Convert.ToInt32(TagArray[i]);
                                    int LineTag2 = Convert.ToInt32(TagArray[i + 1]);
                                    if (map.valuePairs.ContainsKey(LineTag1) && (map.valuePairs.ContainsKey(LineTag2)))
                                    {
                                        var Line = map.wirePointArrays.Where(p => (p.GetPoint.TagID.Equals(LineTag1) && p.GetWirePoint.TagID.Equals(LineTag2)) || (p.GetPoint.TagID.Equals(LineTag2) && p.GetWirePoint.TagID.Equals(LineTag1))).ToList();

                                        if (Line.Count() > 0)
                                        {
                                            int Tag1 = Line[0].GetPoint.TagID;
                                            int Tag2 = Line[0].GetWirePoint.TagID;
                                            if (LineTag1.Equals(Tag1) && LineTag2.Equals(Tag2))
                                            {
                                                if (i == 0)
                                                {
                                                    GetPath = NewdDot(Line[0].GetPoint.SetPoint);
                                                }
                                                Path.Add(map.Draw_Triangle(Line[0].circuitType, Line[0].GetPoint.SetPoint, Line[0].GetWirePoint.SetPoint, true));
                                            }
                                            else
                                            {
                                                if (i == 0)
                                                {
                                                    GetPath = NewdDot(Line[0].GetWirePoint.SetPoint);
                                                }
                                                Path.Add(map.Draw_Triangle(Line[0].circuitType, Line[0].GetWirePoint.SetPoint, Line[0].GetPoint.SetPoint, false));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });

                    Thread.Sleep(1000);
                }
            });
        }




        private Image NewdDot(Point point)
        {
            Image text = new Image();
            text.Source = new BitmapImage(new Uri("pack://application:,,,/image/Dit.png", UriKind.Absolute));
            text.Width = 20;
            text.Height = 20;
            text.Margin = new Thickness(point.X - 10, point.Y - 10, 0, 0);
            Grid.SetZIndex(text, 99999);
            mainPanel.Children.Add(text);
            return text;
        }
        #endregion

        #region 刷新UI界面

        /// <summary>
        /// 刷新UI界面
        /// </summary>
        private void InterfaceUpdate()
        {
            while (!Interface.IsCancellationRequested)
            {
                try
                {
                    UpdateAGVAll();
                    UpdatePort();
                    UpdateNetwork();
                }
                catch (Exception ex)
                {
                    Log4NetHelper.WriteErrorLog(ex.Message,ex);
                }
                Thread.Sleep(800);
            }
        }

        /// <summary>
        /// 刷新所有AGV信息
        /// </summary>
        private void UpdateAGVAll()
        {
            foreach (Ga_agv item in MapRegulate.GetAgvs)
            {
                if (MapRegulate.selAgv == CarStatusDic[item.agvNumber].carNum)
                {
                    UpdateDefaultAGV(CarStatusDic[item.agvNumber]); //更新默认AGV信息
                }
                if (CarStatusDic[item.agvNumber].errorCode == 0 && CarStatusDic[item.agvNumber].carNum == 0)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        item.agvStatic = "连接中";
                        item.ColorBg = new SolidColorBrush(Colors.Green);
                        MapRegulate.GetValuePairs[item.agvNumber].Source = new BitmapImage(new Uri("pack://application:,,,/image/Green_s.gif", UriKind.Absolute));
                    });
                }
                else if (CarStatusDic[item.agvNumber].errorCode == 205)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        item.agvStatic = "离线";
                        item.ColorBg = new SolidColorBrush(Colors.Red);
                        MapRegulate.GetValuePairs[item.agvNumber].Source = new BitmapImage(new Uri("pack://application:,,,/image/Gray_s.png", UriKind.Absolute));
                        MapRegulate.GetAgvs.Where(x=>x.agvNumber== item.agvNumber).ToList().ForEach(p => { p.agvStatic = "离线"; p.Dir = ""; p.Hook = ""; p.PBS = ""; p.Operation = ""; p.RFID = ""; p.Speed = ""; p.Step = ""; p.turn = ""; p.Voltage = ""; p.RouteNo = ""; p.Program = ""; p.ColorBg = new SolidColorBrush(Colors.Red); });
                    });
                }
                else if (CarStatusDic[item.agvNumber].errorCode != 0 && CarStatusDic[item.agvNumber].errorCode != 205)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        item.agvStatic = "报警";
                        item.ColorBg = new SolidColorBrush(Colors.Red);
                        MapRegulate.GetValuePairs[item.agvNumber].Source = new BitmapImage(new Uri("pack://application:,,,/image/Red_s.gif", UriKind.Absolute));
                    });
                }
                else if (CarStatusDic[item.agvNumber].errorCode == 0)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        item.agvStatic = "在线";
                        item.ColorBg = new SolidColorBrush(Colors.Green);
                        MapRegulate.GetValuePairs[item.agvNumber].Source = new BitmapImage(new Uri("pack://application:,,,/image/Green_s.png", UriKind.Absolute));
                    });
                }
                if (CarStatusDic[item.agvNumber].errorCode != 205 && CarStatusDic[item.agvNumber].carNum != 0)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (CarStatusDic[item.agvNumber].IsRunning)
                        {
                            item.Operation = "运行";
                            item.ColorOperation = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            item.Operation = "停止";
                            item.ColorOperation = new SolidColorBrush(Colors.Red);
                        }
                        item.Voltage = CarStatusDic[item.agvNumber].powerCurrentF.ToString() + "V";
                        item.Speed = CarStatusDic[item.agvNumber].speedNo < MainInfo.agvSpeed.Length ? MainInfo.agvSpeed[CarStatusDic[item.agvNumber].speedNo] : "0: 5";
                        if (CarStatusDic[item.agvNumber].agvRunRight)
                        {
                            item.turn = "右转";
                            item.Turncolor = new SolidColorBrush(Colors.Green);
                        }
                        else if (!CarStatusDic[item.agvNumber].agvRunLeft)
                        {
                            item.turn = "左转";
                            item.Turncolor = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            item.turn = "无";
                            item.Turncolor = new SolidColorBrush(Colors.Black);
                        }
                        if (CarStatusDic[item.agvNumber].agvRunDirection)
                        {
                            item.Dircolor = new SolidColorBrush(Colors.Green);
                            item.Dir = "正向";
                        }
                        else
                        {
                            item.Dircolor = new SolidColorBrush(Colors.Red);
                            item.Dir = "反向";
                        }
                        if (CarStatusDic[item.agvNumber].agvHookUP)
                        {
                            item.Hook = "上升";
                            item.Hookcolor = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            item.Hook = "下降";
                            item.Hookcolor = new SolidColorBrush(Colors.Red);
                        }
                        item.RFID = CarStatusDic[item.agvNumber].rfidStatus != "-53328" ? CarStatusDic[item.agvNumber].rfidStatus : "0";
                        item.RouteNo = CarStatusDic[item.agvNumber].agvRouteNo.ToString();
                        item.PBS = CarStatusDic[item.agvNumber].pbsArea.ToString();
                        item.Program = CarStatusDic[item.agvNumber].programNo.ToString();
                        item.Step = CarStatusDic[item.agvNumber].stepNo.ToString();
                    });
                }
                //所有agv位置绘制
                int RFID = CarStatusDic[item.agvNumber].rfidStatus != null ? (FormatVerification.IsFloat(CarStatusDic[item.agvNumber].rfidStatus) ? Convert.ToInt32(CarStatusDic[item.agvNumber].rfidStatus) : 9999) : 9999;
                if (map.valuePairs.ContainsKey(RFID))
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        mainPanel.Children.Remove(MapRegulate.Site[CarStatusDic[item.agvNumber].carNum].agvSite);
                        MapRegulate.Site[CarStatusDic[item.agvNumber].carNum].agvSite = new Label()
                        {
                            Background = MapRegulate.Site[CarStatusDic[item.agvNumber].carNum].agvSiteColor,
                            Foreground = map.valuePairs[RFID].Foreground,
                            Content = CarStatusDic[item.agvNumber].carNum,
                            Width = 38,
                            Height = 23,
                            Margin = map.valuePairs[RFID].Margin,
                            HorizontalContentAlignment = map.valuePairs[RFID].HorizontalContentAlignment,
                            VerticalContentAlignment = map.valuePairs[RFID].VerticalContentAlignment,
                        };
                        Panel.SetZIndex(MapRegulate.Site[CarStatusDic[item.agvNumber].carNum].agvSite, 9999);
                        mainPanel.Children.Add(MapRegulate.Site[CarStatusDic[item.agvNumber].carNum].agvSite);
                    });
                }
            }
        }

        /// <summary>
        /// 更新默认AGV信息
        /// </summary>
        /// <param name="car"></param>
        private void UpdateDefaultAGV(CarStatus car)
        {
            if (car.carNum != 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    MapRegulate.Ga_AgvStatuses[0].StatusValue = car.carNum.ToString();
                    if (car.errorCode == 205)
                    {
                        MapRegulate.Ga_AgvStatuses[1].StatusValue = "离线";
                        MapRegulate.Ga_AgvStatuses[1].ColorBg = new SolidColorBrush(Colors.Red);
                    }
                    else if (car.errorCode != 205 && car.carNum != 0)
                    {
                        MapRegulate.Ga_AgvStatuses[1].StatusValue = "在线";
                        MapRegulate.Ga_AgvStatuses[1].ColorBg = new SolidColorBrush(Colors.Green);
                    }
                    if (car.errorCode != 205 && car.carNum != 0)
                    {
                        if (car.agvRunReady)
                        {
                            MapRegulate.Ga_AgvStatuses[2].StatusValue = "ON";
                            MapRegulate.Ga_AgvStatuses[2].ColorBg = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            MapRegulate.Ga_AgvStatuses[2].StatusValue = "OFF";
                            MapRegulate.Ga_AgvStatuses[2].ColorBg = new SolidColorBrush(Colors.Red);
                        }
                        if (!car.agvDriverDown)
                        {
                            MapRegulate.Ga_AgvStatuses[3].StatusValue = "上升";
                            MapRegulate.Ga_AgvStatuses[3].ColorBg = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            MapRegulate.Ga_AgvStatuses[3].StatusValue = "下降";
                            MapRegulate.Ga_AgvStatuses[3].ColorBg = new SolidColorBrush(Colors.Red);
                        }
                        if (car.agvLineRead)
                        {
                            MapRegulate.Ga_AgvStatuses[4].StatusValue = "正常";
                            MapRegulate.Ga_AgvStatuses[4].ColorBg = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            MapRegulate.Ga_AgvStatuses[4].StatusValue = "脱轨";
                            MapRegulate.Ga_AgvStatuses[4].ColorBg = new SolidColorBrush(Colors.Red);
                        }

                        MapRegulate.Ga_AgvStatuses[7].StatusValue = calR.agvInfoDic[car.carNum].sendTask;
                        MapRegulate.Ga_AgvStatuses[7].ColorBg = new SolidColorBrush(Colors.Black);
                        if (car.errorCode == 0)
                        {
                            MapRegulate.Ga_AgvStatuses[8].StatusValue = "正常";
                            MapRegulate.Ga_AgvStatuses[8].ColorBg = new SolidColorBrush(Colors.Green);
                        }
                        else if (car.errorCode != 205)
                        {
                            MapRegulate.Ga_AgvStatuses[8].StatusValue = Error.errorStr(car.errorCode);
                            MapRegulate.Ga_AgvStatuses[8].ColorBg = new SolidColorBrush(Colors.Green);
                        }
                    }
                    else
                    {
                        MapRegulate.Ga_AgvStatuses[2].StatusValue = "";
                        MapRegulate.Ga_AgvStatuses[3].StatusValue = "";
                        MapRegulate.Ga_AgvStatuses[4].StatusValue = "";
                        MapRegulate.Ga_AgvStatuses[5].StatusValue = "";
                        MapRegulate.Ga_AgvStatuses[6].StatusValue = "";
                        MapRegulate.Ga_AgvStatuses[7].StatusValue = "";
                        MapRegulate.Ga_AgvStatuses[8].StatusValue = "";
                    }
                });
            }
        }

        /// <summary>
        /// 更新串口打开状态
        /// </summary>
        private void UpdatePort()
        {
            foreach (Ga_PortInfo item in MapRegulate.GetPortInfos)
            {
                int PortNum = Convert.ToInt32(item.ComNumber.Substring(3, item.ComNumber.Length - 3));
                if (item.ComPortType == PortType.AGV)
                {
                    AgvPortInfo agvPortInfo = PortInfo.AgvPortsList.FirstOrDefault(x => x.ComNumber.Equals(PortNum));
                    if (agvPortInfo != null)
                    {
                        if (agvPortInfo.IntPtr == (IntPtr)0)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                item.ComStatic = "关闭";
                                item.ComStaticColor = new SolidColorBrush(Colors.Red);
                            });
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                item.ComStatic = "打开";
                                item.ComStaticColor = new SolidColorBrush(Colors.Green);
                            });
                        }

                    }
                }
                else if (item.ComPortType == PortType.PLC || item.ComPortType == PortType.Button || item.ComPortType == PortType.Charge)
                {
                    DevicePortInfo device = PortInfo.DePortsList.FirstOrDefault(p => p.ComNumber.Equals(PortNum));
                    if (device != null)
                    {
                        if (device.Port.IsOpen)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                item.ComStatic = "打开";
                                item.ComStaticColor = new SolidColorBrush(Colors.Green);
                            });
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                item.ComStatic = "关闭";
                                item.ComStaticColor = new SolidColorBrush(Colors.Red);
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新网络连接状态
        /// </summary>
        private void UpdateNetwork()
        {
            foreach (NetworkInfo item in MapRegulate.networkInfos)
            {
                NetworkInfo network = PortInfo.Networks.FirstOrDefault(x => x.IP_Address== item.IP_Address && x.IP_Port==item.IP_Port);
                if (network != null)
                {
                    if (network.TcpClient != null)
                    {
                        if (!network.TcpClient.Connected)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                item.IP_Static = "断开";
                                item.IP_StaticColor = new SolidColorBrush(Colors.Red);
                            });
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                item.IP_Static = "连接";
                                item.IP_StaticColor = new SolidColorBrush(Colors.Green);
                            });
                        }
                    }
                }
            }
        }


        #endregion

        #region 逻辑运算

        private void AgvRunTh(int ComNo)
        {
            int groupNo = ComNo + 1;
            string[] comAgv = PortInfo.AgvPortsList[ComNo].AgvStr.Split(',');
            int comAgvCount = comAgv.Length;
            AGVDLL.AGVDLL agvDll = PortInfo.AgvPortsList[ComNo].AgvDll;

            int[] OffLine = new int[comAgvCount];//agv查到离线次数计数
            int offLimit = 5;//离线次数限指
            for (int i = 0; i < comAgvCount; i++)
            {
                OffLine[i] = 0;
            }
            while (!Cts.Token.IsCancellationRequested)
            {
                //Console.WriteLine("th" + MainInfo.listAgvDll[comNo].dllName);
                agvDll.agvPortClearCache(groupNo);//清除agv串口缓存
                for (int i = 0; i < comAgvCount; i++)
                {
                    int agvNo = int.Parse(comAgv[i]);//agv车号
                    try
                    {
                        CarStatus cs = new CarStatus();
                        if (!realBool)
                        {
                            cs = agvDll.read(new IntPtr(0), groupNo, agvNo);
                        }
                        else
                        {
                            cs = SimAgvInfo.simInfo(agvNo.ToString());
                            //模拟
                        }
                        if (cs.carNum != 0 && cs.errorCode != 205)
                        {
                            //rfid 更改 查询两次rfid
                            if (!cs.IsRunning && cs.agvDriverDown && !cs.rfidStatus.Equals(CarStatusDic[agvNo].rfidStatus))
                            {
                                Thread.Sleep(100);
                                CarStatus agvNewStatus = new CarStatus();
                                if (!realBool)
                                {
                                    agvNewStatus = agvDll.read(new IntPtr(0), groupNo, agvNo);
                                }
                                else
                                {    //模拟
                                    agvNewStatus = SimAgvInfo.simInfo(agvNo.ToString());
                                }

                                while (!agvNewStatus.rfidStatus.Equals(cs.rfidStatus))
                                {
                                    if (agvNewStatus.errorCode == 205)
                                    {
                                        string rfid = CarStatusDic[agvNo].rfidStatus;
                                        cs.rfidStatus = rfid;
                                        break;
                                    }
                                    else
                                    {
                                        string rfid = agvNewStatus.rfidStatus;
                                        cs.rfidStatus = rfid;
                                        Thread.Sleep(100);
                                        if (realBool)
                                        {
                                            agvNewStatus = agvDll.read(new IntPtr(0), groupNo, agvNo);
                                        }
                                        else
                                        {
                                            agvNewStatus = SimAgvInfo.simInfo(agvNo.ToString());
                                            //模拟
                                        }
                                    }
                                }
                            }

                            if (calR.agvInfoDic[agvNo].pcAgvStart == 1)
                            {
                                cs.IsRunning = true;
                            }
                            lock (calR.agvInfoDic)
                            {
                                cs.carNum = agvNo;
                                CarStatusDic[agvNo] = cs;
                            }
                        }
                        else
                        {
                            OffLine[i]++;
                            //bool offResult = true;
                            //for (int k = 0; k < OffLine.Length; k++)
                            //{
                            //    if (OffLine[k] < 5)
                            //    {
                            //        offResult = false;
                            //        break;
                            //    }
                            //}
                            //if (offResult)
                            //{
                            //    //reOpen = true;
                            //}

                            if (OffLine[i] > 5 && OffLine[i] <= 100)
                            {
                                lock (CarStatusDic)
                                {
                                    cs.carNum = agvNo;
                                    cs.rfidStatus = "";
                                    CarStatusDic[agvNo] = cs;
                                }
                            }
                            else if (OffLine[i] > 100)
                            {
                                OffLine[i] = 0;
                            }
                        }
                        if (CarStatusDic[agvNo].carNum == 0)
                        {
                            calR.agvInfoDic[agvNo].agvNumber = agvNo;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log4NetHelper.WriteErrorLog(ex.Message.ToString(),ex);
                    }
                }

                foreach (int item in CarStatusDic.Keys)
                {
                    if (CarStatusDic[item].errorCode != 205 && CarStatusDic[item].carNum != 0)
                    {
                        calR.agvInfoDic[item].agvStatic = agvRun.ON;
                    }
                    else
                    {
                        calR.agvInfoDic[item].agvStatic = agvRun.OFF;
                    }
                    calR.agvInfoDic[item].agvNumber = CarStatusDic[item].carNum;
                    calR.agvInfoDic[item].masterOn = CarStatusDic[item].agvRunReady;
                    calR.agvInfoDic[item].isRunning = CarStatusDic[item].IsRunning;
                    calR.agvInfoDic[item].isLine = CarStatusDic[item].agvLineRead;
                    calR.agvInfoDic[item].driveState = CarStatusDic[item].agvDriverDown;
                    calR.agvInfoDic[item].voltage = CarStatusDic[item].powerCurrentF;
                    calR.agvInfoDic[item].speedNo = CarStatusDic[item].speedNo;
                    calR.agvInfoDic[item].turnLeft = CarStatusDic[item].agvRunLeft;
                    calR.agvInfoDic[item].turnRight = CarStatusDic[item].agvRunRight;
                    calR.agvInfoDic[item].direction = CarStatusDic[item].agvRunDirection;
                    calR.agvInfoDic[item].hook = CarStatusDic[item].agvHookUP;
                    calR.agvInfoDic[item].rfid = CarStatusDic[item].rfidStatus;
                    calR.agvInfoDic[item].routeNo = CarStatusDic[item].agvRouteNo;
                    calR.agvInfoDic[item].pbs = CarStatusDic[item].pbsArea;
                    calR.agvInfoDic[item].step = CarStatusDic[item].stepNo;
                    calR.agvInfoDic[item].programNo = CarStatusDic[item].programNo;
                    calR.agvInfoDic[item].errorSwitch = CarStatusDic[item].errorSwitch;
                    calR.agvInfoDic[item].errorCode = CarStatusDic[item].errorCode;
                    if (string.IsNullOrEmpty(CarStatusDic[item].rfidStatus))
                    {
                        calR.agvInfoDic[item].rfid = "-53328";
                    }
                }

                if (comAgvCount > 1)
                {
                    Thread.Sleep(50);
                }
                else
                {
                    Thread.Sleep(300);
                }

                if (runSwitch)
                {
                    //clockLock = true;

                    #region 用于修复无线启动报警

                    for (int i = 0; i < comAgvCount; i++)
                    {
                        int agvNo = int.Parse(comAgv[i]);//agv车号
                        if (CarStatusDic[agvNo].errorCode == 81 && CarStatusDic[agvNo].agvDriverDown)
                        {
                            agvDll.RestAGV(new IntPtr(0), groupNo, agvNo);
                            string logStr = "AGV" + agvNo + ": " + "Reset";
                            TxtLogShowInfo(logStr, true); //写日志
                            Thread.Sleep(100);
                        }
                    }

                    #endregion 用于修复无线启动报警

                    #region 避让指令发送

                    //for (int i = 0; i < calR.avoidStop.Count; i++)
                    //{
                    //    int agvNum = calR.avoidStop[i][1];
                    //    if (agvNum != 0)
                    //    {
                    //        foreach (string agvStr in comAgv)
                    //        {
                    //            if (agvNum.ToString() == agvStr)
                    //            {
                    //                if (agvCalRoute.avoidStop[i][2] == 0)
                    //                {
                    //                    if (!simInt)
                    //                    {
                    //                        for (int j = 0; j < 3; j++)
                    //                        {
                    //                            (MainInfo.listAgvDll[comNo]).StopAGV(MainInfo.listPtr[comNo], groupNo, agvNum);
                    //                            Thread.Sleep(50);
                    //                        }
                    //                        lock (agvCalRoute.avoidStop)
                    //                        {
                    //                            agvCalRoute.avoidStop[i][2] = 1;
                    //                        }

                    //                        //指令发送成功，由于多线程操作，agv状态还未查询更新，手工置状态未为停止状态
                    //                        for (int j = 0; j < carStatusList.Count; j++)
                    //                        {
                    //                            if (carStatusList[j].carNum == agvCalRoute.avoidStop[i][1])
                    //                            {
                    //                                lock (carStatusList)
                    //                                {
                    //                                    carStatusList[j].IsRunning = false;
                    //                                }
                    //                            }
                    //                        }
                    //                        richTextBoxLog.BeginInvoke(delegateRichText, new object[] { DateTime.Now.ToString() + " Agv:" + agvStr + "stopAvoid" });
                    //                    }
                    //                    else
                    //                    {
                    //                        SimAgvInfo.writeRunning(agvStr, 2);
                    //                        lock (agvCalRoute.avoidStop)
                    //                        {
                    //                            agvCalRoute.avoidStop[i][2] = 1;
                    //                        }

                    //                        //指令发送成功，由于多线程操作，agv状态还未查询更新，手工置状态未为停止状态
                    //                        foreach (CarStatus cs in carStatusList)
                    //                        {
                    //                            if (cs.carNum == agvCalRoute.avoidStop[i][1])
                    //                            {
                    //                                lock (carStatusList)
                    //                                {
                    //                                    cs.IsRunning = false;
                    //                                }
                    //                            }
                    //                        }
                    //                        richTextBoxLog.BeginInvoke(delegateRichText, new object[] { DateTime.Now.ToString() + " Agv:" + agvStr + "stopAvoid" });
                    //                    }
                    //                }
                    //                else if (agvCalRoute.avoidStop[i][2] == 2)
                    //                {
                    //                    if (!simInt)
                    //                    {
                    //                        for (int j = 1; j < 2; j++)
                    //                        {
                    //                            if ((MainInfo.listAgvDll[comNo]).StartAGV(MainInfo.listPtr[comNo], groupNo, agvNum) == 1)
                    //                            {
                    //                                lock (agvCalRoute.avoidStop)
                    //                                {
                    //                                    agvCalRoute.avoidStop[i][1] = 0;
                    //                                    agvCalRoute.avoidStop[i][2] = 0;
                    //                                }
                    //                                richTextBoxLog.BeginInvoke(delegateRichText, new object[] { DateTime.Now.ToString() + " Agv:" + agvStr + "startAvoid" });
                    //                                break;
                    //                            };
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        SimAgvInfo.writeRunning(agvStr, 1);
                    //                        richTextBoxLog.BeginInvoke(delegateRichText, new object[] { DateTime.Now.ToString() + " Agv:" + agvStr + "startAvoid" });
                    //                        lock (agvCalRoute.avoidStop)
                    //                        {
                    //                            agvCalRoute.avoidStop[i][1] = 0;
                    //                            agvCalRoute.avoidStop[i][2] = 0;
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    #endregion 避让指令发送


                    //下发路径
                    for (int i = 0; i < comAgvCount; i++)
                    {
                        int agvNo = int.Parse(comAgv[i]);//agv车号
                        if (calR.agvInfoDic[agvNo].agvActive)
                        {
                            SendRotueActive(calR.agvInfoDic[agvNo].agvRfidList, ComNo, groupNo, agvNo);
                        }
                    }

                    //用于启动buffer车辆
                    for (int i = 0; i < comAgvCount; i++)
                    {
                        int agvNo = int.Parse(comAgv[i]);//agv车号
                        if (calR.agvInfoDic[agvNo].agvBufferStart == 1)
                        {
                            if (!realBool)//不模拟
                            {
                                agvDll.StartAGV(new IntPtr(0), groupNo, agvNo);
                                string logStr = "AGV" + agvNo + ": " + "BufferStart, " + "Rfid " + calR.agvInfoDic[agvNo].rfid;
                                TxtLogShowInfo(logStr, true); //写日志
                            }
                            else
                            {
                                string logStr = "AGV" + agvNo + ": " + "BufferStart, " + "Rfid " + calR.agvInfoDic[agvNo].rfid;
                                TxtLogShowInfo(logStr, false); //写日志
                            }

                            lock (calR.agvInfoDic[agvNo])//启动成功标志位置0
                            {
                                calR.agvInfoDic[agvNo].agvBufferStart = 0;
                            }
                        }
                    }

                    //用于启动所有车辆
                    for (int i = 0; i < comAgvCount; i++)
                    {
                        int agvNo = int.Parse(comAgv[i]);//agv车号
                        if (calR.agvInfoDic[agvNo].agvTimerStart == 1)
                        {
                            if (!realBool)//不模拟
                            {
                                agvDll.StartAGV(new IntPtr(0), groupNo, agvNo);
                                string logStr = "AGV" + agvNo + ": " + "TimerStart, " + "Rfid " + calR.agvInfoDic[agvNo].rfid;
                                TxtLogShowInfo(logStr, true); //写日志
                            }
                            else
                            {
                                string logStr = "AGV" + agvNo + ": " + "TimerStart, " + "Rfid " + calR.agvInfoDic[agvNo].rfid;
                                TxtLogShowInfo(logStr, false); //写日志
                            }

                            lock (calR.agvInfoDic[agvNo])//启动成功标志位置0
                            {
                                calR.agvInfoDic[agvNo].agvTimerStart = 0;
                            }
                        }
                    }

                    if (comAgvCount > 1)
                    {
                        Thread.Sleep(50);
                    }
                    else
                    {
                        Thread.Sleep(300);
                    }
                }
            }
        }

        #endregion

        #region 下发RFID程序

        private void SendRotueActive(List<RFIDClass> RfidClassList, int ComNo, int groupNo, int AgvNum)
        {
            //下发RFID程序
            //DelegateRichText delegateRichText = new DelegateRichText(RichTextString);
            AGVDLL.AGVDLL agvDll = PortInfo.AgvPortsList[ComNo].AgvDll;

            if (calR.agvInfoDic[AgvNum].agvSendResult[0] != -1)
            {
                bool result = true;

                if (!realBool)//不模拟
                {
                    //调路径
                    if (calR.agvInfoDic[AgvNum].sendRfid == 1)
                    {

                        string logStr = "AGV" + AgvNum + "\r\n" + "RouteNo:  " + calR.agvInfoDic[AgvNum].routeNo + "\r\n" + "RouteName:  " + calR.agvInfoDic[AgvNum].agvRouteName + "\r\n";
                        TxtLogShowInfo(logStr, true); //写日志

                        for (int i = 0; i < 2; i++)
                        {
                            if (agvDll.readFlashRFID(new IntPtr(0), groupNo, AgvNum, calR.agvInfoDic[AgvNum].agvRouteNo) == calR.agvInfoDic[AgvNum].agvRouteNo)
                            {
                                break;
                            }
                            else
                            {
                                result = false;
                            }
                        }

                        if (calR.agvInfoDic[AgvNum].agvSendResult[1] == -1 && calR.agvInfoDic[AgvNum].agvSendResult[2] == 0)
                        {
                            Thread.Sleep(500);
                            //agv状态正常而路径调取失败,重新查询agv状态
                            for (int i = 0; i < CarStatusDic.Count; i++)
                            {
                                if (AgvNum == CarStatusDic[i].carNum)
                                {
                                    CarStatus cs = agvDll.read(new IntPtr(0), groupNo, AgvNum);
                                    if (cs.errorCode != 205)
                                    {
                                        CarStatusDic[i].agvRouteNo = cs.agvRouteNo;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    else if (calR.agvInfoDic[AgvNum].sendRfid == 2)
                    {
                        //rfid 逐个下发
                        for (int i = calR.agvInfoDic[AgvNum].agvSendResult[0]; i < RfidClassList.Count; i++)
                        {
                            if (agvDll.sendRFID(new IntPtr(0), groupNo, AgvNum, RfidClassList[i]) == 0)
                            {
                                calR.agvInfoDic[AgvNum].agvSendResult[0] = i;
                                result = false;
                                break;
                            }
                            Thread.Sleep(50);

                            string logStr = "AGV" + AgvNum + ": sendRfid" + "\r\n";
                            logStr += "rfid" + RfidClassList[i].rfidID + "\r\n";
                            logStr += "hook" + RfidClassList[i].hookSwitch + "\r\n";
                            logStr += "dire" + RfidClassList[i].directionSwitch + "\r\n";
                            logStr += "turn" + RfidClassList[i].trunSwitch + "\r\n";
                            logStr += "run" + RfidClassList[i].startSwitch + "\r\n";
                            logStr += "speed" + RfidClassList[i].speedNo + "\r\n";
                            logStr += "pbs" + RfidClassList[i].pbsNo + "\r\n";
                            logStr += "program" + RfidClassList[i].programNo + "\r\n";
                            logStr += "reset" + RfidClassList[i].restSwitch + "\r\n";

                            TxtLogShowInfo(logStr, true); //写日志
                        }
                    }
                }
                else
                {
                    SimAgvInfo.writeRouteNo(AgvNum.ToString(), calR.agvInfoDic[AgvNum].agvRouteNo.ToString());

                    if (calR.agvInfoDic[AgvNum].sendRfid == 2)
                    {
                        //rfid 逐个下发
                        for (int i = calR.agvInfoDic[AgvNum].agvSendResult[0]; i < RfidClassList.Count; i++)
                        {
                            Thread.Sleep(50);

                            string log = string.Empty;
                            log += DateTime.Now.ToString() + "\r\n";
                            log += "AGV:" + AgvNum + "\r\n";
                            log += "rfid" + RfidClassList[i].rfidID + "\r\n";
                            log += "hook" + RfidClassList[i].hookSwitch + "\r\n";
                            log += "dire" + RfidClassList[i].directionSwitch + "\r\n";
                            log += "turn" + RfidClassList[i].trunSwitch + "\r\n";
                            log += "speed" + RfidClassList[i].speedNo + "\r\n";
                            log += "pbs" + RfidClassList[i].pbsNo + "\r\n";
                            log += "program" + RfidClassList[i].programNo + "\r\n";
                            log += "reset" + RfidClassList[i].restSwitch + "\r\n";
                            TxtLogShowInfo(log, true);
                            //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { DateTime.Now.ToString() });
                            //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "AGV:" + AgvNum });
                            //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "rfid" + RfidClassList[i].rfidID });
                            //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "hook" + RfidClassList[i].hookSwitch });
                            //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "dire" + RfidClassList[i].directionSwitch });
                            //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "turn" + RfidClassList[i].trunSwitch });
                            //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "run" + RfidClassList[i].startSwitch });
                            //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "speed" + RfidClassList[i].speedNo });
                            //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "pbs" + RfidClassList[i].pbsNo });
                            //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "program" + RfidClassList[i].programNo });
                            //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "reset" + RfidClassList[i].restSwitch });
                            //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "" });
                        }
                    }
                }

                if (result)
                {

                    string logStr = "AGV" + AgvNum + "\r\n " + "RouteNo " + calR.agvInfoDic[AgvNum].routeNo + " GetSuccess!\r\n";
                    TxtLogShowInfo(logStr, true); //写日志

                    lock (calR.agvInfoDic[AgvNum])
                    {
                        calR.agvInfoDic[AgvNum].agvSendResult[0] = -1;
                    }
                }
            }

            //检查agv状态
            if (calR.agvInfoDic[AgvNum].agvSendResult[0] == -1 && calR.agvInfoDic[AgvNum].agvSendResult[1] != -1)
            {
                if (!realBool)
                {
                    CarStatus cs = CarStatusDic[AgvNum];
                    RFIDClass rfc = new RFIDClass();
                    rfc.rfidID = 999;

                    //MasterOn
                    if (!cs.agvRunReady)
                    {
                        rfc.masterSwitch = 1;
                    }
                    else
                    {
                        rfc.masterSwitch = 0;
                    }

                    //hook
                    if ((cs.agvHookUP && RfidClassList[0].hookSwitch == 0) || (!cs.agvHookUP && RfidClassList[0].hookSwitch == 1))
                    {
                        rfc.hookSwitch = 1;
                    }
                    else
                    {
                        rfc.hookSwitch = 0;
                    }

                    //方向
                    if ((cs.agvRunDirection && RfidClassList[0].directionSwitch == 1) || (!cs.agvDriverDown && RfidClassList[0].directionSwitch == 0))
                    {
                        rfc.directionSwitch = 1;
                    }
                    else
                    {
                        rfc.directionSwitch = 0;
                    }

                    //转弯
                    switch (RfidClassList[0].trunSwitch)
                    {
                        case 0:
                            rfc.trunSwitch = 3;
                            break;

                        case 1:
                            rfc.trunSwitch = 1;
                            break;

                        case 2:
                            rfc.trunSwitch = 2;
                            break;

                        default:
                            rfc.trunSwitch = 3;
                            break;
                    }

                    //速度
                    rfc.speedNo = RfidClassList[0].speedNo;

                    //pbs
                    rfc.pbsNo = RfidClassList[0].pbsNo;

                    //program
                    rfc.programNo = RfidClassList[0].programNo;

                    rfc.startSwitch = 0;

                    rfc.stopMode = 0;

                    for (int i = 0; i < 2; i++)
                    {
                        if (agvDll.sendAllCommand(new IntPtr(0), groupNo, AgvNum, rfc) == 1)
                        {
                            lock (calR.agvInfoDic[AgvNum].agvSendResult)
                            {
                                calR.agvInfoDic[AgvNum].agvSendResult[1] = -1;
                            }

                            string logStr = "AGV" + AgvNum + ": rfidCommand/r/n";
                            logStr += "rfid " + rfc.rfidID + "/r/n";
                            logStr += "hook " + rfc.hookSwitch + "/r/n";
                            logStr += "dire " + rfc.directionSwitch + "/r/n";
                            logStr += "turn " + rfc.trunSwitch + "/r/n";
                            logStr += "run " + rfc.startSwitch + "/r/n";
                            logStr += "speed" + rfc.speedNo + "/r/n";
                            logStr += "pbs " + rfc.pbsNo + "/r/n";
                            logStr += "program " + rfc.programNo + "/r/n";
                            logStr += "reset " + rfc.restSwitch + "/r/n";

                            TxtLogShowInfo(logStr, true); //写日志
                            Thread.Sleep(50);
                            break;
                        }
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        Thread.Sleep(50);
                        if (agvDll.RestAGV(new IntPtr(0), groupNo, AgvNum) == 1)
                            break;
                    }
                    if (calR.agvInfoDic[AgvNum].agvSendResult[1] == -1 && calR.agvInfoDic[AgvNum].agvSendResult[2] == 0)
                    {
                        Thread.Sleep(1500);
                        //如果检查agv状态和启动指令连续发送，重新查询agv状态
                        CarStatus reCs = agvDll.read(new IntPtr(0), groupNo, AgvNum);
                        if (reCs.errorCode != 205)
                        {
                            lock (CarStatusDic)
                            {
                                reCs.carNum = AgvNum;
                                CarStatusDic[AgvNum] = reCs;
                            }
                            string logStr = "AGV" + AgvNum + ": checkStatus/r/n";
                            TxtLogShowInfo(logStr, true); //写日志
                        }
                        Thread.Sleep(100);
                    }
                }
                else  //模拟
                {

                    SimAgvInfo.writeDire(AgvNum.ToString(), RfidClassList[0].directionSwitch);
                    SimAgvInfo.writeHook(AgvNum.ToString(), RfidClassList[0].hookSwitch);

                    lock (calR.agvInfoDic[AgvNum].agvSendResult)
                    {
                        calR.agvInfoDic[AgvNum].agvSendResult[1] = -1;
                    }
                    //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { DateTime.Now.ToString() });
                    //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "agvAllCommand:" + AgvNum });
                    //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "" });
                    string log = $"{DateTime.Now.ToString()} \r\n agvAllCommand:{AgvNum} \r\n";
                    TxtLogShowInfo(log, false);

                    if (calR.agvInfoDic[AgvNum].agvSendResult[1] == -1 && calR.agvInfoDic[AgvNum].agvSendResult[2] == 0)
                    {
                        //for (int i = 0; i < CarStatusDic.Count; i++)
                        //{
                        //    if (AgvNum == CarStatusDic[i].carNum)
                        //    {
                        //        CarStatus cs = SimAgvInfo.simInfo(AgvNum.ToString());
                        //        //cs.carNum = 999;
                        //        CarStatusDic[i] = cs;
                        //        break;
                        //    }
                        //}
                        foreach (var item in CarStatusDic.Keys)
                        {
                            if (AgvNum == CarStatusDic[item].carNum)
                            {
                                CarStatus cs = SimAgvInfo.simInfo(AgvNum.ToString());
                                //cs.carNum = 999;
                                CarStatusDic[item] = cs;
                                break;
                            }
                        }
                    }
                }
            }


            if (calR.agvInfoDic[AgvNum].agvSendResult[0] == -1 && calR.agvInfoDic[AgvNum].agvSendResult[1] == -1 && calR.agvInfoDic[AgvNum].agvSendResult[2] == 0)
            {
                bool checkAgvStatus = true;//agv状态是否正常的标志位

                //检查agv状态是否和执行的状态一致   
                CarStatus cs = CarStatusDic[AgvNum];
                CarStatus csCheck = new CarStatus();
                csCheck.agvRunDirection = RfidClassList[0].directionSwitch == 0 ? true : false;
                csCheck.agvHookUP = RfidClassList[0].hookSwitch == 0 ? false : true;

                //检查agv路径调的是否正确
                if (calR.agvInfoDic[AgvNum].sendRfid == 1)
                {
                    if (cs.agvRouteNo != calR.agvInfoDic[AgvNum].agvRouteNo)
                    {
                        checkAgvStatus = false;
                        lock (calR.agvInfoDic[AgvNum].agvSendResult)
                        {
                            calR.agvInfoDic[AgvNum].agvSendResult[0] = 0;
                        }
                    }
                }

                //检车agv状态是否正确
                if (cs.agvRunDirection != csCheck.agvRunDirection || cs.agvHookUP != csCheck.agvHookUP)
                {
                    checkAgvStatus = false;
                    lock (calR.agvInfoDic[AgvNum].agvSendResult)
                    {
                        calR.agvInfoDic[AgvNum].agvSendResult[1] = 0;
                    }
                }

                //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { DateTime.Now.ToString() });
                //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "agvCheck:" + AgvNum + " checkDire: " + csCheck.agvRunDirection + "agvDire: " + cs.agvRunDirection });
                //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "agvCheck:" + AgvNum + " checkHook: " + csCheck.agvHookUP + "agvHook: " + cs.agvHookUP });
                //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "" });

                string logStr = "AGV" + AgvNum + ": CommandStatus/r/n";
                logStr += "AgvDire: " + csCheck.agvRunDirection + ", CommandDire:" + cs.agvRunDirection + "/r/n";
                logStr += "AgvHook: " + csCheck.agvHookUP + ", CommandHook:" + cs.agvHookUP + "/r/n";
                TxtLogShowInfo(logStr, true); //写日志

                if (!realBool && checkAgvStatus)//非模拟
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (agvDll.StartAGV(new IntPtr(0), groupNo, AgvNum) == 1)
                        {
                            lock (CarStatusDic)
                            {
                                CarStatusDic[AgvNum].IsRunning = true;//一旦启动成工默认两秒为运行状态
                            }
                            string logStr1 = "AGV" + AgvNum + "/r/n";
                            logStr1 += "Start Sucessful!+/r/n";
                            TxtLogShowInfo(logStr, true); //写日志

                            lock (calR.agvInfoDic[AgvNum].agvSendResult)
                            {
                                calR.agvInfoDic[AgvNum].agvSendResult[2] = -1;
                            }
                            lock (calR.agvInfoDic[AgvNum])
                            {
                                calR.agvInfoDic[AgvNum].pcAgvStart = 1;
                            }
                            Thread th = new Thread(PcStartTh);
                            th.Start(AgvNum);
                            Thread.Sleep(100);
                            break;
                        }
                        else
                        {   //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { DateTime.Now.ToString() });
                            //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "Agv:" + AgvNum + " Start:0" });
                            //richTextBoxLog.BeginInvoke(delegateRichText, new object[] { "" });

                            string logStr1 = "AGV" + AgvNum + "/r/n";
                            logStr1 += "Start Error!+/r/n";
                            TxtLogShowInfo(logStr, true); //写日志
                        }
                    }
                }
                else if (checkAgvStatus)
                {
                    //for (int i = 0; i < CarStatusDic.Count; i++)//一旦启动成工默认两秒为运行状态
                    //{
                    //    if (CarStatusDic[i].carNum == AgvNum)
                    //    {
                    //        lock (CarStatusDic)
                    //        {
                    //            CarStatusDic[i].IsRunning = true;
                    //        }
                    //    }
                    //}
                    foreach (var item in CarStatusDic.Keys)
                    {
                        if (CarStatusDic[item].carNum == AgvNum)
                        {
                            lock (CarStatusDic)
                            {
                                CarStatusDic[item].IsRunning = true;
                            }
                        }
                    }

                    lock (calR.agvInfoDic[AgvNum].agvSendResult)
                    {
                        calR.agvInfoDic[AgvNum].agvSendResult[2] = -1;
                    }

                    string Log = $"{DateTime.Now.ToString()}\r\n Agv： {AgvNum} Start:1 \r\n";

                    TxtLogShowInfo(Log, true); //写日志
                    SimAgvInfo.writeRunning(AgvNum.ToString(), 1);
                }
            }

            //发送成功 把线路名称写入发送成功的路径名list。并把需要发送命令的标志位置为false
            if (calR.agvInfoDic[AgvNum].agvSendResult[2] == -1)
            {
                lock (calR.agvInfoDic[AgvNum])
                {
                    calR.agvInfoDic[AgvNum].sendRouteName = calR.agvInfoDic[AgvNum].agvRouteName;
                    calR.agvInfoDic[AgvNum].sendRouteNo = calR.agvInfoDic[AgvNum].agvRouteNo;

                    if (realBool)
                    {
                        SimAgvInfo.writeStatus(AgvNum.ToString(), 1);
                        SimAgvInfo.writeRunning(AgvNum.ToString(), 1);
                    }
                    string taskName = calR.agvInfoDic[AgvNum].agvTask;
                    calR.agvInfoDic[AgvNum].sendTask = taskName;
                    calR.agvInfoDic[AgvNum].agvActive = false;

                    try
                    {
                        //线路发送成功，把发送成功的线路写入数据库
                        GetBLL.LogRouteInfoBLL(AgvNum, new string[] { calR.agvInfoDic[AgvNum].sendRouteName }, calR.agvInfoDic[AgvNum].sendTask);
                    }
                    catch (Exception ex)
                    {
                        Log4NetHelper.WriteErrorLog(ex.Message.ToString(), ex);
                    }
                }
            }
        }
        #endregion

        #region 日志输出

        /// <summary>
        /// 1.写入日志文件，0不写入
        /// </summary>
        /// <param name="MessageStr"></param>
        /// <param name="IsLog"></param>
        private void TxtLogShowInfo(string MessageStr, bool IsLog)
        {
            txtLog.Dispatcher.BeginInvoke(new Action(() =>
            {
                //大于10000行清除记录，可选
                if (txtLog.LineCount > 10000)
                {
                    txtLog.Clear();
                }
                txtLog.AppendText($"{DateTime.Now}\n" + MessageStr + "\r\n\r\n");
                txtLog.ScrollToEnd();
            }));
            if (IsLog == true)
            {
                Log4NetHelper.WriteCommandLog(MessageStr);
            }
        }
        #endregion

        #region 下发路径

        /// <summary>
        /// 下发路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Issue_Line_Click(object sender, RoutedEventArgs e)
        {
            Open_Port.IsEnabled = false;
            StartBrainpower.IsEnabled = false;
            Issue_Line.IsEnabled = false;
            while (true)
            {
                bool IsRun = true;
                for (int i = 0; i < PathTask.Count; i++)
                {
                    if (PathTask[i].Status == TaskStatus.Running)
                    {
                        PathLine.Cancel();
                        IsRun = false;
                    }
                }
                if (IsRun)
                {
                    break;
                }
            }
            PathLine = new CancellationTokenSource();
            PathTask.Add(Task.Factory.StartNew(() => { PathIssued(); }, PathLine.Token));
        }

        /// <summary>
        /// 下发路径
        /// </summary>
        public void PathIssued()
        {
            DataTable Table = MapBLL.BLLMapRouteTable(MapRegulate.UTCTime.ToString()); //查询出所有线路
            List<RFIDLineList> ListRFIDClass = new List<RFIDLineList>();
            int RfidCount = 0; //总rfid个数
            int agvNum = 0;  //总AGV个数
            foreach (DataRow Row in Table.Rows)
            {
                RFIDLineList rFIDLine = new RFIDLineList()
                {
                    GetRFIDs = DrToRfidList(Row),
                    Program = Convert.ToInt32(Row["Program"])
                };
                ListRFIDClass.Add(rFIDLine);
                RfidCount += rFIDLine.GetRFIDs.Select(p => p).Count();
            }
            for (int i = 0; i < PortInfo.AgvPortsList.Count(); i++)
            {
                agvNum += PortInfo.AgvPortsList[i].AgvStr.Split(',').Count();
            }
            int Count = agvNum * RfidCount; //总任务数
            this.Dispatcher.Invoke(new Action(() => { Schedule.Maximum = Count; })); //设置进度条刻度大小
            for (int i = 0; i < PortInfo.AgvPortsList.Count(); i++)
            {
                if (PathLine.IsCancellationRequested)
                {
                    break;
                }
                int Guoup = i + 1;
                string[] comAgv = PortInfo.AgvPortsList[i].AgvStr.Split(','); //找出所有AGV
                for (int s = 0; s < comAgv.Length; s++)
                {
                    if (PathLine.IsCancellationRequested)
                    {
                        break;
                    }
                    foreach (RFIDLineList RFIDClass in ListRFIDClass)
                    {
                        if (PathLine.IsCancellationRequested)
                        {
                            break;
                        }
                        List<RFIDClass> rfidclass = RFIDClass.GetRFIDs;
                        foreach (var rfid in rfidclass)
                        {
                            if(PathLine.IsCancellationRequested)
                            {
                                break;
                            }
                            while (true)
                            {
                                if (PathLine.IsCancellationRequested)
                                {
                                    break;
                                }
                                if (PortInfo.AgvPortsList[i].AgvDll.sendFlashRFID(new IntPtr(0), Guoup, Convert.ToInt32(comAgv[s]), RFIDClass.Program, rfid, rfidclass.Count()) != 1)
                                {
                                    string agvRfidStr = "AGV:" + comAgv[s] + " RouteNo:" + RFIDClass.Program + " Rfid:" + rfid.rfidID + " type: Successful";
                                    TxtLogShowInfo(agvRfidStr, false);
                                    this.Dispatcher.Invoke(() => { Schedule.Value += 1; var values = Schedule.Value; scale.Content = ((values / Count) * 100).ToString("f2") + "%"; });
                                    break;
                                }
                            }
                            Thread.Sleep(100);
                        }
                    }
                }
            }
            if(!PathLine.IsCancellationRequested)
            this.Dispatcher.Invoke(() => { MessageBoxTool.Succeed("路径下发完成！"); Schedule.Value = 0; scale.Content = "0%"; Open_Port.IsEnabled = true; StartBrainpower.IsEnabled = true; Issue_Line.IsEnabled = true; });
        }

        /// <summary>
        /// 取消下发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Issueoff_Click(object sender, RoutedEventArgs e)
        {
            PathLine.Cancel();
            Thread.Sleep(200);
            for (int s = 0; s < 3; s++)
            {
                bool IsRun = true;
                for (int i = 0; i < PathTask.Count; i++)
                {
                    if (PathTask[i].Status == TaskStatus.Running)
                    {
                        PathLine.Cancel();
                        IsRun = false;
                    }
                }
                if (IsRun)
                {
                    break;
                }
            }
            Open_Port.IsEnabled = true;
            StartBrainpower.IsEnabled = true;
            Issue_Line.IsEnabled = true;
            Schedule.Value = 0;
            scale.Content = "0%";
        }

        /// <summary>
        /// RFIDClass
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public List<RFIDClass> DrToRfidList(DataRow dr)
        {
            List<RFIDClass> rfidList = new List<RFIDClass>();
            List<string> rfid = dr[4].ToString().Split(',').ToList<string>();
            List<string> speed = dr[5].ToString().Split(',').ToList<string>();
            List<string> stop = dr[6].ToString().Split(',').ToList<string>();
            List<string> turn = dr[7].ToString().Split(',').ToList<string>();
            List<string> direction = dr[8].ToString().Split(',').ToList<string>();
            List<string> pbs = dr[9].ToString().Split(',').ToList<string>();
            List<string> hook = dr[11].ToString().Split(',').ToList<string>();
            List<string> ChangeProgram = dr[12].ToString().Split(',').ToList<string>();

            RFIDClass rf0 = new RFIDClass();
            rf0.rfidID = 0;
            rf0.startSwitch = 0;
            rf0.restSwitch = 0;
            rf0.masterSwitch = 0;
            rf0.hookSwitch = int.Parse(hook[0]);
            rf0.directionSwitch = int.Parse(direction[0]);
            rf0.programNo = int.Parse(ChangeProgram[0]);
            rf0.speedNo = (speed[0] == "10" ? 999 : int.Parse(speed[0]));
            rf0.pbsNo = (pbs[0] == "16" ? 999 : int.Parse(pbs[0]));
            rf0.stopMode = 0;
            rf0.stopTime = 0;
            rf0.trunSwitch = int.Parse(turn[0]);
            rfidList.Add(rf0);

            for (int j = 1; j < rfid.Count; j++)
            {
                if (stop[j] == "-99")
                {
                    RFIDClass rf = new RFIDClass();
                    rf.rfidID = int.Parse(rfid[j]);
                    rf.startSwitch = 2;
                    rf.restSwitch = 0;
                    rf.masterSwitch = 0;
                    rf.hookSwitch = (hook[j] == "2" ? 0 : 1);
                    rf.directionSwitch = (direction[j] == "2" ? 0 : 1);
                    rf.programNo = 999;
                    rf.speedNo = (speed[j] == "10" ? 999 : int.Parse(speed[j]));
                    rf.pbsNo = (pbs[j] == "16" ? 999 : int.Parse(pbs[j]));
                    rf.stopMode = 0;
                    rf.stopTime = 0;
                    rf.trunSwitch = int.Parse(turn[j]);
                    rfidList.Add(rf);
                }
                else if (hook[j] == "2" && direction[j] == "2" && ChangeProgram[j] == "999")
                {
                    RFIDClass rf = new RFIDClass();
                    rf.rfidID = int.Parse(rfid[j]);
                    rf.startSwitch = 0;
                    rf.restSwitch = 0;
                    rf.masterSwitch = 0;
                    rf.hookSwitch = 0;
                    rf.directionSwitch = 0;
                    rf.programNo = 999;
                    rf.speedNo = (speed[j] == "10" ? 999 : int.Parse(speed[j]));
                    rf.pbsNo = (pbs[j] == "16" ? 999 : int.Parse(pbs[j]));
                    rf.stopMode = 0;
                    rf.stopTime = int.Parse(stop[j]);
                    if (rf.stopTime != 0)
                    {
                        rf.startSwitch = 2;
                    }
                    rf.trunSwitch = int.Parse(turn[j]);
                    rfidList.Add(rf);
                }
                else
                {
                    //有升hook或者反向 停止动作 
                    RFIDClass rf = new RFIDClass();
                    rf.rfidID = int.Parse(rfid[j]);

                    rf.startSwitch = 2;
                    rf.restSwitch = 0;
                    rf.masterSwitch = 0;
                    rf.hookSwitch = (hook[j] == "2" ? 0 : 1);
                    rf.directionSwitch = (direction[j] == "2" ? 0 : 1);
                    rf.programNo = 999;
                    rf.speedNo = 999;//(speed[j] == "10" ? 999 : int.Parse(speed[j]));
                    rf.pbsNo = (pbs[j] == "16" ? 999 : int.Parse(pbs[j]));
                    rf.stopMode = 0;
                    rf.trunSwitch = 0;
                    rfidList.Add(rf);

                    //转弯 启动动作 不是最后指令
                    if (j != rfid.Count - 1)
                    {
                        RFIDClass rf1 = new RFIDClass();
                        rf1.rfidID = int.Parse(rfid[j]);
                        rf1.startSwitch = 1;
                        rf1.restSwitch = 0;
                        rf1.masterSwitch = 0;
                        rf1.hookSwitch = 0;
                        rf1.directionSwitch = 0;
                        rf1.programNo = int.Parse(ChangeProgram[j]);
                        rf1.speedNo = (speed[j] == "10" ? 999 : int.Parse(speed[j]));
                        rf1.pbsNo = (pbs[j] == "16" ? 999 : int.Parse(pbs[j]));
                        rf1.stopMode = 0;
                        rf1.trunSwitch = int.Parse(turn[j]);
                        rfidList.Add(rf1);
                    }
                }
            }
            rfidList[rfidList.Count - 1].startSwitch = 2;
            rfidList[0].rfidID = 0;
            return rfidList;
        }
        #endregion

        private void PcStartTh(object i)
        {
            Thread.Sleep(2500);
            lock (calR.agvInfoDic[(int)i])
            {
                calR.agvInfoDic[(int)i].pcAgvStart = 0;
            }
        }

    }
}
