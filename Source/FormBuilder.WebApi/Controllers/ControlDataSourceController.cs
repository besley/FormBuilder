using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SlickOne.WebUtility;
using FormBuilder.Business.Entity;
using FormBuilder.Business.Service;
using FormBuilder.WebApi.Models;

namespace FormBuilder.WebApi.Controllers
{
    /// <summary>
    /// DataSource 数据WebApi服务
    /// </summary>
    public class ControlDataSourceController : Controller
    {
        #region 属性对象
        private IFormService _formDefineService;
        public IFormService FormDefineService
        {
            get
            {
                if (_formDefineService == null) _formDefineService = new FormService();
                return _formDefineService;
            }
        }

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

        #region 字段绑定数据源操作
        /// <summary>
        /// 加载控件数据源操作
        /// </summary>
        /// <param name="entity">控件字段实体对象</param>
        /// <returns>数据源集合</returns>
        [HttpPost]
        public ResponseResult<List<dynamic>> LoadControlDataSource([FromBody] FormFieldEntity entity)
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
        public ResponseResult<List<FormFieldEntity>> QueryCascadeChildControlList([FromBody] CascadeParentControlInfo entity)
        {
            var result = ResponseResult<List<FormFieldEntity>>.Default();
            try
            {
                var items = FBDataService.QueryCascadeChildControlList(entity);
                result = ResponseResult<List<FormFieldEntity>>.Success(items, "Quering cascade child control list successfully!");
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<FormFieldEntity>>.Error(
                    string.Format("An error occurred when quering cascade child control list, detail:{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion
    }
}
