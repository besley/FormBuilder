using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlickMaster.Builder.Common
{
    /// <summary>
    /// 字段界面输入类型
    ///  "text_field", 
    ///  "drop_down_list", 
    ///  "radio_buttons", 
    ///  "checkbox". 
    /// </summary>
    public enum FieldInputTypeEnum
    {
        /// <summary>
        /// 文本类型
        /// </summary>
        TextBox = 1,

        /// <summary>
        /// 密码框
        /// </summary>
        Password = 2,

        /// <summary>
        /// 下拉框类型
        /// </summary>
        ComboBox = 3,

        /// <summary>
        /// 复选框类型
        /// </summary>
        CheckBoxGroup = 4,

        /// <summary>
        /// Radio类型
        /// </summary>
        RadioGroup = 5,

        /// <summary>
        /// 多选列表
        /// </summary>
        selectmultiplelist = 6,

        /// <summary>
        /// 日期控件
        /// </summary>
        Date = 7,

        /// <summary>
        /// 文本区域
        /// </summary>
        Text = 8,

        /// <summary>
        /// 按钮控件
        /// </summary>
        Button = 16,

        /// <summary>
        /// 图片
        /// </summary>
        Image = 17
    }
}
