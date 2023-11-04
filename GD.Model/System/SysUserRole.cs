using GD.Model.Constant;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD.Model.System
{
    /// <summary>
    /// 用户角色关联表 用户N-1 角色
    /// </summary>
    [SugarTable("sys_user_role", "用户和角色关联表")]
    [Tenant(DBConfigId.System)]
    public class SysUserRole
    {
        [SugarColumn(ColumnName = "user_id", IsPrimaryKey = true)]
        public long UserId { get; set; }

        [SugarColumn(ColumnName = "role_id", IsPrimaryKey = true)]
        public long RoleId { get; set; }
    }
}
