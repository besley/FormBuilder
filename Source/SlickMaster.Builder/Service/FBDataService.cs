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
using SlickMaster.Builder.Utility;
using Slickflow.Engine.Common;


namespace SlickMaster.Builder.Service
{
    public class FBDataService : IFBDataService
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

        #region 实体数据操作
        /// <summary>
        /// 读取最近有数据更新的表单数据
        /// </summary>
        /// <returns></returns>
        public List<EntityInfoView> GetEntityInfoTop10()
        {
            var sql = @"SELECT TOP 10
	                        EI.ID, 
	                        EI.EntityDefID,
	                        ED.EntityName,
	                        ED.EntityTitle,
                            EP.ProcessGUID,
	                        EI.CreatedUserID,
	                        EI.CreatedUserName,
	                        EI.CreatedDatetime,
	                        EI.LastUpdatedUserID,
	                        EI.LastUpdatedUserName,
	                        EI.LastUpdatedDatetime
                        FROM EavEntityInfo EI
                        INNER JOIN EavEntityDef ED
	                        ON EI.EntityDefID = ED.ID
                        LEFT JOIN EavEntityProcess EP
                            ON ED.ID = EP.EntityDefID
                        ORDER BY EI.LastUpdatedDatetime DESC";
            var list = QuickRepository.Query<EntityInfoView>(sql).ToList();
            return list;
        }

        /// <summary>
        /// 根据表单实例ID，读取表单字段数值
        /// </summary>
        /// <param name="entityInfoID"></param>
        /// <returns></returns>
        public List<EntityAttrValueItem> QueryEntityAttrValue(int entityInfoID)
        {
            List<EntityAttrValueItem> list = null;
            try
            {
                var param = new DynamicParameters();
                param.Add("@entityInfoID", entityInfoID);

                using (var conn = SessionFactory.CreateConnection())
                {
                    list = QuickRepository.ExecProcQuery<EntityAttrValueItem>(conn,
                        "pr_eav_EntityAttrValueQuery", param).ToList<EntityAttrValueItem>();
                }

                return list;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取特定表单的实例数据
        /// </summary>
        /// <param name="query">实体查询对象</param>
        /// <returns>表单数值列表</returns>
        public List<dynamic> GetEntityInfoWithAttrValueList(EntityInfoQuery query)
        {
            List<dynamic> list = null;
            try
            {
                var param = new DynamicParameters();
                param.Add("@queryType", 1);
                param.Add("@entityDefID", query.EntityDefID);

                using (var conn = SessionFactory.CreateConnection())
                {
                    list = QuickRepository.ExecProcQuery<dynamic>(conn, "pr_eav_EntityAttrValuePivotGet", param).ToList<dynamic>();
                }
                return list;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 表单单条数据获取
        /// </summary>
        /// <param name="entityInfoID"></param>
        /// <returns></returns>
        public List<dynamic> GetEntityInfoWithAttrValue(int entityInfoID)
        {
            List<dynamic> list = null;
            try
            {
                var param = new DynamicParameters();
                param.Add("@queryType", 2);
                param.Add("@entityInfoID", entityInfoID);

                using (var conn = SessionFactory.CreateConnection())
                {
                    list = QuickRepository.ExecProcQuery<dynamic>(conn, "pr_eav_EntityAttrValuePivotGet", param).ToList<dynamic>();
                }

                if (list != null && list.Count() == 1)
                    return list[0];
                else
                    return null;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取实体及扩展数据的分页方法
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<dynamic> GetEntityInfoWithAttrValueListPaged(EntityInfoAttrQuery query, out int count)
        {
            List<dynamic> list = null;
            try
            {
                var param = new DynamicParameters();
                param.Add("@entityDefID", query.EntityDefID);
                param.Add("@createdUserID", query.CreatedUserID);
                param.Add("@entityInfoID", query.EntityID);

                param.Add("@pageIndex", query.PageIndex);
                param.Add("@pageSize", query.PageSize);
                param.Add("@rowsCount", null, DbType.Int32, ParameterDirection.Output);

                using (var conn = SessionFactory.CreateConnection())
                {
                    list = QuickRepository.ExecProcQuery<dynamic>(conn, "pr_eav_EntityAttrValuePivotGetPaged", param).ToList<dynamic>();
                }


                //得到查询的总记录数目
                count = param.Get<int>("@rowsCount");

                return list;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// 整合插入实体数据
        /// </summary>
        /// <param name="item">实体对象</param>
        /// <returns>新实体的实例ID</returns>
        public int InsertRow(EntityInfoWithAttrValueListItem item)
        {
            int newEntityInfoID = 0;
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();

                //保存实体基本信息
                var entityInfo = item.EntityInfo;
                entityInfo.CreatedDatetime = System.DateTime.Now;
                entityInfo.LastUpdatedDatetime = System.DateTime.Now;
                newEntityInfoID = QuickRepository.Insert<EntityInfoEntity>(session.Connection, entityInfo, session.Transaction);

                //保存实体扩展属性
                foreach (var v in item.EntityAttrValueList)
                {
                    v.EntityInfoID = newEntityInfoID;
                }

                var attrValueList = item.EntityAttrValueList;
                var eavManager = new EntityAttrValueManager();
                eavManager.InsertBatch(session.Connection, attrValueList, session.Transaction);

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
            return newEntityInfoID;
        }

        public void InsertRowFlow(EntityAttrValueFlowItem item)
        {
            //首先保存表单数据
            int newEntityInfoID = 0;
            var session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();

                //保存实体基本信息
                var entityInfo = item.EntityInfo;
                entityInfo.CreatedDatetime = System.DateTime.Now;
                entityInfo.LastUpdatedDatetime = System.DateTime.Now;
                newEntityInfoID = QuickRepository.Insert<EntityInfoEntity>(session.Connection, entityInfo, session.Transaction);

                //保存实体扩展属性
                foreach (var v in item.EntityAttrValueList)
                {
                    v.EntityInfoID = newEntityInfoID;
                }

                var attrValueList = item.EntityAttrValueList;
                var eavManager = new EntityAttrValueManager();
                eavManager.InsertBatch(session.Connection, attrValueList, session.Transaction);

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

            //启动流程

        }

        /// <summary>
        /// 更新实体对象
        /// </summary>
        /// <param name="item">实体对象</param>
        public void UpdateRow(EntityInfoWithAttrValueListItem item)
        {
            var session = SessionFactory.CreateSession();

            try
            {
                session.BeginTrans();

                //更新主表记录
                var entityInfo = QuickRepository.GetById<EntityInfoEntity>(item.EntityInfo.ID);
                entityInfo.LastUpdatedDatetime = System.DateTime.Now;
                entityInfo.LastUpdatedUserID = item.EntityInfo.LastUpdatedUserID;
                entityInfo.LastUpdatedUserName = item.EntityInfo.LastUpdatedUserName;

                QuickRepository.Update<EntityInfoEntity>(session.Connection, entityInfo, session.Transaction);

                //更新扩展属性表记录
                var eavManager = new EntityAttrValueManager();
                eavManager.UpdateItem(session.Connection, item.EntityAttrValueList, session.Transaction);

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
        /// 删除属性
        /// 同时删除主表数据，还有扩展属性数据
        /// </summary>
        /// <param name="entityInfoID"></param>
        public void DeleteRow(int entityInfoID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@entityID", entityInfoID);

                using (IDbConnection conn = SessionFactory.CreateConnection())
                {
                    QuickRepository.ExecuteProc(conn, "pr_eav_EntityInfoDelete", param);
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        public void DeleteEntityInfo(int id)
        {

        }
        #endregion
    }
}
