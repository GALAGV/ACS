using AGVSystem.Infrastructure.agvCommon;
using AGVSystem.Model.DrawMap;
using System;
using System.Collections.Generic;
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

namespace AGVSystem.UI.APP_UI.Edit
{
    /// <summary>
    /// Area.xaml 的交互逻辑
    /// </summary>
    public partial class Area : Window
    {
        private Label GetLabel;
        public Action<int> ArDelete;
        private double SiseWin;
        public Area(Label label,double Size)
        {
            InitializeComponent();
            this.GetLabel = label;
            this.SiseWin = Size;
            AreaShow();
        }

        /// <summary>
        /// 显示区域信息
        /// </summary>
        private void AreaShow()
        {
            MpName.Text = GetLabel.Content.ToString();
            Fontsize.Text = (GetLabel.FontSize / SiseWin).ToString();
            DisX.Text = (GetLabel.Margin.Left / SiseWin).ToString();
            Algcetion.Text = ControlRegulate.aAlignment(GetLabel);
            DisY.Text = (GetLabel.Margin.Top / SiseWin).ToString();
            FontColor.Text = ControlRegulate.AreaColor(GetLabel.Foreground.ToString());
            ArWidth.Text = (GetLabel.Width / SiseWin).ToString();
            BgColor.Text = ControlRegulate.AreaColor(GetLabel.Background.ToString());
            ArHeight.Text = (GetLabel.Height / SiseWin).ToString();
            BrColor.Text = ControlRegulate.AreaColor(GetLabel.BorderBrush.ToString());
        }

        private void Btn_Submit_Click(object sender, RoutedEventArgs e)
        {
            if (!Formverify())
                return;

            GetLabel.Content = MpName.Text.Trim();
            GetLabel.FontSize = Convert.ToDouble(Fontsize.Text.Trim()) * SiseWin;
            GetLabel.Margin = new Thickness(Convert.ToDouble(DisX.Text.Trim()) * SiseWin, Convert.ToDouble(DisY.Text.Trim()) * SiseWin, 0, 0);
            ControlRegulate.aAlignment(Algcetion.Text.Trim(), GetLabel);
            ControlRegulate.AreaColor(GetLabel, FontColor.Text, Colortype.FontColor);
            ControlRegulate.AreaColor(GetLabel, BgColor.Text, Colortype.BgColor);
            ControlRegulate.AreaColor(GetLabel, BrColor.Text, Colortype.BrColor);
            GetLabel.Width = Convert.ToDouble(ArWidth.Text.Trim()) * SiseWin;
            GetLabel.Height = Convert.ToDouble(ArHeight.Text.Trim()) * SiseWin;
            this.Close();
        }


        #region 表单验证

        /// <summary>
        /// 表单验证
        /// </summary>
        /// <returns></returns>
        private bool Formverify()
        {
            if (MpName.Text.ToString().Trim().Equals(""))
            {
                MessageBox.Show("区域名称不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (Fontsize.Text.ToString().Trim().Equals(""))
            {
                MessageBox.Show("字体大小不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (DisX.Text.ToString().Trim().Equals(""))
            {
                MessageBox.Show("X轴距离不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (DisY.Text.ToString().Trim().Equals(""))
            {
                MessageBox.Show("Y轴距离不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (ArWidth.Text.ToString().Trim().Equals(""))
            {
                MessageBox.Show("区域长度不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (ArHeight.Text.ToString().Trim().Equals(""))
            {
                MessageBox.Show("区域宽度不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!FormatVerification.IsFloat(Fontsize.Text.ToString().Trim()))
            {
                MessageBox.Show("字体大小只能为数字", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!FormatVerification.IsFloat(DisX.Text.ToString().Trim()) || !FormatVerification.IsFloat(DisY.Text.ToString().Trim()) || !FormatVerification.IsFloat(ArWidth.Text.ToString().Trim()) || !FormatVerification.IsFloat(ArHeight.Text.ToString().Trim()))
            {
                MessageBox.Show("格式输入错误", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        #endregion

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            ArDelete.Invoke(Convert.ToInt32(GetLabel.Tag));
            this.Close();
        }
    }
}
