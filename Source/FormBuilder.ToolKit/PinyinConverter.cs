using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;
using SlickQua.Toolkit.Entity;
using SlickQua.Toolkit.Helper;

namespace SlickQua.Toolkit
{
    /// <summary>
    /// 汉字转拼音工具类
    /// </summary>
    public class PinyinConverter
    {
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
                outputStr.Append(StringHelper.CreateChar());

                //两位数字后缀
                Random rnd = new Random();
                int mark = rnd.Next(10, 99);
                outputStr.Append(mark.ToString());
            }
            return outputStr.ToString();
        }

        /// <summary>
        /// 汉字转拼音方法
        /// </summary>
        /// <param name="inputStr">输入汉字</param>
        /// <param name="isFirstChar">是否取首拼音</param>
        /// <param name="surfix">是否添加后缀数字</param>
        /// <returns></returns>
        private static string ConvertInternalWithSegment(string inputStr, bool isFirstChar, bool surfix = true)
        {
            //如果是英文字符串，直接返回
            if (CheckEnglishString(inputStr)) return inputStr;

            StringBuilder output = new StringBuilder(128);
            bool isCN = CheckCNString(inputStr);

            //如果不是全部中文字符串，则返回原字符串
            if (isCN == false) { return inputStr; }

            PinyinDictionary dict = new PinyinDictionary();
            List<string> wordList = dict.Dictionary.Keys.ToList<string>();
            List<string> wordsLeft = Segmentation.SegMMLeftToRight(inputStr, ref wordList);
            if (wordsLeft == null)
            {
                //throw new ApplicationException("汉字转拼音失败！");
                //随机生成的6位字符串代替
                var randomString = StringHelper.CreateString(6);
                return randomString;
            }

            string pinyin = "";
            foreach (string word in wordsLeft)
            {
                if (word.Length == 1 && !dict.Dictionary.ContainsKey(word))
                {
                    pinyin = ConvertHelper.GetFirstPinYinCount(word.ToCharArray()[0]).ToLower();
                }
                else
                {
                    pinyin = dict.Dictionary[word].ToLower();
                }
                pinyin = Regex.Replace(pinyin, @"\d", "");
                if (isFirstChar == true)
                {
                    output.Append(pinyin.Substring(0, 1));
                }
                else
                {
                    output.Append(pinyin);
                }
            }

            if (surfix == true)
            {
                Random rnd = new Random();
                int mark = rnd.Next(10, 99);
                output.Append(mark.ToString());
            }
            return output.ToString();
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
