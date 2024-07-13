using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SlickOne.WebUtility;
using FormMaster.Builder.Entity;
using FormMaster.Builder.Service;
using Slickflow.Engine.Xpdl.Entity;
using FormMaster.Designer.Models;

namespace FormMaster.Designer.Controllers.WebApi
{
    /// <summary>
    /// 表单绑定流程控制器
    /// </summary>
    public class EAVProcessController : Controller
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
        #endregion

        /// <summary>
        /// 获取流程记录列表
        /// </summary>
        /// 
        /// <returns>表单实体绑定流程视图</returns>
        [HttpGet]
        public ResponseResult<EntityProcessListView> GetEntityProcessView(int id)
        {
            var result = ResponseResult<EntityProcessListView>.Default();
            try
            {
                var view = new EntityProcessListView();
                var wfAppService = new WfAppService();
                var processList = wfAppService.GetProcessListSimple().ToList();
                view.ProcessList = processList;

                var entityProcess = FBMasterService.GetEntityProcess(id);
                view.EntityProcess = entityProcess;
                
                result = ResponseResult<EntityProcessListView>.Success(view);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<EntityProcessListView>.Error(
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
        public ResponseResult<List<Activity>> GetActivityList(int id)
        {
            var result = ResponseResult<List<Activity>>.Default();
            try
            {
                var wfAppService = new WfAppService();
                var activityList = wfAppService.GetTaskActivityList(id).ToList();

                result = ResponseResult<List<Activity>>.Success(activityList);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<Activity>>.Error(
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
        public ResponseResult BindEntityProcess([FromBody] EntityDefProcessView view)
        {
            var result = ResponseResult.Default();
            try
            {
                FBMasterService.BindEntityProcess(view);
                result = ResponseResult.Success();
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
        public ResponseResult UnbindEntityProcess([FromBody] EntityDefProcessView view)
        {
            var result = ResponseResult.Default();
            try
            {
                FBMasterService.UnbindEntityProcess(view);
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
    }
}
