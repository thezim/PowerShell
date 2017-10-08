using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;

namespace mvc.Controllers
{
    public class ProtocolController : Controller
    {
        public JsonResult Index()
        {
            Hashtable output = new Hashtable
            {
                {"Status"      , "FAILED"},
                {"Description" , "Only valid for HTTPS requests."}
            };
            if (HttpContext.Request.IsHttps)
            {
                output = new Hashtable {
                    {"Status"      , "OK"},
                    {"Protocol"    , Program.Protocol.ToString()}
                };
            };
            return Json(output);
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
