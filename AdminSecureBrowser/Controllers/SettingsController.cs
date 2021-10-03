using AdminSecureBrowser.OtherFunctions.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSecureBrowser.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            Task<FirestoreAdmin> user = FirestoreFunctions.Functions.GetAdmin(User.Identity.Name);
            user.Wait();
            ViewBag.User = user.Result;
            return View();
        }

        public IActionResult Password()
        {
            Task<FirestoreAdmin> user = FirestoreFunctions.Functions.GetAdmin(User.Identity.Name);
            user.Wait();
            ViewBag.User = user.Result;
            return View();
        }
    }
}
