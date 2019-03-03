using System.Text.RegularExpressions;

namespace AGVSystem.Infrastructure.agvCommon
{
    public class FormatVerification
    {
        /// <summary>
        /// 匹配是否为数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsFloat(string str)
        {
            string regextext = @"^(-?\d+)(\.\d+)?$";
            Regex regex = new Regex(regextext, RegexOptions.None);
            return regex.IsMatch(str.Trim());
        }

        /// <summary>
        /// 判断字符串中是否包含中文
        /// </summary>
        /// <param name="str">需要判断的字符串</param>
        /// <returns>判断结果</returns>
        public static bool HasChinese(string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fa5]|[a-zA-Z]");
        }

        /// <summary>
        /// 匹配是否为IP
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool HasIP(string str)
        {
            return Regex.IsMatch(str, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        /// <summary>
        /// 匹配是否为小数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool Strfloat(string str)
        {
            return Regex.IsMatch(str, "^([0-9]{1,}[.][0-9]*)$");
        }
    }
}
