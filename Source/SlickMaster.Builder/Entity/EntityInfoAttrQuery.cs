using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlickMaster.Builder.Entity
{
    public class EntityInfoAttrQuery : QueryBase
    {
        public int EntityDefID { get; set; }
        public string CreatedUserID { get; set; }
        public int EntityID { get; set; }
    }
}
