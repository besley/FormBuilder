using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using FormMaster.Designer.Models;

namespace FormMaster.Designer.Controllers.Mvc
{
    public class PreviewController : Controller
    {
        // GET: PVPage
        public ActionResult Index(int id)
        {
            var eavModel = new EavModel();
            var entity = eavModel.GetEntityDef(id);
            if (entity != null)
            {
                ViewBag.EntityDefID = id;
                ViewBag.Title = entity.EntityTitle;
                ViewBag.HtmlStr = entity.HTMLContent;
            }
            return View();
        }

        public ActionResult Test()
        {
            return View();
        } 
    }
}