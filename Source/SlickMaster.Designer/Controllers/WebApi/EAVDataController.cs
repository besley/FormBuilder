using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SlickOne.Data;
using SlickOne.WebUtility;
using SlickMaster.Builder.Entity;
using SlickMaster.Builder.Service;

namespace SlickMaster.Designer.Controllers.WebApi
{
    /// <summary>
    /// EAV数据WebApi服务
    /// </summary>
    public class EAVDataController : BaseWebApiController
    {
        #region 实体定义操作
        /// <summary>
        /// 根据ID获取表单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult<EntityDefEntity> GetEntityDefByID(int id)
        {
            var result = ResponseResult<EntityDefEntity>.Default();
            try
            {
                var entity = QuickReporsitory.GetById<EntityDefEntity>(id);
                result = ResponseResult<EntityDefEntity>.Success(entity);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<EntityDefEntity>.Error(
                    string.Format("读取{0}数据失败, 错误：{1}", "EntityDef", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 获取前10条数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult<List<EntityDefEntity>> GetEntityDefList2()
        {
            var result = ResponseResult<List<EntityDefEntity>>.Default();
            try
            {
                var list = FBMasterService.GetEntityDefList2();
                result = ResponseResult<List<EntityDefEntity>>.Success(list);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<EntityDefEntity>>.Error(
                    string.Format("读取{0}数据失败, 错误：{1}", "EntityDefList", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 保存表单定义记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<EntityDefEntity> SaveEntityDef(EntityDefEntity entity)
        {
            var result = ResponseResult<EntityDefEntity>.Default();
            try
            {
                IFBMasterService fbService = new FBMasterService();
                var returnEntity = fbService.SaveEntityDef(entity);

                result = ResponseResult<EntityDefEntity>.Success(returnEntity, string.Format("保存{0}数据成功！", "Form"));
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<EntityDefEntity>.Error(
                    string.Format("保存{0}数据失败, 错误：{1}", "Form", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 保存模板及HTML内容
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult SaveTemplateWithHTMLContent(EntityDefEntity entity)
        {
            var result = ResponseResult.Default();
            try
            {
                IFBMasterService fbService = new FBMasterService();
                fbService.SaveTemplateWithHTMLContent(entity);

                result = ResponseResult.Success(string.Format("保存{0}数据成功！", "表单模板"));
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("保存{0}数据失败, 错误：{1}", "表单模板", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 保存表单模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult SaveTemplateContent(EntityDefEntity entity)
        {
            var result = ResponseResult.Default();
            try
            {
                IFBMasterService fbService = new FBMasterService();
                fbService.SaveTemplateContent(entity);

                result = ResponseResult.Success(string.Format("保存{0}数据成功！", "表单模板"));
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("保存{0}数据失败, 错误：{1}", "表单模板", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult DeleteEntityDef(int id)
        {
            var result = ResponseResult.Default();

            try
            {
                IFBMasterService fbService = new FBMasterService();
                fbService.DeleteEntityDef(id);
                result = ResponseResult.Success();
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("删除表单定义数据失败，错误:{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion

        #region 实体属性操作
        /// <summary>
        /// 获取属性字段列表
        /// </summary>
        /// <param name="id">实体定义ID</param>
        /// <returns>字段列表</returns>
        [HttpGet]
        public ResponseResult<List<EntityAttributeEntity>> GetEntityAttributeList(int id) 
        {
            var result = ResponseResult<List<EntityAttributeEntity>>.Default();
            try
            {
                var list = FBMasterService.GetEntityAttributeList(id);
                result = ResponseResult<List<EntityAttributeEntity>>.Success(list);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<EntityAttributeEntity>>.Error(
                    string.Format("读取表单字段发生错误：{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 获取表单字段组合
        /// </summary>
        /// <param name="id">表单ID</param>
        /// <returns>视图对象</returns>
        [HttpGet]
        public ResponseResult<EntityAttributeListView> GetEntityAttributeComp(int id)
        {
            var result = ResponseResult<EntityAttributeListView>.Default();
            try
            {
                var entity = FBMasterService.GetEntityAttributeComp(id);
                result = ResponseResult<EntityAttributeListView>.Success(entity);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<EntityAttributeListView>.Error(
                    string.Format("读取表单字段数据发生错误：{0}", ex.Message)
                );
            }
            return result;
        }
        /// <summary>
        /// 保存实体属性定义
        /// </summary>
        /// <param name="entity">字段实体</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<EntityAttributeEntity> SaveAttribute(EntityAttributeEntity entity)
        {
            var result = ResponseResult<EntityAttributeEntity>.Default();
            try
            {
                var attrEntity = FBMasterService.SaveAttribute(entity);
                result = ResponseResult<EntityAttributeEntity>.Success(attrEntity);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<EntityAttributeEntity>.Error(
                    string.Format("保存表单字段失败， 错误:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 保存字段和模板内容，同时保存
        /// </summary>
        /// <param name="view">视图对象</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<EntityAttributeEntity> SaveAttributeWithTemplate(EntityAttributeView view)
        {
            var result = ResponseResult<EntityAttributeEntity>.Default();
            try
            {
                var attrEntity = FBMasterService.SaveAttributeWithTemplate(view);
                result = ResponseResult<EntityAttributeEntity>.Success(attrEntity);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<EntityAttributeEntity>.Error(
                    string.Format("保存字段和表单模板内容失败， 错误:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 删除属性记录，并且更新模板内容
        /// </summary>
        /// <param name="view">视图对象</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult DeleteAttributeWithTemplate(EntityAttributeListView view)
        {
            var result = ResponseResult.Default();
            try
            {
                var isOk = FBMasterService.DeleteAttributeWithTemplate(view);
                result = ResponseResult.Success();
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("保存字段和表单模板内容失败， 错误:{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion
    }
}
