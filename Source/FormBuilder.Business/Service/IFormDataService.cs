using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBuilder.Business.Entity;
using FormBuilder.Business.Manager;

namespace FormBuilder.Business.Service
{
    /// <summary>
    /// 表单定制接口
    /// </summary>
    public interface IFormDataService
    {
        //form content
        FormDataEntity GetFormData(int dataRowID);
        FormDataView GetFormDataView(int dataRowID);
        List<FormDataView> GetFormDataViewList(int formID);
        int SaveFormData(FormDataEntity entity);
        int SaveFormData(IDbConnection conn, FormDataEntity entity, IDbTransaction trans);
        void DeleteFormData(int dataRowID);
        //control data source
        List<dynamic> LoadControlDataSource(FormFieldEntity entity);
        List<dynamic> LoadCascadeControlDataSource(CascadeControlInfo entity);
        List<FormFieldEntity> QueryCascadeChildControlList(CascadeParentControlInfo entity);
    }
}
