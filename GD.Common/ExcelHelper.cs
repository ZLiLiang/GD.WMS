using GD.Infrastructure.App;
using Microsoft.AspNetCore.Hosting;
using MiniExcelLibs;

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
        public static (string, string) ExportExcelMini<T>(List<T> list, string sheetName, string fileName)
        {
            IWebHostEnvironment webHostEnvironment = (IWebHostEnvironment)App.ServiceProvider.GetService(typeof(IWebHostEnvironment));
            string sFileName = $"{fileName}{DateTime.Now:MM-dd-HHmmss}.xlsx";
            string fullPath = Path.Combine(webHostEnvironment.WebRootPath, "export", sFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            MiniExcel.SaveAs(fullPath, list, sheetName: sheetName);
            return (sFileName, fullPath);
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
    }
}
