using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SlickOne.WebUtility;
using Slickflow.Engine.Common;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Core.Result;
using Slickflow.Engine.Service;
using FormBuilder.WebApi.Models;

namespace FormBuilder.WebApi.Controllers
{
    /// <summary>
    /// 流程控制器
    /// </summary>
    public class Wf2XmlController : Controller
    {
        #region 流程流转步骤获取
        /// <summary>
        /// 获取下一步步骤列表
        /// </summary>
        /// <param name="runner">运行者</param>
        /// <returns>步骤列表</returns>
        [HttpPost]
        public ResponseResult<NextStepInfo> GetNextStepInfo([FromBody] WfAppRunner runner)
        {
            var result = ResponseResult<NextStepInfo>.Default();
            try
            {
                var wfService = new WorkflowService();
                var nextStepInfo = wfService.GetNextStepInfo(runner);

                //从下一步预选步骤人员中追加用户到步骤列表中去
                var nextStepTree = nextStepInfo.NextActivityRoleUserTree.ToList();
                var nextActivityPerformers = nextStepInfo.NextActivityPerformers;

                if (nextActivityPerformers != null)
                {
                    //加载预选用户
                    nextStepTree = ProcessModelMimic.AppendPremilinaryUser(nextStepTree, nextActivityPerformers, true);
                }
                else
                {
                    //追加模拟用户
                    nextStepTree = ProcessModelMimic.AppendMimicUser(nextStepTree, runner).ToList();
                }
                result = ResponseResult<NextStepInfo>.Success(nextStepInfo);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<NextStepInfo>.Error(ex.Message);
            }
            return result;
        }
        #endregion

        #region 任务数据获取
        /// <summary>
        /// 获取待办任务
        /// </summary>
        /// <param name="query">查询实体</param>
        /// <returns>待办任务列表</returns>
        [HttpPost]
        public ResponseResult<List<TaskViewEntity>> GetReadyTaskList([FromBody] TaskQuery query)
        {
            var result = ResponseResult<List<TaskViewEntity>>.Default();
            try
            {
                var wfService = new WorkflowService();
                var taskList = wfService.GetReadyTasks(query).ToList();
                result = ResponseResult<List<TaskViewEntity>>.Success(taskList);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<TaskViewEntity>>.Error(
                    string.Format("An error occurred when reading task list, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 获取已办任务
        /// </summary>
        /// <param name="query">查询实体</param>
        /// <returns>已办任务列表</returns>
        [HttpPost]
        public ResponseResult<List<TaskViewEntity>> GetDoneTaskList([FromBody] TaskQuery query)
        {
            var result = ResponseResult<List<TaskViewEntity>>.Default();
            try
            {
                var wfService = new WorkflowService();
                var taskList = wfService.GetCompletedTasks(query).ToList() ;
                result = ResponseResult<List<TaskViewEntity>>.Success(taskList);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<TaskViewEntity>>.Error(
                    string.Format("An error occurred when reading task done list, detail:{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion

        #region 流程运行接口
        /// <summary>
        /// 流程流转接口
        /// </summary>
        /// <param name="runner">运行者</param>
        /// <returns>执行结果</returns>
        [HttpPost]
        public ResponseResult RunProcess([FromBody] WfAppRunner runner)
        {
            var result = ResponseResult.Default();
            try
            {
                var wfService = new WorkflowService();
                var wfResult = wfService.RunProcess(runner);
                if (wfResult.Status == WfExecutedStatus.Success)
                    result = ResponseResult.Success();
                else
                    result = ResponseResult.Error(wfResult.Message);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("An error occurred when running process, detail:{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion
    }
}
