using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Reflection;

namespace SlickQua.Toolkit.Entity
{
    class PinyinDictionary
    {
        //定义词典
        private Dictionary<string, string> dictionary;
        public Dictionary<string, string> Dictionary
        {
            get { return dictionary; }
        }

        /// <summary>
        /// 构造函数将生成词典
        /// </summary>
        /// <param name="filename">词典文件的路径</param>
        public PinyinDictionary()
        {
            //指定储存字典的对象
            dictionary = new Dictionary<string, string>();

            //用于临时存储的键值字符串
            string cn = "";
            string pinyin = "";

            //尝试读取指定的xml字典文件
            var strDictionary = ReadXmlResource();
            using (XmlTextReader reader = new XmlTextReader(strDictionary))
            {
                //循环读取节点，直到文件末尾
                while (reader.Read())
                {
                    //判断节点类型，只处理Element
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        //如果读取到指定的节点名称，就把节点内容，复制到键值对中
                        switch (reader.LocalName)
                        {
                            case "InputString":
                                pinyin = reader.ReadElementContentAsString();
                                break;
                            case "OutputString":
                                cn = reader.ReadElementContentAsString();
                                break;
                            case "DictionaryEntry":
                                {
                                    //如果键值对都不为空，就添加到字典中
                                    if (!string.IsNullOrEmpty(cn) && !string.IsNullOrEmpty(pinyin))
                                    {
                                        dictionary.Add(cn, pinyin);
                                        //添加后，键值对清空，防止误添加
                                        cn = "";
                                        pinyin = "";
                                    }
                                }
                                break;
                        }
                    }
                }
            }
                

        }

        /// <summary>
        /// 根据给出的中文词汇，在字典中查找对应拼音。
        /// 找不到就返回null。
        /// 注意：dictionary不能为空
        /// </summary>
        /// <param name="cn"></param>
        /// <returns>中文词汇，对应的拼音</returns>
        public string GetPinyin (string cn)
        {
            //中文不能为空
            if (string.IsNullOrEmpty(cn))
                return null;

            //词典不能为空
            if (dictionary == null)
                return null;

            //词典必须有内容
            if (dictionary.Count == 0)
                return null;

            //在字典中查找指定中文的拼音，找不到就返回null
            if (dictionary.ContainsKey(cn))
                return dictionary[cn];
            else
                return null;
        }

        /// <summary>
        /// 根据中文词汇，通过查字典，得到拆分后，每个中文单字对应的拼音
        /// </summary>
        /// <param name="cn"></param>
        /// <returns>每个单字对应拼音，组成的字典</returns>
        public Dictionary<char, string> GetCnCharPinyin(string cn)
        {
            //中文不能为空
            if (string.IsNullOrEmpty(cn))
                return null;

            //词典不能为空
            if (dictionary == null)
                return null;

            //词典必须有内容
            if (dictionary.Count == 0)
                return null;

            //如果在字典中不存在指定词条，就返回null
            if (!dictionary.ContainsKey(cn))
                return null;

            //定义保存中文单字和对应拼音的词典
            Dictionary<char, string> cnCharPinyin = new Dictionary<char,string>();


            //把词汇对应的拼音，拆分成单个中文字的拼音
            string[] pinyins = dictionary[cn].Split(' ');

            //把中文词汇拆分成单字
            char[] cnChars = cn.ToCharArray();

            //将单字和拼音对应起来，添加到单字拼音字典
            for (int i = 0; i< cn.Length; i++)
            {
                cnCharPinyin.Add(cnChars[i], pinyins[i]);
            }

            //返回这个词汇的单字拼音字典
            return cnCharPinyin;
        }

        /// <summary>
        /// 读取字库XML资源
        /// </summary>
        /// <returns></returns>
        private string ReadXmlResource()
        {
            string result = string.Empty;
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "SlickQua.Toolkit.IMEResource.xml";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }



    }
}
