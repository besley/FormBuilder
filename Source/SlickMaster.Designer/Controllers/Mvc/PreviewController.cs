using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SlickMaster.Designer.Models;

namespace SlickMaster.Designer.Controllers.Mvc
{
    public class PreviewController : BaseMvcController
    {
        // GET: PVPage
        public ActionResult Index(int id)
        {
            var eavModel = new EavModel();
            var entity = eavModel.GetEntityDef(id);
            ViewBag.Title = entity.EntityTitle;
            ViewBag.HtmlStr = entity.HTMLContent;

            return View();
        }
    }
}