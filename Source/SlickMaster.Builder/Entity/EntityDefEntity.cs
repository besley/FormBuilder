using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlickMaster.Builder.Entity
{
    /// <summary>
    /// 实体对象
    /// </summary>
    [Table("EavEntityDef")]
    public class EntityDefEntity
    {
        public int ID { get; set; }
        public string EntityTitle { get; set; }
        public string EntityName { get; set; }
        public string EntityCode { get; set; }
        public string TemplateContent { get; set; }
        public string HTMLContent { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
    }
}
