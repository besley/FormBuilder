using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using DapperExtensions;
using SlickOne.Data;
using SlickMaster.Builder.Entity;
using SlickMaster.Builder.Manager;
using SlickMaster.Toolkit;
using Slickflow.Engine.Common;

namespace SlickMaster.Builder.Service
{
    /// <summary>
    /// 表单定制服务的对外接口
    /// </summary>
    public class FBMasterService : IFBMasterService
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

        #region 表单定义操作
        /// <summary>
        /// 获取表单定义的HTML页面内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EntityDefEntity GetEntityDef(int id)
        {
            var entity = QuickRepository.GetById<EntityDefEntity>(id);
            return entity;
        }

        /// <summary>
        /// 表单实体列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<EntityDefEntity> GetEntityDefList2()
        {
            var sql = @"SELECT ID, 
                            EntityTitle, 
                            EntityName, 
                            EntityCode, 
                            Description, 
                            CreatedDate, 
                            LastUpdatedDate 
                        FROM EavEntityDef";
            var list = QuickRepository.Query<EntityDefEntity>(sql, null)
                        .ToList();
            return list;
        }

        /// <summary>
        /// 表单视图列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<EntityDefView> GetEntityDefViewList()
        {
            var sql = @"SELECT TOP 10 
                            ED.ID, 
                            ED.EntityTitle, 
                            ED.EntityName, 
                            ED.EntityCode, 
                            EP.ProcessGUID
                        FROM EavEntityDef ED
                        LEFT JOIN EavEntityProcess EP
                            ON ED.ID=EP.EntityDefID";
            var list = QuickRepository.Query<EntityDefView>(sql, null)
                        .ToList();
            return list;
        }

        /// <summary>
        /// 表单定义保存操作
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public EntityDefEntity SaveEntityDef(EntityDefEntity entity)
        {
            EntityDefEntity returnEntity = null;
            if (entity.ID == 0)
            {
                entity.EntityCode = PinyinConverter.ConvertFirst(entity.EntityName);
                entity.CreatedDate = System.DateTime.Now;
                var entityDefID = QuickRepository.Insert<EntityDefEntity>(entity);
                entity.ID = entityDefID;

                returnEntity = entity;
            }
            else
            {
                var updEntity = QuickRepository.GetById<EntityDefEntity>(entity.ID);
                updEntity.EntityTitle = entity.EntityTitle;
                updEntity.EntityName = entity.EntityName;
                updEntity.EntityCode = PinyinConverter.ConvertFirst(entity.EntityName);
                updEntity.Description = entity.Description;
                updEntity.LastUpdatedDate = System.DateTime.Now;
                QuickRepository.Update<EntityDefEntity>(updEntity);

                returnEntity = updEntity;
            }
            return returnEntity;
        }


        /// <summary>
        /// 保存属性
        /// </summary>
        /// <param name="entity"></param>
        public EntityAttributeEntity SaveAttribute(EntityAttributeEntity entity)
        {
            EntityAttributeEntity attrEntity = null;
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();

                //保存字段
                var eam = new EntityAttributeManager();
                attrEntity = eam.SaveAttribute(session.Connection, entity, session.Transaction);

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
            return attrEntity;
        }

        /// <summary>
        /// 保存字段同时，更新模板
        /// </summary>
        /// <param name="view"></param>
        public EntityAttributeEntity SaveAttributeWithTemplate(EntityAttributeView view)
        {
            EntityAttributeEntity attrEntity = null;
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();

                //保存字段
                var eam = new EntityAttributeManager();
                attrEntity = eam.SaveAttribute(session.Connection, view.EntityAttribute, session.Transaction);

                //保存模板内容
                var edm = new EntityDefManager();
                edm.SaveTemplateWithHTMLContent(session.Connection, view.EntityDef, session.Transaction);

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
            return attrEntity;
        }


        /// <summary>
        /// 保存表单模板内容
        /// </summary>
        /// <param name="entity"></param>
        public void SaveTemplateContent(EntityDefEntity entity)
        {
            var edm = new EntityDefManager();
            edm.SaveTemplateContent(entity);
        }

        public void SaveTemplateWithHTMLContent(EntityDefEntity entity)
        {
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();

                //保存模板内容
                var edm = new EntityDefManager();
                edm.SaveTemplateWithHTMLContent(session.Connection, entity, session.Transaction);

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
        /// <summary>
        /// 删除实体定义
        /// </summary>
        /// <param name="id"></param>
        public void DeleteEntityDef(int id)
        {
            //删除实体同时，要删除掉属性定义，删除掉所有属性取值5张表的数据
            try
            {
                var param = new DynamicParameters();
                param.Add("@entityDefID", id);

                using (IDbConnection conn = SessionFactory.CreateConnection())
                {
                    QuickRepository.ExecuteProc(conn, "pr_eav_EntityDefDelete", param);
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region 实体属性定义
        /// <summary>
        /// 获取属性列表
        /// </summary>
        /// <param name="entityDefID">实体定义ID</param>
        /// <returns>属性列表</returns>
        public List<EntityAttributeEntity> GetEntityAttributeList(int entityDefID) 
        {
            var sql = @"SELECT * FROM EavEntityAttribute
                        WHERE EntityDefID=@entityDefID
                        ORDER BY DivCtrlKey";
            var list = QuickRepository.Query<EntityAttributeEntity>(sql, new { entityDefID = entityDefID })
                        .ToList() ;
            return list;
        }

        /// <summary>
        /// 读取表单和字段的组合数据
        /// </summary>
        /// <param name="entityDefID">表单ID</param>
        /// <returns></returns>
        public EntityAttributeListView GetEntityAttributeComp(int entityDefID)
        {
            var entity = new EntityAttributeListView();
            entity.EntityDef = QuickRepository.GetById<EntityDefEntity>(entityDefID);
            entity.EntityAttributeList = GetEntityAttributeList(entityDefID);

            return entity;
        }


        /// <summary>
        /// 删除字段，同时更新模板内容
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public bool DeleteAttributeWithTemplate(EntityAttributeListView view)
        {
            bool isOk = false;
            if (view.EntityAttributeList != null 
                && view.EntityAttributeList.Count > 0)
            {
                var session = SessionFactory.CreateSession();
                try
                {
                    session.BeginTrans();

                    //删除字段
                    var eam = new EntityAttributeManager();
                    eam.DeleteAttribute(session.Connection, view.EntityAttributeList, session.Transaction);

                    //更新模板内容
                    var edm = new EntityDefManager();
                    edm.SaveTemplateWithHTMLContent(session.Connection, view.EntityDef, session.Transaction);

                    session.Commit();

                    isOk = true;
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
            
            return isOk;
        }
        #endregion
    }
}
