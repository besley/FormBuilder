using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SlickOne.WebUtility;
using FormBuilder.Business.Entity;
using FormBuilder.Business.Service;
using Slickflow.Engine.Xpdl.Entity;
using Slickflow.Engine.Service;
using FormBuilder.WebApi.Models;

namespace FormBuilder.WebApi.Controllers
{
    /// <summary>
    /// 表单绑定流程控制器
    /// </summary>
    public class FormProcessController : Controller
    {
        #region 属性对象
        private IFormService _formService = null;
        public IFormService FormService
        {
            get
            {
                if (_formService == null) _formService = new FormService();
                return _formService;
            }
        }
        #endregion

        #region 获取表单绑定流程信息
        /// <summary>
        /// 获取表单绑定流程信息
        /// </summary>
        /// <param name="id">表单ID</param>
        /// <returns></returns>
        public ResponseResult<List<FormProcessEntity>> GetFormProcess(int id)
        {
            var result = ResponseResult<List<FormProcessEntity>>.Default();
            try
            {
                var fdService = new FormService();
                var list = fdService.GetFormProcess(id);

                result = ResponseResult<List<FormProcessEntity>>.Success(list);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<FormProcessEntity>>.Error(
                    string.Format("An error occurred when reading form binding process info, detail:{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion

        #region 表单绑定流程
        /// <summary>
        /// 获取流程记录列表
        /// </summary>
        /// 
        /// <returns>表单实体绑定流程视图</returns>
        [HttpGet]
        public ResponseResult<FormProcessListView> GetFormProcessView(int id)
        {
            var result = ResponseResult<FormProcessListView>.Default();
            try
            {
                var view = new FormProcessListView();
                var wfService = new WorkflowService();
                var processList = wfService.GetProcessListSimple().ToList();
                view.ProcessList = processList;

                var entityProcess = FormService.GetFormProcess(id);
                view.FormProcessList = entityProcess;
                
                result = ResponseResult<FormProcessListView>.Success(view);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<FormProcessListView>.Error(
                    string.Format("An error occurred when reading form binding process info, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 获取流程下的任务节点列表
        /// </summary>
        /// <param name="id">流程id</param>
        /// <returns>任务节点列表</returns>
        [HttpGet]
        public ResponseResult<List<ActivityEntity>> GetActivityList(int id)
        {
            var result = ResponseResult<List<ActivityEntity>>.Default();
            try
            {
                var wfService = new WorkflowService();
                var activityList = wfService.GetTaskActivityList(id).ToList();

                result = ResponseResult<List<ActivityEntity>>.Success(activityList);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<ActivityEntity>>.Error(
                    string.Format("An error occurred when reading activity list of the process, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 表单绑定流程信息
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult BindFormProcess([FromBody] FormProcessView view)
        {
            var result = ResponseResult.Default();
            try
            {
                var formProcessList = FormService.GetFormProcess(view.FormID);
                var process = formProcessList.Find(a => a.ProcessID == view.ProcessID);
                if (process == null) 
                {
                    FormService.BindFormProcess(view);
                    result = ResponseResult.Success();
                }
                else
                {
                    result = ResponseResult.Error("The process has been binded with this form!");
                }
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("An error occurred when binding form with process, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 表单解除绑定流程信息
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult UnbindFormProcess([FromBody] FormProcessView view)
        {
            var result = ResponseResult.Default();
            try
            {
                FormService.UnbindFormProcess(view);
                result = ResponseResult.Success();
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("An error occurred when unbinding form with process, detail:{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion

        #region 字段权限读取
        /// <summary>
        /// 读取表单字段节点权限
        /// </summary>
        /// <param name="query">查询实体</param>
        /// <returns>字段权限列表</returns>
        [HttpPost]
        public ResponseResult<List<FormFieldActivityEditEntity>> QueryFormFieldActivityEditList([FromBody] FormFieldActivityQuery query)
        {
            var result = ResponseResult<List<FormFieldActivityEditEntity>>.Default();
            try
            {
                var fbService = new FormService();
                var fieldList = fbService.GetFormFieldList(query.FormID);

                var fieldEditList = fbService.GetFormFieldActivityEditList(query.FormID,
                    query.ProcessID,
                    query.ActivityGUID);

                foreach (var field in fieldList)
                {
                    var fe = fieldEditList.Find(s => s.FieldID == field.ID);
                    if (fe == null)
                    {
                        fieldEditList.Add(new FormFieldActivityEditEntity
                        {
                            FormID = query.FormID,
                            ProcessID = query.ProcessID,
                            ActivityGUID = query.ActivityGUID,
                            FieldID = field.ID,
                            FieldGUID = field.FieldGUID,
                            FieldName = field.FieldName
                        });
                    }
                }
                result = ResponseResult<List<FormFieldActivityEditEntity>>.Success(fieldEditList);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<FormFieldActivityEditEntity>>.Error(
                    string.Format("An error occurred when reading form activity permission, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 根据任务ID读取节点字段权限数据
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<List<FormFieldActivityEditEntity>> QueryFormFieldActivityEditListByTask([FromBody] FormFieldActivityQuery query)
        {
            var result = ResponseResult<List<FormFieldActivityEditEntity>>.Default();
            try
            {
                var wfService = new WorkflowService();
                var taskView = wfService.GetTaskView(query.TaskID);

                //读取表单字段权限
                var fbService = new FormService();
                var fieldEditList = fbService.GetFormFieldActivityEditList(query.FormID, taskView.ProcessGUID, taskView.Version, taskView.ActivityGUID);

                //表单字段列表
                var fieldList = fbService.GetFormFieldList(query.FormID);

                foreach (var field in fieldList)
                {
                    var fe = fieldEditList.Find(s => s.FieldID == field.ID);
                    if (fe == null)
                    {
                        fieldEditList.Add(new FormFieldActivityEditEntity
                        {
                            FormID = query.FormID,
                            ProcessID = query.ProcessID,
                            ActivityGUID = taskView.ActivityGUID,
                            FieldID = field.ID,
                            FieldGUID = field.FieldGUID,
                            FieldName = field.FieldName
                        });
                    }
                }
                result = ResponseResult<List<FormFieldActivityEditEntity>>.Success(fieldEditList);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<FormFieldActivityEditEntity>>.Error(
                    string.Format("An error occurred when reading form activity permission, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 保存表单字段节点权限
        /// </summary>
        /// <param name="comp">权限实体对象</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult SaveFormFieldActivityEditList([FromBody] FormFieldActivityEditListComp comp)
        {
            var result = ResponseResult.Default();
            try
            {
                var fbService = new FormService();
                fbService.SaveFormFieldActivityEditList(comp);

                result = ResponseResult.Success();
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("An error occurred when saving form activity permission, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 删除节点字段权限
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ResponseResult DeleteFormFieldActivityEdit([FromBody] FormFieldActivityQuery query)
        {
            var result = ResponseResult.Default();
            try
            {
                var fbService = new FormService();
                var entity = fbService.DeleteFormFieldActivityEdit(query.FormID, query.ProcessID, query.ActivityGUID);
                result = ResponseResult.Success(entity);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("An error occurred when deleting form activity permission, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 删除流程下所有权限数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ResponseResult ClearFormFieldactivityEdit([FromBody] FormFieldActivityQuery query)
        {
            var result = ResponseResult.Default();
            try
            {
                var fbService = new FormService();
                var entity = fbService.ClearFormFieldActivityEdit(query.FormID, query.ProcessID);
                result = ResponseResult.Success(entity);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("An error occurred when clearing form activity permission, detail:{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion 
    }
}
