using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entity
{
    [Table("FbFormProcess")]
    public class FormProcessEntity
    {
        public int ID { get; set; }
        public int FormID { get; set; }
        public string Version { get; set; }
        public int ProcessID { get; set; }
        public string ProcessGUID { get; set; }
        public string ProcessName { get; set; }
        public string ProcessVersion { get; set; }

    }
}
