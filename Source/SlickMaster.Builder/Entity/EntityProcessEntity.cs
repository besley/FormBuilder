using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlickMaster.Builder.Entity
{
    [Table("EavEntityProcess")]
    public class EntityProcessEntity
    {
        public int ID { get; set; }
        public int EntityDefID { get; set; }
        public string ProcessGUID { get; set; }
        public string ProcessName { get; set; }
    }
}
