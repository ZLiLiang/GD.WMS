using GD.Model.Enums;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD.Model.Dto
{
    public class RoleMenuExportDto
    {
        /// <summary>
        /// 一级目录名
        /// </summary>
        [ExcelColumn(Name = "菜单", Width = 50)]
        public string MenuName { get; set; }
 
        [ExcelColumn(Name = "路径", Width = 20)]
        public string Path { get; set; }
        [ExcelColumn(Name = "组件名", Width = 20)]
        public string Component { get; set; }
        [ExcelColumn(Name = "权限字符", Width = 20)]
        public string Perms { get; set; }
        [ExcelColumn(Name = "菜单类型")]
        public MenuType MenuType { get; set; }
        [ExcelColumn(Name = "菜单状态")]
        public MenuStatus Status { get; set; }
    }
}
