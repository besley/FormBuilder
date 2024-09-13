using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SlickOne.WebUtility;
using Slickflow.Engine.Business.Entity;
using FormBuilder.Business.Entity;
using FormBuilder.Business.Service;
using FormBuilder.WebApi.Models;

namespace FormBuilder.WebApi.Controllers
{
    /// <summary>
    /// 动态表单数据WebApi服务
    /// </summary>
    public class FormController : Controller
    {
        #region 属性对象
        private IFormService _formService;
        public IFormService FormService
        {
            get
            {
                if (_formService == null) _formService = new FormService();
                return _formService;
            }
        }
        #endregion

        #region 动态表单定义记录读取
        /// <summary>
        /// 读取表单定义记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult<FormEntity> GetForm(int id)
        {
            var result = ResponseResult<FormEntity>.Default();
            try
            {
                var entity = FormService.GetForm(id);
                result = ResponseResult<FormEntity>.Success(entity);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<FormEntity>.Error(
                    string.Format("An error occurred when reading form list, detail:{0}", ex.Message)
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
        public ResponseResult UpgradeForm(int id)
        {
            var result = ResponseResult.Default();
            try
            {
                FormService.UpgradeForm(id);
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

        /// <summary>
        /// 删除表单记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult DeleteForm(int id)
        {
            var result = ResponseResult.Default();
            try
            {
                FormService.DeleteForm(id);
                result = ResponseResult.Success();
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("An error occurred when deleting form, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 读取表单记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult<List<FormEntity>> GetFormList2()
        {
            var result = ResponseResult<List<FormEntity>>.Default();
            try
            {
                var list = FormService.GetFormList2();
                result = ResponseResult<List<FormEntity>>.Success(list);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<FormEntity>>.Error(
                    string.Format("An error occurred when reading form list, detail:{0}", ex.Message)
                );
            }
            return result;
        }


        /// <summary>
        /// 创建表单
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<FormEntity> CreateForm([FromBody] FormEntity entity)
        {
            var result = ResponseResult<FormEntity>.Default();
            try
            {
                var item = FormService.SaveForm(entity);
                result = ResponseResult<FormEntity>.Success(item);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<FormEntity>.Error(
                    string.Format("An error occurred when saving form entity, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 保存表单模板
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult SaveTemplate([FromBody] FormEntity entity)
        {
            var result = ResponseResult.Default();
            try
            {
                FormService.SaveTemplateContent(entity);
                result = ResponseResult.Success();
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("An error occurred when saving form entity, detail:{0}", ex.Message)
                );
            }
            return result;
        }


        /// <summary>
        /// 读取表单记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult<List<FormProcessView>> GetFormProcessViewList()
        {
            var result = ResponseResult<List<FormProcessView>>.Default();
            try
            {
                var list = FormService.GetFormViewList();
                result = ResponseResult<List<FormProcessView>>.Success(list);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<FormProcessView>>.Error(
                    string.Format("An error occurred when reading form view list, detail:{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion
    }
}
