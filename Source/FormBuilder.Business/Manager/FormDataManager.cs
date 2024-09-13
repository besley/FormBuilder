using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slickflow.Data;
using FormBuilder.Business.Entity;

namespace FormBuilder.Business.Manager
{
    /// <summary>
    /// 表单内容管理器
    /// </summary>
    internal class FormDataManager: ManagerBase
    {
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="dataRowID">表单数据ID</param>
        /// <returns>表单内容实体</returns>
        internal FormDataEntity GetByID(int dataRowID)
        {
            return Repository.GetById<FormDataEntity>(dataRowID);
        }

        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="dataRowID">表单数据ID</param>
        /// <returns>表单内容实体</returns>
        internal FormDataView GetFormDataView(int dataRowID)
        {
            return Repository.GetById<FormDataView>(dataRowID);
        }

        /// <summary>
        /// 获取表单内容视图列表
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        internal List<FormDataView> GetFormDataViewList(int formID)
        {
            var sqlQuery = (from fc in Repository.GetAll<FormDataView>()
                            where fc.FormID == formID
                            orderby fc.ID descending
                            select fc);
            var list = sqlQuery.ToList<FormDataView>();
            return list;
        }
    }
}
