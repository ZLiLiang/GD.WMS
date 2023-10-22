using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD.Model.System
{
    /// <summary>
    /// 角色表 sys_role
    /// </summary>
    [SugarTable("sys_role", "角色表")]
    [Tenant("0")]
    public class SysRole : SysBase
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [SugarColumn(Length = 30, ExtendedAttribute = 0)]
        public string RoleName { get; set; }

        /// <summary>
        /// 角色排序
        /// </summary>
        [SugarColumn(ExtendedAttribute = 0)]
        public int RoleSort { get; set; }

        /// <summary>
        /// 帐号状态（0正常 1停用）
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public int Status { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public int DelFlag { get; set; }

        /// <summary>
        /// 菜单树选择项是否关联显示
        /// </summary>
        [SugarColumn(ColumnName = "menu_check_strictly")]
        public bool MenuCheckStrictly { get; set; } = true;

        /// <summary>
        /// 菜单组
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public long[] MenuIds { get; set; }

        /// <summary>
        /// 用户个数
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int UserNum { get; set; }

        public SysRole() { }

        public SysRole(long roleId)
        {
            RoleId = roleId;
        }

        public bool IsAdmin()
        {
            return IsAdmin(RoleId);
        }

        public static bool IsAdmin(long roleId)
        {
            return 1 == roleId;
        }
    }
}
