using GD.Model.Dto.System;
using GD.Model.Generate;
using GD.Model.System;
using GD.Model.Vo;
using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD.Service.Interface.System
{
    public interface ISysMenuService : IBaseService<SysMenu>
    {
        //List<SysMenu> SelectMenuList(long userId);

        List<SysMenu> SelectMenuList(MenuQueryDto menu, long userId);
        List<SysMenu> SelectTreeMenuList(MenuQueryDto menu, long userId);

        SysMenu GetMenuByMenuId(int menuId);
        List<SysMenu> GetMenusByMenuId(int menuId, long userId);
        long AddMenu(SysMenu menu);

        long EditMenu(SysMenu menu);

        int DeleteMenuById(int menuId);

        string CheckMenuNameUnique(SysMenu menu);

        int ChangeSortMenu(MenuDto menuDto);

        bool HasChildByMenuId(long menuId);

        List<SysMenu> SelectMenuTreeByUserId(long userId);

        //List<SysMenu> SelectMenuPermsListByUserId(long userId);

        //bool CheckMenuExistRole(long menuId);

        List<RouterVo> BuildMenus(List<SysMenu> menus);

        List<TreeSelectVo> BuildMenuTreeSelect(List<SysMenu> menus);

        void AddSysMenu(GenTable genTableInfo, bool showEdit, bool showExport, bool showImport);
        List<SysMenu> SelectTreeMenuListByRoles(MenuQueryDto menu, List<long> roles);
        List<RoleMenuExportDto> SelectRoleMenuListByRole(MenuQueryDto menu, int roleId);
    }

}
