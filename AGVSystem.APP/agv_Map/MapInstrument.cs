using AGVSystem.Model.DrawMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using AGVSystem.BLL.ServiceLogicBLL;

namespace AGVSystem.APP.agv_Map
{
    public class MapInstrument
    {
        private int s = 1;//信标起始索引
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
        public Dictionary<int, WirePointArray> wires = new Dictionary<int, WirePointArray>();//暂时存放拖动时所用关联线路（线路起始点和结束点存在共用情况需做判断）


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
                        //foreach (WirePoint item in Pairsarray)//重新添加Tag 
                        //{
                        //    GetCanvas.Children.Add(valuePairs[item.TagID]);
                        //}

                    }
                    Pairsarray.Clear();//移除暂存起始结束点
                }
                else if (GetCircuitType.Equals(CircuitType.Clear))
                {
                    ClearCircuit();//清除路线
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
        public void ClearCircuit()
        {
            //double ey = Pairsarray[0].SetPoint.X - Pairsarray[1].SetPoint.X; //大于0下方上方小于0上方
            foreach (WirePointArray item in wirePointArrays.ToArray())
            {
                if ((item.GetPoint.TagID.Equals(Pairsarray[0].TagID) && item.GetWirePoint.TagID.Equals(Pairsarray[1].TagID)) || (item.GetPoint.TagID.Equals(Pairsarray[1].TagID) && item.GetWirePoint.TagID.Equals(Pairsarray[0].TagID)))
                {
                    //if (item.circuitType.Equals(CircuitType.Broken))
                    //{
                    CrearS(item);
                    break;
                    //}
                    //else if (item.circuitType.Equals(CircuitType.Semicircle))
                    //{
                    //    CrearS(item);
                    //    break;
                    //}
                    //else
                    //{
                    //    CrearS(item);
                    //    break;
                    //}
                }
            }
            Pairsarray.Clear();
        }

        public void CrearS(WirePointArray item)
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
                    path = painting.Line(wires[item].GetPoint.SetPoint, wires[item].GetWirePoint.SetPoint, GetCanvas);
                    ((WirePointLine)wirePointArrays[item]).GetPath = path;//更新线路对象
                }
                else if (wires[item].circuitType.Equals(CircuitType.Semicircle))//半圆
                {
                    path = painting.DrawingSemicircle(wires[item].GetPoint.SetPoint, wires[item].GetWirePoint.SetPoint, GetCanvas);
                    ((WirePointLine)wirePointArrays[item]).GetPath = path;//更新线路对象
                }
                else if (wires[item].circuitType.Equals(CircuitType.Broken))//折线
                {
                    Genpaths = painting.DrawingBroken(wires[item].GetPoint.SetPoint, wires[item].GetWirePoint.SetPoint, GetCanvas);
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
                Path path = painting.Line(Pairsarray[0].SetPoint, Pairsarray[1].SetPoint, GetCanvas);//绘制直线
                wirePointArrays.Add(new WirePointLine { GetPoint = Pairsarray[0], GetWirePoint = Pairsarray[1], GetPath = path, circuitType = CircuitType.Line });//将绘制的线路添加到线路集合中
            }
            else if (GetCircuitType.Equals(CircuitType.Semicircle))
            {
                double ey = Pairsarray[0].SetPoint.X - Pairsarray[1].SetPoint.X; //大于0下方上方小于0上方
                Path path = painting.DrawingSemicircle(Pairsarray[0].SetPoint, Pairsarray[1].SetPoint, GetCanvas);//绘制半圆
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
                List<Path> Pathr = painting.DrawingBroken(Pairsarray[0].SetPoint, Pairsarray[1].SetPoint, GetCanvas);
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
            if (e.LeftButton == MouseButtonState.Released)
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

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Label tmp = (Label)sender;
                pos = e.GetPosition(null);
                tmp.CaptureMouse();
                tmp.Cursor = Cursors.Hand;
            }


            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (PathStatic)
                    return;


                MessageBox.Show("");
                //TagRedact mapRedact = new TagRedact(Convert.ToInt32(tmp.Tag), GetCanvas);
                //mapRedact.GetMovement += MouseMove;
                //mapRedact.ShowDialog();
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
            aAlignment(FontPosition, labelArea);
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
                if (dx > 0 && dy > 0)
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
                //MapRedact mapRedact = new MapRedact(Convert.ToInt32(tmp.Tag), GetCanvas);
                //mapRedact.ShowDialog();
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

        #endregion

        #region 加载编辑地图数据

        /// <summary>
        /// 载入编辑地图数据
        /// </summary>
        /// <param name="Time"></param>
        public void LoadEditMap(long Time, bool event_type, bool tagtype)
        {
            wirePointArrays.Clear();
            keyValuePairs.Clear();
            valuePairs.Clear();
            GetKeyValues.Clear();
            siseWin = 1;
            painting.Line_Width = 3;
            LoadTag(Time, event_type);
            LoadLine(Time);
            WidgetLoad(Time, event_type);

            Mapmagnify(MapSise, tagtype);
        }

        /// <summary>
        /// 加载所有Tag
        /// </summary>
        /// <param name="painti"></param>
        public void LoadTag(long Times, bool Tagevent)
        {
            MySqlDataReader item = IO_AGVMapService.RataTable(Times.ToString());
            while (item.Read())
            {
                int id = Convert.ToInt32(item["TagName"].ToString());
                TagCreate(new Point() { X = (Convert.ToDouble(item["X"].ToString()) * 10) - 19, Y = (Convert.ToDouble(item["Y"].ToString()) * 10) - 11.5 }, Convert.ToInt32(item["TagName"].ToString()), true, Tagevent);
            }
            item.Close();
        }
        /// <summary>
        /// 载入线路
        /// </summary>
        /// <param name="Times"></param>
        public void LoadLine(long Times)
        {
            MySqlDataReader item = IO_AGVMapService.LinelistArrer(Times.ToString());
            while (item.Read())
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
            item.Close();
        }

        /// <summary>
        /// 载入区域和文字
        /// </summary>
        /// <param name="Times"></param>
        public void WidgetLoad(long Times, bool Widgetevent)
        {
            MySqlDataReader item = IO_AGVMapService.GetWidget(Times.ToString());
            int Indexs = 0;
            while (item.Read())
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
            GetCanvas.Width = Width * Size;
            GetCanvas.Height = Height * Size;
            painting.Canvas_X = 10 * Size;
            painting.Canvas_Y = 10 * Size;
            painting.Coordinate(GetCanvas);
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
            GetCanvas.Width = Width * Size;
            GetCanvas.Height = Height * Size;
            painting.Canvas_X = 10 * Size;
            painting.Canvas_Y = 10 * Size;
            painting.Coordinate(GetCanvas);
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
            painting.Coordinate(GetCanvas);
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
                    path = painting.Line(item.GetPoint.SetPoint, item.GetWirePoint.SetPoint, mainPan);
                    path.StrokeThickness = ((WirePointLine)item).GetPath.StrokeThickness;
                    path.Stroke = ((WirePointLine)item).GetPath.Stroke;
                    ((WirePointLine)item).GetPath = path;
                }
                else if (item.circuitType.Equals(CircuitType.Semicircle))//半圆
                {
                    path = painting.DrawingSemicircle(item.GetPoint.SetPoint, item.GetWirePoint.SetPoint, mainPan);
                    path.StrokeThickness = ((WirePointLine)item).GetPath.StrokeThickness;
                    path.Stroke = ((WirePointLine)item).GetPath.Stroke;
                    ((WirePointLine)item).GetPath = path;
                }
                else if (item.circuitType.Equals(CircuitType.Broken))//折线
                {
                    Kaths = painting.DrawingBroken(item.GetPoint.SetPoint, item.GetWirePoint.SetPoint, mainPan);
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

        #region 获取/设置字体对齐方式

        /// <summary>
        /// 区域文字对齐方式
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public string aAlignment(Label label)
        {
            if (label.HorizontalContentAlignment.Equals(HorizontalAlignment.Center) && label.VerticalContentAlignment.Equals(VerticalAlignment.Center))
            {
                return "居中对齐";
            }
            else if (label.HorizontalContentAlignment.Equals(HorizontalAlignment.Left) && label.VerticalContentAlignment.Equals(VerticalAlignment.Center))
            {
                return "靠左对齐";
            }
            else if (label.HorizontalContentAlignment.Equals(HorizontalAlignment.Right) && label.VerticalContentAlignment.Equals(VerticalAlignment.Center))
            {
                return "靠右对齐";
            }
            else if (label.HorizontalContentAlignment.Equals(HorizontalAlignment.Center) && label.VerticalContentAlignment.Equals(VerticalAlignment.Top))
            {
                return "顶部对齐";
            }
            else if (label.HorizontalContentAlignment.Equals(HorizontalAlignment.Center) && label.VerticalContentAlignment.Equals(VerticalAlignment.Bottom))
            {
                return "底部对齐";
            }
            else
            {
                return "居中对齐";
            }
        }


        /// <summary>
        /// 设置文字对齐方式
        /// </summary>
        /// <param name="alignment">对齐方式</param>
        /// <param name="label">控件</param>
        public void aAlignment(string alignment, Label label)
        {
            if (alignment.Equals("居中对齐") || alignment.Equals("水平居中"))
            {
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.VerticalContentAlignment = VerticalAlignment.Center;
            }
            else if (alignment.Equals("靠左对齐"))
            {
                label.HorizontalContentAlignment = HorizontalAlignment.Left;
                label.VerticalContentAlignment = VerticalAlignment.Center;
            }
            else if (alignment.Equals("靠右对齐"))
            {
                label.HorizontalContentAlignment = HorizontalAlignment.Right;
                label.VerticalContentAlignment = VerticalAlignment.Center;
            }
            else if (alignment.Equals("顶部对齐"))
            {
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.VerticalContentAlignment = VerticalAlignment.Top;
            }
            else if (alignment.Equals("底部对齐"))
            {
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.VerticalContentAlignment = VerticalAlignment.Bottom;
            }
        }

        #endregion

        #region 获取/设置背景/字体颜色


        /// <summary>
        /// 区域背景/字体颜色
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public string AreaColor(string bgColor)
        {
            if (bgColor.Equals((Colors.White).ToString()))
            {
                return "白色";
            }
            else if (bgColor.Equals((Colors.Black).ToString()))
            {
                return "黑色";
            }
            else if (bgColor.Equals((Colors.Red).ToString()))
            {
                return "红色";
            }
            else if (bgColor.Equals((Colors.Orange).ToString()))
            {
                return "橙色";
            }
            else if (bgColor.Equals((Colors.Yellow).ToString()))
            {
                return "黄色";
            }
            else if (bgColor.Equals((Colors.Green).ToString()))
            {
                return "绿色";
            }
            else if (bgColor.Equals((Colors.Cyan).ToString()))
            {
                return "青色";
            }
            else if (bgColor.Equals((Colors.Blue).ToString()))
            {
                return "蓝色";
            }
            else if (bgColor.Equals((Colors.Violet).ToString()))
            {
                return "紫色";
            }
            return "";
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="color">颜色名称</param>
        public void AreaColor(Control control, string color, Colortype existx)
        {
            if (color.Equals("白色"))
            {
                if (existx.Equals(Colortype.FontColor))
                {
                    control.Foreground = new SolidColorBrush(Colors.White);
                }
                else if (existx.Equals(Colortype.BgColor))
                {
                    control.Background = new SolidColorBrush(Colors.White);
                }
                else if (existx.Equals(Colortype.BrColor))
                {
                    control.BorderBrush = new SolidColorBrush(Colors.White);
                }
            }
            else if (color.Equals("黑色"))
            {
                if (existx.Equals(Colortype.FontColor))
                {
                    control.Foreground = new SolidColorBrush(Colors.Black);
                }
                else if (existx.Equals(Colortype.BgColor))
                {
                    control.Background = new SolidColorBrush(Colors.Black);
                }
                else if (existx.Equals(Colortype.BrColor))
                {
                    control.BorderBrush = new SolidColorBrush(Colors.Black);
                }
            }
            else if (color.Equals("红色"))
            {
                if (existx.Equals(Colortype.FontColor))
                {
                    control.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (existx.Equals(Colortype.BgColor))
                {
                    control.Background = new SolidColorBrush(Colors.Red);
                }
                else if (existx.Equals(Colortype.BrColor))
                {
                    control.BorderBrush = new SolidColorBrush(Colors.Red);
                }
            }
            else if (color.Equals("橙色"))
            {
                if (existx.Equals(Colortype.FontColor))
                {
                    control.Foreground = new SolidColorBrush(Colors.Orange);
                }
                else if (existx.Equals(Colortype.BgColor))
                {
                    control.Background = new SolidColorBrush(Colors.Orange);
                }
                else if (existx.Equals(Colortype.BrColor))
                {
                    control.BorderBrush = new SolidColorBrush(Colors.Orange);
                }
            }
            else if (color.Equals("黄色"))
            {
                if (existx.Equals(Colortype.FontColor))
                {
                    control.Foreground = new SolidColorBrush(Colors.Yellow);
                }
                else if (existx.Equals(Colortype.BgColor))
                {
                    control.Background = new SolidColorBrush(Colors.Yellow);
                }
                else if (existx.Equals(Colortype.BrColor))
                {
                    control.BorderBrush = new SolidColorBrush(Colors.Yellow);
                }
            }
            else if (color.Equals("绿色"))
            {
                if (existx.Equals(Colortype.FontColor))
                {
                    control.Foreground = new SolidColorBrush(Colors.Green);
                }
                else if (existx.Equals(Colortype.BgColor))
                {
                    control.Background = new SolidColorBrush(Colors.Green);
                }
                else if (existx.Equals(Colortype.BrColor))
                {
                    control.BorderBrush = new SolidColorBrush(Colors.Green);
                }
            }
            else if (color.Equals("青色"))
            {
                if (existx.Equals(Colortype.FontColor))
                {
                    control.Foreground = new SolidColorBrush(Colors.Cyan);
                }
                else if (existx.Equals(Colortype.BgColor))
                {
                    control.Background = new SolidColorBrush(Colors.Cyan);
                }
                else if (existx.Equals(Colortype.BrColor))
                {
                    control.BorderBrush = new SolidColorBrush(Colors.Cyan);
                }
            }
            else if (color.Equals("蓝色"))
            {
                if (existx.Equals(Colortype.FontColor))
                {
                    control.Foreground = new SolidColorBrush(Colors.Blue);
                }
                else if (existx.Equals(Colortype.BgColor))
                {
                    control.Background = new SolidColorBrush(Colors.Blue);
                }
                else if (existx.Equals(Colortype.BrColor))
                {
                    control.BorderBrush = new SolidColorBrush(Colors.Blue);
                }
            }
            else if (color.Equals("紫色"))
            {
                if (existx.Equals(Colortype.FontColor))
                {
                    control.Foreground = new SolidColorBrush(Colors.Violet);
                }
                else if (existx.Equals(Colortype.BgColor))
                {
                    control.Background = new SolidColorBrush(Colors.Violet);
                }
                else if (existx.Equals(Colortype.BrColor))
                {
                    control.BorderBrush = new SolidColorBrush(Colors.Violet);
                }
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

    }
}
