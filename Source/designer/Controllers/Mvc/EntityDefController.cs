using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace FormMaster.Designer.Controllers.Mvc
{
    public class EntityDefController : Controller
    {
        // GET: Entity
        public ActionResult List()
        {
            return View();
        }
    }
}