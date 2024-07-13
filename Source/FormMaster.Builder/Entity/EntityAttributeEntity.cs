using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormMaster.Builder.Entity
{
    /// <summary>
    /// 实体属性
    /// </summary>
    [Table("EavEntityAttribute")]
    public class EntityAttributeEntity
    {
        public int ID { get; set; }
        public int EntityDefID { get; set; }
        public byte StorageType { get; set; }
        public string DivCtrlKey { get; set; }
        public string AttrName { get; set; }
        public string AttrCode { get; set; }
        public short AttrDataType { get; set; }
        public short FieldInputType { get; set; }
        public string ConditionKey { get; set; }
        public string VariableName { get; set; }
        public int RefEntityDefID { get; set; }
        public byte DataSourceType { get; set; }
        public string DataEntityName { get; set; }
        public string DataValueField { get; set; }
        public string DataTextField { get; set; }
        public string CascadeControlID { get; set; }
        public string CascadeFieldName { get; set; }
        public string Format { get; set; }
        public string CommandText { get; set; }
        public string Url { get; set; }
        public byte IsMandatory { get; set; }
        public int OrderID { get; set; }
        public string Description { get; set; }
    }
}