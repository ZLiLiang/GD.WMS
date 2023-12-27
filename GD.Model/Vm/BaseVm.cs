using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD.Model.Vm
{
    public class BaseVm
    {
        [ExcelColumn(Name = "创建者")]
        [JsonProperty(propertyName: "CreateBy")]
        public string Create_by { get; set; } = string.Empty;

        [ExcelColumn(Name = "创建时间")]
        [JsonProperty(propertyName: "CreateTime")]
        public DateTime Create_time { get; set; }

        [ExcelColumn(Ignore = true)]
        [JsonProperty(propertyName: "UpdateBy")]
        public string Update_by { get; set; } = string.Empty;

        [ExcelColumn(Ignore = true)]
        [JsonProperty(propertyName: "UpdateTime")]
        public DateTime? Update_time { get; set; }
    }
}
