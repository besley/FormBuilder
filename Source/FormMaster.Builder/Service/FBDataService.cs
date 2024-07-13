using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Dapper;
using DapperExtensions;
using SlickOne.WebUtility;
using Slickflow.Data;
using FormMaster.Builder.Common;
using FormMaster.Builder.Entity;
using FormMaster.Builder.Manager;
using FormMaster.Builder.Utility;


namespace FormMaster.Builder.Service
{
    /// <summary>
    /// 表单数据查询服务
    /// </summary>
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
        /// 获取表单实例实体
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns>表单实例</returns>
        public EntityInfoEntity GetEntityInfo(int id)
        {
            var entity = QuickRepository.GetById<EntityInfoEntity>(id);
            return entity;
        }

        /// <summary>
        /// 保存表单实例信息
        /// </summary>
        /// <param name="entity">表单实例</param>
        public EntityInfoEntity SaveEntityInfo(EntityInfoEntity entity)
        {
            if (entity.ID == 0)
            {
                var newID = QuickRepository.Insert<EntityInfoEntity>(entity);
                entity.ID = newID;
            }
            else
            {
                QuickRepository.Update<EntityInfoEntity>(entity);
            }
            return entity;
        }

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
        /// 查询表单实例数据列表（100条）
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <returns>查询结果</returns>
        public List<EntityInfoView> GetEntityInfoSimpleList(EntityInfoQuery query)
        {
            var sql = @"SELECT TOP 100 
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
                        WHERE EI.EntityDefID=@entityDefID  
                        ORDER BY EI.LastUpdatedDatetime DESC";

            var list = QuickRepository.Query<EntityInfoView>(sql, new { entityDefID=query.EntityDefID }).ToList();
            return list;
        }

        /// <summary>
        /// 根据表单实例ID，读取表单字段数值
        /// </summary>
        /// <param name="entityInfoID">实体数据ID</param>
        /// <returns>属性值列表</returns>
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
        /// <param name="entityInfoID">实体ID</param>
        /// <returns>属性列表</returns>
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
        /// <param name="query">查询</param>
        /// <returns>属性列表</returns>
        public List<dynamic> GetEntityInfoWithAttrValueListPaged(EntityInfoAttrQuery query, out int count)
        {
            List<dynamic> list = null;
            try
            {
                var param = new DynamicParameters();
                param.Add("@entityDefID", query.EntityDefID);
                param.Add("@createdUserID", query.CreatedUserID);
                param.Add("@entityInfoID", query.EntityInfoID);

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
        /// 根据属性查询
        /// Example:
        /// {"EntityDefID": "32", "WhereSql": "CCSMU12=\'gdg\' or JEFWB35=\'5000以内\'"}
        /// SQL:
        /// SELECT * FROM ##myPivotTable030609021 WHERE CCSMU12='gdg' OR JEFWB35='5000以内'
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <returns>属性列表</returns>
        public List<dynamic> QueryEntityInfoWithAttrValueListDynamic(EntityInfoAttrQuery query)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@entityDefID", query.EntityDefID);
                param.Add("@createdBeginDateTime", query.CreatedBeginDateTime);
                param.Add("@createdEndDateTime", query.CreatedEndDateTime);
                param.Add("@whereSql", query.WhereSQL);

                using (var conn = SessionFactory.CreateConnection())
                {
                    var list = QuickRepository.ExecProcQuery<dynamic>(conn, "pr_eav_EntityAttrValuePivotQuery", param).ToList();
                    return list;
                }
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
                param.Add("@entityInfoID", entityInfoID);

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

        /// <summary>
        /// 删除表单实例数据
        /// </summary>
        /// <param name="id">表单实例ID</param>
        public void DeleteEntityInfo(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@entityInfoID", id);

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
        #endregion

        #region 控件数据源加载
        /// <summary>
        /// 加载控件数据源
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>对象列表</returns>
        public List<dynamic> LoadControlDataSource(EntityAttributeEntity entity)
        {
            var repository = new Repository();
            var sql = string.Empty;
            DataSourceTypeEnum dsType = EnumHelper.ParseEnum<DataSourceTypeEnum>(entity.DataSourceType.ToString());
            if (dsType == DataSourceTypeEnum.LocalDataTable)
            {
                sql = string.Format("SELECT {0}, {1} FROM {2}", entity.DataValueField, entity.DataTextField, entity.DataEntityName);
                var list = repository.Query<dynamic>(sql).ToList();
                return list;
            }
            else if (dsType == DataSourceTypeEnum.SQL)
            {
                
                sql = entity.DataEntityName;
                var list = repository.Query<dynamic>(sql).ToList();
                return list;
            }
            else if (dsType == DataSourceTypeEnum.StoreProcedure)
            {
                var spName = entity.DataEntityName;
                var list = repository.ExecProcQuery<dynamic>(spName, null).ToList();
                return list;
            }
            else if (dsType == DataSourceTypeEnum.WebAPIHttp)
            {
                var url = entity.DataEntityName;
                var httpClient = HttpClientHelper.CreateHelper(url);
                var list = httpClient.GetList<dynamic>();
                return list;
            }
            return null;
        }

        /// <summary>
        /// 加载控件数据源
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>对象列表</returns>
        public List<dynamic> LoadCascadeControlDataSource(CascadeControlInfo entity)
        {
            var repository = new Repository();
            var cascadeSql = string.Format("{0} WHERE {1}={2}", entity.DataEntityName, entity.CascadeFieldName, entity.ParentControlValue);
            var cascadeList = repository.Query<dynamic>(cascadeSql).ToList();
            return cascadeList;
        }

        /// <summary>
        /// 查询级联控件列表
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>对象列表</returns>
        public List<EntityAttributeEntity> QueryCascadeChildControlList(CascadeParentControlInfo entity)
        {
            var sql = @"SELECT * FROM EavEntityAttribute
                        WHERE EntityDefID=@entityDefID 
                            AND CascadeControlID=@attrID";
            var repository = new Repository();
            var list = repository.Query<EntityAttributeEntity>(sql,
                new
                {
                    entityDefID = entity.EntityDefID,
                    attrID = entity.AttributeID
                }).ToList();

            return list;
        }
        #endregion
    }
}
