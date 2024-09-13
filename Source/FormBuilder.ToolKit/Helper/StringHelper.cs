using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlickQua.Toolkit.Helper
{
    /// <summary>
    /// 字符串帮助类
    /// </summary>
    public class StringHelper
    {
        static Random rd = new Random();
        internal static string CreateString(int stringLength)
        {
            //const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
            const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
            char[] chars = new char[stringLength];

            for (int i = 0; i < stringLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        /// <summary>
        /// 获取单个随机字母串
        /// </summary>
        /// <returns></returns>
        internal static char CreateChar()
        {
            const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            char c = allowedChars[rd.Next(0, allowedChars.Length)];
            return c;
        }
    }
}
