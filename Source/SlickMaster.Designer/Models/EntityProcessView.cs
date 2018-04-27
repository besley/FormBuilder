using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SlickMaster.Builder.Entity;
using Slickflow.Engine.Business.Entity;

namespace SlickMaster.Designer.Models
{
    /// <summary>
    /// 表单定义绑定流程数据视图
    /// </summary>
    public class EntityProcessView
    {
        public List<ProcessEntity> ProcessList { get; set; }
        public EntityProcessEntity EntityProcess { get; set; }
    }
}