using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SlickOne.WebUtility;
using Slickflow.Engine.Common;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Core.Result;
using FormMaster.Designer.Models;

namespace FormMaster.Designer.Controllers.WebApi
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
        public ResponseResult<List<NodeView>> GetNextStepInfo([FromBody] WfAppRunner runner)
        {
            var result = ResponseResult<List<NodeView>>.Default();
            try
            {
                var wfAppService = new WfAppService();
                var nextStepInfo = wfAppService.GetNextStepInfo(runner);

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
                result = ResponseResult<List<NodeView>>.Success(nextStepTree);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<NodeView>>.Error(ex.Message);
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
                var wfAppService = new WfAppService();
                var taskList = wfAppService.GetReadyTaskList(query);
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
        public ResponseResult<List<TaskViewEntity>> GetDoneList([FromBody] TaskQuery query)
        {
            var result = ResponseResult<List<TaskViewEntity>>.Default();
            try
            {
                var wfAppService = new WfAppService();
                var taskList = wfAppService.GetDoneTaskList(query);
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
                var wfAppService = new WfAppService();
                var wfResult = wfAppService.RunProcess(runner);
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
