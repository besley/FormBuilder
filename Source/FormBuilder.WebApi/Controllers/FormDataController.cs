using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Slickflow.Data;
using SlickOne.WebUtility;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Common;
using Slickflow.Engine.Core.Result;
using Slickflow.Engine.Service;
using FormBuilder.Business.Entity;
using FormBuilder.Business.Service;
using FormBuilder.WebApi.Models;

namespace FormBuilder.WebApi.Controllers
{
    /// <summary>
    /// 表单数据控制器
    /// </summary>
    public class FormDataController : Controller
    {
        #region 属性对象
        private IFormDataService _fbDataService;
        public IFormDataService FBDataService
        {
            get
            {
                if (_fbDataService == null) _fbDataService = new FormDataService();
                return _fbDataService;
            }
        }
        #endregion

        #region 表单内容
        /// <summary>
        /// 读取表单内容记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult<FormDataEntity> GetFormData(int id)
        {
            var result = ResponseResult<FormDataEntity>.Default();
            try
            {
                var fbService = new FormDataService();
                var entity = fbService.GetFormData(id);
                result = ResponseResult<FormDataEntity>.Success(entity);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<FormDataEntity>.Error(
                    string.Format("An error occurred when reading form content, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 获取表单内容视图列表
        /// </summary>
        /// <param name="id">表单ID</param>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult<List<FormDataView>> GetFormDataViewList(int id)
        {
            var result = ResponseResult<List<FormDataView>>.Default();
            try
            {
                var fdService = new FormDataService();
                var list = fdService.GetFormDataViewList(id);
                result = ResponseResult<List<FormDataView>>.Success(list);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<FormDataView>>.Error(
                    string.Format("An error occurred when saving form content view list, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 获取表单数据视图列表，表单流程绑定列表
        /// </summary>
        /// <param name="id">表单ID</param>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult<FormDataProcessListView> GetFormDataProcessListView(int id)
        {
            var result = ResponseResult<FormDataProcessListView>.Default();
            try
            {
                var fdService = new FormDataService();
                var formDataList = fdService.GetFormDataViewList(id);
                var formService = new FormService();
                var formProcessList = formService.GetFormProcess(id);

                var viewEntity = new FormDataProcessListView();
                viewEntity.FormDataViewList = formDataList;
                viewEntity.FormProcessList = formProcessList;

                result = ResponseResult<FormDataProcessListView>.Success(viewEntity);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<FormDataProcessListView>.Error(
                    string.Format("An error occurred when getting form data process view list, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 保存表单内容实体
        /// </summary>
        /// <param name="entity">表单内容实体</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<FormDataEntity> SaveFormData([FromBody] FormDataEntity entity)
        {
            var result = ResponseResult<FormDataEntity>.Default();
            try
            {
                var fdService = new FormDataService();
                int entityID = fdService.SaveFormData(entity);
                var newItem = new FormDataEntity
                {
                    ID = entityID
                };
                result = ResponseResult<FormDataEntity>.Success(newItem);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<FormDataEntity>.Error(
                    string.Format("An error occurred when saving form content, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 删除表单实例
        /// </summary>
        /// <param name="id">表单实例ID</param>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult DeleteFormContent(int id)
        {
            var result = ResponseResult.Default();
            try
            {
                var fdService = new FormDataService();
                fdService.DeleteFormData(id);
                result = ResponseResult.Success();
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("An error occurred when deleting form content, detail:{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion
    }
}
