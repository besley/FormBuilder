using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entity
{
    /// <summary>
    /// 实体实例信息表
    /// </summary>
    [Table("FbFormData")]
    public class FormDataEntity
    {
        public int ID { get; set; }
        public int FormID { get; set; }
        public string FormDataContent { get; set; }
        public string CreatedUserID { get; set; }
        public string CreatedUserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedUserID { get; set; }
        public string LastUpdatedUserName { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
