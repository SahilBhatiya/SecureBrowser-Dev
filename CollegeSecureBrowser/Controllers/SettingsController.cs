using CollegeSecureBrowser.OtherFunctions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeSecureBrowser.Controllers
{
    [Authorize(Roles = "College")]
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            Task<FirestoreCollege> user = FirestoreFunctions.Functions.GetCollege(User.Identity.Name);
            user.Wait();
            ViewBag.User = user.Result;
            return View();
        }

        public IActionResult Password()
        {
            Task<FirestoreCollege> user = FirestoreFunctions.Functions.GetCollege(User.Identity.Name);
            user.Wait();
            ViewBag.User = user.Result;
            return View();
        }
    }
}
