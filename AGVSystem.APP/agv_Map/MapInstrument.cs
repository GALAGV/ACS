using AGVSystem.Model.DrawMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AGVSystem.BLL.ServiceLogicBLL;
using AGVSystem.Infrastructure.agvCommon;
using System.Data;

namespace AGVSystem.APP.agv_Map
{
    public class MapInstrument
    {
        private int s = 1; //信标起始索引
        private int index = 1;//区域起始索引
        private int TextInx = 1;//文字索引
        public double MapSise = 1; //画布默认缩放大小
        private bool tongs = false; //画布移动标志位
        private Painting painting = new Painting();//地图绘制
        private Ga_mapBLL IO_AGVMapService = new Ga_mapBLL();
        private Point pos = new Point();//记录移动时Tag位置
        private Point jos = new Point();//记录移动时工作区位置
        public Canvas GetCanvas = new Canvas();//绘制容器
        public Point point = new Point();//记录滚动条位置动态调整生成控件位置
        public bool PathStatic = false;//绘制状态（true:绘制状态 false 不是绘制状态）
        public CircuitType GetCircuitType = new CircuitType();//绘制线路类型 （直线，折线，曲线）
        private List<WirePoint> Pairsarray = new List<WirePoint>();//（暂时存放绘制线路两点位置）
        public List<WirePointArray> wirePointArrays = new List<WirePointArray>();//路线集合
        public Dictionary<int, Label> keyValuePairs = new Dictionary<int, Label>();//区域集合
        public Dictionary<int, Label> valuePairs = new Dictionary<int, Label>();//信标集合
        public Dictionary<int, Label> GetKeyValues = new Dictionary<int, Label>();//文字集合
        public Dictionary<int, WirePointArray> wires = new Dictionary<int, WirePointArray>();//暂时存放拖动时所用关联线路
        public Action<Label> action; //编辑信标委托
        public Action<Label> AreaAction; //编辑区域委托

        #region 创建Tag
        /// <summary>
        /// 创建Tag
        /// </summary>
        /// <param name="point"></param>
        /// <param name="TagID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Label TagCreate(Point point, int TagID, bool type, bool tagevent)
        {
            Label labelStrn = new Label()
            {
                Content = TagID.ToString(),
                //Background = new SolidColorBrush(Color.FromRgb(80, 150, 255)),
                Background = Brushes.Black,
                Foreground = new SolidColorBrush(Colors.WhiteSmoke),
                Width = 38,
                Height = 23,
                Margin = new Thickness(point.X + (type.Equals(false) ? 120 : 0), point.Y + (type.Equals(false) ? 120 : 0), 0, 0),//120(位置偏移量)
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                //Cursor = Cursors.Hand,
                Tag = TagID
            };

            Canvas.SetZIndex(labelStrn, 999999);
            if (tagevent)
            {
                labelStrn.MouseDown += LabelStrn_MouseDown;
                labelStrn.MouseMove += LabelStrn_MouseMove;
                labelStrn.MouseUp += LabelStrn_MouseUp;
            }
            valuePairs.Add(TagID, labelStrn);
            return labelStrn;
        }


        #endregion

        #region 添加线路
        public void AddCircuit(int tmp, Point point)
        {
            #region 判断位置是否存在
            bool TagExistx = false; //判断位置是否存在
            foreach (WirePoint item in Pairsarray)
            {
                if (tmp.Equals(item.TagID))//如果TagID相同则存在
                    TagExistx = true;
            }
            #endregion

            #region 所有Tag改为原色
            foreach (int item in valuePairs.Keys)
            {
                valuePairs[item].Background = Brushes.Black;
            }
            #endregion

            if (!TagExistx) //位置不存在则添加，防止重复添加
            {
                Pairsarray.Add(new WirePoint { TagID = tmp, SetPoint = point });
            }

            #region 线路绘制点改为红色
            foreach (int item in valuePairs.Keys)
            {
                foreach (WirePoint itms in Pairsarray)
                {
                    if (itms.TagID.Equals(item))
                    {
                        valuePairs[item].Background = new SolidColorBrush(Colors.Red);
                    }
                }
            }
            #endregion

            if (Pairsarray.Count.Equals(2))//起始位置存在则开始绘制
            {
                if (!GetCircuitType.Equals(CircuitType.Clear) && !GetCircuitType.Equals(CircuitType.Align) && !GetCircuitType.Equals(CircuitType.vertical))
                {
                    #region 判断线路是否存在
                    bool lineExistx = false; //判断线路是否存在
                    double tr = Pairsarray[0].SetPoint.X - Pairsarray[1].SetPoint.X; //大于0下方上方小于0上方
                    foreach (WirePointArray item in wirePointArrays)
                    {
                        if (GetCircuitType.Equals(CircuitType.Semicircle))
                        {
                            //如果（线路起始位置和结束位置一一对应）则视为线路存在
                            if (((item.GetPoint.SetPoint.Equals(Pairsarray[0].SetPoint) && item.GetWirePoint.SetPoint.Equals(Pairsarray[1].SetPoint)) || (item.GetPoint.SetPoint.Equals(Pairsarray[1].SetPoint) && item.GetWirePoint.SetPoint.Equals(Pairsarray[0].SetPoint))) && item.circuitType.Equals(GetCircuitType) && item.Direction.Equals(tr > 0 ? 1 : 0))
                                lineExistx = true;
                        }
                        else if (GetCircuitType.Equals(CircuitType.Broken))
                        {

                            double diff = Pairsarray[0].SetPoint.Y - Pairsarray[1].SetPoint.Y;
                            //如果（线路起始位置和结束位置一一对应）则视为线路存在
                            if (((item.GetPoint.SetPoint.Equals(Pairsarray[0].SetPoint) && item.GetWirePoint.SetPoint.Equals(Pairsarray[1].SetPoint)) || (item.GetPoint.SetPoint.Equals(Pairsarray[1].SetPoint) && item.GetWirePoint.SetPoint.Equals(Pairsarray[0].SetPoint))) && item.circuitType.Equals(GetCircuitType) && item.Direction.Equals(diff < 0 ? 0 : 1))
                                lineExistx = true;
                        }
                        else
                        {
                            //如果（线路起始位置和结束位置一一对应）或（线路起始位置和结束位置相同，结束位置和起始位置相同）则视为线路存在
                            if (((item.GetPoint.SetPoint.Equals(Pairsarray[0].SetPoint) && item.GetWirePoint.SetPoint.Equals(Pairsarray[1].SetPoint)) || (item.GetPoint.SetPoint.Equals(Pairsarray[1].SetPoint) && item.GetWirePoint.SetPoint.Equals(Pairsarray[0].SetPoint))) && item.circuitType.Equals(GetCircuitType))
                                lineExistx = true;
                        }
                    }
                    #endregion

                    if (!lineExistx)//线路不存在则绘制线路
                    {
                        AddLine();
                    }
                    Pairsarray.Clear();//移除暂存起始结束点
                }
                else if (GetCircuitType.Equals(CircuitType.Clear))
                {
                    ClearCircuit(Pairsarray[0].TagID, Pairsarray[1].TagID);//清除路线
                }
                else if (GetCircuitType.Equals(CircuitType.Align))
                {
                    if (!Pairsarray[1].SetPoint.X.Equals(Pairsarray[0].SetPoint.X))
                    {
                        DeleteLine();
                        valuePairs[Pairsarray[1].TagID].Margin = new Thickness(valuePairs[Pairsarray[1].TagID].Margin.Left, valuePairs[Pairsarray[0].TagID].Margin.Top, 0, 0);
                        MouseMove(new Point() { X = valuePairs[Pairsarray[1].TagID].Margin.Left + 19, Y = valuePairs[Pairsarray[1].TagID].Margin.Top + 11.5 }, Pairsarray[1].TagID);
                        Pairsarray.Clear();
                    }
                }
                else if (GetCircuitType.Equals(CircuitType.vertical))
                {
                    if (!Pairsarray[1].SetPoint.Y.Equals(Pairsarray[0].SetPoint.Y))
                    {
                        DeleteLine();
                        valuePairs[Pairsarray[1].TagID].Margin = new Thickness(valuePairs[Pairsarray[0].TagID].Margin.Left, valuePairs[Pairsarray[1].TagID].Margin.Top, 0, 0);
                        MouseMove(new Point() { X = valuePairs[Pairsarray[1].TagID].Margin.Left + 19, Y = valuePairs[Pairsarray[1].TagID].Margin.Top + 11.5 }, Pairsarray[1].TagID);
                        Pairsarray.Clear();
                    }
                }
            }
        }
        #endregion

        #region 清除线路

        /// <summary>
        /// 清除线路
        /// </summary>
        public void ClearCircuit(int GetPointID,int GetWirePointID)
        {
            foreach (WirePointArray item in wirePointArrays.ToArray())
            {
                if ((item.GetPoint.TagID.Equals(GetPointID) && item.GetWirePoint.TagID.Equals(GetWirePointID)) || (item.GetPoint.TagID.Equals(GetWirePointID) && item.GetWirePoint.TagID.Equals(GetPointID)))
                {
                    CrearS(item,true);
                    break;
                }
            }
            Pairsarray.Clear();
        }

        public void CrearS(WirePointArray item, bool type)
        {
            if (item.circuitType == CircuitType.Line || item.circuitType == CircuitType.Semicircle)
            {
                GetCanvas.Children.Remove(((WirePointLine)item).GetPath);
            }
            else
            {
                if (((WirePointBroken)item).Paths != null)
                {
                    foreach (var it in ((WirePointBroken)item).Paths)
                    {
                        GetCanvas.Children.Remove(it);
                    }
                }
            }
            if (type)
                wirePointArrays.Remove(item);
        }




        public void DeleteLine()
        {
            foreach (WirePointArray item in wirePointArrays)//查询所有关联线路
            {
                if (item.GetPoint.TagID.Equals(Pairsarray[1].TagID) || item.GetWirePoint.TagID.Equals(Pairsarray[1].TagID))//暂时移除拖动时关联线路
                {
                    if (item.circuitType == CircuitType.Line || item.circuitType == CircuitType.Semicircle)
                    {
                        GetCanvas.Children.Remove(((WirePointLine)item).GetPath);
                    }
                    else
                    {
                        if (((WirePointBroken)item).Paths != null)
                        {
                            foreach (var it in ((WirePointBroken)item).Paths)
                            {
                                GetCanvas.Children.Remove(it);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region 信标移动

        /// <summary>
        /// 信标移动
        /// </summary>
        /// <param name="point"></param>
        /// <param name="TagID"></param>
        public void MouseMove(Point point, int TagID)
        {
            #region 线路移动
            wires.Clear();//清空集合
            for (int i = 0; i < wirePointArrays.Count; i++)//查找拖动时所有关联线路
            {
                if (wirePointArrays[i].GetPoint.TagID.Equals(TagID))//如果起始位置相同则线路和拖动Tag存在关联
                {
                    wirePointArrays[i].GetPoint.SetPoint = point; //更新线路起始点位置
                    wires.Add(i, wirePointArrays[i]);//暂时存放线路
                    GetCanvas.Children.Remove(valuePairs[wirePointArrays[i].GetPoint.TagID]); //移除关联线路起始位置Tag
                    GetCanvas.Children.Remove(valuePairs[wirePointArrays[i].GetWirePoint.TagID]);//移除关联线路结束位置Tag
                }
                else if (wirePointArrays[i].GetWirePoint.TagID.Equals(TagID))// 如果结束位置相同则线路和拖动Tag存在关联
                {
                    GetCanvas.Children.Remove(valuePairs[wirePointArrays[i].GetPoint.TagID]);//移除关联线路起始位置Tag
                    GetCanvas.Children.Remove(valuePairs[wirePointArrays[i].GetWirePoint.TagID]);//移除关联线路结束位置Tag
                    wirePointArrays[i].GetWirePoint.SetPoint = point; //更新线路结束点位置
                    wires.Add(i, wirePointArrays[i]);//暂时存放线路
                }
            }
            List<Label> labels = new List<Label>(); //存放拖动时所有关联Tag
            foreach (int item in wires.Keys)//重新绘制所有关联线路
            {
                Path path = null;
                List<Path> Genpaths = null;
                if (wires[item].circuitType.Equals(CircuitType.Line))//直线
                {
                    path = painting.Line(wires[item].GetPoint.SetPoint, wires[item].GetWirePoint.SetPoint);
                    ((WirePointLine)wirePointArrays[item]).GetPath = path;//更新线路对象
                }
                else if (wires[item].circuitType.Equals(CircuitType.Semicircle))//半圆
                {
                    path = painting.DrawingSemicircle(wires[item].GetPoint.SetPoint, wires[item].GetWirePoint.SetPoint);
                    ((WirePointLine)wirePointArrays[item]).GetPath = path;//更新线路对象
                }
                else if (wires[item].circuitType.Equals(CircuitType.Broken))//折线
                {
                    Genpaths = painting.DrawingBroken(wires[item].GetPoint.SetPoint, wires[item].GetWirePoint.SetPoint);
                    ((WirePointBroken)wirePointArrays[item]).Paths = Genpaths;
                }
            }
            foreach (int item in wires.Keys)//因为线路关联Tag存在重复，为防止重复添加需做判断（及线路起始点和结束点Tag存在共用情况）
            {
                bool lableExistx = true;//判断起始点Tag是否存在
                foreach (Label irn in labels)
                {
                    if (wires[item].GetPoint.TagID == Convert.ToInt32(irn.Tag))//ID相同则存在
                    {
                        lableExistx = false;
                    }
                }
                if (lableExistx)//不存在则添加起始点Tag
                {
                    GetCanvas.Children.Add(valuePairs[wires[item].GetPoint.TagID]);//绘制起始点Tag
                    labels.Add(valuePairs[wires[item].GetPoint.TagID]);//将起始点Tag添加至集合
                }
                bool lableExi = true;//判断结束点Tag是否存在
                foreach (Label irn in labels)
                {
                    if (wires[item].GetWirePoint.TagID == Convert.ToInt32(irn.Tag))//ID相同则存在
                    {
                        lableExi = false;
                    }
                }
                if (lableExi)//不存在则添加结束点Tag
                {
                    GetCanvas.Children.Add(valuePairs[wires[item].GetWirePoint.TagID]);//绘制结束点Tag
                    labels.Add(valuePairs[wires[item].GetWirePoint.TagID]);//将结束点Tag添加至集合
                }
            }
            #endregion
        }

        #endregion

        #region 绘制线路


        public void AddLine()
        {
            //foreach (WirePoint item in Pairsarray)//移除Tag （否则会出现线路遮挡Tag问题）
            //{
            //    GetCanvas.Children.Remove(valuePairs[item.TagID]);
            //}
            if (GetCircuitType.Equals(CircuitType.Line))//绘制直线
            {
                Path path = painting.Line(Pairsarray[0].SetPoint, Pairsarray[1].SetPoint);//绘制直线
                wirePointArrays.Add(new WirePointLine { GetPoint = Pairsarray[0], GetWirePoint = Pairsarray[1], GetPath = path, circuitType = CircuitType.Line });//将绘制的线路添加到线路集合中
            }
            else if (GetCircuitType.Equals(CircuitType.Semicircle))
            {
                double ey = Pairsarray[0].SetPoint.X - Pairsarray[1].SetPoint.X; //大于0下方上方小于0上方
                Path path = painting.DrawingSemicircle(Pairsarray[0].SetPoint, Pairsarray[1].SetPoint);//绘制半圆
                if (ey > 0)
                {
                    wirePointArrays.Add(new WirePointLine { GetPoint = Pairsarray[0], GetWirePoint = Pairsarray[1], GetPath = path, circuitType = CircuitType.Semicircle, Direction = 1 });//将绘制的线路添加到线路集合中
                }
                else
                {
                    wirePointArrays.Add(new WirePointLine { GetPoint = Pairsarray[0], GetWirePoint = Pairsarray[1], GetPath = path, circuitType = CircuitType.Semicircle, Direction = 0 });//将绘制的线路添加到线路集合中
                }
            }
            else if (GetCircuitType.Equals(CircuitType.Broken))//折线
            {
                List<Path> Pathr = painting.DrawingBroken(Pairsarray[0].SetPoint, Pairsarray[1].SetPoint);
                double drn = Pairsarray[0].SetPoint.X - Pairsarray[1].SetPoint.X;
                double hrn = Pairsarray[0].SetPoint.Y - Pairsarray[1].SetPoint.Y;

                if ((drn > 0 && hrn < 0) || (drn < 0 && hrn > 0))
                {
                    double diff = Pairsarray[0].SetPoint.Y - Pairsarray[1].SetPoint.Y;
                    if (diff < 0)
                    {
                        wirePointArrays.Add(new WirePointBroken { GetPoint = Pairsarray[0], GetWirePoint = Pairsarray[1], Paths = Pathr, circuitType = CircuitType.Broken, Direction = 0 });//将绘制的线路添加到线路集合中
                    }
                    else
                    {
                        wirePointArrays.Add(new WirePointBroken { GetPoint = Pairsarray[0], GetWirePoint = Pairsarray[1], Paths = Pathr, circuitType = CircuitType.Broken, Direction = 1 });//将绘制的线路添加到线路集合中
                    }
                }
                else
                {
                    double diff = Pairsarray[0].SetPoint.Y - Pairsarray[1].SetPoint.Y;
                    if (diff < 0)
                    {
                        wirePointArrays.Add(new WirePointBroken { GetPoint = Pairsarray[0], GetWirePoint = Pairsarray[1], Paths = Pathr, circuitType = CircuitType.Broken, Direction = 0 });//将绘制的线路添加到线路集合中
                    }
                    else
                    {
                        wirePointArrays.Add(new WirePointBroken { GetPoint = Pairsarray[0], GetWirePoint = Pairsarray[1], Paths = Pathr, circuitType = CircuitType.Broken, Direction = 1 });//将绘制的线路添加到线路集合中
                    }
                }
            }
        }

        #endregion

        #region Tag鼠标动作事件


        /// <summary>
        /// 释放鼠标发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelStrn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (tongs)
                return;
            if (e.ChangedButton==MouseButton.Left)
            {
                Label tmp = (Label)sender;
                tmp.ReleaseMouseCapture();
                Point point = new Point { X = tmp.Margin.Left + 19, Y = tmp.Margin.Top + 11.5 }; //19X轴偏移位置,11.5Y轴偏移位置（信标中心点）
                if (PathStatic)
                {
                    AddCircuit(Convert.ToInt32(tmp.Tag), point);
                }
                else
                {
                    MouseMove(point, Convert.ToInt32(tmp.Tag));//信标移动
                }
            }

        }



        /// <summary>
        /// 移动鼠标发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelStrn_MouseMove(object sender, MouseEventArgs e)
        {
            if (tongs)
                return;

            if (PathStatic )
                return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Label tmp = (Label)sender;
                double dx = e.GetPosition(null).X - pos.X + tmp.Margin.Left;
                double dy = e.GetPosition(null).Y - pos.Y + tmp.Margin.Top;
                if (dx > 0 && dy > 0 && dx < GetCanvas.Width - 38 && dy < GetCanvas.Height - 23)
                {
                    tmp.Margin = new Thickness(dx, dy, 0, 0);
                }
                pos = e.GetPosition(null);
                foreach (WirePointArray item in wirePointArrays)//查询所有关联线路
                {
                    if (item.GetPoint.TagID.Equals(Convert.ToInt32(tmp.Tag)) || item.GetWirePoint.TagID.Equals(Convert.ToInt32(tmp.Tag)))//暂时移除拖动时关联线路
                    {
                        if (item.circuitType == CircuitType.Line || item.circuitType == CircuitType.Semicircle)
                        {
                            GetCanvas.Children.Remove(((WirePointLine)item).GetPath);
                        }
                        else
                        {
                            if (((WirePointBroken)item).Paths != null)
                            {
                                foreach (var it in ((WirePointBroken)item).Paths)
                                {
                                    GetCanvas.Children.Remove(it);
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 按下鼠标发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelStrn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tongs)
                return;
            Label tmp = (Label)sender;
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                pos = e.GetPosition(null);
                tmp.CaptureMouse();
                tmp.Cursor = Cursors.Hand;
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                if (PathStatic)
                    return;
                action.Invoke(tmp);
            }
        }
        #endregion

        #region 所有Tag改为原色
        public void TagFormer()
        {
            GetCanvas.MouseUp -= MainPanel_MouseUp;
            GetCanvas.MouseMove -= MainPanel_MouseMove;
            GetCanvas.MouseDown -= MainPanel_MouseDown;
            Pairsarray.Clear();//移除暂存起始点
            tongs = false;
            GetCanvas.Cursor = Cursors.Arrow;
            foreach (int item in valuePairs.Keys)
            {
                valuePairs[item].Background = Brushes.Black;
            }
        }
        #endregion

        #region 动态生成区域

        /// <summary>
        /// 动态生成工作区
        /// </summary>
        /// <param name="mainPanel"></param>
        /// <param name="point"></param>
        public void MapAreaNew(Point point)
        {
            var pairs = keyValuePairs.OrderByDescending(x => x.Key).ToArray();
            int CountTg = pairs.Count();
            index = CountTg > 0 ? pairs[0].Key : 1;
            if (CountTg > 0)
            { index++; }
            Label labelArea = NewArea(point, null, index, "000000", "FFFFFF", "000000", 21, 100, 100, false, "居中对齐", true);
            GetCanvas.Children.Add(labelArea);
            index++;
        }

        public Label NewArea(Point point, string Text, int ArID, string bgColor, string FontColor, string BrColor, double FontSise, double MpWidth, double MpHeight, bool type, string FontPosition, bool Areaevent)
        {
            Label labelArea = new Label()
            {
                Content = type.Equals(false) ? "工作区" + index.ToString() : Text,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + bgColor + "")),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + FontColor + "")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + BrColor + "")),
                BorderThickness = new Thickness(2, 2, 2, 2),
                FontSize = FontSise * siseWin,
                Width = MpWidth * siseWin,
                Height = MpHeight * siseWin,
                Margin = new Thickness(point.X + (type.Equals(false) ? 120 : 0), point.Y + (type.Equals(false) ? 120 : 0), 0, 0),//120(位置偏移量)
                Cursor = Cursors.Hand,
                Tag = ArID,
            };
            ControlRegulate.aAlignment(FontPosition, labelArea);
            if (Areaevent)
            {
                labelArea.MouseDown += LabelArea_MouseDown;
                labelArea.MouseMove += LabelArea_MouseMove;
                labelArea.MouseUp += LabelArea_MouseUp;
            }
            keyValuePairs.Add(ArID, labelArea);
            return labelArea;
        }


        /// <summary>
        /// 释放鼠标发送（工作区）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelArea_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (tongs)
                return;

            Label tmp = (Label)sender;
            tmp.ReleaseMouseCapture();
        }

        /// <summary>
        /// 移动鼠标发生（工作区）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (tongs)
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Label tmp = (Label)sender;
                double dx = e.GetPosition(null).X - jos.X + tmp.Margin.Left;
                double dy = e.GetPosition(null).Y - jos.Y + tmp.Margin.Top;
                if (dx > 0 && dy > 0 && dx < GetCanvas.Width - tmp .Width  && dy < GetCanvas.Height - tmp.Height )
                {
                    tmp.Margin = new Thickness(dx, dy, 0, 0);
                }
                jos = e.GetPosition(null);
            }
        }

        /// <summary>
        /// 按下鼠标发生（工作区）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tongs)
                return;

            Label tmp = (Label)sender;
            jos = e.GetPosition(null);
            tmp.CaptureMouse();
            tmp.Cursor = Cursors.Hand;
            if (e.RightButton == MouseButtonState.Pressed)
            {
                AreaAction(tmp);
            }
        }

        #endregion

        #region 动态生成文字

        /// <summary>
        /// 生成文字
        /// </summary>
        public void TextNew(Canvas mainPanel, Point point)
        {
            Label labelText = FontNew(point, null, TextInx, 35, "000000", false, true);
            mainPanel.Children.Add(labelText);
            GetCanvas = mainPanel;
            TextInx++;
        }


        public Label FontNew(Point point, string Text, int TextInx, double FontSise, string fontColor, bool type, bool Fontevent)
        {
            Label labelText = new Label()
            {
                Content = type.Equals(false) ? "文字" + TextInx.ToString() : Text,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + fontColor + "")),
                FontSize = FontSise,
                Margin = new Thickness(point.X + (type.Equals(false) ? 120 : 0), point.Y + (type.Equals(false) ? 120 : 0), 0, 0),//120(位置偏移量)
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Cursor = Cursors.Hand,
                Tag = TextInx,
            };
            if (Fontevent)
            {
                labelText.MouseDown += LabelText_MouseDown;
                labelText.MouseMove += LabelText_MouseMove;
                labelText.MouseUp += LabelText_MouseUp;
            }
            GetKeyValues.Add(TextInx, labelText);
            return labelText;
        }

        private void LabelText_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (tongs)
                return;

            Label tmp = (Label)sender;
            tmp.ReleaseMouseCapture();
        }

        private void LabelText_MouseMove(object sender, MouseEventArgs e)
        {
            if (tongs)
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Label tmp = (Label)sender;
                double dx = e.GetPosition(null).X - jos.X + tmp.Margin.Left;
                double dy = e.GetPosition(null).Y - jos.Y + tmp.Margin.Top;
                if (dx > 0 && dy > 0)
                {
                    tmp.Margin = new Thickness(dx, dy, 0, 0);
                }
                jos = e.GetPosition(null);
            }
        }

        private void LabelText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tongs)
                return;

            Label tmp = (Label)sender;
            jos = e.GetPosition(null);
            tmp.CaptureMouse();
            tmp.Cursor = Cursors.Hand;
            if (e.RightButton == MouseButtonState.Pressed)
            {

            }
        }




        #endregion

        #region 功能菜单


        /// <summary>
        /// NewTag
        /// </summary>
        public void TagNew()
        {
            PathStatic = false;
            var pairs = valuePairs.OrderByDescending(x => x.Key).ToArray();
            s = pairs.Count() > 0 ? pairs[0].Key : 0;
            s++;
            TagFormer();
            Label label = TagCreate(point, s, false, true);
            label.Background = new SolidColorBrush(Colors.Green);
            GetCanvas.Children.Add(label);

        }

        /// <summary>
        /// New Area
        /// </summary>
        public void AreaNew()
        {
            PathStatic = false;
            MapAreaNew(point);
            TagFormer();
        }




        /// <summary>
        /// 绘制直线
        /// </summary>
        public void DrawLine()
        {
            PathStatic = true;
            GetCircuitType = CircuitType.Line;
            TagFormer();
        }

        /// <summary>
        /// 折线
        /// </summary>
        public void BrokenLine()
        {
            PathStatic = true;
            GetCircuitType = CircuitType.Broken;
            TagFormer();
        }

        /// <summary>
        /// 水平对齐
        /// </summary>
        public void align()
        {
            PathStatic = true;
            GetCircuitType = CircuitType.Align;//信标对齐
            TagFormer();
        }

        /// <summary>
        /// 垂直对齐
        /// </summary>
        public void alignVe()
        {
            PathStatic = true;
            GetCircuitType = CircuitType.vertical;//垂直对齐
            TagFormer();
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void ClearTen()
        {
            PathStatic = true;
            GetCircuitType = CircuitType.Clear;//清除
            TagFormer();
        }

        /// <summary>
        /// 半圆
        /// </summary>
        public void Semicircle()
        {
            PathStatic = true;
            GetCircuitType = CircuitType.Semicircle;
            TagFormer();
        }


        /// <summary>
        /// 鼠标指针
        /// </summary>
        public void MousePointer()
        {
            PathStatic = false;
            TagFormer();
        }



        /// <summary>
        /// 画布鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Canvas tmp = (Canvas)sender;
            jos = e.GetPosition(null);
            tmp.CaptureMouse();
            tmp.Cursor = Cursors.Cross;
        }


        /// <summary>
        /// 画布鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Canvas tmp = (Canvas)sender;
                double dx = e.GetPosition(null).X - jos.X + tmp.Margin.Left;
                double dy = e.GetPosition(null).Y - jos.Y + tmp.Margin.Top;
                //if (dx > 0 && dy > 0)
                //{
                tmp.Margin = new Thickness(dx, dy, 0, 0);
                //}
                jos = e.GetPosition(null);
            }
        }

        /// <summary>
        /// 画布鼠标释放事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Canvas tmp = (Canvas)sender;
            tmp.ReleaseMouseCapture();
        }

        /// <summary>
        /// 画布移动
        /// </summary>
        public void ToolBarMap()
        {
            tongs = true;
            GetCanvas.MouseUp += MainPanel_MouseUp;
            GetCanvas.MouseMove += MainPanel_MouseMove;
            GetCanvas.MouseDown += MainPanel_MouseDown;
        }


        /// <summary>
        /// 根据AreaID查询字典中的Label
        /// </summary>
        /// <returns></returns>
        public Label AreaSelct(int AreaID)
        {
            foreach (int item in keyValuePairs.Keys)
            {
                if (item.Equals(AreaID))
                {
                    return keyValuePairs[item];
                }
            }
            return null;
        }


        /// <summary>
        /// 区域删除
        /// </summary>
        /// <param name="AreaID"></param>
        /// <returns></returns>
        public void ArDelete(int AreaID, Canvas GetCanvas)
        {
            GetCanvas.Children.Remove(keyValuePairs[AreaID]);
            keyValuePairs.Remove(AreaID);
        }


        #endregion

        #region 加载编辑地图数据

        /// <summary>
        /// 载入编辑地图数据
        /// </summary>
        /// <param name="Time"></param>
        public void LoadEditMap(long Time, bool event_type, bool tagtype)
        {
            siseWin = 1;
            Remove_LineDate();
            //painting.CoordinateX();
            LoadTag(Time, event_type);
            LoadLine(Time);
            WidgetLoad(Time, event_type);
            Mapmagnify(MapSise, tagtype);
        }

        /// <summary>
        /// 载入编辑地图数据
        /// </summary>
        /// <param name="Time"></param>
        public void LoadEditMap(long Time, double Width, double Height, bool event_type)
        {
            LoadTag(Time, event_type);
            LoadLine(Time);
            WidgetLoad(Time, event_type);
            Mapmagnify(MapSise, Width, Height);
        }

        public void Remove_LineDate()
        {
            wirePointArrays.Clear();
            keyValuePairs.Clear();
            valuePairs.Clear();
            GetKeyValues.Clear();
        }

        /// <summary>
        /// 加载所有Tag
        /// </summary>
        /// <param name="painti"></param>
        public void LoadTag(long Times, bool Tagevent)
        {
            string Key = $"Tag_{Times}";
            DataTable Data = CachePlant.GetResult(Key, () =>
            {
                return IO_AGVMapService.RataTable(Times.ToString());
            });
            foreach (DataRow item in Data.Rows)
            {
                int id = Convert.ToInt32(item["TagName"].ToString());
                TagCreate(new Point() { X = (Convert.ToDouble(item["X"].ToString()) * 10) - 19, Y = (Convert.ToDouble(item["Y"].ToString()) * 10) - 11.5 }, Convert.ToInt32(item["TagName"].ToString()), true, Tagevent);
            }
        }
        /// <summary>
        /// 载入线路
        /// </summary>
        /// <param name="Times"></param>
        public void LoadLine(long Times)
        {
            string Key = $"Line_{Times}";
            DataTable Data = CachePlant.GetResult(Key, () => { return IO_AGVMapService.LinelistArrer(Times.ToString()); });
            foreach (DataRow item in Data.Rows)
            {
                if (Convert.ToInt32(item["LineStyel"].ToString()) == 1)
                {
                    GetCircuitType = (CircuitType.Line);
                }
                else if (Convert.ToInt32(item["LineStyel"].ToString()) == 2)
                {
                    GetCircuitType = (CircuitType.Broken);
                }
                else
                {
                    GetCircuitType = (CircuitType.Semicircle);
                }
                Pairsarray.Add(new WirePoint() { TagID = Convert.ToInt32(item["Tag1"].ToString().Substring(2)), SetPoint = new Point() { X = Convert.ToDouble(item["StartX"].ToString()) * 10, Y = Convert.ToDouble(item["StartY"].ToString()) * 10 } });
                Pairsarray.Add(new WirePoint() { TagID = Convert.ToInt32(item["Tag2"].ToString().Substring(2)), SetPoint = new Point() { X = Convert.ToDouble(item["EndX"].ToString()) * 10, Y = Convert.ToDouble(item["EndY"].ToString()) * 10 } });
                AddLine();
                Pairsarray.Clear();
            }
        }

        /// <summary>
        /// 载入区域和文字
        /// </summary>
        /// <param name="Times"></param>
        public void WidgetLoad(long Times, bool Widgetevent)
        {
            string Key = $" Widget_{Times}";
            DataTable Data = CachePlant.GetResult(Key, () => { return IO_AGVMapService.GetWidget(Times.ToString()); });
            int Indexs = 0;
            foreach (DataRow item in Data.Rows)
            {
                if (item["WidgetNo"].ToString().Substring(0, 2).Equals("AR"))
                {
                    NewArea((new Point() { X = Convert.ToDouble(item["X"].ToString()) * 10, Y = Convert.ToDouble(item["Y"].ToString()) * 10 }), item["Name"].ToString(), Indexs, item["BackColor"].ToString(), item["ForeColor"].ToString(), item["BorderColor"].ToString(), Convert.ToDouble(item["FontSize"].ToString()), Convert.ToDouble(item["Width"].ToString()) * 10, Convert.ToDouble(item["Height"].ToString()) * 10, true, item["FontPosition"].ToString(), Widgetevent);
                    Indexs++;
                    index = Indexs;
                }
                else if (item["WidgetNo"].ToString().Substring(0, 2).Equals("TE"))
                {
                    FontNew((new Point() { X = Convert.ToDouble(item["X"].ToString()) * 10, Y = Convert.ToDouble(item["Y"].ToString()) * 10 }), item["Name"].ToString(), Indexs, Convert.ToDouble(item["FontSize"].ToString()), item["ForeColor"].ToString(), true, Widgetevent);
                    Indexs++;
                    TextInx = Indexs;
                }
            }
            TextInx++;
            index++;
        }

        #endregion

        #region 比例尺缩放

        /// <summary>
        /// 地图比例尺缩放
        /// </summary>
        /// <param name="Size"></param>
        /// <param name="mainPanel"></param>
        /// <param name="mainPane2"></param>
        public void Mapmagnify(double Size, double Width, double Height)
        {
            GetCanvas.Children.Clear();
            Main_ChangeSize(Width * Size, Height * Size);
            painting.Canvas_X = 10 * Size;
            painting.Canvas_Y = 10 * Size;
            painting.CoordinateX();
            painting.Coordinate();
            Zoom(Size, GetCanvas, true);
            siseWin = Size;
        }

        /// <summary>
        /// 地图比例尺缩放
        /// </summary>
        /// <param name="Size"></param>
        /// <param name="mainPanel"></param>
        /// <param name="mainPane2"></param>
        public void Mapmagnify(double Size, double Width, double Height, bool type)
        {
            GetCanvas.Children.Clear();
            Main_ChangeSize(Width * Size, Height * Size);
            painting.Canvas_X = 10 * Size;
            painting.Canvas_Y = 10 * Size;
            painting.CoordinateX();
            painting.Coordinate();
            Zoom(Size, GetCanvas, type);
            siseWin = Size;
        }

        /// <summary>
        /// 地图比例尺缩放
        /// </summary>
        /// <param name="Size"></param>
        /// <param name="mainPanel"></param>
        /// <param name="mainPane2"></param>
        public void Mapmagnify(double Size, bool type)
        {
            GetCanvas.Children.Clear();
            painting.CoordinateX();
            painting.Coordinate();
            Zoom(Size, GetCanvas, type);
            siseWin = Size;
        }


        public double siseWin = 1;//上一次缩放大小

        /// <summary>
        /// 比例尺控件缩放
        /// </summary>
        /// <param name="Sise"></param>
        public void Zoom(double Sise, Canvas mainPan, bool tagMap)
        {
            foreach (WirePointArray item in wirePointArrays)//线路缩放
            {
                Path path = null;
                List<Path> Kaths = null;
                Point Henpoint = item.GetPoint.SetPoint;
                Henpoint.X = ((Henpoint.X / siseWin) * Sise);
                Henpoint.Y = (((Henpoint.Y) / siseWin) * Sise);
                item.GetPoint.SetPoint = Henpoint;
                Point point = item.GetWirePoint.SetPoint;
                point.X = (((point.X) / siseWin)) * Sise;
                point.Y = (((point.Y) / siseWin) * Sise);
                item.GetWirePoint.SetPoint = point;
                if (item.circuitType.Equals(CircuitType.Line))//直线
                {
                    path = painting.Line(item.GetPoint.SetPoint, item.GetWirePoint.SetPoint);
                    path.StrokeThickness = ((WirePointLine)item).GetPath.StrokeThickness;
                    path.Stroke = ((WirePointLine)item).GetPath.Stroke;
                    ((WirePointLine)item).GetPath = path;
                }
                else if (item.circuitType.Equals(CircuitType.Semicircle))//半圆
                {
                    path = painting.DrawingSemicircle(item.GetPoint.SetPoint, item.GetWirePoint.SetPoint);
                    path.StrokeThickness = ((WirePointLine)item).GetPath.StrokeThickness;
                    path.Stroke = ((WirePointLine)item).GetPath.Stroke;
                    ((WirePointLine)item).GetPath = path;
                }
                else if (item.circuitType.Equals(CircuitType.Broken))//折线
                {
                    Kaths = painting.DrawingBroken(item.GetPoint.SetPoint, item.GetWirePoint.SetPoint);
                    foreach (var ite in Kaths)
                    {
                        foreach (var it in ((WirePointBroken)item).Paths)
                        {
                            ite.StrokeThickness = it.StrokeThickness;
                            ite.Stroke = it.Stroke;
                        }
                    }
                     ((WirePointBroken)item).Paths = Kaths;
                }


            }
            foreach (int item in valuePairs.Keys)//信标缩放
            {
                valuePairs[item].Margin = new Thickness(((valuePairs[item].Margin.Left + 19) / siseWin) * Sise - 19, ((valuePairs[item].Margin.Top + 11.5) / siseWin) * Sise - 11.5, 0, 0);
                if (tagMap)
                    mainPan.Children.Add(valuePairs[item]);
            }
            foreach (int item in keyValuePairs.Keys)//区域缩放
            {
                keyValuePairs[item].Margin = new Thickness((keyValuePairs[item].Margin.Left / siseWin) * Sise, (keyValuePairs[item].Margin.Top / siseWin) * Sise, 0, 0);
                //长宽计算
                keyValuePairs[item].Width = keyValuePairs[item].Width / siseWin * Sise;
                keyValuePairs[item].Height = keyValuePairs[item].Height / siseWin * Sise;
                //字体计算
                keyValuePairs[item].FontSize = keyValuePairs[item].FontSize / siseWin * Sise;
                mainPan.Children.Add(keyValuePairs[item]);
            }

            foreach (int item in GetKeyValues.Keys)//文字缩放
            {
                GetKeyValues[item].Margin = new Thickness((GetKeyValues[item].Margin.Left / siseWin) * Sise, (GetKeyValues[item].Margin.Top / siseWin) * Sise, 0, 0);
                GetKeyValues[item].FontSize = GetKeyValues[item].FontSize / siseWin * Sise;
                mainPan.Children.Add(GetKeyValues[item]);
            }
        }

        #endregion

        #region 实时绘制AGV位置

        public void DrawShowTag(int TagIndex)
        {
            Label label = valuePairs.FirstOrDefault(x => x.Key.Equals(TagIndex)).Value;
        }

        #endregion

        #region 保存地图

        /// <summary>
        /// 保存地图
        /// </summary>
        /// <param name="Times">UTC</param>
        /// <param name="type">是否是新建地图</param>
        /// <param name="Name">地图名称</param>
        /// <param name="Width">地图宽度</param>
        /// <param name="Height">地图高度</param>
        /// <returns></returns>
        public bool MapPreserve(string Times, bool type, string Name, double Width, double Height)
        {
            return IO_AGVMapService.SaveMapInfo(Times, type, Name, Width / 10, Height / 10, "0", 0, MapSise, valuePairs, keyValuePairs, GetKeyValues, wirePointArrays);
        }

        #endregion

        #region 初始化容器大小

        public void Initial_Canvas(Canvas mainPane_X, Canvas mainPane_Y, Canvas Canvas_Main, double Width, double Height)
        {
            this.GetCanvas = Canvas_Main;
            painting.InitializeCanvas(mainPane_X, mainPane_Y, Canvas_Main, Width, Height);
        }

        public void Main_ChangeSize(double Width, double Height)
        {
            this.GetCanvas.Width = Width;
            this.GetCanvas.Height = Height;
            painting.Change_Size(Width, Height);
        }
        #endregion

        #region 绘制画布刻度
        public void Canvas_Draw()
        {
            painting.Coordinate();
            painting.CoordinateX();
        }
        #endregion

        #region 线路标记红色

        public void SignLine(List<WirePointArray> pointArrays, Brush brushes, double LintWidth,int Index)
        {
            pointArrays.ForEach(
                    p =>
                    {
                        if (p.circuitType.Equals(CircuitType.Line) || p.circuitType.Equals(CircuitType.Semicircle))
                        {
                            Path path = ((WirePointLine)p).GetPath;
                            path.Stroke = brushes;
                            path.StrokeThickness = LintWidth;
                            Panel.SetZIndex(path, Index);
                        }
                        else if (p.circuitType.Equals(CircuitType.Broken))
                        {
                            List<Path> path = ((WirePointBroken)p).Paths;
                            path.ForEach(x => { x.Stroke = brushes; x.StrokeThickness = LintWidth; Panel.SetZIndex(x, Index); });
                        }
                    });
        }

        #endregion

        public Path Draw_Triangle(CircuitType type, Point startPt, Point endPt, bool direction)
        {
            return painting.DrawArrow(type, startPt, endPt, direction);
        }
    }
}
