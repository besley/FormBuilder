using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SlickOne.WebUtility;
using Slickflow.Engine.Business.Entity;
using FormMaster.Builder.Entity;
using FormMaster.Builder.Service;
using FormMaster.Designer.Models;

namespace FormMaster.Designer.Controllers.WebApi
{
    /// <summary>
    /// 动态表单数据WebApi服务
    /// </summary>
    public class FBMasterController : Controller
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
        public ResponseResult<List<ProcessEntity>> GetProcessByEntityDef()
        {
            var result = ResponseResult<List<ProcessEntity>>.Default();
            try
            {
                var wfAppService = new WfAppService();
                var list = wfAppService.GetProcessListSimple().ToList();
                result = ResponseResult<List<ProcessEntity>>.Success(list);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<ProcessEntity>>.Error(
                    string.Format("An error occurred when reading process list, detail:{0}", ex.Message)
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
        public ResponseResult<EntityDefEntity> QueryEntityDef([FromBody] EntityDefEntity query)
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
                    string.Format("An error occurred when reading form list, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 读取表单定义流程关联记录
        /// </summary>
        /// <param name="query">查询实体</param>
        /// <returns>表单和流程关联视图</returns>
        [HttpPost]
        public ResponseResult<EntityDefProcessView> QueryEntityDefProcessView([FromBody] EntityDefQuery query)
        {
            var result = ResponseResult<EntityDefProcessView>.Default();
            try
            {
                var fbService = new FBMasterService();
                var entity = fbService.GetEntityDefProcessView(query.ID);
                result = ResponseResult<EntityDefProcessView>.Success(entity);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<EntityDefProcessView>.Error(
                    string.Format("An error occurred when reading form binding process info, detail:{0}", ex.Message)
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
                    string.Format("An error occurred when reading form list, detail:{0}", ex.Message)
                );
            }
            return result;
        }


        /// <summary>
        /// 读取表单记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult<List<EntityDefProcessView>> GetEntityDefViewList()
        {
            var result = ResponseResult<List<EntityDefProcessView>>.Default();
            try
            {
                var fbService = new FBMasterService();
                var list = fbService.GetEntityDefViewList();
                result = ResponseResult<List<EntityDefProcessView>>.Success(list);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<EntityDefProcessView>>.Error(
                    string.Format("An error occurred when reading form view list, detail:{0}", ex.Message)
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
                    string.Format("An error occurred when reading form attribute list, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 获取属性字段列表，仅限于数值录入
        /// </summary>
        /// <param name="id">实体定义ID</param>
        /// <returns>字段列表</returns>
        [HttpGet]
        public ResponseResult<List<EntityAttributeEntity>> GetEntityAttributeListOnlyInfoValue(int id)
        {
            var result = ResponseResult<List<EntityAttributeEntity>>.Default();
            try
            {
                var list = FBMasterService.GetEntityAttributeListOnlyInfoValue(id);
                result = ResponseResult<List<EntityAttributeEntity>>.Success(list);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<EntityAttributeEntity>>.Error(
                    string.Format("An error occurred when reading single attribute info value, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 查询字段绑定事件
        /// </summary>
        /// <param name="entity">查询实体</param>
        /// <returns>属性列表</returns>
        [HttpPost]
        public ResponseResult<List<EntityAttributeEventView>> GetEntityAttributeEventList([FromBody] EntityAttributeEventEntity query)
        {
            var result = ResponseResult<List<EntityAttributeEventView>>.Default();
            try
            {
                var items = FBMasterService.GetEntityAttributeEventList(query.EntityDefID, query.AttrID);
                result = ResponseResult<List<EntityAttributeEventView>>.Success(items, "Quering attribute binding event list successfully!");
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<EntityAttributeEventView>>.Error(
                    string.Format("An error occurred when quering attribute binding event list, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 查询字段绑定事件
        /// </summary>
        /// <param name="entity">查询实体</param>
        /// <returns>属性列表</returns>
        [HttpPost]
        public ResponseResult<List<EntityAttributeEventView>> GetEntityAttributeEventListByForm([FromBody] EntityAttributeEventEntity query)
        {
            var result = ResponseResult<List<EntityAttributeEventView>>.Default();
            try
            {
                var items = FBMasterService.GetEntityAttributeEventListByForm(query.EntityDefID);
                result = ResponseResult<List<EntityAttributeEventView>>.Success(items, "Quering attribute binding event list successfully!");
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<EntityAttributeEventView>>.Error(
                    string.Format("An error occurred when quering attribute binding event list, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="entity">属性事件实体</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<EntityAttributeEventEntity> SaveAttributeEvent([FromBody] EntityAttributeEventEntity entity)
        {
            var result = ResponseResult<EntityAttributeEventEntity>.Default();
            try
            {
                var items = FBMasterService.SaveAttributeEvent(entity);
                result = ResponseResult<EntityAttributeEventEntity>.Success(items, "Saving attribute binding event successfully!");
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<EntityAttributeEventEntity>.Error(
                    string.Format("An error occurred when saving attribute binding event, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 删除事件实体
        /// </summary>
        /// <param name="entity">事件实体</param>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult DeleteAttributeEvent(int id)
        {
            var result = ResponseResult.Default();
            try
            {
                FBMasterService.DeleteAttributeEvent(id);
                result = ResponseResult.Success("Deleted attribute binding event successfully!");
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("An error occurred when deleting attribute binding event, detail:{0}", ex.Message)
                );
            }
            return result;
        }

        /// <summary>
        /// 根据字段删除事件实体列表
        /// </summary>
        /// <param name="entity">事件实体</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult DeleteAttributeEventBatch([FromBody] EntityAttributeEventEntity entity)
        {
            var result = ResponseResult.Default();
            try
            {
                FBMasterService.DeleteAttributeEvent(entity.EntityDefID, entity.AttrID);
                result = ResponseResult.Success("Deleted attribute binding event successfully!");
            }
            catch (System.Exception ex)
            {
                result = ResponseResult.Error(
                    string.Format("An error occurred when deleting attribute binding event, detail:{0}", ex.Message)
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
        public ResponseResult<List<EntityAttrActivityEditEntity>> QueryEntityAttrActivityEditList([FromBody] EntityAttrActivityQuery query)
        {
            var result = ResponseResult<List<EntityAttrActivityEditEntity>>.Default();
            try
            {
                var fbService = new FBMasterService();
                var attributeList = fbService.GetEntityAttributeList(query.EntityDefID);

                var attributeEditList = fbService.GetEntityAttrActivityEditList(query.EntityDefID, 
                    query.ProcessID, 
                    query.ActivityGUID);

                foreach (var attr in attributeList)
                {
                    var attribute = attributeEditList.Find(s => s.AttrID == attr.ID);
                    if (attribute == null)
                    {
                        attributeEditList.Add(new EntityAttrActivityEditEntity
                        {
                            EntityDefID = query.EntityDefID,
                            ProcessID = query.ProcessID,
                            ActivityGUID = query.ActivityGUID,
                            AttrID = attr.ID,
                            AttrName = attr.AttrName,
                            IsNotVisible = 0,
                            IsReadOnly = 0
                        });
                    }
                }
                result = ResponseResult<List<EntityAttrActivityEditEntity>>.Success(attributeEditList);
            }
            catch (System.Exception ex)
            {
                result = ResponseResult<List<EntityAttrActivityEditEntity>>.Error(
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
        public ResponseResult SaveEntityAttrActivityEditList([FromBody] EntityAttrActivityEditListComp comp)
        {
            var result = ResponseResult.Default();
            try
            {
                var processID = comp.ProcessID;
                var fbService = new FBMasterService();
                fbService.SaveEntityAttrActivityEditList(processID, comp.AttrEditList);

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
        public ResponseResult DeleteEntityAttrActivityEdit([FromBody] EntityAttrActivityQuery query)
        {
            var result = ResponseResult.Default();
            try
            {
                var fbService = new FBMasterService();
                var entity = fbService.DeleteEntityAttrActivityEdit(query.EntityDefID, query.ProcessID, query.ActivityGUID);
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
        public ResponseResult ClearEntityAttractivityEdit([FromBody] EntityAttrActivityQuery query)
        {
            var result = ResponseResult.Default();
            try
            {
                var fbService = new FBMasterService();
                var entity = fbService.ClearEntityAttrActivityEdit(query.EntityDefID);
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
