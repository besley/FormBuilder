using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlickQua.Toolkit.Helper
{
    /// <summary>
    /// 分词算法
    /// 引用开源代码：http://my.oschina.net/u/1270374/blog/164042?fromerr=atsCBM01
    /// </summary>
    static class Segmentation
    {
        /// <summary>
        /// 用最大匹配算法进行分词，正反向均可。
        /// 为了节约内存，词典参数是引用传递
        /// </summary>
        /// <param name="inputStr">要进行分词的字符串</param>
        /// <param name="wordList">词典</param>
        /// <param name="leftToRight">true为从左到右分词，false为从右到左分词</param>
        /// <param name="maxLength">每个分词的最大长度</param>
        /// <returns>储存了分词结果的字符串数组</returns>
        public static List<string> SegMM(string inputStr, ref List<string> wordList, bool leftToRight, int maxLength)
        {
            //指定词典不能为空
            if (wordList == null)
                return null;

            //指定要分词的字符串也不能为空
            if (string.IsNullOrEmpty(inputStr))
                return null;

            //取词的最大长度，必须大于0
            if (!(maxLength > 0))
                return null;

            //分词的方向，true=从左到右,false=从右到左

            //用于储存正向分词的字符串数组
            List<string> segWords = new List<string>();
            //用于储存逆向分词的字符串数组
            List<string> segWordsReverse = new List<string>();

            //用于尝试分词的取词字符串
            string word = "";


            //取词的当前长度
            int wordLength = maxLength;

            //分词操作中，处于字符串中的当前位置
            int position = 0;

            //分词操作中，已经处理的字符串总长度
            int segLength = 0;

            //开始分词，循环以下操作，直到全部完成
            while (segLength < inputStr.Length)
            {
                //如果还没有进行分词的字符串长度，小于取词的最大长度，则只在字符串长度内处理
                if ((inputStr.Length - segLength) < maxLength)
                    wordLength = inputStr.Length - segLength;
                //否则，按最大长度处理
                else
                    wordLength = maxLength;

                //从左到右 和 从右到左截取时，起始位置不同
                //刚开始，截取位置是字符串两头，随着不断循环分词，截取位置会不断推进
                if (leftToRight)
                    position = segLength;
                else
                    position = inputStr.Length - segLength - wordLength;

                //按照指定长度，从字符串截取一个词
                word = inputStr.Substring(position, wordLength);


                //在字典中查找，是否存在这样一个词
                //如果不包含，就减少一个字符，再次在字典中查找
                //如此循环，直到只剩下一个字为止
                while (!wordList.Contains(word))
                {
                    //如果只剩下一个单字，就直接退出循环
                    if (word.Length == 1)
                        break;

                    //把截取的字符串，最边上的一个字去掉
                    //注意，从左到右 和 从右到左时，截掉的字符的位置不同
                    if (leftToRight)
                        word = word.Substring(0, word.Length - 1);
                    else
                        word = word.Substring(1);
                }

                //将分出的词，加入到分词字符串数组中，正向和逆向不同
                if (leftToRight)
                    segWords.Add(word);
                else
                    segWordsReverse.Add(word);

                //已经完成分词的字符串长度，要相应增加
                segLength += word.Length;

            }

            //如果是逆向分词，还需要对分词结果反转排序
            if (!leftToRight)
            {
                for (int i = 0; i < segWordsReverse.Count; i++)
                {
                    //将反转的结果，保存在正向分词数组中
                    segWords.Add(segWordsReverse[segWordsReverse.Count - 1 - i]);
                }
            }

            //返回储存着正向分词的字符串数组
            return segWords;

        }

        /// <summary>
        /// 用最大匹配算法进行分词，正反向均可，每个分词最大长度是7。
        /// 为了节约内存，词典参数是引用传递
        /// </summary>
        /// <param name="inputStr">要进行分词的字符串</param>
        /// <param name="wordList">词典</param>
        /// <param name="leftToRight">true为从左到右分词，false为从右到左分词</param>
        /// <returns>储存了分词结果的字符串数组</returns>
        public static List<string> SegMM(string inputStr, ref List<string> wordList, bool leftToRight)
        {
            return SegMM(inputStr, ref wordList, leftToRight, 7);
        }

        /// <summary>
        /// 用最大匹配算法进行分词，正向，每个分词最大长度是7。
        /// 为了节约内存，词典参数是引用传递
        /// </summary>
        /// <param name="inputStr">要进行分词的字符串</param>
        /// <param name="wordList">词典</param>
        /// <returns>储存了分词结果的字符串数组</returns>
        public static List<string> SegMMLeftToRight(string inputStr, ref List<string> wordList)
        {
            return SegMM(inputStr, ref wordList, true, 7);
        }

        /// <summary>
        /// 用最大匹配算法进行分词，反向，每个分词最大长度是7。
        /// 为了节约内存，词典参数是引用传递
        /// </summary>
        /// <param name="inputStr">要进行分词的字符串</param>
        /// <param name="wordList">词典</param>
        /// <returns>储存了分词结果的字符串数组</returns>
        public static List<string> SegMMRightToLeft(string inputStr, ref List<string> wordList)
        {
            return SegMM(inputStr, ref wordList, false, 7);
        }

        /// <summary>
        /// 比较两个字符串数组，是否所有内容完全相等。
        /// 为了节约内存，参数是引用传递
        /// </summary>
        /// <param name="strList1">待比较字符串数组01</param>
        /// <param name="strList2">待比较字符串数组02</param>
        /// <returns>完全相同返回true</returns>
        private static bool CompStringList(ref List<string> strList1, ref List<string> strList2)
        {
            //待比较的字符串数组不能为空
            if (strList1 == null || strList2 == null)
                return false;

            //待比较的字符串数组长度不同，就说明不相等
            if (strList1.Count != strList2.Count)
                return false;
            else
            {
                //逐个比较数组中，每个字符串是否相同
                for (int i = 0; i < strList1.Count; i++)
                {
                    //只要有一个不同，就说明字符串不同
                    if (strList1[i] != strList2[i])
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 用最大匹配算法进行分词，双向，每个分词最大长度是7。
        /// 为了节约内存，字典参数是引用传递
        /// </summary>
        /// <param name="inputStr">要进行分词的字符串</param>
        /// <param name="wordList">词典</param>
        /// <returns>储存了分词结果的字符串数组</returns>
        public static List<string> SegMMDouble(string inputStr, ref List<string> wordList)
        {

            //用于储存分词的字符串数组
            //正向
            List<string> segWordsLeftToRight = new List<string>();

            //逆向
            List<string> segWordsRightToLeft = new List<string>();

            //定义拼接后的分词数组
            List<string> segWordsFinal = new List<string>();

            //用于保存需要拼接的左、右、中间分词碎块
            List<string> wordsFromLeft = new List<string>();
            List<string> wordsFromRight = new List<string>();
            List<string> wordsAtMiddle = new List<string>();

            //通过循环，进行正反向分词，如果有歧义，就截短字符串两头，继续分词，直到消除歧义，才结束
            //整个思路就像贪吃蛇，从两头，一直吃到中间，把一个字符串吃完。
            //
            //每次循环，得到正反向分词后，进行比较，判断是否有歧义
            //如果没有歧义，贪吃蛇就不用继续吃了，把分词结果保存，待会用于拼接
            //如果有歧义，就取正向分词的第一个词，和反向分词的最后一个词，拼接到最终分词结果的头尾
            //而输入字符串，则相应的揭短掉头尾，得到的子字符串，重新进行正反向分词
            //如此循环，直到完成整个输入字符串
            //
            //循环结束之后，就是把上面"贪吃蛇"吃掉的左、右分词结果以及没有歧义的中间分词结果，拼接起来。

            //进行正反向分词
            //正向
            segWordsLeftToRight = SegMMLeftToRight(inputStr, ref wordList);

            //逆向
            segWordsRightToLeft = SegMMRightToLeft(inputStr, ref wordList);

            //判断两头的分词拼接，是否已经在输入字符串的中间交汇，只要没有交汇，就不停循环
            while ((segWordsLeftToRight[0].Length + segWordsRightToLeft[segWordsRightToLeft.Count-1].Length) < inputStr.Length)
            {

                //如果正反向的分词结果相同，就说明没有歧义，可以退出循环了
                //正反向分词中，随便取一个，复制给middle的临时变量即可
                if (CompStringList(ref segWordsLeftToRight, ref segWordsRightToLeft))
                {
                    wordsAtMiddle = segWordsLeftToRight.ToList<string>();
                    break;
                }

                //如果正反向分词结果不同，则取分词数量较少的那个，不用再循环
                if (segWordsLeftToRight.Count < segWordsRightToLeft.Count)
                {
                    wordsAtMiddle = segWordsLeftToRight.ToList<string>();
                    break;
                }
                else if (segWordsLeftToRight.Count > segWordsRightToLeft.Count)
                {
                    wordsAtMiddle = segWordsRightToLeft.ToList<string>();
                    break;
                }

                //如果正反分词数量相同，则返回其中单字较少的那个，也不用再循环
                {
                    //计算正向分词结果中，单字的个数
                    int singleCharLeftToRight = 0;
                    for (int i = 0; i < segWordsLeftToRight.Count; i++)
                    {
                        if (segWordsLeftToRight[i].Length == 1)
                            singleCharLeftToRight++;
                    }

                    //计算反向分词结果中，单字的个数
                    int singleCharRightToLeft = 0;
                    for (int j = 0; j < segWordsRightToLeft.Count; j++)
                    {
                        if (segWordsRightToLeft[j].Length == 1)
                            singleCharRightToLeft++;
                    }

                    //比较单字个数多少，返回单字较少的那个
                    if (singleCharLeftToRight < singleCharRightToLeft)
                    {
                        wordsAtMiddle = segWordsLeftToRight.ToList<string>();
                        break;
                    }
                    else if (singleCharLeftToRight > singleCharRightToLeft)
                    {
                        wordsAtMiddle = segWordsRightToLeft.ToList<string>();
                        break;
                    }
                }


                //如果以上措施都不能消除歧义，就需要继续循环

                //将正向"贪吃蛇"的第一个分词，放入临时变量中，用于结束循环后拼接
                wordsFromLeft.Add(segWordsLeftToRight[0]);
                //将逆向"贪吃蛇"的最后一个分词，放入临时变量，用于结束循环后拼接
                wordsFromRight.Add(segWordsRightToLeft[segWordsRightToLeft.Count-1]);

                //将要处理的字符串从两头去掉已经分好的词
                inputStr = inputStr.Substring(segWordsLeftToRight[0].Length);
                inputStr = inputStr.Substring(0, inputStr.Length - segWordsRightToLeft[segWordsRightToLeft.Count - 1].Length);

                //继续次循环分词
                //分词之前，清理保存正反分词的变量，防止出错
                segWordsLeftToRight.Clear();
                segWordsRightToLeft.Clear();

                //进行正反向分词
                //正向
                segWordsLeftToRight = SegMMLeftToRight(inputStr, ref wordList);

                //逆向
                segWordsRightToLeft = SegMMRightToLeft(inputStr, ref wordList);

            }

            //循环结束，说明要么分词没有歧义了，要么"贪吃蛇"从两头吃到中间交汇了
            //如果是在中间交汇，交汇时的分词结果，还要进行以下判断：
            //如果中间交汇有重叠了：
            //   正向第一个分词的长度 + 反向最后一个分词的长度 > 输入字符串总长度，就直接取正向的
            //   因为剩下的字符串太短了，2个分词就超出长度了
            if ((segWordsLeftToRight[0].Length + segWordsRightToLeft[segWordsRightToLeft.Count-1].Length) > inputStr.Length)
            {
                wordsAtMiddle = segWordsLeftToRight.ToList<string>();
            }
            //如果中间交汇，刚好吃完，没有重叠：
            //   正向第一个分词 + 反向最后一个分词的长度 = 输入字符串总长度，那么正反向一拼即可
            else if ((segWordsLeftToRight[0].Length + segWordsRightToLeft[segWordsRightToLeft.Count - 1].Length) == inputStr.Length)
            {
                wordsAtMiddle.Add(segWordsLeftToRight[0]);
                wordsAtMiddle.Add(segWordsRightToLeft[segWordsRightToLeft.Count - 1]);
            }

            //将之前"贪吃蛇"正反向得到的分词，以及中间没有歧义的分词，进行合并。
            //将左边的贪吃蛇的分词，添加到最终分词词组
            foreach (string wordLeft in wordsFromLeft)
            {
                segWordsFinal.Add(wordLeft);
            }
            //将中间没有歧义的分词，添加到最终分词词组
            foreach (string wordMiddle in wordsAtMiddle)
            {
                segWordsFinal.Add(wordMiddle);
            }
            //将右边的贪吃蛇的分词，添加到最终分词词组，注意，右边的添加是逆向的
            for (int i = 0; i < wordsFromRight.Count; i++ )
            {
                segWordsFinal.Add(wordsFromRight[wordsFromRight.Count-1-i]);
            }

            //返回完成的最终分词
            return segWordsFinal;

        }
        

    }
}
