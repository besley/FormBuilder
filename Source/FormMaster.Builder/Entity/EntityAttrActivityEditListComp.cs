using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormMaster.Builder.Entity
{
    /// <summary>
    /// 字段权限列表实体
    /// </summary>
    public class EntityAttrActivityEditListComp
    {
        public int EntityDefID { get; set; }
        public int ProcessID { get; set; }
        public string ActivityGUID { get; set; }
        public List<EntityAttrActivityEditEntity> AttrEditList { get; set; }
    }
}
