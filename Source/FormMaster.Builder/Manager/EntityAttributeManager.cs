using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Slickflow.Data;
using FormMaster.Builder.Entity;
using FormMaster.Builder.Utility;

namespace FormMaster.Builder.Manager
{
    /// <summary>
    /// 属性管理器
    /// </summary>
    internal class EntityAttributeManager : ManagerBase
    {

        /// <summary>
        /// 保存字段
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="entity">实体</param>
        /// <param name="trans">事务</param>
        /// <returns></returns>
        internal EntityAttributeEntity SaveAttribute(IDbConnection conn, EntityAttributeEntity entity, 
            IDbTransaction trans)
        {
            var fem = new EntityAttributeManager();
            int attrID = IsExistKey(conn, entity, trans);

            if (attrID > 0)
            {
                entity.ID = attrID;
                entity.AttrCode = PinyinConverter.ConvertFirst(entity.AttrName);
                Repository.Update<EntityAttributeEntity>(conn, entity, trans);
            }
            else
            {
                entity.AttrCode = PinyinConverter.ConvertFirst(entity.AttrName);
                var newAttrID = Repository.Insert<EntityAttributeEntity>(conn, entity, trans);
                entity.ID = newAttrID;
            }
                
            return entity;
        }

        /// <summary>
        /// 查询属性字段是否已经存在
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        private int IsExistKey(IDbConnection conn, EntityAttributeEntity entity, IDbTransaction trans)
        {
            int id = -1;
            var sql = @"SELECT ID FROM EavEntityAttribute 
                        WHERE EntityDefID=@entityDefID 
                            AND DivCtrlKey=@divCtrlKey";
            var list = Repository.Query<EntityAttributeEntity>(conn, sql,
                new { 
                    entityDefID = entity.EntityDefID, 
                    divCtrlKey = entity.DivCtrlKey 
                }, trans).ToList(); ;

            if (list != null && list.Count() == 1)
            {
                id = list[0].ID;
            }

            return id;
        }

        /// <summary>
        /// 根据表单id和控件key删除字段
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="list"></param>
        /// <param name="trans"></param>
        internal void DeleteAttribute(IDbConnection conn, List<EntityAttributeEntity> list, IDbTransaction trans)
        {
            string sql = @"DELETE FROM EavEntityAttribute
                            WHERE EntityDefID=@entityDefID
                                AND DivCtrlKey=@divCtrlKey";
            foreach (var entity in list)
            {
                Repository.Execute(conn, sql, 
                    new { 
                        entityDefID = entity.EntityDefID,
                        divCtrlKey = entity.DivCtrlKey
                    }, trans);
            }
        }
    }
}
