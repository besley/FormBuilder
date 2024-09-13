using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FormBuilder.Business.Entity;
using Slickflow.Engine.Business.Entity;

namespace FormBuilder.WebApi.Models
{
    /// <summary>
    /// 表单定义绑定流程数据视图
    /// </summary>
    public class FormProcessListView
    {
        public List<ProcessEntity> ProcessList { get; set; }
        public List<FormProcessEntity> FormProcessList { get; set; }
    }
}