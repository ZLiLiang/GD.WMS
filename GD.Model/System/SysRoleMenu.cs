using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD.Model.System
{
    /// <summary>
    /// 角色菜单
    /// </summary>
    [SugarTable("sys_role_menu", "角色菜单")]
    [Tenant("0")]
    public class SysRoleMenu : SysBase
    {
        [JsonProperty("roleId")]
        [SugarColumn(IsPrimaryKey = true, ExtendedAttribute = 0)]
        public long Role_id { get; set; }
        [JsonProperty("menuId")]
        [SugarColumn(IsPrimaryKey = true, ExtendedAttribute = 0)]
        public long Menu_id { get; set; }
    }
}
