using GD.Model;
using GD.Model.Constant;
using GD.Model.Generate;
using GD.Model.System;
using GD.Model.WarehouseManagement;
using SqlSugar;
using SqlSugar.IOC;
using System.Reflection;

namespace GD.WMS.WebApi.SqlSugar
{
    /// <summary>
    /// 初始化表
    /// </summary>
    public class InitTable
    {
        /// <summary>
        /// 创建db、表
        /// </summary>
        public static void InitDb()
        {
            var db = DbScoped.SugarScope;
            var childDb0 = db.GetConnection(DBConfigId.System);
            var childDb1 = db.GetConnection(DBConfigId.WarehouseManagement);
            //建库：如果不存在创建数据库存在不会重复创建 
            // 注意 ：Oracle和个别国产库需不支持该方法，需要手动建库 
            childDb0.DbMaintenance.CreateDatabase();
            childDb1.DbMaintenance.CreateDatabase();

            //Type baseType = typeof(Base);
            //Assembly assembly = Assembly.Load(baseType.Assembly.GetName());
            //var entityes = assembly.GetTypes().Where(p =>
            //{
            //    var attr = p.GetCustomAttribute<TenantAttribute>();
            //    if (attr != null)
            //    {
            //        //return p.IsSubclassOf(baseType) && (attr.configId.Equals("0")|| attr.configId.Equals("1"));
            //        return p.IsSubclassOf(baseType) && attr.configId.Equals("1");

            //    }
            //    else
            //    {
            //        return false;
            //    }

            //}).ToArray();
            //db.CodeFirst.InitTables(entityes);

            //24个表,建议先使用下面方法初始化表，方便排查问题
            //childDb0.CodeFirst.InitTables(typeof(SysUser));
            //childDb0.CodeFirst.InitTables(typeof(SysRole));
            //childDb0.CodeFirst.InitTables(typeof(SysLogininfor));
            //childDb0.CodeFirst.InitTables(typeof(SysOperLog));
            //childDb0.CodeFirst.InitTables(typeof(SysMenu));
            //childDb0.CodeFirst.InitTables(typeof(SysRoleMenu));
            //childDb0.CodeFirst.InitTables(typeof(SysUserRole));
            //childDb0.CodeFirst.InitTables(typeof(GenTable));
            //childDb0.CodeFirst.InitTables(typeof(GenTableColumn));
            //childDb0.CodeFirst.InitTables(typeof(SqlDiffLog));


            //childDb1.CodeFirst.InitTables(typeof(Company));
            //childDb1.CodeFirst.InitTables(typeof(Category));
            //childDb1.CodeFirst.InitTables(typeof(Supplier));
            //childDb1.CodeFirst.InitTables(typeof(CommoditySPU));
            //childDb1.CodeFirst.InitTables(typeof(CommoditySKU));
            //childDb1.CodeFirst.InitTables(typeof(Warehouse));
            //childDb1.CodeFirst.InitTables(typeof(Region));
            //childDb1.CodeFirst.InitTables(typeof(Location));
            //childDb1.CodeFirst.InitTables(typeof(Owner));
        }

    }
}
