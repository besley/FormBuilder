using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SlickOne.WebUtility;
using FormMaster.Builder.Entity;
using FormMaster.Builder.Service;
using FormMaster.Designer.Models;

namespace FormMaster.Designer.Controllers.WebApi
{
    /// <summary>
    /// EAV数据WebApi服务
    /// </summary>
    public class EAVDataController : Controller
    {
        #region 属性对象
        private IFBMasterService _fbMasterService;
        public IFBMasterService FBMasterService
        {
            get
            {
                if (_fbMasterService == null) _fbMasterService = new FBMasterService();
                return _fbMasterService;
            }
        }

        private IFBDataService _fbDataService;
        public IFBDataService FBDataService
        {
            get
            {
                if (_fbDataService == null) _fbDataService = new FBDataService();
                return _fbDataService;
            }
        }
        #endregion

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
                var entity = FBMasterService.GetEntityDef(id);
                result = ResponseResult<EntityDefEntity>.Success(entity);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<EntityDefEntity>.Error(
                    string.Format("An error occurred when reading form list, detail:{0}", ex.Message)
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
                    string.Format("An error occurred when reading form list, detail:{0}", ex.Message)
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
        public ResponseResult<EntityDefEntity> SaveEntityDef([FromBody] EntityDefEntity entity)
        {
            var result = ResponseResult<EntityDefEntity>.Default();
            try
            {
                var returnEntity = FBMasterService.SaveEntityDef(entity);
                result = ResponseResult<EntityDefEntity>.Success(returnEntity, "Saved form data successfully!");
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<EntityDefEntity>.Error(
                    string.Format("An error occurred when saving form definition, detail:{0}", ex.Message)
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
        public ResponseResult SaveTemplateWithHTMLContent([FromBody] EntityDefEntity entity)
        {
            var result = ResponseResult.Default();
            try
            {
                FBMasterService.SaveTemplateWithHTMLContent(entity);
                result = ResponseResult.Success("Saved form template HTML content successfully!");
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("An error occurred when saving form template HTML content, detail:{0}", ex.Message)
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
        public ResponseResult SaveTemplateContent([FromBody] EntityDefEntity entity)
        {
            var result = ResponseResult.Default();
            try
            {
                FBMasterService.SaveTemplateContent(entity);
                result = ResponseResult.Success("Saved form template content successfully!");
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("An error occurred when reading form template content, detail:{0}", ex.Message)
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
                FBMasterService.DeleteEntityDef(id);
                result = ResponseResult.Success();
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("An error occurred when deleting form record, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 实体版本升级
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult UpgradeEntityDef(int id)
        {
            var result = ResponseResult.Default();

            try
            {
                FBMasterService.UpgradeEntityDef(id);
                result = ResponseResult.Success();
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("An error occurred when upgrade form record, detail:{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion

        #region 实体属性操作
        /// <summary>
        /// 读取字段属性
        /// </summary>
        /// <param name="id">字段ID</param>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult<EntityAttributeEntity> GetAttributeEntity(int id)
        {
            var result = ResponseResult<EntityAttributeEntity>.Default();
            try
            {
                var entity = FBMasterService.GetEntityAttribute(id);
                result = ResponseResult<EntityAttributeEntity>.Success(entity);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<EntityAttributeEntity>.Error(
                    string.Format("An error occurred when reading attribute, detail:{0}", ex.Message)
                );
            }
            return result;
        }

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
                    string.Format("An error occurred when reading attribute list, detail:{0}", ex.Message)
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
                    string.Format("An error occurred when reading attribute, detail:{0}", ex.Message)
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
        public ResponseResult<EntityAttributeEntity> SaveAttribute([FromBody] EntityAttributeEntity entity)
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
                    string.Format("An error occurred when saving attribute, detail:{0}", ex.Message)
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
        public ResponseResult<EntityAttributeEntity> SaveAttributeWithTemplate([FromBody] EntityAttributeView view)
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
                    string.Format("An error occurred when saving attribute and template, detail:{0}", ex.Message)
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
        public ResponseResult DeleteAttributeWithTemplate([FromBody] EntityAttributeListView view)
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
                    string.Format("An error occurred when deleting attribute and updating template, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 获取流程记录列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult<AttrEntityView> GetAttributeEntityView(int id)
        {
            var result = ResponseResult<AttrEntityView>.Default();
            try
            {
                var view = new AttrEntityView();
                var attrEntity = FBMasterService.GetEntityAttribute(id);
                view.AttributeEntity = attrEntity;

                var list = FBMasterService.GetEntityDefList2();
                view.EntityDefList = list;

                result = ResponseResult<AttrEntityView>.Success(view);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<AttrEntityView>.Error(
                    string.Format("An error occurred when getting attribute entity view, detail:{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion

        #region 字段绑定操作
        /// <summary>
        /// 加载控件数据源操作
        /// </summary>
        /// <param name="entity">控件字段实体对象</param>
        /// <returns>数据源集合</returns>
        [HttpPost]
        public ResponseResult<List<dynamic>> LoadControlDataSource([FromBody] EntityAttributeEntity entity)
        {
            var result = ResponseResult<List<dynamic>>.Default();
            try
            {
                var items = FBDataService.LoadControlDataSource(entity);
                result = ResponseResult<List<dynamic>>.Success(items, "Loading control data source successfully!");
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<dynamic>>.Error(
                    string.Format("An error occurred when loading control data source, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 加载级联控件数据源操作
        /// </summary>
        /// <param name="entity">控件字段实体对象</param>
        /// <returns>数据源集合</returns>
        [HttpPost]
        public ResponseResult<List<dynamic>> LoadCascadeControlDataSource([FromBody] CascadeControlInfo entity)
        {
            var result = ResponseResult<List<dynamic>>.Default();
            try
            {
                var items = FBDataService.LoadCascadeControlDataSource(entity);
                result = ResponseResult<List<dynamic>>.Success(items, "Loading cascade control data source successfully!");
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<dynamic>>.Error(
                    string.Format("An error occurred when loading cascade control data source, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 查询级联属性字段列表
        /// </summary>
        /// <param name="entity">查询实体</param>
        /// <returns>属性列表</returns>
        [HttpPost]
        public ResponseResult<List<EntityAttributeEntity>> QueryCascadeChildControlList([FromBody] CascadeParentControlInfo entity)
        {
            var result = ResponseResult<List<EntityAttributeEntity>>.Default();
            try
            {
                var items = FBDataService.QueryCascadeChildControlList(entity);
                result = ResponseResult<List<EntityAttributeEntity>>.Success(items, "Quering cascade child control list successfully!");
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<EntityAttributeEntity>>.Error(
                    string.Format("An error occurred when quering cascade child control list, detail:{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion
    }
}
