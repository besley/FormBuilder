using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Slickflow.Data;
using FormBuilder.Business.Entity;

namespace FormBuilder.Business.Manager
{
    /// <summary>
    /// 实体管理器
    /// </summary>
    internal class FormManager : ManagerBase
    {
        /// <summary>
        /// 获取表单实体
        /// </summary>
        /// <param name="formID">表单ID</param>
        /// <returns></returns>
        internal FormEntity GetByID(int formID)
        {
            return Repository.GetById<FormEntity>(formID);
        }

        /// <summary>
        /// 获取表单实体
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="formID">表单ID</param>
        /// <param name="trans">事务</param>
        internal FormEntity GetByID(IDbConnection conn, int formID, IDbTransaction trans)
        {
            return Repository.GetById<FormEntity>(conn, formID, trans);
        }

        /// <summary>
        /// 更新模板内容
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="entityDef">表单实体</param>
        /// <param name="trans">事务</param>
        internal void SaveTemplateWithHTMLContent(IDbConnection conn, FormEntity entityDef, IDbTransaction trans)
        {
            var form = GetByID(conn, entityDef.ID, trans);
            form.TemplateContent = entityDef.TemplateContent;
            form.HTMLContent = entityDef.HTMLContent;
            Repository.Update<FormEntity>(conn, form, trans);
        }

        /// <summary>
        /// 保存表单模板内容
        /// </summary>
        /// <param name="entity">实体</param>
        internal FormEntity SaveTemplateContent(FormEntity entity)
        {
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                if (entity.ID != 0)
                {
                    var form = Repository.GetById<FormEntity>(session.Connection, entity.ID, session.Transaction);
                    form.FormName = entity.FormName;
                    form.TemplateContent = entity.TemplateContent;
                    form.LastUpdatedDate = DateTime.Now;
                    Repository.Update<FormEntity>(session.Connection, form, session.Transaction);
                }
                else
                {
                    entity.CreatedDate = DateTime.Now;
                    var newID = Repository.Insert<FormEntity>(session.Connection, entity, session.Transaction);
                    entity.ID = newID;
                }
                session.Commit();
                return entity;
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
        /// 删除表单
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="formID">表单ID</param>
        /// <param name="trans">事务</param>
        internal void DeleteForm(IDbConnection conn, int formID, IDbTransaction trans)
        {
            Repository.Delete<FormEntity>(conn, formID, trans);
        }
    }
}
