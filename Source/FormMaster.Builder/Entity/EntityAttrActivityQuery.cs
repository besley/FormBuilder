using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormMaster.Builder.Entity
{
    /// <summary>
    /// 表单字段节点权限查询实体
    /// </summary>
    public class EntityAttrActivityQuery
    {
        public int EntityDefID { get; set; }
        public int ProcessID { get; set; }
        public string ActivityGUID { get; set; }
    }
}
