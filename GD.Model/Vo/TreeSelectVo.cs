﻿using GD.Model.System;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD.Model.Vo
{
    /// <summary>
    /// Treeselect树结构实体类
    /// </summary>
    public class TreeSelectVo
    {
        /// <summary>
        /// 节点Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Label { get; set; }
        public string Status { get; set; }
        public string MenuType { get; set; }

        public TreeSelectVo() { }

        public TreeSelectVo(SysMenu menu)
        {
            Id = menu.MenuId;
            Label = menu.MenuName;
            Status = menu.Status;
            MenuType = menu.MenuType;

            List<TreeSelectVo> child = new List<TreeSelectVo>();
            foreach (var item in menu.Children)
            {
                child.Add(new TreeSelectVo(item));
            }

            Children = child;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<TreeSelectVo> Children { get; set; }
    }
}
