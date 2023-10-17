using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD.Model.System
{
    [SugarTable("sys_role_dept", "角色部门")]
    [Tenant(0)]
    public class SysRoleDept
    {
        [SugarColumn(ExtendedAttribute = 0, IsPrimaryKey = true)]
        public long RoleId { get; set; }

        [SugarColumn(ExtendedAttribute = 0, IsPrimaryKey = true)]
        public long DeptId { get; set; }
    }
}
