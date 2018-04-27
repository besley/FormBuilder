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
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Service;
using SlickMaster.Designer.Models;

namespace SlickMaster.Designer.Controllers.WebApi
{
    /// <summary>
    /// 表单绑定流程控制器
    /// </summary>
    public class EAVProcessController : BaseWebApiController
    {
        /// <summary>
        /// 获取流程记录列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult<EntityProcessView> GetEntityProcessView(int id)
        {
            var result = ResponseResult<EntityProcessView>.Default();
            try
            {
                var view = new EntityProcessView();
                var wfService = new WorkflowService();
                var processList = wfService.GetProcessListSimple().ToList();
                view.ProcessList = processList;

                var entityProcess = FBMasterService.GetEntityProcess(id);
                view.EntityProcess = entityProcess;
                
                result = ResponseResult<EntityProcessView>.Success(view);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<EntityProcessView>.Error(
                    string.Format("获取表单绑定流程数据失败！{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 表单绑定流程信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult SaveEntityProcess(EntityProcessEntity entity)
        {
            var result = ResponseResult.Default();
            try
            {
                FBMasterService.SaveEntityProcess(entity);
                result = ResponseResult.Success();
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("表单绑定流程数据失败！{0}", ex.Message)
                );
            }
            return result;
        }
    }
}
