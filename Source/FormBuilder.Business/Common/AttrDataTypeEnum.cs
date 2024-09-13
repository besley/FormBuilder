using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Common
{
    /// <summary>
    /// 字段类型
    /// </summary>
    public enum FieldDataTypeEnum
    {
        /// <summary>
        /// 非数据类型
        /// </summary>
        NONE = 0,

        /// <summary>
        /// 字符串
        /// </summary>
        VARCHAR = 1,

        /// <summary>
        /// 整型
        /// </summary>
        INT = 2,

        /// <summary>
        /// 数字
        /// </summary>
        DECIMAL = 3,

        /// <summary>
        /// 日期
        /// </summary>
        DATETIME = 4,

        /// <summary>
        /// 大文本
        /// </summary>
        TEXT = 5,

        /// <summary>
        /// 图片
        /// </summary>
        IMAGE = 6
    }
}
