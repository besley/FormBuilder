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
    /// 属性管理器
    /// </summary>
    internal class EntityAttributeEventManager : ManagerBase
    {
        /// <summary>
        /// 查询事件列表
        /// </summary>
        /// <param name="entityDefID">表单ID</param>
        /// <param name="attrID">属性ID</param>
        /// <returns></returns>
        internal List<EntityAttributeEventView> GetEventList(int entityDefID, int attrID)
        {
            var sql = @"SELECT * FROM vw_EavEntityAttributeEvent
                        WHERE EntityDefID=@entityDefID
                            AND AttrID=@attrID";
            var list = Repository.Query<EntityAttributeEventView>(sql, new { entityDefID = entityDefID, attrID = attrID }).ToList();
            return list;
        }

        /// <summary>
        /// 查询事件列表
        /// </summary>
        /// <param name="entityDefID">表单ID</param>
        /// <returns></returns>
        internal List<EntityAttributeEventView> GetEventListByForm(int entityDefID)
        {
            var sql = @"SELECT * FROM vw_EavEntityAttributeEvent
                        WHERE EntityDefID=@entityDefID";
            var list = Repository.Query<EntityAttributeEventView>(sql, new { entityDefID = entityDefID}).ToList();
            return list;
        }

        /// <summary>
        /// 保存字段
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="entity">实体</param>
        /// <param name="trans">事务</param>
        /// <returns>事件实体</returns>
        internal EntityAttributeEventEntity SaveAttribute(IDbConnection conn, EntityAttributeEventEntity entity, 
            IDbTransaction trans)
        {
            int attrID = IsExistEvent(conn, entity, trans);
            if (attrID > 0)
            {
                Repository.Update<EntityAttributeEventEntity>(conn, entity, trans);
            }
            else
            {
                var newEventID = Repository.Insert<EntityAttributeEventEntity>(conn, entity, trans);
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
        private int IsExistEvent(IDbConnection conn, EntityAttributeEventEntity entity, IDbTransaction trans)
        {
            int id = -1;
            var sql = @"SELECT ID FROM EavEntityAttributeEvent 
                        WHERE EntityDefID=@entityDefID 
                            AND AttrID=@attrID
                            AND EventName=@eventName";
            var list = Repository.Query<EntityAttributeEntity>(conn, sql,
                new { 
                    entityDefID = entity.EntityDefID,
                    attrID = entity.AttrID,
                    eventName = entity.EventName
                }, trans).ToList(); ;

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
            return Repository.Delete<EntityAttributeEventEntity>(id);
        }

        /// <summary>
        /// 删除字段下的事件列表
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="entity">实体</param>
        /// <param name="trans">事务</param>
        /// <return></return>
        internal void DeleteByAttribute(IDbConnection conn, EntityAttributeEventEntity entity, IDbTransaction trans)
        {
            var sql = @"SELECT ID FROM EavEntityAttribute 
                        WHERE EntityDefID=@entityDefID 
                            AND AttrID=@attrID";
            Repository.Execute(conn, sql, 
                new {
                    entityDefID = entity.EntityDefID,
                    attrID = entity.AttrID
                }, trans);
        }

        /// <summary>
        /// 删除字段下的事件列表
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="entity">实体</param>
        /// <param name="trans">事务</param>
        /// <return></return>
        internal void DeleteByAttributeList(IDbConnection conn, List<EntityAttributeEntity> list, IDbTransaction trans)
        {
            var sql = @"SELECT ID FROM EavEntityAttributeEvent 
                        WHERE EntityDefID=@entityDefID 
                            AND AttrID=@attrID";

            foreach (var entity in list)
            {
                Repository.Execute(conn, sql,
                    new
                    {
                        entityDefID = entity.EntityDefID,
                        attrID = entity.ID
                    }, trans);
            }
        }
    }
}
