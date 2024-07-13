using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FormMaster.Designer.Models;

namespace FormMaster.Designer.Controllers.Mvc
{
    /// <summary>
    /// 表单绑定流程
    /// </summary>
    public class FlowController : Controller
    {
        public IActionResult Index(int id)
        {
            var eavModel = new EavModel();
            var entity = eavModel.GetEntityDef(id);
            if (entity != null)
            {
                ViewBag.EntityDefID = id;
            }
            return View();
        }
    }
}