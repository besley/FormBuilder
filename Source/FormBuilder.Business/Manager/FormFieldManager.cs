using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Slickflow.Data;
using FormBuilder.Business.Entity;
using FormBuilder.Business.Utility;

namespace FormBuilder.Business.Manager
{
    /// <summary>
    /// 属性管理器
    /// </summary>
    internal class FormFieldManager : ManagerBase
    {
        /// <summary>
        /// 获取字段记录
        /// </summary>
        /// <param name="formID">表单ID</param>
        /// <param name="fieldGUID">字段GUID</param>
        /// <returns></returns>
        internal FormFieldEntity GetFieldByGUID(int formID, string fieldGUID)
        {
            using (var session = SessionFactory.CreateSession())
            {
                return GetFieldByGUID(session.Connection, formID, fieldGUID, session.Transaction);
            }
        }

        /// <summary>
        /// 获取字段记录
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="formID">表单ID</param>
        /// <param name="fieldGUID">字段GUID</param>
        /// <param name="trans">事务</param>
        /// <returns></returns>
        internal FormFieldEntity GetFieldByGUID(IDbConnection conn, int formID, string fieldGUID, IDbTransaction trans)
        {
            FormFieldEntity entity = null;
            var sqlQuery = (from f in Repository.GetAll<FormFieldEntity>(conn, trans)
                            where f.FormID == formID && f.FieldGUID == fieldGUID
                            select f);
            var list = sqlQuery.ToList<FormFieldEntity>();
            if (list.Count() == 1)
            {
                entity = list[0];
            }
            return entity;
        }

        /// <summary>
        /// 获取字段实体
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="fieldID">字段ID</param>
        /// <param name="trans">事务</param>
        /// <returns></returns>
        internal FormFieldEntity GetFormField(IDbConnection conn, int fieldID, IDbTransaction trans)
        {
            return Repository.GetById<FormFieldEntity>(conn, fieldID, trans);
        }

        /// <summary>
        /// 保存字段
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="entity">实体</param>
        /// <param name="trans">事务</param>
        /// <returns></returns>
        internal FormFieldEntity SaveField(IDbConnection conn, FormFieldEntity entity, 
            IDbTransaction trans)
        {
            var item = GetFieldByGUID(conn, entity.FormID, entity.FieldGUID, trans);
            if (item != null)
            {
                entity.FieldCode = PinyinConverter.ConvertFirst(entity.FieldName);
                Repository.Update<FormFieldEntity>(conn, entity, trans);
            }
            else
            {
                entity.FieldCode = PinyinConverter.ConvertFirst(entity.FieldName);
                var newFieldID = Repository.Insert<FormFieldEntity>(conn, entity, trans);
                entity.ID = newFieldID;
            }
                
            return entity;
        }

        /// <summary>
        /// 保存字段列表
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="entity">实体</param>
        /// <param name="trans">事务</param>
        /// <returns></returns>
        internal void SaveFieldList(IDbConnection conn, 
            List<FormFieldEntity> fieldList,
            IDbTransaction trans)
        {
            foreach (var f in fieldList)
            {
                SaveField(conn, f, trans);
            }
        }

        /// <summary>
        /// 查询属性字段是否已经存在
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        private int IsExistKey(IDbConnection conn, FormFieldEntity entity, IDbTransaction trans)
        {
            int id = -1;
            var sqlQuery = (from f in Repository.GetAll<FormFieldEntity>()
                            where f.FormID == entity.FormID && f.FieldGUID == entity.FieldGUID
                            select f);
            var list = sqlQuery.ToList<FormFieldEntity>();
            if (list.Count() == 1)
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
        internal void DeleteFieldBatch(IDbConnection conn, List<FormFieldEntity> list, IDbTransaction trans)
        {
            foreach (var entity in list)
            {
                var id = entity.ID;
                Repository.Delete<FormFieldEntity>(conn, id, trans);
            }
        }

        /// <summary>
        /// 根据表单id和控件key删除字段
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="list"></param>
        /// <param name="trans"></param>
        internal void DeleteField(IDbConnection conn, int fieldID, IDbTransaction trans)
        {
            Repository.Delete<FormFieldEntity>(conn, fieldID, trans);
        }
    }
}
