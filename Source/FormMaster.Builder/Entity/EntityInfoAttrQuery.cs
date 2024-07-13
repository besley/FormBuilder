using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormMaster.Builder.Entity
{
    /// <summary>
    /// 实体属性查询
    /// </summary>
    public class EntityInfoAttrQuery : QueryBase
    {
        public int EntityDefID { get; set; }
        public string CreatedUserID { get; set; }
        public int EntityInfoID { get; set; }
        public Nullable<DateTime> CreatedBeginDateTime { get; set; }
        public Nullable<DateTime> CreatedEndDateTime { get; set; }
        public string WhereSQL { get; set; }
    }
}
