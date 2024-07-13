using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;


namespace FormMaster.Builder.Utility
{
    /// <summary>
    /// 汉字转拼音工具类
    /// </summary>
    public class PinyinConverter
    {
        static Random rd = new Random();
        /// <summary>
        /// 获取拼音首字母：
        /// 天下=>tx
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static string ConvertFirst(string inputStr)
        {
            string result = ConvertInternalSingle(inputStr, true);
            return result;
        }

        /// <summary>
        /// 获取全拼
        /// 天下=>tianxia
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static string ConvertWhole(string inputStr)
        {
            string result = ConvertInternalSingle(inputStr, false);
            return result;
        }

        /// <summary>
        /// 获取全拼，不分词
        /// 馄饨=>hun dun
        /// </summary>
        /// <param name="inputStr"></param>
        /// <param name="isFirstChar"></param>
        /// <param name="surfix"></param>
        /// <returns></returns>
        private static string ConvertInternalSingle(string inputStr, bool isFirstChar, bool surfix = true)
        {
            StringBuilder outputStr = new StringBuilder(128);
            char[] inputChar = inputStr.ToCharArray();
            string pinyin = string.Empty;
            foreach (char ch in inputChar)
            {
                if (CheckCNString(ch.ToString()) == true)
                {
                    //单字取拼音
                    pinyin = ConvertHelper.GetFirstPinYinCount(ch);
                    if (isFirstChar == true)
                    {
                        pinyin = pinyin.Substring(0, 1);
                    }
                    pinyin = Regex.Replace(pinyin, @"\d", "");
                    outputStr.Append(pinyin);
                }
                else
                {
                    outputStr.Append(ch);
                }
            }

            //添加后缀
            if (surfix == true)
            {
                //单字母后缀
                outputStr.Append(CreateChar());

                //两位数字后缀
                Random rnd = new Random();
                int mark = rnd.Next(10, 99);
                outputStr.Append(mark.ToString());
            }
            return outputStr.ToString();
        }

        /// <summary>
        /// 获取单个随机字母串
        /// </summary>
        /// <returns></returns>
        private static char CreateChar()
        {
            const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            char c = allowedChars[rd.Next(0, allowedChars.Length)];
            return c;
        }

        /// <summary>
        /// 判断是否为中文字串
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        private static bool CheckCNString(string inputStr)
        {
            //字符串不能为空
            bool isCN = false;
            if (string.IsNullOrEmpty(inputStr)) return isCN;

            //中文匹配
            string patternCN = @"^[\u4e00-\u9fa5\r\n]+$";

            if (Regex.IsMatch(inputStr, patternCN))
            {
                isCN = true;
            }
            return isCN;
        }

        /// <summary>
        /// 判断字符串是否为英文
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        private static bool CheckEnglishString(string inputStr)
        {
            bool isEn = false;
            Regex rex = new Regex("[a-z0-9A-Z_]+");
            Match ma = rex.Match(inputStr);
            if (ma.Success)
            {
                isEn = true;
            }
            return isEn;
        }
    }
}
