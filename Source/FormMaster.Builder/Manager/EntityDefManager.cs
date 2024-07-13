using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Slickflow.Data;
using FormMaster.Builder.Entity;

namespace FormMaster.Builder.Manager
{
    /// <summary>
    /// 实体管理器
    /// </summary>
    internal class EntityDefManager : ManagerBase
    {
        /// <summary>
        /// 更新模板内容
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="entityDef"></param>
        /// <param name="trans"></param>
        internal void SaveTemplateWithHTMLContent(IDbConnection conn, EntityDefEntity entityDef, IDbTransaction trans)
        {
            string sql = @"UPDATE EavEntityDef
                            SET TemplateContent=@templateContent,
                                HTMLContent=@htmlContent
                            WHERE ID=@id";
            Repository.Execute(conn, sql, new
            {
                id = entityDef.ID,
                templateContent = entityDef.TemplateContent,
                htmlContent = entityDef.HTMLContent
            }, trans);
        }

        /// <summary>
        /// 保存表单模板内容
        /// </summary>
        /// <param name="entity"></param>
        internal void SaveTemplateContent(EntityDefEntity entity)
        {
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                string sql = @"UPDATE EavEntityDef
                                SET TemplateContent=@templateContent
                                WHERE ID=@id";
                Repository.Execute(session.Connection, sql, new
                {
                    id = entity.ID,
                    templateContent = entity.TemplateContent
                }, session.Transaction);
                session.Commit();
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
    }
}
