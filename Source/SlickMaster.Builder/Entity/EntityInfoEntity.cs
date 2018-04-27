using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlickMaster.Builder.Entity
{
    /// <summary>
    /// 实体实例信息表
    /// </summary>
    [Table("EavEntityInfo")]
    public class EntityInfoEntity
    {
        public int ID { get; set; }
        public int EntityDefID { get; set; }
        public string ProcessGUID { get; set; }
        public string CreatedUserID { get; set; }
        public string CreatedUserName { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public string LastUpdatedUserID { get; set; }
        public string LastUpdatedUserName { get; set; }
        public DateTime LastUpdatedDatetime { get; set; }
        
    }
}
