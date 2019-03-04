using AGVSystem.Infrastructure.agvCommon;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AGVSystem.UI.APP_UI.Edit
{
    /// <summary>
    /// Tag.xaml 的交互逻辑
    /// </summary>
    public partial class Tag : Window
    {
        private Label ObjTag;
        private double MapSize;
        public Action<Point, int> action; //信标移动委托
        public Action<int> ClearLine;  //清除线路委托
        public Func<int, int, bool> exist; //判断编号是否存在委托
        public Action<Label> ClearTag;

        public Tag(Label label,double Size)
        {
            InitializeComponent();
            ObjTag = label;
            MapSize = Size;
            EditLoad();
        }

        private void EditLoad()
        {
            TagNum.Text = ObjTag.Content.ToString();
            Tag_X.Text = (ObjTag.Margin.Left / MapSize).ToString();
            Text_Y.Text = (ObjTag.Margin.Top / MapSize).ToString();
        }

        private void SaveTag_Click(object sender, RoutedEventArgs e)
        {
            if (FormatVerification.IsFloat(TagNum.Text.ToString()) && FormatVerification.IsFloat(Tag_X.Text.ToString()) && FormatVerification.IsFloat(Text_Y.Text.ToString()))
            {
                if (!exist(Convert.ToInt32(ObjTag.Content.ToString()), Convert.ToInt32(TagNum.Text.ToString().Trim())) || ObjTag.Content.ToString().Equals(TagNum.Text.ToString()))
                {
                    ClearLine(Convert.ToInt32(ObjTag.Content.ToString()));
                    ObjTag.Margin = new Thickness(Convert.ToDouble(Tag_X.Text.Trim()) * MapSize, Convert.ToDouble(Text_Y.Text.Trim()) * MapSize, 0, 0);
                    ObjTag.Content = TagNum.Text.ToString();
                    action(new Point() { X = ObjTag.Margin.Left + 19, Y = ObjTag.Margin.Top + 11.5 }, Convert.ToInt32(ObjTag.Content));
                    this.Close();
                }
                else
                {
                    MessageBox.Show("编号已存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                MessageBox.Show("格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteTag_Click(object sender, RoutedEventArgs e)
        {
            ClearTag(ObjTag);
            this.Close();
        }
    }
}
