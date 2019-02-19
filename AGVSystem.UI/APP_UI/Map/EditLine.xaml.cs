using AGVSystem.APP.agv_Map;
using AGVSystem.Infrastructure.agvCommon;
using AGVSystem.IService.IO_System;
using AGVSystem.Model.DrawMap;
using AGVSystem.Model.Ga_agvModels;
using AGVSystem.Model.LogicData;
using AGVSystem.UI.APP_UI.Setting;
using OperateIni;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
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

        Painting GetPainting = new Painting();
        MapInstrument map = new MapInstrument();
        IO_AGVMapService mapService = new agvMapRegulate(); //业务逻辑接口
        double CanvasWidth, CanvasHeight; //初始宽高
        ObservableCollection<Ga_Map> MapList = new ObservableCollection<Ga_Map>();
        ObservableCollection<Route> GetRoutes = new ObservableCollection<Route>();
        Route RouteData = new Route();
        ObservableCollection<Route> routes = new ObservableCollection<Route>();
        long UTCTime;
        bool edit = true;

        public EditLine(ObservableCollection<Ga_Map> Map)
        {
            InitializeComponent();
            MapList = Map;
            Setting_Map();
        }
        /// <summary>
        /// 绑定数据
        /// </summary>
        public void Setting_Map()
        {
            MapMenu.ItemsSource = MapList;
            LoadMap(MapList[0]);
        }


        public void LoadMap(Ga_Map GetMap)
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\setting.ini"))
            {
                string Size = IniFile.ReadIniData("AGV", "MapSise", "", AppDomain.CurrentDomain.BaseDirectory + "\\setting.ini");
                map.MapSise = Convert.ToDouble(Size);
            }
            CanvasWidth = GetMap.Width * 10;
            CanvasHeight = GetMap.Height * 10;
            double CanvasWidths = GetMap.Width * 10 * map.MapSise;
            double CanvasHeights = GetMap.Height * 10 * map.MapSise;
            TopX.Width = CanvasWidths;
            TopY.Height = CanvasHeights;
            mainPanel.Width = CanvasWidths;
            mainPanel.Height = CanvasHeights;
            map.GetCanvas = mainPanel;
            TopX.Children.Clear();
            TopY.Children.Clear();
            GetPainting.CoordinateX(TopX, TopY);
            UTCTime = UTC.ConvertDateTimeLong(Convert.ToDateTime(GetMap.CreateTime));
            map.LoadEditMap(UTCTime, false,true);
            map.valuePairs.Select(p => p.Value).ToList().ForEach(p => { p.MouseDown += Value_MouseDown;p.Cursor = Cursors.Hand;});
            GetRoutes = mapService.GetrouteList(UTCTime.ToString());
            Line.ItemsSource = GetRoutes;
            Line.SelectedIndex = 0;
        }
        List<string> associatedTag = new List<string>();

        private void Value_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Label tag = (Label)sender;
            if (routes.Count().Equals(0) || (associatedTag.Where(x => x.Equals(tag.Tag.ToString())).Count() >= 1) || (edit == true && (associatedTag.Where(x => x.Equals(tag.Tag.ToString())).Count() >= 1)))
            {
                AssociatedTag(Convert.ToInt32(tag.Tag));
                int TagPoint = routes.Count() > 0 ? Convert.ToInt32(routes[routes.Count() - 1].Tag) : 0;
                int TagWirePoint = Convert.ToInt32(tag.Tag);

                SignLine(map.wirePointArrays.Where(p => (
                p.GetPoint.TagID.Equals(TagPoint) && p.GetWirePoint.TagID.Equals(TagWirePoint)) || (p.GetPoint.TagID.Equals(TagWirePoint) && p.GetWirePoint.TagID.Equals(TagPoint))).ToList(), Brushes.Red, 8);
                routes.Add(NewRoute(new Route()
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
                if (!edit)
                {
                    RouteName.Text = $"Ga_{routes[0].Tag}-{routes[routes.Count() - 1].Tag}";
                }
               
            }
        }



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


        private void SignLine(List<WirePointArray> pointArrays, Brush brushes, int LintWidth)
        {
            pointArrays.ForEach(
                    p =>
                    {
                        if (p.circuitType.Equals(CircuitType.Line) || p.circuitType.Equals(CircuitType.Semicircle))
                        {
                            System.Windows.Shapes.Path path = ((WirePointLine)p).GetPath;
                            path.Stroke = brushes;
                            path.StrokeThickness = LintWidth;
                        }
                        else if (p.circuitType.Equals(CircuitType.Broken))
                        {
                            List<System.Windows.Shapes.Path> path = ((WirePointBroken)p).Paths;
                            path.ForEach(x => { x.Stroke = brushes; x.StrokeThickness = LintWidth; });
                        }
                    });
        }


        private void TagRecover()
        {
            map.valuePairs.Select(p => p.Value).ToList().ForEach(p => p.Background = new SolidColorBrush(Colors.Black));
        }




        private void MapMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MapMenu.Items.Count > 0 && !string.IsNullOrEmpty(MapMenu.Text))
            {
                SerialPortData.DataContext = null;
                LoadMap(MapList.FirstOrDefault(x => x.CreateTime.Equals(MapMenu.SelectedValue.ToString())));
            }
        }

        List<Ga_agvStatus> speed = MainInfo.agvSpeed.Select(p => new Ga_agvStatus() { StatusName = p, statusValue = MainInfo.agvSpeed.ToList().IndexOf(p).ToString() }).ToList();
        List<Ga_agvStatus> pbs = MainInfo.agvPbs.Select(p => new Ga_agvStatus() { StatusName = p, statusValue = MainInfo.agvPbs.ToList().IndexOf(p).ToString() }).ToList();
        List<Ga_agvStatus> turn = MainInfo.agvTurn.Select(p => new Ga_agvStatus() { StatusName = p, statusValue = MainInfo.agvTurn.ToList().IndexOf(p).ToString() }).ToList();
        List<Ga_agvStatus> direction = MainInfo.agvDire.Select(p => new Ga_agvStatus() { StatusName = p, statusValue = MainInfo.agvDire.ToList().IndexOf(p).ToString() }).ToList();
        List<Ga_agvStatus> hook = MainInfo.agvHook.Select(p => new Ga_agvStatus() { StatusName = p, statusValue = MainInfo.agvHook.ToList().IndexOf(p).ToString() }).ToList();

        private void Line_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Line.SelectedValue == null)
                return;

            if (Line.Items.Count > 0 && Line.SelectedValue != null)
            {
                edit = true;
                routes = new ObservableCollection<Route>();
                RouteData = GetRoutes.FirstOrDefault(x => x.Program.Equals(Convert.ToInt32(Line.SelectedValue.ToString())));
                if (RouteData != null)
                {
                    SignLine(map.wirePointArrays, Brushes.Black, 3);
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
                            List<WirePointArray> wiress = map.wirePointArrays.Where(p => (p.GetPoint.TagID.Equals(Convert.ToInt32(TagArray[i])) && p.GetWirePoint.TagID.Equals(TagArray[i + 1]))).ToList();
                            List<WirePointArray> wirebb = map.wirePointArrays.Where(p => (p.GetPoint.TagID.Equals(TagArray[i + 1]) && p.GetWirePoint.TagID.Equals(TagArray[i]))).ToList();
                            List<WirePointArray> wires = map.wirePointArrays.Where(p => (p.GetPoint.TagID.Equals(Convert.ToInt32(TagArray[i])) && p.GetWirePoint.TagID.Equals(Convert.ToInt32(TagArray[i + 1]))) || (p.GetPoint.TagID.Equals(Convert.ToInt32(TagArray[i + 1])) && p.GetWirePoint.TagID.Equals(Convert.ToInt32(TagArray[i])))).ToList();
                            SignLine(wires, Brushes.Red, 8);
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
        }


        private Route NewRoute(Route route)
        {
            return new Route()
            {
                agvSetSpeed = speed,
                agvSetPBS = pbs,
                agvSetTurn = turn,
                agvSetDirection = direction,
                agvSetHook = hook,
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
            TopX.Children.Clear();
            TopY.Children.Clear();
            TopX.Width = CanvasWidth * map.MapSise;
            TopY.Height = CanvasHeight * map.MapSise;
            GetPainting.Scale_X = 10 * map.MapSise;
            GetPainting.Scale_Y = 10 * map.MapSise;
            GetPainting.CoordinateX(TopX, TopY); //绘制X轴Y轴刻度
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


        private void ListT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListT_DropDownClosed(object sender, EventArgs e)
        {
            if (sender is ComboBox)
            {
                int indexs = SerialPortData.SelectedIndex;
                ComboBox box = ((ComboBox)sender);
                Ga_agvStatus ga = (Ga_agvStatus)box.SelectedItem;
                int SelectValue = Convert.ToInt32(ga.statusValue);
                if (box.Name.Equals("listSeep"))
                {
                    routes[indexs].Speed = ga.StatusName;
                    routes[indexs].SpeedColor = SelectValue.Equals(10) ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Color.FromRgb(245, 0, 98));
                }
                else if (box.Name.Equals("listPBS"))
                {
                    routes[indexs].Pbs = ga.StatusName;
                    routes[indexs].PbsColor = SelectValue.Equals(16) ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Green);
                }
                else if (box.Name.Equals("listTurn"))
                {
                    routes[indexs].Turn = ga.StatusName;
                    routes[indexs].TurnColor = brushTurn(SelectValue);
                }
                else if (box.Name.Equals("listDirection"))
                {
                    routes[indexs].Direction = ga.StatusName;
                    routes[indexs].DirectionColor = HookORDirectionColor(SelectValue, false);
                }
                else if (box.Name.Equals("listHook"))
                {
                    routes[indexs].Hook = ga.StatusName;
                    routes[indexs].HookColor = HookORDirectionColor(SelectValue, true);
                }
                this.SerialPortData.CommitEdit();
            }
        }

        private void SerialPortData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int indexs = SerialPortData.SelectedIndex;
            int index = SerialPortData.CurrentCell.Column.DisplayIndex;

            if (!index.Equals(0))
            {
                this.SerialPortData.BeginEdit();
            }
        }

        private void RouteSave_Click(object sender, RoutedEventArgs e)
        {
            if (ProgramNum.Text.Trim() != "" && RouteName.Text.Trim() != "" && FormatVerification.IsFloat(ProgramNum.Text.Trim()))
            {
                if (mapService.ExistsSystem(ProgramNum.Text.Trim(), UTCTime, RouteData.CreateTime))
                {
                    MessageBox.Show("线路号已存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    var route = new Route()
                    {
                        Name = RouteName.Text,
                        Program = Convert.ToInt32(ProgramNum.Text.Trim()),
                        CreateTime = edit ==true ? RouteData.CreateTime : UTC.ConvertDateTimeLong(DateTime.Now),
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

        private void Line_Add_Click(object sender, RoutedEventArgs e)
        {
            edit = false;
            TagRecover();
            routes.Clear();
            RouteName.Text = "";
            ProgramNum.Text = (GetRoutes.Max(x => x.Program) + 1).ToString();
            SignLine(map.wirePointArrays, Brushes.Black, 3);
        }

        private void Line_Delete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            LineConfig line = new LineConfig();
            line.ShowDialog();
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
        }
    }
}
