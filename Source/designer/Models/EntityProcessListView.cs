using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FormMaster.Builder.Entity;
using Slickflow.Engine.Business.Entity;

namespace FormMaster.Designer.Models
{
    /// <summary>
    /// 表单定义绑定流程数据视图
    /// </summary>
    public class EntityProcessListView
    {
        public List<ProcessEntity> ProcessList { get; set; }
        public EntityProcessEntity EntityProcess { get; set; }
    }
}