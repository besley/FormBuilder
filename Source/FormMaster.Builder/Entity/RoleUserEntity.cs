using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormMaster.Builder.Entity
{
    public class RoleEntity
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public List<UserEntity> UserList { get; set; }
    }
}
