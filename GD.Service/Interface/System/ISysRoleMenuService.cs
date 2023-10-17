using GD.Model.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD.Service.Interface.System
{
    /// <summary>
    /// 角色菜单
    /// </summary>
    public interface ISysRoleMenuService : IBaseService<SysRoleMenu>
    {
        bool CheckMenuExistRole(long menuId);
        /// <summary>
        /// 根据角色获取菜单id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        List<SysRoleMenu> SelectRoleMenuByRoleId(long roleId);

        /// <summary>
        /// 根据用户所有角色获取菜单
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        List<SysRoleMenu> SelectRoleMenuByRoleIds(long[] roleIds);

        /// <summary>
        /// 批量插入用户菜单
        /// </summary>
        /// <param name="sysRoleMenus"></param>
        /// <returns></returns>
        int AddRoleMenu(List<SysRoleMenu> sysRoleMenus);

        /// <summary>
        /// 删除角色与菜单关联
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        int DeleteRoleMenuByRoleId(long roleId);

        /// <summary>
        /// 删除角色指定菜单
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="menuIds"></param>
        /// <returns></returns>
        bool DeleteRoleMenuByRoleIdMenuIds(long roleId, long[] menuIds);
    }
}
