using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace FormMaster.Designer.Controllers.Mvc
{
    public class AttributeController : Controller
    {
        // GET: Attr
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Event()
        {
            return View();
        }
    }
}