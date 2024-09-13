using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entity
{
    /// <summary>
    /// 表单内容视图
    /// </summary>
    [Table("vw_FbFormDataView")]
    public class FormDataView
    {
        public int ID { get; set; }
        public int FormID { get; set; }
        public string FormName { get; set; }
        public string FormCode { get; set; }
        public string Version { get; set; }
        public string CreatedUserID { get; set; }
        public string CreatedUserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedUserID { get; set; }
        public string LastUpdatedUserName { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
