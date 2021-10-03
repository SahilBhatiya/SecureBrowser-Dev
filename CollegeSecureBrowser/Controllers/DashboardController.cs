using CollegeSecureBrowser.OtherFunctions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CollegeSecureBrowser.Controllers
{
    [Authorize(Roles = "College")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                Task<FirestoreCollege> user = FirestoreFunctions.Functions.GetCollege(User.Identity.Name);
                user.Wait();
                ViewBag.User = user.Result;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public IActionResult Settings()
        {
            Task<FirestoreCollege> user = FirestoreFunctions.Functions.GetCollege(User.Identity.Name);
            user.Wait();
            ViewBag.User = user.Result;
            return View();
        }


        [HttpPost]
        public IActionResult GetUser()
        {
            Task<FirestoreCollege> user = FirestoreFunctions.Functions.GetCollege(User.Identity.Name);
            user.Wait();
            return Ok(Json(JsonConvert.SerializeObject(user.Result)));
        }

        [HttpPost]
        public IActionResult UpdateCollege(College college)
        {
            college.Email = User.Identity.Name;
            var task = FirestoreFunctions.Functions.UpdateCollege(college);
            task.Wait();
            bool isValid = task.Result;


            return Ok(Json(JsonConvert.SerializeObject(isValid)));
        }

        [HttpPost]
        public IActionResult UpdatePassword(College college)
        {
            college.Email = User.Identity.Name;
            var task = FirestoreFunctions.Functions.UpdatePassword(college);
            task.Wait();
            bool isValid = task.Result;

            return Ok(Json(JsonConvert.SerializeObject(isValid)));
        }


    }
}
