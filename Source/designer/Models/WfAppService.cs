using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlickOne.WebUtility;
using Slickflow.Engine.Common;
using Slickflow.Engine.Core.Result;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Xpdl.Entity;
using FormMaster.Designer.Configuration;

namespace FormMaster.Designer.Models
{
    /// <summary>
    /// 流程服务交互接口
    /// </summary>
    public class WfAppService
    {
        #region 流程步骤查询接口
        /// <summary>
        /// 获取流程下一步列表
        /// </summary>
        /// <param name="runner">运行者</param>
        /// <returns>下一步信息</returns>
        public NextStepInfo GetNextStepInfo(WfAppRunner runner)
        {
            try
            {
                var url = string.Format("{0}/WfService/GetNextStepInfo", WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Post<WfAppRunner, ResponseResult<NextStepInfo>>(runner);
                if (response.Status == -1)
                {
                    throw new ApplicationException(string.Format("An error occurred when getting next step, detail:{0}", response.Message));
                }
                return response.Entity;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region 流程执行接口
        /// <summary>
        /// 启动流程
        /// </summary>
        /// <param name="runner">运行者</param>
        public WfExecutedResult StartProcess(WfAppRunner runner)
        {
            try
            {
                var url = string.Format("{0}/WfService/StartProcess", WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Post<WfAppRunner, ResponseResult<WfExecutedResult>>(runner);
                if (response.Status == -1)
                {
                    throw new ApplicationException(string.Format("An error occurred when starting up process, detail:{0}", response.Message));
                }
                return response.Entity;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 运行流程
        /// </summary>
        /// <param name="runner">运行者</param>
        public WfExecutedResult RunProcess(WfAppRunner runner)
        {
            try
            {
                var url = string.Format("{0}/WfService/RunProcess", WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Post<WfAppRunner, ResponseResult<WfExecutedResult>>(runner);
                if (response.Status == -1)
                {
                    throw new ApplicationException(string.Format("An error occurred when running process, detail:{0}", response.Message));
                }
                return response.Entity;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 退回流程
        /// </summary>
        /// <param name="runner">运行者</param>
        public WfExecutedResult SendBackProcess(WfAppRunner runner)
        {
            try
            {
                var url = string.Format("{0}/WfService/SendBackProcess", WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Post<WfAppRunner, ResponseResult<WfExecutedResult>>(runner);
                if (response.Status == -1)
                {
                    throw new ApplicationException(string.Format("An error occurred when backwarding process, detail:{0}", response.Message));
                }
                return response.Entity;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 撤销流程
        /// </summary>
        /// <param name="runner">运行者</param>
        public WfExecutedResult WithdrawProcess(WfAppRunner runner)
        {
            try
            {
                var url = string.Format("{0}/WfService/WithdrawProcess", WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Post<WfAppRunner, ResponseResult<WfExecutedResult>>(runner);
                if (response.Status == -1)
                {
                    throw new ApplicationException(string.Format("An error occurred when withdrawing process, detail:{0}", response.Message));
                }
                return response.Entity;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 驳回流程
        /// </summary>
        /// <param name="runner">运行者</param>
        public WfExecutedResult RejectProcess(WfAppRunner runner)
        {
            try
            {
                var url = string.Format("{0}/WfService/RejectProcess", WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Post<WfAppRunner, ResponseResult<WfExecutedResult>>(runner);
                if (response.Status == -1)
                {
                    throw new ApplicationException(string.Format("An error occurred when rejecting process, detail:{0}", response.Message));
                }
                return response.Entity;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 加签流程
        /// </summary>
        /// <param name="runner">运行者</param>
        public WfExecutedResult SignForwardProcess(WfAppRunner runner)
        {
            try
            {
                var url = string.Format("{0}/WfService/SignForwardProcess", WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Post<WfAppRunner, ResponseResult<WfExecutedResult>>(runner);
                if (response.Status == -1)
                {
                    throw new ApplicationException(string.Format("An error occurred when signing forward process, detail:{0}", response.Message));
                }
                return response.Entity;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 关闭流程
        /// </summary>
        /// <param name="runner">运行者</param>
        public WfExecutedResult CloseProcess(WfAppRunner runner)
        {
            try
            {
                var url = string.Format("{0}/WfService/CloseProcess", WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Post<WfAppRunner, ResponseResult<WfExecutedResult>>(runner);
                if (response.Status == -1)
                {
                    throw new ApplicationException(string.Format("An error occurred when closing process, detail:{0}", response.Message));
                }
                return response.Entity;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 修订流程
        /// </summary>
        /// <param name="runner">运行者</param>
        public WfExecutedResult ReviseProcess(WfAppRunner runner)
        {
            try
            {
                var url = string.Format("{0}/WfService/ReviseProcess", WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Post<WfAppRunner, ResponseResult<WfExecutedResult>>(runner);
                if (response.Status == -1)
                {
                    throw new ApplicationException(string.Format("An error occurred when revising process, detail:{0}", response.Message));
                }
                return response.Entity;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 跳转流程
        /// </summary>
        /// <param name="runner">运行者</param>
        public WfExecutedResult JumpProcess(WfAppRunner runner)
        {
            try
            {
                var url = string.Format("{0}/WfService/JumpProcess", WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Post<WfAppRunner, ResponseResult<WfExecutedResult>>(runner);
                if (response.Status == -1)
                {
                    throw new ApplicationException(string.Format("An error occurred when jumping process, detail:{0}", response.Message));
                }
                return response.Entity;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// 返签流程
        /// </summary>
        /// <param name="runner">运行者</param>
        public WfExecutedResult ReverseProcess(WfAppRunner runner)
        {
            try
            {
                var url = string.Format("{0}/WfService/ReverseProcess", WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Post<WfAppRunner, ResponseResult<WfExecutedResult>>(runner);
                if (response.Status == -1)
                {
                    throw new ApplicationException(string.Format("An error occurred when reversing process, detail:{0}", response.Message));
                }
                return response.Entity;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region 流程定义查询接口
        /// <summary>
        /// 获取流程定义实体
        /// </summary>
        /// <param name="runner">查询实体</param>
        /// <returns>流程定义实体</returns>
        public ProcessEntity GetProcessByVerseion(ProcessQuery query)
        {
            try
            {
                var url = string.Format("{0}/WfService/GetProcessByVersion", WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Post<ProcessQuery, ResponseResult<ProcessEntity>>(query);
                var entity = response.Entity;

                return entity;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取流程定义列表
        /// </summary>
        /// <returns>流程记录列表</returns>
        public IList<ProcessEntity> GetProcessListSimple()
        {
            try
            {
                var url = string.Format("{0}/WfService/GetProcessListSimple", WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Get<ResponseResult<List<ProcessEntity>>>();
                var list = response.Entity;

                return list;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取任务节点列表
        /// </summary>
        /// <param name="processID">流程ID</param>
        /// <returns>节点列表</returns>
        public IList<Activity> GetTaskActivityList(int processID)
        {
            try
            {
                var url = string.Format("{0}/WfService/GetTaskActivityList/" + processID.ToString(), WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Get<ResponseResult<List<Activity>>>();
                var list = response.Entity;
                return list;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取任务视图
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <returns>任务视图</returns>
        public TaskViewEntity GetTaskView(int taskID)
        {
            try
            {
                var url = string.Format("{0}/WfService/GetTaskView/" + taskID.ToString(), WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Get<ResponseResult<TaskViewEntity>>();
                var entity = response.Entity;
                return entity;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 导入流程
        /// </summary>
        /// <param name="entity">流程实体</param>
        public void ImportProcess(ProcessEntity entity)
        {
            try
            {
                var url = string.Format("{0}/WfService/ImportProcess", WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Post<ProcessEntity, ResponseResult>(entity);
                if (response.Status == -1)
                {
                    throw new ApplicationException(string.Format("An error occurred when importing process xml, detail:{0}", response.Message));
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region 任务数据查询接口
        /// <summary>
        /// 待办任务查询
        /// </summary>
        /// <param name="query">查询实体</param>
        /// <returns>任务列表</returns>
        public List<TaskViewEntity> GetReadyTaskList(TaskQuery query)
        {
            try
            {
                var url = string.Format("{0}/WfService/GetReadyTaskList", WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Post<TaskQuery, ResponseResult<List<TaskViewEntity>>>(query);
                if (response.Status == -1)
                {
                    throw new ApplicationException(string.Format("An error occurred when getting task list, detail:{0}", response.Message));
                }
                return response.Entity;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 待办任务查询
        /// </summary>
        /// <param name="query">查询实体</param>
        /// <returns>任务列表</returns>
        public List<TaskViewEntity> GetDoneTaskList(TaskQuery query)
        {
            try
            {
                var url = string.Format("{0}/WfService/GetDoneTaskList", WebConfiguration.SfWebAPIHostUrl);
                var clientHelper = HttpClientHelper.CreateHelper(url);
                var response = clientHelper.Post<TaskQuery, ResponseResult<List<TaskViewEntity>>>(query);
                if (response.Status == -1)
                {
                    throw new ApplicationException(string.Format("An error occurred when getting task list, detail:{0}", response.Message));
                }
                return response.Entity;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
