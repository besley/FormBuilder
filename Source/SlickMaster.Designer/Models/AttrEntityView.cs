using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SlickMaster.Builder.Entity;
using Slickflow.Engine.Business.Entity;

namespace SlickMaster.Designer.Models
{
    /// <summary>
    /// 字段实体视图
    /// </summary>
    public class AttrEntityView
    {
        public List<EntityDefEntity> EntityDefList { get; set; }
        public EntityAttributeEntity AttributeEntity { get; set; }
    }
}