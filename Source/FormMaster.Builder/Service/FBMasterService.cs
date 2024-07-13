using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using DapperExtensions;
using Slickflow.Data;
using FormMaster.Builder.Entity;
using FormMaster.Builder.Manager;
using FormMaster.Builder.Utility;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Service;

namespace FormMaster.Builder.Service
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
        /// <param name="entityDefID"></param>
        /// <returns></returns>
        public EntityDefEntity GetEntityDef(int entityDefID)
        {
            var entity = QuickRepository.GetById<EntityDefEntity>(entityDefID);
            return entity;
        }

        /// <summary>
        /// 获取表单定义的HTML页面内容
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="entityDefID">ID</param>
        /// <param name="trans">事务</param>
        /// <returns></returns>
        public EntityDefEntity GetEntityDef(IDbConnection conn, int entityDefID, IDbTransaction trans)
        {
            return QuickRepository.GetById<EntityDefEntity>(conn, entityDefID, trans);
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
                            Version,
                            Description, 
                            CreatedDate, 
                            LastUpdatedDate 
                        FROM EavEntityDef
                        ORDER BY ID DESC";
            var list = QuickRepository.Query<EntityDefEntity>(sql, null)
                        .ToList();
            return list;
        }

        /// <summary>
        /// 表单视图列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<EntityDefProcessView> GetEntityDefViewList()
        {
            var sql = @"SELECT ED.ID, 
                            ED.EntityTitle, 
                            ED.EntityName, 
                            ED.EntityCode, 
                            EP.ProcessGUID,
                            EP.Version
                        FROM EavEntityDef ED
                        LEFT JOIN EavEntityProcess EP
                            ON ED.ID=EP.EntityDefID";
            var list = QuickRepository.Query<EntityDefProcessView>(sql, null)
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
                updEntity.Version = entity.Version;
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
        /// <param name="entityDefID"></param>
        public void DeleteEntityDef(int entityDefID)
        {
            //删除实体同时，要删除掉属性定义，删除掉所有属性取值5张表的数据
            try
            {
                var param = new DynamicParameters();
                param.Add("@entityDefID", entityDefID);

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

        /// <summary>
        /// 表单升级
        /// </summary>
        /// <param name="entityDefID">主键ID</param>
        public void UpgradeEntityDef(int entityDefID)
        {
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                var entityDef = GetEntityDef(session.Connection, entityDefID, session.Transaction);
                int newVersion = 1;
                var parsed = int.TryParse(entityDef.Version, out newVersion);
                if (parsed == true) newVersion = newVersion +1;
                entityDef.Version = (int.Parse(entityDef.Version) + 1).ToString();
                var entityDefInserted = QuickRepository.Insert<EntityDefEntity>(session.Connection, entityDef, session.Transaction);

                //读取字段列表
                var attributeList = GetEntityAttributeList(session.Connection, entityDefID, session.Transaction);
                foreach (var attr in attributeList)
                {
                    attr.EntityDefID = entityDefInserted;
                    QuickRepository.Insert<EntityAttributeEntity>(session.Connection, attr, session.Transaction);
                }
                session.Commit();
            }
            catch (System.Exception ex)
            {
                session.Rollback();
                throw;
            }
        }
        #endregion

        #region 表单绑定流程

        /// <summary>
        /// 获取表单绑定流程信息
        /// </summary>
        /// <param name="entityDefID">实体定义ID</param>
        /// <returns>表单流程实体</returns>
        public EntityProcessEntity GetEntityProcess(int entityDefID)
        {
            EntityProcessEntity entity = null;
            var sql = @"SELECT *
                        FROM EavEntityProcess
                        WHERE EntityDefID=@entityDefID";
            var list = QuickRepository.Query<EntityProcessEntity>(sql, new
            {
                entityDefID = entityDefID
            }).ToList();

            if (list != null && list.Count() == 1)
            {
                entity = list[0];
            }
            return entity;
        }

        /// <summary>
        /// 获取表单绑定流程信息
        /// </summary>
        /// <param name="entityDefID">实体定义ID</param>
        /// <returns>表单流程实体视图</returns>
        public EntityDefProcessView GetEntityDefProcessView(int entityDefID)
        {
            EntityDefProcessView entity = null;
            var sql = @"SELECT 
                            D.ID,
                            D.EntityTitle,
                            D.EntityName,
                            D.EntityCode,
                            D.TemplateContent,
                            D.HTMLContent,
                            D.MobileTemplateContent,
                            EP.ProcessID,
                            EP.ProcessGUID,
                            EP.Version,
                            P.ProcessName,
                            P.ProcessCode 
                        FROM EavEntityDef D 
                        INNER JOIN EavEntityProcess EP 
                            ON D.ID=EP.EntityDefID 
                        INNER JOIN WfProcess P
                            ON EP.ProcessID=P.ID 
                        WHERE D.ID=@entityDefID";
            var list = QuickRepository.Query<EntityDefProcessView>(sql, new
            {
                entityDefID = entityDefID
            }).ToList();

            if (list != null && list.Count() == 1)
            {
                entity = list[0];
            }
            return entity;
        }

        /// <summary>
        /// 保存绑定流程信息
        /// </summary>
        /// <param name="entity"></param>
        public void BindEntityProcess(EntityDefProcessView view)
        {
            //查找流程GUID
            var processEntity = QuickRepository.GetById<ProcessEntity>(view.ProcessID);
            var entityProcessEntity = new EntityProcessEntity {
                ProcessID = processEntity.ID,
                ProcessGUID = processEntity.ProcessGUID,
                Version = processEntity.Version,
                ProcessName = processEntity.ProcessName,
                EntityDefID = view.ID
            };

            //保存绑定信息
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                //先删除之前绑定的流程信息
                RemoveEntityProcess(session.Connection, view.ID, session.Transaction);
                QuickRepository.Insert<EntityProcessEntity>(session.Connection, entityProcessEntity, session.Transaction);
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
        /// 解除绑定信息
        /// </summary>
        /// <param name="view"></param>
        public void UnbindEntityProcess(EntityDefProcessView view)
        {
            //解除绑定信息
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                //先删除之前绑定的流程信息
                RemoveEntityProcess(session.Connection, view.ID, session.Transaction);
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
        /// 删除流程绑定信息
        /// </summary>
        /// <param name="entityDefID">表单ID</param>
        public void RemoveEntityProcess(int entityDefID)
        {
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                RemoveEntityProcess(session.Connection, entityDefID, session.Transaction);
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
        /// 删除流程绑定信息
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="entityDefID">表单ID</param>
        /// <param name="trans">交易</param>
        private void RemoveEntityProcess(IDbConnection conn, int entityDefID, IDbTransaction trans)
        {
            //删除流程绑定记录
            var sql = @"DELETE 
                        FROM EavEntityProcess
                        WHERE EntityDefID=@entityDefID";
            QuickRepository.Execute(conn, sql, new { entityDefID = entityDefID }, trans);
            //删除表单字段节点权限数据
            ClearEntityAttrActivityEdit(conn, entityDefID, trans);
        }

        /// <summary>
        /// 判断流程是否绑定了表单
        /// </summary>
        /// <param name="processID">流程ID</param>
        /// <returns>是否绑定</returns>
        public Boolean IsEntityDefProcessBinding(int processID)
        {
            var isBinding = false;
            var sql = @"SELECT *
                        FROM EavEntityProcess
                        WHERE ProcessID=@processID";
            var list = QuickRepository.Query<EntityProcessEntity>(sql, new
            {
                processID = processID
            }).ToList();

            if (list.Count() > 0)
            {
                isBinding = true;
            }
            return isBinding;
        }
        #endregion

        #region 实体属性定义
        /// <summary>
        /// 获取属性实体
        /// </summary>
        /// <param name="attrID">属性id</param>
        /// <returns>属性实体</returns>
        public EntityAttributeEntity GetEntityAttribute(int attrID)
        {
            var entity = QuickRepository.GetById<EntityAttributeEntity>(attrID);
            return entity;

        }

        /// <summary>
        /// 获取属性列表
        /// </summary>
        /// <param name="entityDefID">实体定义ID</param>
        /// <returns>属性列表</returns>
        public List<EntityAttributeEntity> GetEntityAttributeList(int entityDefID)
        {
            using (var session = SessionFactory.CreateSession())
            {
                var attributeList = GetEntityAttributeList(session.Connection, entityDefID, session.Transaction);
                return attributeList;
            }
        }

        /// <summary>
        /// 获取属性列表
        /// </summary>
        /// <param name="conn">数据库链接</param>
        /// <param name="entityDefID">实体定义ID</param>
        /// <param name="trans">数据库交易</param>
        /// <returns>属性列表</returns>
        public List<EntityAttributeEntity> GetEntityAttributeList(IDbConnection conn,
            int entityDefID,
            IDbTransaction trans)
        {
            var sql = @"SELECT * FROM EavEntityAttribute
                        WHERE EntityDefID=@entityDefID
                        ORDER BY DivCtrlKey";
            var list = QuickRepository.Query<EntityAttributeEntity>(conn, sql, new { entityDefID = entityDefID }, trans)
                        .ToList();
            return list;
        }

        /// <summary>
        /// 获取数值录入字段列表
        /// </summary>
        /// <param name="entityDefID">实体定义ID</param>
        /// <returns>属性列表</returns>
        public List<EntityAttributeEntity> GetEntityAttributeListOnlyInfoValue(int entityDefID)
        {
            var sql = @"SELECT * FROM EavEntityAttribute
                        WHERE EntityDefID=@entityDefID
                            AND StorageType = 1
                        ORDER BY DivCtrlKey";
            var list = QuickRepository.Query<EntityAttributeEntity>(sql, new { entityDefID = entityDefID })
                        .ToList();
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

                    //删除字段事件
                    var eaem = new EntityAttributeEventManager();
                    eaem.DeleteByAttributeList(session.Connection, view.EntityAttributeList, session.Transaction);

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

        #region 字段事件定义
        /// <summary>
        /// 字段绑定事件列表
        /// </summary>
        /// <param name="entityDefID">实体ID</param>
        /// <param name="attrID">属性ID</param>
        /// <returns>事件列表</returns>
        public List<EntityAttributeEventView> GetEntityAttributeEventList(int entityDefID, int attrID)
        {
            var eaem = new EntityAttributeEventManager();
            return eaem.GetEventList(entityDefID, attrID);
        }

        /// <summary>
        /// 字段绑定事件列表
        /// </summary>
        /// <param name="entityDefID">实体ID</param>
        /// <returns>事件列表</returns>
        public List<EntityAttributeEventView> GetEntityAttributeEventListByForm(int entityDefID)
        {
            var eaem = new EntityAttributeEventManager();
            return eaem.GetEventListByForm(entityDefID);
        }

        /// <summary>
        /// 保存属性事件
        /// </summary>
        /// <param name="entity">事件实体</param>
        /// <returns></returns>
        public EntityAttributeEventEntity SaveAttributeEvent(EntityAttributeEventEntity entity)
        {
            EntityAttributeEventEntity eventEntity = null;
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                var eaem = new EntityAttributeEventManager();
                eventEntity = eaem.SaveAttribute(session.Connection, entity, session.Transaction);
                session.Commit();
            }
            catch(System.Exception ex)
            {
                session.Rollback();
                throw;
            }
            finally
            {
                session.Dispose();
            }
            return eventEntity;
        }

        /// <summary>
        /// 删除属性事件
        /// </summary>
        /// <param name="eventID">事件ID</param>
        public void DeleteAttributeEvent(int eventID)
        {
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                var eaem = new EntityAttributeEventManager();
                eaem.Delete(session.Connection, eventID, session.Transaction);
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
        /// 删除属性事件列表
        /// </summary>
        /// <param name="entityDefID"></param>
        /// <param name="attrID"></param>
        public void DeleteAttributeEvent(int entityDefID, int attrID)
        {
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                var sql = @"DELETE FROM EavEntityAttributeEvent
                        WHERE EntityDefID=@entityDefID
                            AND AttrID=@attrID";
                QuickRepository.Execute(session.Connection, sql,
                     new
                     {
                         entityDefID = entityDefID,
                         attrID = attrID
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
        #endregion

        #region 字段权限维护
        /// <summary>
        /// 获取表单字段权限数据
        /// </summary>
        /// <param name="entityDefID">表单</param>
        /// <param name="processID">流程ID</param>
        /// <param name="activityGUID">节点GUID</param>
        /// <returns>字段列表</returns>
        public List<EntityAttrActivityEditEntity> GetEntityAttrActivityEditList(int entityDefID, 
            int processID, 
            string activityGUID)
        {
            var sql = @"SELECT * FROM EavEntityAttrActivityEdit
                        WHERE EntityDefID=@entityDefID
                            AND ProcessID=@processID
                            AND ActivityGUID=@activityGUID";

            var list = QuickRepository.Query<EntityAttrActivityEditEntity>(sql, 
                new {
                    entityDefID = entityDefID,
                    processID = processID,
                    activityGUID = activityGUID
                }
            ).ToList();
            return list;
        }

        /// <summary>
        /// 获取表单字段权限数据
        /// </summary>
        /// <param name="entityDefID">表单</param>
        /// <param name="processGUID">流程GUID</param>
        /// <param name="version">版本</param>
        /// <param name="activityGUID">节点GUID</param>
        /// <returns>字段列表</returns>
        public List<EntityAttrActivityEditEntity> GetEntityAttrActivityEditList(int entityDefID, 
            string processGUID, 
            string version, 
            string activityGUID)
        {
            var sql = @"SELECT * FROM EavEntityAttrActivityEdit
                        WHERE EntityDefID=@entityDefID
                            AND ProcessGUID=@processGUID
                            AND Version=@version
                            AND ActivityGUID=@activityGUID";

            var list = QuickRepository.Query<EntityAttrActivityEditEntity>(sql,
                new
                {
                    entityDefID = entityDefID,
                    processGUID = processGUID,
                    version = version,
                    activityGUID = activityGUID
                }
            ).ToList();
            return list;
        }

        /// <summary>
        /// 查询当字段权限实体
        /// </summary>
        /// <param name="entityDefID">表单ID</param>
        /// <param name="processID">流程ID</param>
        /// <param name="activityGUID">节点GUID</param>
        /// <param name="attrID">字段ID</param>
        /// <returns>权限实体</returns>
        private EntityAttrActivityEditEntity QuerySingleAttrActivityEdit(int entityDefID,
            int processID, string activityGUID, int attrID)
        {
            EntityAttrActivityEditEntity entity = null;
            var sql = @"SELECT * FROM EavEntityAttrActivityEdit
                        WHERE EntityDefID=@entityDefID
                            AND ProcessID=@processID
                            AND ActivityGUID=@activityGUID
                            AND AttrID=@attrID";

            var list = QuickRepository.Query<EntityAttrActivityEditEntity>(sql,
                new
                {
                    entityDefID = entityDefID,
                    processID = processID,
                    activityGUID = activityGUID,
                    attrID = attrID
                }
            ).ToList();

            if (list != null && list.Count() == 1)
                entity = list[0];
            return entity;
        }

        /// <summary>
        /// 保存表单字段节点权限列表
        /// </summary>
        /// <param name="attrEditList">字段列表</param>
        public void SaveEntityAttrActivityEditList(int processID, List<EntityAttrActivityEditEntity> attrEditList)
        {
            var wfService = new WorkflowService();
            var process = wfService.GetProcessByID(processID);

            foreach (var attrEdit in attrEditList)
            {
                attrEdit.ProcessGUID = process.ProcessGUID;
                attrEdit.Version = process.Version;
                SaveSingleEntityAttrActivityEdit(attrEdit);
            }
        }

        /// <summary>
        /// 保存单个实体对象
        /// </summary>
        /// <param name="entity"></param>
        private void SaveSingleEntityAttrActivityEdit(EntityAttrActivityEditEntity entity)
        {
            var attrEditEntity = QuerySingleAttrActivityEdit(entity.EntityDefID, entity.ProcessID, entity.ActivityGUID, entity.AttrID);
            if (attrEditEntity != null)
            {
                entity.ID = attrEditEntity.ID;
                var attrEntity = GetEntityAttribute(entity.AttrID);
                entity.AttrName = attrEntity.AttrName;
                QuickRepository.Update<EntityAttrActivityEditEntity>(entity);
            }
            else
            {
                QuickRepository.Insert<EntityAttrActivityEditEntity>(entity);
            }

            //删除无用节点权限数据
            RemoveUnusefulAttrActivityEditList(entity.EntityDefID, entity.ProcessID, entity.ActivityGUID);
        }

        /// <summary>
        /// 删除无用的节点字段权限数据
        /// </summary>
        /// <param name="entityDefID">表单</param>
        /// <param name="processID">流程ID</param>
        /// <param name="activityGUID">节点GUID</param>
        private void RemoveUnusefulAttrActivityEditList(int entityDefID, int processID, string activityGUID)
        {
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                var sql = @"DELETE FROM EavEntityAttrActivityEdit
                        WHERE EntityDefID=@entityDefID
                            AND ProcessID=@processID
                            AND ActivityGUID=@activityGUID
                            AND IsNotVisible=0 AND IsReadOnly=0";
                QuickRepository.Execute(session.Connection, sql,
                    new { entityDefID = entityDefID, processID = processID, activityGUID = activityGUID }, session.Transaction);

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
        /// 删除字段权限
        /// </summary>
        /// <param name="entityDefID">表单</param>
        /// <param name="processID">流程ID</param>
        /// <param name="activityGUID">节点GUID</param>
        /// <returns>删除结果</returns>
        public bool DeleteEntityAttrActivityEdit(int entityDefID, int processID, string activityGUID)
        {
            bool isOk = false;
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                var sql = @"DELETE FROM EavEntityAttrActivityEdit
                        WHERE EntityDefID=@entityDefID
                            AND ProcessID=@processID
                            AND ActivityGUID=@activityGUID";
                QuickRepository.Execute(session.Connection, sql, 
                    new { entityDefID = entityDefID, processID = processID, activityGUID = activityGUID }, session.Transaction);
              
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
            return isOk;
        }

        /// <summary>
        /// 删除表单节点字段权限
        /// </summary>
        /// <param name="entityDefID">表单</param>
        /// <param name="processID">流程ID</param>
        /// <returns>删除结果</returns>
        public bool ClearEntityAttrActivityEdit(int entityDefID)
        {
            bool isOk = false;
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                ClearEntityAttrActivityEdit(session.Connection, entityDefID, session.Transaction);
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
            return isOk;
        }

        /// <summary>
        /// 删除表单节点字段权限
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="entityDefID">表单</param>
        /// <param name="processID">流程ID</param>
        /// <param name="trans">事务</param>
        private void ClearEntityAttrActivityEdit(IDbConnection conn, int entityDefID, IDbTransaction trans)
        {
            var sql = @"DELETE FROM EavEntityAttrActivityEdit
                        WHERE EntityDefID=@entityDefID";
            QuickRepository.Execute(conn, sql,
                new { entityDefID = entityDefID }, trans);
        }
        #endregion
    }
}
