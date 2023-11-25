using GD.Infrastructure.App;
using Microsoft.AspNetCore.Hosting;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using System.Collections;
using System.Reflection;

namespace GD.Common
{
    public class ExcelHelper
    {
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sheetName"></param>
        /// <param name="fileName"></param>
        public static string ExportExcel<T>(List<T> list, string sheetName, string fileName)
        {
            return ExportExcelMini(list, sheetName, fileName).Item1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sheetName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static (string, string) ExportExcelMini<T>(List<T> list, string sheetName, string fileName, IConfiguration config = null)
        {
            IWebHostEnvironment webHostEnvironment = (IWebHostEnvironment)App.ServiceProvider.GetService(typeof(IWebHostEnvironment));
            string sFileName = $"{fileName}{DateTime.Now:MM-dd-HHmmss}.xlsx";
            string fullPath = Path.Combine(webHostEnvironment.WebRootPath, "export", sFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            MiniExcel.SaveAs(fullPath, list, sheetName: sheetName, configuration: config);
            return (sFileName, fullPath);
        }

        /// <summary>
        /// 导出二级数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sheetName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static (string, string) ExportExcelMiniTwoNestList<T>(List<T> list, string sheetName, string fileName)
        {
            IWebHostEnvironment webHostEnvironment = (IWebHostEnvironment)App.ServiceProvider.GetService(typeof(IWebHostEnvironment));
            string sFileName = $"{fileName}{DateTime.Now:MM-dd-HHmmss}.xlsx";
            string fullPath = Path.Combine(webHostEnvironment.WebRootPath, "export", sFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            var data = ConvertToDictionary(list);
            MiniExcel.SaveAs(fullPath, data, sheetName: sheetName);
            return (sFileName, fullPath);
        }


        /// <summary>
        /// 将二层嵌套list转换为单层list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<Dictionary<string, object>> ConvertToDictionary<T>(List<T> list)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            //遍历第一层list
            foreach (var item in list)
            {
                var type = item.GetType();
                var properties = type.GetProperties();
                var innerListProperty = properties
                  .Where(it => it.PropertyType.Name.Equals("List`1"))//List`1
                  .First();
                var innerListValue = innerListProperty.GetValue(item) as IList;
                //遍历第二次list
                foreach (var innerItem in innerListValue)
                {
                    var innerProperties = innerItem.GetType().GetProperties();
                    var resultValue = new Dictionary<string, object>();
                    //获取第一层实例的属性的值
                    foreach (var outerValue in properties)
                    {
                        if (outerValue.GetCustomAttribute<ExcelColumnAttribute>().Ignore == true) continue;
                        var key = outerValue.GetCustomAttribute<ExcelColumnAttribute>().Name;
                        var value = outerValue.GetValue(item);
                        resultValue.Add(key, value);
                    }
                    //获取第二层实例的属性的值
                    foreach (var innerValue in innerProperties)
                    {
                        var key = innerValue.GetCustomAttribute<ExcelColumnAttribute>().Name;
                        var value = innerValue.GetValue(innerItem);
                        resultValue.Add(key, value);
                    }
                    result.Add(resultValue);
                }
            }
            return result;

        }

        /// <summary>
        /// 导出多个工作表(Sheet)
        /// </summary>
        /// <param name="sheets"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static (string, string) ExportExcelMini(Dictionary<string, object> sheets, string fileName)
        {
            IWebHostEnvironment webHostEnvironment = (IWebHostEnvironment)App.ServiceProvider.GetService(typeof(IWebHostEnvironment));
            string sFileName = $"{fileName}{DateTime.Now:MM-dd-HHmmss}.xlsx";
            string fullPath = Path.Combine(webHostEnvironment.WebRootPath, "export", sFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            MiniExcel.SaveAs(fullPath, sheets);
            return (sFileName, fullPath);
        }

        /// <summary>
        /// 下载导入模板
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="list">空数据类型集合</param>
        /// <param name="fileName">下载文件名</param>
        /// <returns></returns>
        public static (string, string) DownloadImportTemplate<T>(List<T> list, string fileName)
        {
            IWebHostEnvironment webHostEnvironment = App.WebHostEnvironment;
            string sFileName = $"{fileName}.xlsx";
            string fullPath = Path.Combine(webHostEnvironment.WebRootPath, "ImportTemplate", sFileName);

            //不存在模板创建模板
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }
            if (!File.Exists(fullPath))
            {
                MiniExcel.SaveAs(fullPath, list, overwriteFile: true);
            }
            return (sFileName, fullPath);
        }

        /// <summary>
        /// 下载指定文件模板
        /// </summary>
        /// <param name="fileName">下载文件名</param>
        /// <returns></returns>
        public static (string, string) DownloadImportTemplate(string fileName)
        {
            IWebHostEnvironment webHostEnvironment = (IWebHostEnvironment)App.ServiceProvider.GetService(typeof(IWebHostEnvironment));
            string sFileName = $"{fileName}.xlsx";
            string fullPath = Path.Combine(webHostEnvironment.WebRootPath, "ImportTemplate", sFileName);

            return (sFileName, fullPath);
        }
    }
}
