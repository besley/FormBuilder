using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Slickflow.Data;
using FormBuilder.Business.Common;
using FormBuilder.Business.Entity;
using FormBuilder.Business.Utility;

namespace FormBuilder.Business.Manager
{
    /// <summary>
    /// 属性取值管理器
    /// </summary>
    public class FormFieldValueManager : ManagerBase
    {
        #region 基本构造方法
        private static readonly IDictionary<FieldDataTypeEnum, string> DictFormFieldValueItemTableName;
        static FormFieldValueManager()
        {
            DictFormFieldValueItemTableName = new Dictionary<FieldDataTypeEnum, string>();
            DictFormFieldValueItemTableName.Add(FieldDataTypeEnum.VARCHAR, "EavFormFieldVarchar");
            DictFormFieldValueItemTableName.Add(FieldDataTypeEnum.INT, "EavFormFieldInt");
            DictFormFieldValueItemTableName.Add(FieldDataTypeEnum.DECIMAL, "EavFormFieldDecimal");
            DictFormFieldValueItemTableName.Add(FieldDataTypeEnum.DATETIME, "EavFormFieldDatetime");
            DictFormFieldValueItemTableName.Add(FieldDataTypeEnum.TEXT, "EavFormFieldText");
        }

        private string GetTableName(int type)
        {
            FieldDataTypeEnum key = EnumHelper.ParseEnum<FieldDataTypeEnum>(type.ToString());
            if (DictFormFieldValueItemTableName.ContainsKey(key))
                return DictFormFieldValueItemTableName[key];

            throw new ApplicationException("FormFieldValueItem 的数据类型无效，不能识别对应的数据表名称！");
        }

        /// <summary>
        /// 根据表单实例ID，属性ID获取扩展属性
        /// </summary>
        /// <param name="entityInfoID"></param>
        /// <param name="attrID"></param>
        /// <returns></returns>
        private bool IsExist(string tblName, int entityInfoID, int attrID)
        {
            bool isExist = false;
            var sql = string.Format("SELECT * FROM {0} WHERE FormInfoID={1} AND AttrID={2}",
                tblName, entityInfoID, attrID);

            var list = Repository.Query<FormFieldValueItem>(sql).ToList<FormFieldValueItem>();
            if (list != null && list.Count == 1)
                isExist = true;

            return isExist;
        }
        #endregion

        #region 插入方法
        /// <summary>
        /// 批量插入方法
        /// </summary>
        /// <param name="conn">数据库链接</param>
        /// <param name="list">列表</param>
        /// <param name="trans">数据库事务</param>
        internal void InsertBatch(IDbConnection conn, List<FormFieldValueItem> list, IDbTransaction trans)
        {
            if (list != null && list.Count() > 0)
            {
                var insSql = GetInsertSql(list);
                if (!string.IsNullOrEmpty(insSql))
                {
                    Repository.Execute(conn, insSql, null, trans);
                }
            }
            else
            {
                throw new ApplicationException(string.Format("插入失败，传入的列表对象{0}不能为空！", "List<FormFieldValueItem>"));
            }
        }

        /// <summary>
        /// 获取批量插入的SQL语句
        /// </summary>
        /// <param name="list">列表</param>
        /// <returns>SQL语句</returns>
        private string GetInsertSql(List<FormFieldValueItem> list)
        {
            string tblName = string.Empty;
            string insSql = string.Empty;
            var sqlBuilder = new StringBuilder(1024);

            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    tblName = GetTableName(item.FieldDataType);
                    if (item.FieldDataType == (int)FieldDataTypeEnum.VARCHAR
                        || item.FieldDataType == (int)FieldDataTypeEnum.TEXT
                        || item.FieldDataType == (int)FieldDataTypeEnum.DATETIME)
                     {
                        insSql = string.Format("INSERT INTO {0}(FormID, FormInfoID, AttrID, Value) VALUES({1}, {2}, {3}, '{4}');",
                            tblName, item.FormID, item.FormInfoID, item.ID, item.Value);
                    }
                    else
                    {
                        insSql = string.Format("INSERT INTO {0}(FormID, FormInfoID, AttrID, Value) VALUES({1}, {2}, {3}, {4});",
                            tblName, item.FormID, item.FormInfoID, item.ID, item.Value);
                    }
                    sqlBuilder.Append(insSql);
                }
            }
            return sqlBuilder.ToString();
        }
        #endregion

        #region 更新方法
        /// <summary>
        /// 更新扩展属性方法
        /// 1. 先删除
        /// 2. 后插入
        /// </summary>
        /// <param name="conn">数据库链接</param>
        /// <param name="list">列表</param>
        /// <param name="trans">事务</param>
        internal void UpdateItem(IDbConnection conn, List<FormFieldValueItem> list, IDbTransaction trans)
        {
            var listD = (from a in list
                         where (String.IsNullOrEmpty(a.Value) == true)
                         select a).ToList<FormFieldValueItem>();

            var delSQL = GetDeleteSql(listD);

            var listUP = (from a in list
                          where (String.IsNullOrEmpty(a.Value) == false)
                          select a).ToList<FormFieldValueItem>();

            var delFirstSql = GetDeleteSql(listUP);
            var insSecondSql = GetInsertSql(listUP);

            var sqlBuilder = new StringBuilder(1024);
            sqlBuilder.Append(delSQL);
            sqlBuilder.Append(delFirstSql);
            sqlBuilder.Append(insSecondSql);

            var sql = sqlBuilder.ToString();

            if (!string.IsNullOrEmpty(sql))
            {
                Repository.Execute(conn, sql, null, trans);
            }
        }

        /// <summary>
        /// 获取删除语句
        /// </summary>
        /// <param name="list">列表对象</param>
        /// <returns>删除SQL语句</returns>
        private string GetDeleteSql(List<FormFieldValueItem> list)
        {
            if (list == null || list.Count() == 0)
                return string.Empty;

            string tblName = string.Empty;
            string delSql = string.Empty;
            var sqlBuilder = new StringBuilder(1024);

            foreach (var entity in list)
            {
                if (entity.FieldDataType > 0)
                {
                    tblName = GetTableName(entity.FieldDataType);
                    delSql = string.Format("DELETE FROM {0} WHERE FormInfoID={1} AND AttrID={2};",
                        tblName, entity.FormInfoID, entity.ID);
                    sqlBuilder.Append(delSql);
                }
            }
            return sqlBuilder.ToString();
        }
        #endregion
    }
}

