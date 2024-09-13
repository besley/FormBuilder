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
    /// 属性管理器
    /// </summary>
    internal class FormFieldEventManager : ManagerBase
    {
        /// <summary>
        /// 查询事件列表
        /// </summary>
        /// <param name="formID">表单ID</param>
        /// <param name="fieldID">属性ID</param>
        /// <returns></returns>
        internal List<FormFieldEventEntity> GetEventList(int formID, int fieldID)
        {
            var sqlQuery = (from e in Repository.GetAll<FormFieldEventEntity>()
                            where e.FormID == formID && e.FieldID == fieldID
                            select e);
            var list = sqlQuery.ToList();
            return list;
        }

        /// <summary>
        /// 查询事件列表
        /// </summary>
        /// <param name="formID">表单ID</param>
        /// <returns></returns>
        internal List<FormFieldEventEntity> GetEventListByForm(int formID)
        {
            var sqlQuery = (from e in Repository.GetAll<FormFieldEventEntity>()
                            where e.FormID == formID
                            select e);
            var list = sqlQuery.ToList();
            return list;
        }

        /// <summary>
        /// 保存字段
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="entity">实体</param>
        /// <param name="trans">事务</param>
        /// <returns>事件实体</returns>
        internal FormFieldEventEntity SaveEvent(IDbConnection conn, FormFieldEventEntity entity, 
            IDbTransaction trans)
        {
            int attrID = IsExistEvent(conn, entity, trans);
            if (attrID > 0)
            {
                Repository.Update<FormFieldEventEntity>(conn, entity, trans);
            }
            else
            {
                var newEventID = Repository.Insert<FormFieldEventEntity>(conn, entity, trans);
                entity.ID = newEventID;
            }
                
            return entity;
        }

        /// <summary>
        /// 查询属性字段是否已经存在
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="entity">实体</param>
        /// <param name="trans">事务</param>
        /// <returns>ID</returns>
        private int IsExistEvent(IDbConnection conn, FormFieldEventEntity entity, IDbTransaction trans)
        {
            int id = -1;

            var sqlQuery = (from e in Repository.GetAll<FormFieldEventEntity>()
                            where e.FormID == entity.FormID && e.FieldID == entity.FieldID && e.EventName == entity.EventName
                            select e);
            var list = sqlQuery.ToList();

            if (list != null && list.Count() == 1)
            {
                id = list[0].ID;
            }

            return id;
        }

        /// <summary>
        /// 根据ID删除事件
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="entity">实体</param>
        /// <param name="trans">事务</param>
        /// <return></return>
        internal Boolean Delete(IDbConnection conn, int id, IDbTransaction trans)
        {
            return Repository.Delete<FormFieldEventEntity>(id);
        }

        /// <summary>
        /// 删除字段下的事件列表
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="fieldID">字段ID</param>
        /// <param name="trans">事务</param>
        /// <return></return>
        internal void DeleteByField(IDbConnection conn, int fieldID, IDbTransaction trans)
        {
            var sqlQuery = (from e in Repository.GetAll<FormFieldEventEntity>()
                            where e.FieldID == fieldID
                            select new
                            {
                                ID = e.ID
                            });
            var eventList = sqlQuery.ToList();
            foreach (var e in eventList)
            {
                Repository.Delete<FormFieldEventEntity>(conn, e.ID, trans);
            }
        }

        /// <summary>
        /// 删除字段下的事件列表
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="entity">实体</param>
        /// <param name="trans">事务</param>
        /// <return></return>
        internal void DeleteByFieldList(IDbConnection conn, List<FormFieldEntity> list, IDbTransaction trans)
        {
            var fieldIDs = list.Select(f => f.ID).ToArray();
            var sqlQuery = (from e in Repository.GetAll<FormFieldEventEntity>()
                            where fieldIDs.Contains(e.FieldID)
                            select new
                            {
                                ID = e.ID
                            });
            var eventList = sqlQuery.ToList();
            foreach (var e in eventList)
            {
                Repository.Delete<FormFieldEventEntity>(conn, e.ID, trans);
            }
        }
    }
}
