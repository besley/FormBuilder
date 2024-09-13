using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SlickOne.WebUtility;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Service;


namespace FormBuilder.WebApi.Controllers
{
    /// <summary>
    /// 测试控制器
    /// </summary>
    public class TestController : Controller
    {
        #region 流程列表
        [HttpGet]
        public List<ProcessEntity> GetProcessList()
        {
            try
            {
                var wfService = new WorkflowService();
                var processList = wfService.GetProcessListSimple().ToList();
                return processList;
            }
            catch(System.Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
