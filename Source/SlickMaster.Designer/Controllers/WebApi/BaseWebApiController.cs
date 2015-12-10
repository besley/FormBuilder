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

namespace SlickMaster.Designer.Controllers.WebApi
{
    /// <summary>
    /// Webapi 基类
    /// </summary>
    public class BaseWebApiController : ApiController
    {
        #region 属性对象
        private Repository _quickRepository;
        public Repository QuickReporsitory
        {
            get
            {
                if (_quickRepository == null) _quickRepository = new Repository();
                return _quickRepository;
            }
        }

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
    }
}
