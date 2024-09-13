using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slickflow.Data;
using FormBuilder.Business.Entity;
using FormBuilder.Business.Manager;

namespace FormBuilder.Business.Service
{
    /// <summary>
    /// 保存表单数据
    /// </summary>
    public class FormDataService : IFormDataService
    {
        #region 基本属性
        private Repository _quickRepository;
        public Repository QuickRepository
        {
            get
            {
                if (_quickRepository == null) _quickRepository = new Repository();
                return _quickRepository;
            }
        }
        #endregion

        /// <summary>
        /// 删除数据实体
        /// </summary>
        /// <param name="dataRowID">主键ID</param>
        public void DeleteFormData(int dataRowID)
        {
            QuickRepository.Delete<FormDataEntity>(dataRowID);
        }

        /// <summary>
        /// 获取表单数据实体
        /// </summary>
        /// <param name="dataRowID">主键ID</param>
        /// <returns></returns>
        public FormDataEntity GetFormData(int dataRowID)
        {
            var fdm = new FormDataManager();
            return fdm.GetByID(dataRowID);
        }

        /// <summary>
        /// 获取表单数据实体
        /// </summary>
        /// <param name="dataRowID">主键ID</param>
        /// <returns></returns>
        public FormDataView GetFormDataView(int dataRowID)
        {
            var fdm = new FormDataManager();
            return fdm.GetFormDataView(dataRowID);
        }

        /// <summary>
        /// 获取表单数据实体视图列表
        /// </summary>
        /// <param name="formID">表单ID</param>
        /// <returns>表单数据列表</returns>
        public List<FormDataView> GetFormDataViewList(int formID)
        {
            var fdm = new FormDataManager();
            return fdm.GetFormDataViewList(formID);
        }

        public List<dynamic> LoadCascadeControlDataSource(CascadeControlInfo entity)
        {
            throw new NotImplementedException();
        }

        public List<dynamic> LoadControlDataSource(FormFieldEntity entity)
        {
            throw new NotImplementedException();
        }

        public List<FormFieldEntity> QueryCascadeChildControlList(CascadeParentControlInfo entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 保存表单数据
        /// </summary>
        /// <param name="entity">表单数据实体</param>
        /// <returns>表单数据ID</returns>
        public int SaveFormData(FormDataEntity entity)
        {
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                int entityID = SaveFormData(session.Connection, entity, session.Transaction);
                session.Commit();
                return entityID;
            }
            catch (System.Exception ex)
            {
                session.Rollback();
                throw;
            }
            finally
            {
                session.Dispose();
            }
        }

        /// <summary>
        /// 保存表单数据
        /// </summary>
        /// <param name="conn">数据库链接</param>
        /// <param name="entity">表单数据实体</param>
        /// <param name="trans">数据库事务</param>
        /// <returns></returns>
        public int SaveFormData(IDbConnection conn, FormDataEntity entity, IDbTransaction trans)
        {
            int entityID = 0;
            var formData = QuickRepository.GetById<FormDataEntity>(conn, entity.ID, trans);
            if (formData != null)
            {
                formData.LastUpdatedDate = System.DateTime.Now;
                formData.FormDataContent = entity.FormDataContent;
                QuickRepository.Update<FormDataEntity>(conn, formData, trans);
                entityID = formData.ID;
            }
            else
            {
                entity.CreatedDate = System.DateTime.Now;
                entity.LastUpdatedDate = System.DateTime.Now;
                entityID = QuickRepository.Insert<FormDataEntity>(conn, entity, trans);
            }
            return entityID;
        }
    }
}
