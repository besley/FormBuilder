using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SlickMaster.Designer.Controllers.Mvc
{
    public class FileController : Controller
    {
        // GET: File
        public ActionResult Upload()
        {
            return View();
        }
    }
}