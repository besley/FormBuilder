using FormBuilder.Business.Entity;
using Slickflow.Engine.Business.Entity;

namespace FormBuilder.WebApi.Models
{
    /// <summary>
    /// 表单数据流程列表实体视图
    /// </summary>
    public class FormDataProcessListView
    {
        public List<FormDataView> FormDataViewList { get; set; }
        public List<FormProcessEntity> FormProcessList { get; set; }
    }
}
