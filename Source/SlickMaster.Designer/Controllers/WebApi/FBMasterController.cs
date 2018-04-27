using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SlickOne.Data;
using SlickOne.WebUtility;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Service;
using SlickMaster.Builder.Entity;
using SlickMaster.Builder.Service;

namespace SlickMaster.Designer.Controllers.WebApi
{
    /// <summary>
    /// 动态表单数据WebApi服务
    /// </summary>
    public class FBMasterController : ApiController
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

        #region 流程定义记录
        /// <summary>
        /// 获取流程定义记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult<List<ProcessEntity>> GetProcessByEntityDef(int id)
        {
            var result = ResponseResult<List<ProcessEntity>>.Default();
            try
            {
                var wfService = new WorkflowService();
                var list = wfService.GetProcessListSimple().ToList();
                result = ResponseResult<List<ProcessEntity>>.Success(list);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<ProcessEntity>>.Error(
                    string.Format("读取流程定义记录失败：{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion

        #region 动态表单定义记录读取
        /// <summary>
        /// 读取表单定义记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<EntityDefEntity> QueryEntityDef(EntityDefEntity query)
        {
            var result = ResponseResult<EntityDefEntity>.Default();
            try
            {
                var fbService = new FBMasterService();
                var entity = fbService.GetEntityDef(query.ID);
                result = ResponseResult<EntityDefEntity>.Success(entity);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<EntityDefEntity>.Error(
                    string.Format("读取表单定义记录失败：{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 读取表单记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult<List<EntityDefEntity>> GetEntityDefList2()
        {
            var result = ResponseResult<List<EntityDefEntity>>.Default();
            try
            {
                var fbService = new FBMasterService();
                var list = fbService.GetEntityDefList2();
                result = ResponseResult<List<EntityDefEntity>>.Success(list);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<EntityDefEntity>>.Error(
                    string.Format("读取表单定义列表失败：{0}", ex.Message)
                );
            }
            return result;
        }


        /// <summary>
        /// 读取表单记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult<List<EntityDefView>> GetEntityDefViewList()
        {
            var result = ResponseResult<List<EntityDefView>>.Default();
            try
            {
                var fbService = new FBMasterService();
                var list = fbService.GetEntityDefViewList();
                result = ResponseResult<List<EntityDefView>>.Success(list);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<EntityDefView>>.Error(
                    string.Format("读取表单定义列表失败：{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion

        #region 字段定义读取
        /// <summary>
        /// 获取属性字段列表
        /// </summary>
        /// <param name="id">实体定义ID</param>
        /// <returns>字段列表</returns>
        [HttpGet]
        public ResponseResult<List<EntityAttributeEntity>> GetEntityAttributeList(int id)
        {
            var result = ResponseResult<List<EntityAttributeEntity>>.Default();
            try
            {
                var list = FBMasterService.GetEntityAttributeList(id);
                result = ResponseResult<List<EntityAttributeEntity>>.Success(list);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<EntityAttributeEntity>>.Error(
                    string.Format("读取表单字段发生错误：{0}", ex.Message)
                );
            }
            return result;
        }
        #endregion
    }
}
