using AGVSystem.APP.agv_Map;
using AGVSystem.Infrastructure.agvCommon;
using AGVSystem.Model.DrawMap;
using AGVSystem.Model.Ga_agvModels;
using AGVSystem.Model.LogicData;
using AGVSystem.Model.MapData;
using AGVSystem.UI.APP_UI.Setting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AGVSystem.UI.APP_UI.Map
{
    /// <summary>
    /// EditLine.xaml 的交互逻辑
    /// </summary>
    public partial class EditLine : Window
    {
        private MapInstrument map = new MapInstrument();
        private agvMapRegulate mapService = new agvMapRegulate();
        private double CanvasWidth, CanvasHeight; //初始宽高
        private ObservableCollection<Ga_Map> MapList = new ObservableCollection<Ga_Map>();
        private ObservableCollection<Route> GetRoutes = new ObservableCollection<Route>();
        private Route RouteData = new Route();
        private ObservableCollection<Route> routes = new ObservableCollection<Route>();
        private List<string> associatedTag = new List<string>(); //关联Tag集合
        private long UTCTime;
        private bool edit = true;

        public EditLine(ObservableCollection<Ga_Map> Map)
        {
            InitializeComponent();
            Setting_Map();
        }
        /// <summary>
        /// 绑定数据
        /// </summary>
        public void Setting_Map()
        {
            MapList = mapService.GetMapRegulate(); ;
            MapMenu.ItemsSource = MapList;
            if (MapList.Count() > 0)
            {
                LoadMap(MapList[0]);
                if (OperateIniTool.Exist)
                {
                    string Size = OperateIniTool.OperateIniRead("AGV", "MapSise");
                    map.MapSise = Convert.ToDouble(Size);
                }
            }
        }

        public void LoadMap(Ga_Map GetMap)
        {
            CanvasWidth = GetMap.Width * 10;
            CanvasHeight = GetMap.Height * 10;
            double CanvasWidths = GetMap.Width * 10 * map.MapSise;
            double CanvasHeights = GetMap.Height * 10 * map.MapSise;
            map.Initial_Canvas(TopX, TopY, mainPanel, CanvasWidths, CanvasHeights);
            UTCTime = UTC.ConvertDateTimeLong(Convert.ToDateTime(GetMap.CreateTime));
            map.LoadEditMap(UTCTime, false, true);
            map.valuePairs.Select(p => p.Value).ToList().ForEach(p => { p.MouseDown += Value_MouseDown; p.Cursor = Cursors.Hand; });
            GetRoutes = mapService.GetrouteList(UTCTime.ToString());
            Line.ItemsSource = GetRoutes;
            Line.SelectedIndex = 0;
        }



        #region 添加线路点

       
        /// <summary>
        /// 添加线路点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Value_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Label tag = (Label)sender;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (routes.Count().Equals(0) || (associatedTag.Where(x => x.Equals(tag.Tag.ToString())).Count() >= 1) || (edit == true && (associatedTag.Where(x => x.Equals(tag.Tag.ToString())).Count() >= 1)))
                {
                    AssociatedTag(Convert.ToInt32(tag.Tag)); //标记关联Tag为绿色
                    int TagPoint = routes.Count() > 0 ? Convert.ToInt32(routes[routes.Count() - 1].Tag) : 0; //起始Tag（表格最后一个Tag）
                    int TagWirePoint = Convert.ToInt32(tag.Tag);  //结束Tag（点击Tag）
                    List<WirePointArray> wires = map.wirePointArrays.Where(p => (   //查找关联线路
                     p.GetPoint.TagID.Equals(TagPoint) && p.GetWirePoint.TagID.Equals(TagWirePoint)) || (p.GetPoint.TagID.Equals(TagWirePoint) && p.GetWirePoint.TagID.Equals(TagPoint))).ToList();
                    map.SignLine(wires, Brushes.Red, 8,999);  //关联线路标记为红色
                    routes.Add(NewRoute(new Route() //添加线路
                    {
                        Stop = "0",
                        Tag = tag.Tag.ToString(),
                        ChangeProgram = "999",
                        Pbs = (MainInfo.agvPbs.Length - 1).ToString(),
                        Speed = (MainInfo.agvSpeed.Length - 1).ToString(),
                        Turn = "0",
                        Hook = (MainInfo.agvHook.Length - 1).ToString(),
                        Direction = (MainInfo.agvDire.Length - 1).ToString(),
                    }));

                    #region 计算方向

                    if (MapRegulate.DirectionLine == 1)  //是否开启方向自动计算
                    {
                        if (routes.Count() > 0 && wires.Count() > 0 && wires[0].circuitType == CircuitType.Broken)
                        {
                            Point startPt = new Point() { X = map.valuePairs[TagPoint].Margin.Left - 19, Y = map.valuePairs[TagPoint].Margin.Top - 11.5 };
                            Point endPt = new Point() { X = map.valuePairs[TagWirePoint].Margin.Left - 19, Y = map.valuePairs[TagWirePoint].Margin.Top - 11.5 };

                            //map.Draw_Triangle(startPt,endPt);


                            Point X = wires[0].GetPoint.SetPoint;
                            Point Y = wires[0].GetWirePoint.SetPoint; ;

                            double XP = (X.X - Y.X);
                            double YP = (X.Y - Y.Y);

                            double LineStart = (startPt.X - endPt.X);
                            double endStart = (startPt.Y - endPt.Y);

                            int Counts = routes.Count;

                            if ((LineStart < 0 && endStart > 0))
                            {
                                if (XP < 0 && YP > 0)
                                {
                                    routes[Counts - 2].Turn = "左转";
                                }
                                else
                                {
                                    routes[Counts - 2].Turn = "右转";
                                }
                            }
                            else if (LineStart > 0 && endStart > 0)
                            {
                                if (XP > 0 && YP > 0)
                                {
                                    routes[Counts - 2].Turn = "右转";
                                }
                                else
                                {
                                    routes[Counts - 2].Turn = "左转";
                                }
                            }
                            else if (LineStart > 0 && endStart < 0)
                            {

                                if (XP > 0 && YP < 0)
                                {
                                    routes[Counts - 2].Turn = "左转";
                                }
                                else
                                {
                                    routes[Counts - 2].Turn = "右转";
                                }
                            }
                            else if (LineStart < 0 && endStart < 0)
                            {
                                if (XP < 0 && YP < 0)
                                {
                                    routes[Counts - 2].Turn = "右转";
                                }
                                else
                                {
                                    routes[Counts - 2].Turn = "左转";
                                }
                            }
                            routes[Counts - 1].Turn = "取消转弯";
                            routes[Counts - 1].TurnColor = brushTurn(Convert.ToInt32(MainInfo.agvTurn.ToList().IndexOf(routes[Counts - 1].Turn)));
                            routes[Counts - 2].TurnColor = brushTurn(Convert.ToInt32(MainInfo.agvTurn.ToList().IndexOf(routes[Counts - 2].Turn)));
                        }
                    }
                    #endregion

                    #region 新建线路规则匹配

                    if (!edit) //不等于编辑模式
                    {
                        if (!string.IsNullOrWhiteSpace(MapRegulate.TemplateName)) //线路名称模板不为空
                        {
                            string Name = MapRegulate.TemplateName;
                            string TemplateText = string.Empty;
                            if (Name.Contains("[StartTag]") && Name.Contains("[StopTag]")) //确认匹配规则是否匹配
                            {
                                TemplateText = Name.Replace("[StartTag]", $"{routes[0].Tag}"); //替换起始点
                                TemplateText = TemplateText.Replace("[StopTag]", $"{routes[routes.Count() - 1].Tag}"); //替换结束点
                                RouteName.Text = TemplateText;
                            }
                        }
                        else
                        {
                            RouteName.Text = $"{routes[0].Tag}-{routes[routes.Count() - 1].Tag}"; //模板为空，则执行默认规则
                        }
                    }
                    #endregion
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)  //移除线路点
            {
                Route route = routes.FirstOrDefault(x => x.Tag.Equals(tag.Tag.ToString()));
                if (route != null)
                {
                    TagRecover();
                    map.SignLine(map.wirePointArrays, Brushes.Black, 1.7,1);
                    int Index = routes.IndexOf(route);
                    int CountLine = routes.Count;
                    for (int i = 0; i < CountLine; i++)
                    {
                        if (i > Index)
                        {
                            routes.RemoveAt(Index + 1);
                        }
                        else
                        {
                            if (i != Index)
                            {
                                if (!i.Equals(CountLine - 1))
                                    map.SignLine(map.wirePointArrays.Where(p => (
                                      p.GetPoint.TagID.Equals(Convert.ToInt32(routes[i].Tag)) && p.GetWirePoint.TagID.Equals(Convert.ToInt32(routes[i + 1].Tag))) ||
                                      (p.GetPoint.TagID.Equals(Convert.ToInt32(routes[i + 1].Tag)) && p.GetWirePoint.TagID.Equals(Convert.ToInt32(routes[i].Tag)))).ToList(), Brushes.Red, 8,999);
                            }
                        }
                    }
                    AssociatedTag(Convert.ToInt32(routes[routes.Count - 1].Tag));
                }
            }
        }
        #endregion

        #region 点击Tag标记红色，关联Tag标记绿色

        /// <summary>
        /// 点击Tag标记红色，关联Tag标记绿色
        /// </summary>
        /// <param name="TagID"></param>
        private void AssociatedTag(int TagID)
        {
            string[] TagArray = mapService.SelectTagSystem(UTCTime, TagID.ToString());
            associatedTag = TagArray.ToList();
            TagRecover();
            map.valuePairs.FirstOrDefault(p => p.Key.Equals(TagID)).Value.Background = new SolidColorBrush(Colors.Red);
            for (int i = 0; i < TagArray.Length; i++)
            {
                map.valuePairs.FirstOrDefault(p => p.Key.Equals(Convert.ToInt32(TagArray[i]))).Value.Background = new SolidColorBrush(Colors.Green);
            }
        }
        #endregion

        #region Tag颜色复原

        private void TagRecover()
        {
            map.valuePairs.Select(p => p.Value).ToList().ForEach(p => p.Background = new SolidColorBrush(Colors.Black));
        }
        #endregion

        #region 地图选项更改事件

       
        private void MapMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MapMenu.Items.Count > 0 && !string.IsNullOrEmpty(MapMenu.Text))
            {
                SerialPortData.DataContext = null;
                LoadMap(MapList.FirstOrDefault(x => x.CreateTime.Equals(MapMenu.SelectedValue.ToString())));

            }
        }
        #endregion

        #region 线路选项更改

        private void Line_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Line.SelectedValue == null)
            {
                routes.Clear();
                SerialPortData.DataContext = routes;
                return;
            }
            if (Line.Items.Count > 0 && Line.SelectedValue != null)
            {
                edit = true;
                routes = new ObservableCollection<Route>();
                RouteData = GetRoutes.FirstOrDefault(x => x.Program.Equals(Convert.ToInt32(Line.SelectedValue.ToString())));
                if (RouteData != null)
                {
                    map.SignLine(map.wirePointArrays, Brushes.Black, 1.7,1);
                    TagRecover();
                    RouteName.Text = RouteData.Name;
                    ProgramNum.Text = RouteData.Program.ToString();
                    string[] PbsArray = RouteData.Pbs.Split(',');
                    string[] TagArray = RouteData.Tag.Split(',');
                    string[] ChangeProgramArray = RouteData.ChangeProgram.Split(',');
                    string[] TurnArray = RouteData.Turn.Split(',');
                    string[] StopArray = RouteData.Stop.Split(',');
                    string[] HookArray = RouteData.Hook.Split(',');
                    string[] Speed = RouteData.Speed.Split(',');
                    string[] DirectionArray = RouteData.Direction.Split(',');
                    for (int i = 0; i < TagArray.Length; i++)
                    {
                        routes.Add(NewRoute(new Route()
                        {
                            Name = RouteData.Name,
                            Program = RouteData.Program,
                            Stop = StopArray[i],
                            Tag = TagArray[i],
                            ChangeProgram = ChangeProgramArray[i],
                            Pbs = PbsArray[i],
                            Speed = Speed[i],
                            Turn = TurnArray[i],
                            Hook = HookArray[i],
                            Direction = DirectionArray[i],
                        }));
                        if (!i.Equals(TagArray.Length - 1))
                        {
                            string aa = TagArray[i];
                            string bb = TagArray[i + 1];
                            List<WirePointArray> wires = map.wirePointArrays.Where(p => (p.GetPoint.TagID.Equals(Convert.ToInt32(TagArray[i])) && p.GetWirePoint.TagID.Equals(Convert.ToInt32(TagArray[i + 1]))) || (p.GetPoint.TagID.Equals(Convert.ToInt32(TagArray[i + 1])) && p.GetWirePoint.TagID.Equals(Convert.ToInt32(TagArray[i])))).ToList();
                            map.SignLine(wires, Brushes.Red, 8,999);
                        }
                        else
                        {
                            AssociatedTag(Convert.ToInt32(TagArray[i]));
                        }
                    }
                    SerialPortData.DataContext = routes;
                    SerialPortData.AutoGenerateColumns = false;
                }
            }
            else
            {
                SerialPortData.DataContext = new ObservableCollection<Route>();
            }
        }
        #endregion

        #region 格式化线路数据
     
        private Route NewRoute(Route route)
        {
            return new Route()
            {
                agvSetSpeed = MapRegulate.speed,
                agvSetPBS = MapRegulate.pbs,
                agvSetTurn = MapRegulate.turn,
                agvSetDirection = MapRegulate.direction,
                agvSetHook = MapRegulate.hook,
                Stop = route.Stop,
                Tag = route.Tag,
                ChangeProgram = route.ChangeProgram,
                Program = RouteData.Program,
                CreateTime = RouteData.CreateTime,
                Pbs = MainInfo.agvPbs[Convert.ToInt32(route.Pbs)],
                Speed = MainInfo.agvSpeed[Convert.ToInt32(route.Speed)],
                Turn = MainInfo.agvTurn[Convert.ToInt32(route.Turn)],
                Hook = MainInfo.agvHook[Convert.ToInt32(route.Hook)],
                Direction = MainInfo.agvDire[Convert.ToInt32(route.Direction)],
                PbsColor = Convert.ToInt32(route.Pbs).Equals(16) ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Green),
                TagColor = new SolidColorBrush(Colors.Red),
                TurnColor = brushTurn(Convert.ToInt32(route.Turn)),
                HookColor = HookORDirectionColor(Convert.ToInt32(route.Hook), true),
                SpeedColor = Convert.ToInt32(route.Speed).Equals(10) ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Color.FromRgb(245, 0, 98)),
                DirectionColor = HookORDirectionColor(Convert.ToInt32(route.Direction), false)
            };
        }
        #endregion

        #region 数据选项框关闭事件
       
        private void ListT_DropDownClosed(object sender, EventArgs e)
        {
            this.SerialPortData.CommitEdit();
        }
        #endregion

        #region 线路选项列更改事件

        /// <summary>
        /// 线路选项列更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPortData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int indexs = SerialPortData.SelectedIndex;
            int index = SerialPortData.CurrentCell.Column.DisplayIndex;

            if (!index.Equals(0))
            {
                this.SerialPortData.BeginEdit();

                DataGridTemplateColumn templeColumn = SerialPortData.Columns[index] as DataGridTemplateColumn;
                if (templeColumn == null) return;

                object item = SerialPortData.CurrentCell.Item;
                FrameworkElement element = templeColumn.GetCellContent(item);
                if (element != null)
                {
                    ComboBox expander = templeColumn.CellEditingTemplate.FindName("listT", element) as ComboBox;
                    if (expander != null)
                    {
                        expander.IsDropDownOpen = true;
                    }
                }
            }
        }

        #endregion

        #region 保存线路

        private void RouteSave_Click(object sender, RoutedEventArgs e)
        {
            if (ProgramNum.Text.Trim() != "" && RouteName.Text.Trim() != "" && FormatVerification.IsFloat(ProgramNum.Text.Trim()))
            {
                if (mapService.ExistsSystem(ProgramNum.Text.Trim(), UTCTime, RouteData.CreateTime))
                {
                    MessageBox.Show("线路号已存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (routes.Count == 0)
                {
                    MessageBox.Show("请添加线路！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    var route = new Route()
                    {
                        Name = RouteName.Text,
                        Program = Convert.ToInt32(ProgramNum.Text.Trim()),
                        CreateTime = edit == true ? RouteData.CreateTime : UTC.ConvertDateTimeLong(DateTime.Now),
                        Pbs = string.Join(",", routes.Select(p => MainInfo.agvPbs.ToList().IndexOf(p.Pbs))),
                        Hook = string.Join(",", routes.Select(p => MainInfo.agvHook.ToList().IndexOf(p.Hook))),
                        Turn = string.Join(",", routes.Select(p => MainInfo.agvTurn.ToList().IndexOf(p.Turn))),
                        Direction = string.Join(",", routes.Select(p => MainInfo.agvDire.ToList().IndexOf(p.Direction))),
                        Speed = string.Join(",", routes.Select(p => MainInfo.agvSpeed.ToList().IndexOf(p.Speed))),
                        Stop = string.Join(",", routes.Select(p => p.Stop)),
                        Tag = string.Join(",", routes.Select(p => p.Tag)),
                        ChangeProgram = string.Join(",", routes.Select(p => p.ChangeProgram)),
                    };
                    if (mapService.SaveRouteSystem(route, edit, UTC.ConvertDateTimeLong(Convert.ToDateTime(MapMenu.SelectedValue.ToString()))))
                    {
                        edit = true;
                        Task.Factory.StartNew(() =>
                        {
                            GetRoutes = mapService.GetrouteList(UTCTime.ToString());
                            this.Dispatcher.Invoke(() => { Line.ItemsSource = GetRoutes; Line.Text = route.Name; });
                        });
                        MessageBox.Show("保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("保存失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("格式输入错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region 添加线路

        private void Line_Add_Click(object sender, RoutedEventArgs e)
        {
            edit = false;
            TagRecover();
            routes.Clear();
            RouteName.Text = "";
            ProgramNum.Text = GetRoutes.Count > 0 ? (GetRoutes.Max(x => x.Program) + 1).ToString() : "1";
            map.SignLine(map.wirePointArrays, Brushes.Black, 1.7,1);
        }
        #endregion

        #region 删除线路

        private void Line_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (edit)
            {
                MessageBoxResult confirmToDel = MessageBox.Show("确认要删除线路吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirmToDel == MessageBoxResult.Yes)
                {
                    if (mapService.DelRouteMapSystem(UTC.ConvertDateTimeLong(Convert.ToDateTime(MapMenu.SelectedValue.ToString())), RouteData.Program))
                    {
                        Task.Factory.StartNew(() =>
                        {
                            GetRoutes = mapService.GetrouteList(UTCTime.ToString());
                            this.Dispatcher.BeginInvoke(new Action(() => { Line.ItemsSource = GetRoutes; Line.SelectedIndex = 0; }));
                        });
                        MessageBox.Show("删除成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("删除失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                Line_Add_Click(null, null);
            }
        }

        #endregion

        #region 颜色获取

        public Brush brushTurn(int id)
        {
            if (id.Equals(0))
            {
                return new SolidColorBrush(Colors.Black);
            }
            else if (id.Equals(1))
            {
                return new SolidColorBrush(Color.FromRgb(213, 3, 238));
            }
            else if (id.Equals(2))
            {
                return new SolidColorBrush(Colors.Green);
            }
            else if (id.Equals(3))
            {
                return new SolidColorBrush(Colors.Red);
            }
            return new SolidColorBrush(Colors.Black);
        }

        public Brush HookORDirectionColor(int id, bool Hook)
        {
            if (id.Equals(0))
            {
                if (Hook)
                    return new SolidColorBrush(Colors.Red);
                else
                    return new SolidColorBrush(Colors.Green);
            }
            else if (id.Equals(1))
            {
                if (Hook)
                    return new SolidColorBrush(Colors.Green);
                else
                    return new SolidColorBrush(Colors.Red);
            }
            else
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        #endregion

        #region 缩放

        private void Magnify_Click(object sender, RoutedEventArgs e)
        {
            map.MapSise += 0.2;
            if (map.MapSise > 5)
            {
                return;
            }
            CanvasMapZoom();
        }

        /// <summary>
        /// 缩放
        /// </summary>
        private void CanvasMapZoom()
        {
            map.Mapmagnify(map.MapSise, CanvasWidth, CanvasHeight, true);
        }
      

        private void Shrink_Click(object sender, RoutedEventArgs e)
        {
            map.MapSise -= 0.2;
            if (map.MapSise < 1)
            {
                map.MapSise = 1;
                return;
            }
            CanvasMapZoom();
        }

        private void Recover_Click(object sender, RoutedEventArgs e)
        {
            map.MapSise = 1;
            CanvasMapZoom();
        }

        private void SrcCount_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            SrcX.ScrollToHorizontalOffset(e.HorizontalOffset);//X轴标尺跟随移动
            SrcY.ScrollToVerticalOffset(e.VerticalOffset); //Y轴标尺等随移动
        }
        #endregion



        private void Config_Click(object sender, RoutedEventArgs e)
        {
            LineConfig line = new LineConfig();
            line.ShowDialog();
        }




        private void SerialPortData_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            int indexs = e.Column.DisplayIndex;
            int index = e.Row.GetIndex();
            Route route = e.Row.DataContext as Route;
            if (route != null)
            {
                if (indexs.Equals(1))
                {
                    routes[index].SpeedColor = MainInfo.agvSpeed.ToList().IndexOf(route.Speed).Equals(10) ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Color.FromRgb(245, 0, 98));
                }
                else if (indexs.Equals(2))
                {
                    routes[index].PbsColor = MainInfo.agvPbs.ToList().IndexOf(route.Pbs).Equals(16) ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Green);
                }
                else if (indexs.Equals(3))
                {
                    routes[index].TurnColor = brushTurn(MainInfo.agvTurn.ToList().IndexOf(route.Turn));
                }
                else if (indexs.Equals(4))
                {
                    routes[index].DirectionColor = HookORDirectionColor(MainInfo.agvDire.ToList().IndexOf(route.Direction), false);
                }
                else if (indexs.Equals(5))
                {
                    routes[index].HookColor = HookORDirectionColor(MainInfo.agvHook.ToList().IndexOf(route.Hook), true);
                }
            }
        }

        private void Inquire_Line_Click(object sender, RoutedEventArgs e)
        {
            GetRoutes = mapService.GetrouteList(UTCTime.ToString(), RouteName.Text.Trim(), ProgramNum.Text.Trim());
            Line.ItemsSource = GetRoutes;
            Line.SelectedIndex = 0;
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as ToolBar).ToolBar_Formatting();
        }
    }
}
