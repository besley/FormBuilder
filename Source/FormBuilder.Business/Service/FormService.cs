using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using DapperExtensions;
using Slickflow.Data;
using FormBuilder.Business.Entity;
using FormBuilder.Business.Manager;
using FormBuilder.Business.Utility;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Service;

namespace FormBuilder.Business.Service
{
    /// <summary>
    /// 表单定制服务的对外接口
    /// </summary>
    public class FormService : IFormService
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
        /// <param name="formID"></param>
        /// <returns></returns>
        public FormEntity GetForm(int formID)
        {
            var entity = QuickRepository.GetById<FormEntity>(formID);
            return entity;
        }

        /// <summary>
        /// 获取表单定义的HTML页面内容
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="formID">ID</param>
        /// <param name="trans">事务</param>
        /// <returns></returns>
        public FormEntity GetForm(IDbConnection conn, int formID, IDbTransaction trans)
        {
            return QuickRepository.GetById<FormEntity>(conn, formID, trans);
        }

        /// <summary>
        /// 表单实体列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<FormEntity> GetFormList2()
        {
            //var sql = @"SELECT ID, 
            //                EntityTitle, 
            //                EntityName, 
            //                EntityCode, 
            //                Version,
            //                Description, 
            //                CreatedDate, 
            //                LastUpdatedDate 
            //            FROM EavForm
            //            ORDER BY ID DESC";
            //var list = QuickRepository.Query<FormEntity>(sql, null)
            //            .ToList();
            var sqlQuery = (from f in QuickRepository.GetAll<FormEntity>()
                            orderby f.ID descending
                            select new FormEntity
                            {
                                ID = f.ID,
                                FormName = f.FormName,
                                FormCode = f.FormCode,
                                Version = f.Version,
                                Description = f.Description,
                                CreatedDate = f.CreatedDate,
                                LastUpdatedDate = f.LastUpdatedDate
                            });
            var list = sqlQuery.ToList();
            return list;
        }

        /// <summary>
        /// 表单视图列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<FormProcessView> GetFormViewList()
        {
            var sql = @"SELECT ED.ID, 
                            ED.EntityTitle, 
                            ED.EntityName, 
                            ED.EntityCode, 
                            EP.ProcessGUID,
                            EP.Version
                        FROM EavForm ED
                        LEFT JOIN EavEntityProcess EP
                            ON ED.ID=EP.FormID";
            var list = QuickRepository.Query<FormProcessView>(sql, null)
                        .ToList();
            return list;
        }

        /// <summary>
        /// 表单定义保存操作
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public FormEntity SaveForm(FormEntity entity)
        {
            FormEntity returnEntity = null;
            if (entity.ID == 0)
            {
                entity.FormCode = PinyinConverter.ConvertFirst(entity.FormName);
                entity.CreatedDate = System.DateTime.Now;
                var entityDefID = QuickRepository.Insert<FormEntity>(entity);
                entity.ID = entityDefID;

                returnEntity = entity;
            }
            else
            {
                var updEntity = QuickRepository.GetById<FormEntity>(entity.ID);
                updEntity.FormName = entity.FormName;
                updEntity.FormCode = PinyinConverter.ConvertFirst(entity.FormName);
                updEntity.Version = entity.Version;
                updEntity.Description = entity.Description;
                updEntity.LastUpdatedDate = System.DateTime.Now;
                QuickRepository.Update<FormEntity>(updEntity);

                returnEntity = updEntity;
            }
            return returnEntity;
        }

        /// <summary>
        /// 保存属性
        /// </summary>
        /// <param name="entity"></param>
        public FormFieldEntity SaveFormField(FormFieldEntity entity)
        {
            FormFieldEntity attrEntity = null;
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();

                //保存字段
                var ffm = new FormFieldManager();
                attrEntity = ffm.SaveField(session.Connection, entity, session.Transaction);

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
        public void SaveTemplateWithFieldList(FormFieldListView view)
        {
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();

                //保存字段列表
                var ffm = new FormFieldManager();
                ffm.SaveFieldList(session.Connection, view.FormFieldList, session.Transaction);

                //保存模板内容
                var edm = new FormManager();
                edm.SaveTemplateWithHTMLContent(session.Connection, view.Form, session.Transaction);

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
        /// 保存表单模板内容
        /// </summary>
        /// <param name="entity"></param>
        public void SaveTemplateContent(FormEntity entity)
        {
            var fm = new FormManager();
            fm.SaveTemplateContent(entity);
        }

        public void SaveTemplateWithHTMLContent(FormEntity entity)
        {
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();

                //保存模板内容
                var edm = new FormManager();
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
        /// <param name="formID"></param>
        public void DeleteForm(int formID)
        {
            //删除实体同时，要删除掉属性定义，删除掉所有属性取值5张表的数据
            var param = new DynamicParameters();
            param.Add("@formID", formID);

            using (IDbConnection conn = SessionFactory.CreateConnection())
            {
                QuickRepository.ExecuteProc(conn, "pr_fb_FormDelete", param);
            }
        }

        /// <summary>
        /// 表单升级
        /// </summary>
        /// <param name="formID">主键ID</param>
        public void UpgradeForm(int formID)
        {
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                var form = GetForm(session.Connection, formID, session.Transaction);
                int newVersion = 1;
                var parsed = int.TryParse(form.Version, out newVersion);
                if (parsed == true) newVersion = newVersion +1;
                form.Version = (int.Parse(form.Version) + 1).ToString();
                var formInserted = QuickRepository.Insert<FormEntity>(session.Connection, form, session.Transaction);

                //读取字段列表
                var fieldList = GetFormFieldList(session.Connection, formID, session.Transaction);
                foreach (var attr in fieldList)
                {
                    attr.FormID = formInserted;
                    QuickRepository.Insert<FormFieldEntity>(session.Connection, attr, session.Transaction);
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
        /// <param name="formID">实体定义ID</param>
        /// <returns>表单流程实体列表</returns>
        public List<FormProcessEntity> GetFormProcess(int formID)
        {
            FormProcessEntity entity = null;
            var sqlQuery = (from fp in QuickRepository.GetAll<FormProcessEntity>()
                            where fp.FormID == formID
                            select fp);

            var list = sqlQuery.ToList();
            return list;
        }

        /// <summary>
        /// 保存绑定流程信息
        /// </summary>
        /// <param name="entity"></param>
        public void BindFormProcess(FormProcessView view)
        {
            //保存绑定信息
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                //查找表单
                var formEntity = QuickRepository.GetById<FormEntity>(session.Connection, view.FormID, session.Transaction);
                //查找流程
                var processEntity = QuickRepository.GetById<ProcessEntity>(session.Connection, view.ProcessID, session.Transaction);
                var formProcessEntity = new FormProcessEntity
                {
                    ProcessID = processEntity.ID,
                    ProcessGUID = processEntity.ProcessGUID,
                    ProcessVersion = processEntity.Version,
                    ProcessName = processEntity.ProcessName,
                    FormID = view.FormID,
                    Version = formEntity.Version
                };

                //先删除之前绑定的流程信息
                //RemoveEntityProcess(session.Connection, view.FormID, session.Transaction);
                QuickRepository.Insert<FormProcessEntity>(session.Connection, formProcessEntity, session.Transaction);
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
        public void UnbindFormProcess(FormProcessView view)
        {
            //解除绑定信息
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                //先删除之前绑定的流程信息
                RemoveFormProcess(session.Connection, view.FormID, view.ProcessID, session.Transaction);
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
        /// <param name="formID">表单ID</param>
        public void RemoveFormProcess(int formID)
        {
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                var processList = GetFormProcess(formID);
                foreach (var process in processList)
                {
                    RemoveFormProcess(session.Connection, formID, process.ProcessID, session.Transaction);
                }
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
        /// <param name="formID">表单ID</param>
        /// <param name="processID">流程ID</param>
        /// <param name="trans">交易</param>
        private void RemoveFormProcess(IDbConnection conn, int formID, int processID, IDbTransaction trans)
        {
            //删除表单字段节点权限数据
            ClearFormFieldActivityEdit(conn, formID, processID, trans);

            //删除流程绑定记录
            var sqlQuery = (from fp in QuickRepository.GetAll<FormProcessEntity>()
                            where fp.FormID == formID 
                                && fp.ProcessID == processID 
                            select fp);
            var list = sqlQuery.ToList();

            foreach(var fp in list)
            {
                QuickRepository.Delete<FormProcessEntity>(conn, fp.ID, trans);
            }
        }

        /// <summary>
        /// 判断流程是否绑定了表单
        /// </summary>
        /// <param name="processID">流程ID</param>
        /// <returns>是否绑定</returns>
        public Boolean IsFormProcessBinding(int processID)
        {
            var isBinding = false;
            var sqlQuery = (from fp in QuickRepository.GetAll<FormProcessEntity>()
                            where fp.ProcessID == processID
                            select fp);
            var list = sqlQuery.ToList();

            if (list.Count() > 0)
            {
                isBinding = true;
            }
            return isBinding;
        }
        #endregion

        #region 字段操作
        /// <summary>
        /// 获取属性实体
        /// </summary>
        /// <param name="fieldID">属性id</param>
        /// <returns>属性实体</returns>
        public FormFieldEntity GetFormField(int fieldID)
        {
            var entity = QuickRepository.GetById<FormFieldEntity>(fieldID);
            return entity;

        }

        /// <summary>
        /// 查询表单字段
        /// </summary>
        /// <param name="query">查询实体</param>
        /// <returns>字段对象</returns>
        public FormFieldEntity GetFormFieldByGUID(FormFieldQuery query)
        {
            FormFieldEntity entity = null;
            var sqlQuery = (from f in QuickRepository.GetAll<FormFieldEntity>()
                            where f.FormID == query.FormID && f.FieldGUID == query.FieldGUID
                            select f
                            );
            var list = sqlQuery.ToList<FormFieldEntity>();
            if (list.Count == 1)
            {
                entity = list[0];
            }
            return entity;
        }

        /// <summary>
        /// 获取属性列表
        /// </summary>
        /// <param name="formID">实体定义ID</param>
        /// <returns>属性列表</returns>
        public List<FormFieldEntity> GetFormFieldList(int formID)
        {
            using (var session = SessionFactory.CreateSession())
            {
                var attributeList = GetFormFieldList(session.Connection, formID, session.Transaction);
                return attributeList;
            }
        }

        /// <summary>
        /// 获取属性列表
        /// </summary>
        /// <param name="conn">数据库链接</param>
        /// <param name="formID">实体定义ID</param>
        /// <param name="trans">数据库交易</param>
        /// <returns>属性列表</returns>
        public List<FormFieldEntity> GetFormFieldList(IDbConnection conn,
            int formID,
            IDbTransaction trans)
        {
            var sqlQuery = (from f in QuickRepository.GetAll<FormFieldEntity>()
                            where f.FormID == formID
                            select f);
            var list = sqlQuery.ToList<FormFieldEntity>();
            return list;
        }

        /// <summary>
        /// 获取数值录入字段列表
        /// </summary>
        /// <param name="entityDefID">实体定义ID</param>
        /// <returns>属性列表</returns>
        public List<FormFieldEntity> GetFormFieldListOnlyInfoValue(int entityDefID)
        {
            var sql = @"SELECT * FROM EavFormField
                        WHERE FormID=@entityDefID
                            AND StorageType = 1
                        ORDER BY DivCtrlKey";
            var list = QuickRepository.Query<FormFieldEntity>(sql, new { entityDefID = entityDefID })
                        .ToList();
            return list;
        }

        /// <summary>
        /// 读取表单和字段的组合数据
        /// </summary>
        /// <param name="entityDefID">表单ID</param>
        /// <returns></returns>
        public FormFieldListView GetFormFieldComp(int entityDefID)
        {
            var entity = new FormFieldListView();
            entity.Form = QuickRepository.GetById<FormEntity>(entityDefID);
            entity.FormFieldList = GetFormFieldList(entityDefID);

            return entity;
        }


        /// <summary>
        /// 删除字段，同时更新模板内容
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public bool DeleteFieldWithTemplate(FormFieldListView view)
        {
            bool isOk = false;
            if (view.FormFieldList != null
                && view.FormFieldList.Count > 0)
            {
                var session = SessionFactory.CreateSession();
                try
                {
                    session.BeginTrans();

                    //删除字段事件
                    var ffem = new FormFieldEventManager();
                    ffem.DeleteByFieldList(session.Connection, view.FormFieldList, session.Transaction);

                    //删除字段
                    var ffm = new FormFieldManager();
                    ffm.DeleteFieldBatch(session.Connection, view.FormFieldList, session.Transaction);

                    //更新模板内容
                    var fm = new FormManager();
                    fm.SaveTemplateWithHTMLContent(session.Connection, view.Form, session.Transaction);

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

        /// <summary>
        /// 删除组件
        /// </summary>
        /// <param name="query"></param>
        public Boolean DeleteComponent(FormFieldQuery query)
        {
            var isDeleted = false;
            var ffm = new FormFieldManager();
            var field = ffm.GetFieldByGUID(query.FormID, query.FieldGUID);
            if (field != null)
            {
                var session = SessionFactory.CreateSession();
                try
                {
                    session.BeginTrans();

                    //删除字段事件
                    var ffem = new FormFieldEventManager();
                    ffem.DeleteByField(session.Connection, field.ID, session.Transaction);

                    //删除字段
                    ffm.DeleteField(session.Connection, field.ID, session.Transaction);

                    session.Commit();

                    isDeleted = true;
                }
                catch (System.Exception)
                {
                    session.Rollback();
                    throw;
                }
                finally
                {
                    session.Dispose();
                }
            }
            return isDeleted;
        }
        #endregion

        #region 字段事件定义
        /// <summary>
        /// 字段绑定事件列表
        /// </summary>
        /// <param name="formID">实体ID</param>
        /// <param name="fieldID">属性ID</param>
        /// <returns>事件列表</returns>
        public List<FormFieldEventEntity> GetFormFieldEventList(int formID, int fieldID)
        {
            var ffem = new FormFieldEventManager();
            return ffem.GetEventList(formID, fieldID);
        }

        /// <summary>
        /// 字段绑定事件列表
        /// </summary>
        /// <param name="formID">实体ID</param>
        /// <returns>事件列表</returns>
        public List<FormFieldEventEntity> GetFormFieldEventListByForm(int formID)
        {
            var eaem = new FormFieldEventManager();
            return eaem.GetEventListByForm(formID);
        }

        /// <summary>
        /// 保存属性事件
        /// </summary>
        /// <param name="entity">事件实体</param>
        /// <returns></returns>
        public FormFieldEventEntity SaveFieldEvent(FormFieldEventEntity entity)
        {
            FormFieldEventEntity eventEntity = null;
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                var ffem = new FormFieldEventManager();
                eventEntity = ffem.SaveEvent(session.Connection, entity, session.Transaction);
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
        public void DeleteFieldEvent(int eventID)
        {
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                var eaem = new FormFieldEventManager();
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
        public void DeleteFieldEvent(int entityDefID, int attrID)
        {
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                var sql = @"DELETE FROM EavFormFieldEvent
                        WHERE FormID=@entityDefID
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
        /// <param name="formID">表单</param>
        /// <param name="processID">流程ID</param>
        /// <param name="activityGUID">节点GUID</param>
        /// <returns>字段列表</returns>
        public List<FormFieldActivityEditEntity> GetFormFieldActivityEditList(int formID, 
            int processID, 
            string activityGUID)
        {
            var sqlQuery = (from fae in QuickRepository.GetAll<FormFieldActivityEditEntity>()
                            where fae.FormID == formID
                                && fae.ProcessID == processID
                                && fae.ActivityGUID == activityGUID
                            select fae);
            var list = sqlQuery.ToList();
            return list;
        }

        /// <summary>
        /// 获取表单字段权限数据
        /// </summary>
        /// <param name="formID">表单</param>
        /// <param name="processGUID">流程GUID</param>
        /// <param name="processVersion">流程版本</param>
        /// <param name="activityGUID">节点GUID</param>
        /// <returns>字段列表</returns>
        public List<FormFieldActivityEditEntity> GetFormFieldActivityEditList(int formID, 
            string processGUID, 
            string processVersion, 
            string activityGUID)
        {
            var sqlQuery = (from fae in QuickRepository.GetAll<FormFieldActivityEditEntity>()
                            where fae.FormID == formID
                                && fae.ProcessGUID == processGUID
                                && fae.ProcessVersion == processVersion
                                && fae.ActivityGUID == activityGUID
                            select fae);
            var list = sqlQuery.ToList();
            return list;
        }

        /// <summary>
        /// 获取表单字段权限数据
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <returns>字段权限列表</returns>
        public List<FormFieldActivityEditEntity> GetFormFieldActivityEditList(int taskID)
        {
            var wfService = new WorkflowService();
            var taskView = wfService.GetTaskView(taskID);
            var formContentID = int.Parse(taskView.AppInstanceID);

            //获取表单实例
            var fcService = new FormDataService();
            var formContent = fcService.GetFormData(formContentID);

            //读取表单字段权限
            var list = GetFormFieldActivityEditList(formContent.FormID, taskView.ProcessGUID, taskView.Version, taskView.ActivityGUID);
            return list;
        }

        /// <summary>
        /// 查询当字段权限实体
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="formID">表单ID</param>
        /// <param name="processID">流程ID</param>
        /// <param name="activityGUID">节点GUID</param>
        /// <param name="fieldID">字段ID</param>
        /// <param name="trans">事务</param>
        /// <returns>权限实体</returns>
        private FormFieldActivityEditEntity QuerySingleAttrActivityEdit(IDbConnection conn,
            int formID,
            int processID, 
            string activityGUID, 
            int fieldID,
            IDbTransaction trans)
        {
            FormFieldActivityEditEntity entity = null;
            var sqlQuery = (from fae in QuickRepository.GetAll<FormFieldActivityEditEntity>(conn, trans)
                            where fae.FormID == formID
                                && fae.ProcessID == processID
                                && fae.ActivityGUID == activityGUID
                                && fae.FieldID == fieldID
                            select fae);
            var list = sqlQuery.ToList();
            if (list != null && list.Count() == 1)
                entity = list[0];
            return entity;
        }

        /// <summary>
        /// 保存表单字段节点权限列表
        /// </summary>
        /// <param name="comp">组合实体对象</param>
        public void SaveFormFieldActivityEditList(FormFieldActivityEditListComp comp)
        {
            var wfService = new WorkflowService();
            var process = wfService.GetProcessByID(comp.ProcessID);
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                foreach (var item in comp.FieldEditList)
                {
                    item.ProcessGUID = process.ProcessGUID;
                    item.ProcessVersion = process.Version;
                    item.ProcessID = process.ID;
                    item.ActivityGUID = comp.ActivityGUID;
                    item.ActivityName = comp.ActivityName;
                    SaveSingleFormFieldActivityEdit(session.Connection, item, session.Transaction);
                }
                session.Commit();

                //删除无用节点权限数据
                RemoveUnusefulAttrActivityEditList(comp.FormID, comp.ProcessID, comp.ActivityGUID);
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

        }

        /// <summary>
        /// 保存单个实体对象
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="item"></param>
        /// <param name="trans">事务</param>
        private void SaveSingleFormFieldActivityEdit(IDbConnection conn,
            FormFieldActivityEditEntity item,
            IDbTransaction trans)
        {
            var fieldEntity = (new FormFieldManager()).GetFormField(conn, item.FieldID, trans);
            item.FieldGUID = fieldEntity.FieldGUID;
            item.FieldName = fieldEntity.FieldName;
            item.Version = fieldEntity.Version;

            var editEntity = QuerySingleAttrActivityEdit(conn, item.FormID, item.ProcessID, item.ActivityGUID, item.FieldID, trans);
            if (editEntity != null)
            {
                item.ID = editEntity.ID;
                QuickRepository.Update<FormFieldActivityEditEntity>(conn, item, trans);
            }
            else
            {
                QuickRepository.Insert<FormFieldActivityEditEntity>(conn, item, trans);
            }
        }

        /// <summary>
        /// 删除无用的节点字段权限数据
        /// </summary>
        /// <param name="formID">表单</param>
        /// <param name="processID">流程ID</param>
        /// <param name="activityGUID">节点GUID</param>
        private void RemoveUnusefulAttrActivityEditList(int formID, 
            int processID, 
            string activityGUID)
        {
            var sqlQuery = (from fp in QuickRepository.GetAll<FormFieldActivityEditEntity>()
                            where fp.FormID == formID
                                && fp.ProcessID == processID
                                && fp.ActivityGUID == activityGUID
                                && fp.IsNotVisible == false
                                && fp.IsReadOnly == false
                            select fp);
            var list = sqlQuery.ToList();
            foreach (var item in list)
            {
                QuickRepository.Delete<FormFieldActivityEditEntity>(item.ID);
            }
        }

        /// <summary>
        /// 删除字段权限
        /// </summary>
        /// <param name="formID">表单</param>
        /// <param name="processID">流程ID</param>
        /// <param name="activityGUID">节点GUID</param>
        /// <returns>删除结果</returns>
        public bool DeleteFormFieldActivityEdit(int formID, int processID, string activityGUID)
        {
            bool isOk = false;
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                var sqlQuery = (from fp in QuickRepository.GetAll<FormFieldActivityEditEntity>()
                                where  fp.FormID == formID
                                    && fp.ProcessID == processID
                                    && fp.ActivityGUID == activityGUID
                                select fp);
                var list = sqlQuery.ToList();
                foreach(var item in list)
                {
                    QuickRepository.Delete<FormFieldActivityEditEntity>(session.Connection, item.ID, session.Transaction);
                }            
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
        /// <param name="formID">表单</param>
        /// <param name="processID">流程ID</param>
        /// <returns>删除结果</returns>
        public bool ClearFormFieldActivityEdit(int formID, int processID)
        {
            bool isOk = false;
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                ClearFormFieldActivityEdit(session.Connection, formID, processID, session.Transaction);
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
        /// <param name="formID">表单</param>
        /// <param name="processID">流程ID</param>
        /// <param name="trans">事务</param>
        private void ClearFormFieldActivityEdit(IDbConnection conn, int formID, int processID, IDbTransaction trans)
        {
            var sqlQuery = (from fp in QuickRepository.GetAll<FormFieldActivityEditEntity>()
                            where fp.FormID == formID
                                && fp.ProcessID == processID
                            select fp);
            var list = sqlQuery.ToList();

            foreach (var fp in list)
            {
                QuickRepository.Delete<FormFieldActivityEditEntity>(conn, fp.ID, trans);
            }
        }
        #endregion
    }
}
