using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using FormMaster.Designer.Models;

namespace FormMaster.Designer.Controllers.Mvc
{
    /// <summary>
    /// 设置控制器
    /// </summary>
    public class SettingController : Controller
    {
        // GET: Setting
        public ActionResult Index(int id)
        {
            var eavModel = new EavModel();
            var entity = eavModel.GetEntityDef(id);
            if (entity != null)
            {
                ViewBag.EntityDefID = id;
            }
            return View();
        }

        public ActionResult Table()
        {
            return View();
        }
    }
}