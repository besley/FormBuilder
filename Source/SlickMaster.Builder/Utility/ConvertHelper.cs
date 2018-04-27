using System;
using System.Text;
using Microsoft.International.Converters.PinYinConverter;
using System.Collections.ObjectModel;

namespace SlickMaster.Builder.Utility
{
    /// <summary>
    /// 转换帮助类
    /// </summary>
    static class ConvertHelper
    {
        /// <summary>
        /// 返回单个简体中文字的拼音列表
        /// </summary>
        /// <param name="inputChar">简体中文单字</param>      
        public static ReadOnlyCollection<string> GetPinYinWithTone(Char inputChar)
        {
            ChineseChar chineseChar = new ChineseChar(inputChar);
            return chineseChar.Pinyins;
        }

        /// <summary>
        /// 返回单个简体中文字的拼音个数
        /// </summary>
        /// <param name="inputChar">简体中文单字</param>      
        public static short GetPinYinCount(Char inputChar)
        {
            ChineseChar chineseChar = new ChineseChar(inputChar);
            return chineseChar.PinyinCount;
        }

        /// <summary>
        /// 返回单个简体中文字拼音列表中的第一个拼音
        /// </summary>
        /// <param name="inputChar">简体中文单字</param>      
        public static string GetFirstPinYinCount(Char inputChar)
        {
            //得到第一个拼音
            return GetPinYinWithTone(inputChar)[0];
        }

    }
}
