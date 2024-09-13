using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entity
{
    /// <summary>
    /// 实体属性
    /// </summary>
    [Table("FbFormField")]
    public class FormFieldEntity
    {
        public int ID { get; set; }
        public int FormID { get; set; }
        public string Version { get; set; }
        public string FieldType { get; set; }
        public string FieldName { get; set; }
        public string FieldCode { get; set; }
        public string FieldGUID { get; set; }
        public string Description { get; set; }
        public string ControlStyle { get; set; }
        public bool IsMandatory { get; set; }
        public short FieldDataType { get; set; }
        public string ConditionKey { get; set; }
        public string VariableName { get; set; }
        public int RefFormID { get; set; }
        public string DataSourceType { get; set; }
        public string DataEntityOptions { get; set; }
        public string DataEntityName { get; set; }
        public string DataValueField { get; set; }
        public string DataTextField { get; set; }
        public string CascadeControlCode { get; set; }
        public string CascadeFieldName { get; set; }
        public string Format { get; set; }
        public string Url { get; set; }
        public int OrderID { get; set; }
    }
}