using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SlickOne.WebUtility;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Common;
using Slickflow.Engine.Core.Result;
using FormMaster.Builder.Entity;
using FormMaster.Builder.Service;
using FormMaster.Designer.Models;

namespace FormMaster.Designer.Controllers.WebApi
{
    /// <summary>
    /// 表单数据控制器
    /// </summary>
    public class FBDataController : Controller
    {
        #region 属性对象
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

        #region 实体扩展属性取值操作
        /// <summary>
        /// 获取最近有更新的表单记录
        /// </summary>
        /// <returns>实体列表</returns>
        [HttpGet]
        public ResponseResult<List<EntityInfoView>> GetEntityInfoTop10()
        {
            var result = ResponseResult<List<EntityInfoView>>.Default();
            try
            {
                var list = FBDataService.GetEntityInfoTop10();
                result = ResponseResult<List<EntityInfoView>>.Success(list);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<EntityInfoView>>.Error(
                    string.Format("An error occurred when reading form data list, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 获取最近有更新的表单记录
        /// </summary>
        /// <param name="query">查询实体</param>
        /// <returns>实体列表</returns>
        [HttpPost]
        public ResponseResult<List<EntityInfoView>> QueryEntityInfoSimpleList([FromBody] EntityInfoQuery query)
        {
            var result = ResponseResult<List<EntityInfoView>>.Default();
            try
            {
                var list = FBDataService.GetEntityInfoSimpleList(query);
                result = ResponseResult<List<EntityInfoView>>.Success(list);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<EntityInfoView>>.Error(
                    string.Format("An error occurred when reading form data list, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 获取表单字段数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<List<EntityAttrValueItem>> QueryEntityAttrValue([FromBody] EntityInfoEntity entity)
        {
            var result = ResponseResult<List<EntityAttrValueItem>>.Default();
            try
            {
                var list = FBDataService.QueryEntityAttrValue(entity.ID);
                result = ResponseResult<List<EntityAttrValueItem>>.Success(list);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<EntityAttrValueItem>>.Error(
                    string.Format("An error occurred when reading form attribute value, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 表单单条数据获取方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult<dynamic> GetEntityInfoWithAttrValue(int id)
        {
            var result = ResponseResult<dynamic>.Default();
            try
            {
                var entity = FBDataService.GetEntityInfoWithAttrValue(id);
                result = ResponseResult<dynamic>.Success(entity);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<dynamic>.Error(
                    string.Format("An error occurred when reading single form attribute value, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 表单单条数据获取方法
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<List<dynamic>> GetEntityInfoWithAttrValueList([FromBody] EntityInfoQuery query)
        {
            var result = ResponseResult<List<dynamic>>.Default();
            try
            {
                var entity = FBDataService.GetEntityInfoWithAttrValueList(query);
                result = ResponseResult<List<dynamic>>.Success(entity);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<dynamic>>.Error(
                    string.Format("An error occurred when reading form info with attribute value, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 表单基本数据和扩展属性分页获取
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<List<dynamic>> GetWithAttrValueList([FromBody] EntityInfoAttrQuery query)
        {
            var result = ResponseResult<List<dynamic>>.Default();
            try
            {
                var count = 0;
                var list = FBDataService.GetEntityInfoWithAttrValueListPaged(query, out count);
                if (list != null && list.Count == 0)
                    result = ResponseResult<List<dynamic>>.Success(null);
                else
                    result = ResponseResult<List<dynamic>>.Success(list);
                result.TotalRowsCount = count;
                result.TotalPages = (count + query.PageSize - 1) / query.PageSize;
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<dynamic>>.Error(
                    string.Format("An error occurred when reading attribute list paged, detail:{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion

        #region 流程数据交互
        /// <summary>
        /// 保存实体及其扩展属性
        /// </summary>
        /// <param name="eav"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult UpdateEntityRow([FromBody] EntityInfoWithAttrValueListItem eav)
        {
            var result = ResponseResult.Default();
            try
            {
                var entityInfo = eav.EntityInfo;
                if (entityInfo.ID == 0)
                {
                    var newID = FBDataService.InsertRow(eav);
                    result = ResponseResult.Success(newID);
                }
                else if (entityInfo.ID > 0)
                {
                    FBDataService.UpdateRow(eav);
                    result = ResponseResult.Success();
                }
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("An error occurred when saving form data with attribute value, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 运行流程
        /// </summary>
        /// <param name="eav"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<EntityInfoEntity> InsertEntityRowFlow([FromBody] dynamic eav)
        {
            var result = ResponseResult<EntityInfoEntity>.Default();
            try
            {
                //此处代码仅作为简单DEMO示例，请不要复制到生产环境，完整事务处理方案请使用其它技术方案替代。
                //20190819
                //Besley
                dynamic clientEntity = JsonConvert.DeserializeObject<dynamic>(eav.ToString());

                //先保存表单
                EntityInfoWithAttrValueListItem entityAttrValueItem = JsonConvert.DeserializeObject<EntityInfoWithAttrValueListItem>(
                    clientEntity.EntityInfoWithAttrValueListItem.ToString());
                int newEntityInfoID = FBDataService.InsertRow(entityAttrValueItem);

                //启动流程
                var wfAppRunner = JsonConvert.DeserializeObject<WfAppRunner>(clientEntity.WfAppRunner.ToString());
                wfAppRunner.AppInstanceID = newEntityInfoID.ToString();
                var wfAppService = new WfAppService();
                var wfResult = wfAppService.StartProcess(wfAppRunner);

                //更新表单流程数据
                var entityInfo = FBDataService.GetEntityInfo(newEntityInfoID);
                entityInfo.ProcessGUID = wfAppRunner.ProcessGUID;
                entityInfo.Version = wfAppRunner.Version;
                entityInfo.ProcessInstanceID = wfResult.ProcessInstanceIDStarted;
                entityInfo.CreatedUserID = wfAppRunner.UserID;
                entityInfo.CreatedUserName = wfAppRunner.UserName;
                entityInfo.CreatedDatetime = System.DateTime.Now;
                FBDataService.SaveEntityInfo(entityInfo);

                //返回结果
                if (wfResult.Status == WfExecutedStatus.Success)
                    result = ResponseResult<EntityInfoEntity>.Success(entityInfo, "Form data saved and process started successfully!");
                else
                    result = ResponseResult<EntityInfoEntity>.Error(wfResult.Message);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<EntityInfoEntity>.Error(
                    string.Format("An error occurred when saving form data with attribute value, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 查询节点字段权限
        /// </summary>
        /// <param name="runner">运行者</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<List<EntityAttrActivityEditEntity>> QueryAttrActivityPermission([FromBody] WfAppRunner runner)
        {
            var result = ResponseResult<List<EntityAttrActivityEditEntity>>.Default();
            try
            {
                //获取表单实例数据
                var fbDataService = new FBDataService();
                var entityInfo = fbDataService.GetEntityInfo(int.Parse(runner.AppInstanceID));

                var query = new ProcessQuery
                {
                    ProcessGUID = entityInfo.ProcessGUID,
                    Version = entityInfo.Version
                };

                //获取流程定义实体
                var wfAppService = new WfAppService();
                var processEntity = wfAppService.GetProcessByVerseion(query);

                //获取当前运行节点实例数据
                var currentActivityGUID = string.Empty;
                var taskView = wfAppService.GetTaskView(runner.TaskID.Value);

                //获取字段权限数据
                var fbMasterService = new FBMasterService();
                var editList = fbMasterService.GetEntityAttrActivityEditList(entityInfo.EntityDefID, 
                    taskView.ProcessGUID, taskView.Version, taskView.ActivityGUID);

                result = ResponseResult<List<EntityAttrActivityEditEntity>>.Success(editList);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<EntityAttrActivityEditEntity>>.Error(
                    string.Format("An error occurred when reading activity permission, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 删除表单实例数据及流程数据
        /// </summary>
        /// <param name="query">查询实体</param>
        /// <returns>结果</returns>
        [HttpPost]
        public ResponseResult DeleteEntityInfo([FromBody] EntityInfoQuery query)
        {
            var result = ResponseResult.Default();
            try
            {
                FBDataService.DeleteEntityInfo(query.EntityInfoID);
                result = ResponseResult.Success();
            }
            catch(System.Exception ex)
            {
                result = ResponseResult.Error(ex.Message);
            }
            return result;
        }
        #endregion
    }
}
