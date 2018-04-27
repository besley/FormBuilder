using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using SlickOne.Data;
using SlickOne.WebUtility;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Service;
using Slickflow.Engine.Core.Result;
using Slickflow.Engine.Common;
using SlickMaster.Builder.Entity;
using SlickMaster.Builder.Service;

namespace SlickMaster.Designer.Controllers.WebApi
{
    /// <summary>
    /// 表单数据控制器
    /// </summary>
    public class FBDataController : ApiController
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
        /// <returns></returns>
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
                    string.Format("获取有最近更新的表单记录失败，错误{0}", ex.Message)
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
        public ResponseResult<List<EntityAttrValueItem>> QueryEntityAttrValue(EntityInfoEntity entity)
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
                    string.Format("获取表单属性记录失败，错误{0}", ex.Message)
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
                    string.Format("获取单挑表单数据失败，错误{0}", ex.Message)
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
        public ResponseResult<List<dynamic>> GetEntityInfoWithAttrValueList(EntityInfoQuery query)
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
                    string.Format("获取单挑表单数据失败，错误{0}", ex.Message)
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
        public ResponseResult<List<dynamic>> GetWithAttrValueList(EntityInfoAttrQuery query)
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
                    string.Format("获取基表单及扩展属性的分页数据失败，错误{0}", ex.Message)
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
        public ResponseResult UpdateEntityRow(EntityInfoWithAttrValueListItem eav)
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
                    string.Format("保存实体及其扩展属性失败， 错误:{0}", ex.Message)
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
        public ResponseResult<EntityInfoEntity> InsertEntityRowFlow(dynamic eav)
        {
            var result = ResponseResult<EntityInfoEntity>.Default();
            try
            {
                //先保存表单
                EntityInfoWithAttrValueListItem entityAttrValueItem = JsonConvert.DeserializeObject<EntityInfoWithAttrValueListItem>(
                    eav.EntityInfoWithAttrValueListItem.ToString());
                int newID = FBDataService.InsertRow(entityAttrValueItem);
                var entityInfo = new EntityInfoEntity();
                entityInfo.ID = newID;

                var wfAppRunner = JsonConvert.DeserializeObject<WfAppRunner>(eav.WfAppRunner.ToString());
                wfAppRunner.AppInstanceID = newID.ToString();
                WfAppInteropService wfService = new WfAppInteropService();
                WfExecutedResult wfResult = wfService.StartProcess(wfAppRunner);

                //返回结果
                if (wfResult.Status == WfExecutedStatus.Success)
                    result = ResponseResult<EntityInfoEntity>.Success(entityInfo, "表单数据已经保存，流程已经启动！");
                else
                    result = ResponseResult<EntityInfoEntity>.Error(wfResult.Message);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<EntityInfoEntity>.Error(
                    string.Format("保存实体及其扩展属性失败， 错误:{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion
    }
}
