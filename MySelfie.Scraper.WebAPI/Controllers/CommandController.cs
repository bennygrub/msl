using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Management.Automations;

namespace MySelfie.Scraper.WebAPI.Controllers
{
    public class CommandController : Controller
    {
        //
        // GET: /Command/

        public JsonResult Reset()
        {
            using (var ps = PowerShell.Create())
            {
            }
            return Json(new {
                Success = true
            });
        }

    }
}
