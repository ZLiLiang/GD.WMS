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
        [JsonProperty(propertyName: "CreateBy")]
        public string Create_by { get; set; } = string.Empty;

        [JsonProperty(propertyName: "CreateTime")]
        public DateTime Create_time { get; set; }

        [JsonProperty(propertyName: "UpdateBy")]
        public string Update_by { get; set; } = string.Empty;

        [JsonProperty(propertyName: "UpdateTime")]
        public DateTime? Update_time { get; set; }
    }
}
