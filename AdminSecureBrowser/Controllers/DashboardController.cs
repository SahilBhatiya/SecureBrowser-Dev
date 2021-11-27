using AdminSecureBrowser.OtherFunctions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSecureBrowser.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                Task<FirestoreAdmin> user = FirestoreFunctions.Functions.GetAdmin(User.Identity.Name);
                user.Wait();
                ViewBag.User = user.Result;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public async Task<IActionResult> CollegesAsync()
        {
            Task<FirestoreAdmin> user = FirestoreFunctions.Functions.GetAdmin(User.Identity.Name);
            user.Wait();
            ViewBag.User = user.Result;
            List<FirestoreCollege> model = await FirestoreFunctions.Functions.GetAllCollege();
            return View(model);
        }

        public IActionResult Admin()
        {
            Task<FirestoreAdmin> user = FirestoreFunctions.Functions.GetAdmin(User.Identity.Name);
            user.Wait();
            ViewBag.User = user.Result;
            return View();
        }


        public IActionResult Settings()
        {
            Task<FirestoreAdmin> user = FirestoreFunctions.Functions.GetAdmin(User.Identity.Name);
            user.Wait();
            ViewBag.User = user.Result;
            return View();
        }

        [HttpPost]
        public IActionResult GetUser()
        {
            Task<FirestoreAdmin> user = FirestoreFunctions.Functions.GetAdmin(User.Identity.Name);
            user.Wait();
            return Ok(Json(JsonConvert.SerializeObject(user.Result)));
        }

        [HttpPost]
        public IActionResult UpdateAdmin(Admin model)
        {
            model.Email = User.Identity.Name;
            var task = FirestoreFunctions.Functions.UpdateAdmin(model);
            task.Wait();
            bool isValid = task.Result;


            return Ok(Json(JsonConvert.SerializeObject(isValid)));
        }

        [HttpPost]
        public IActionResult UpdatePassword(Admin model)
        {
            model.Email = User.Identity.Name;
            var task = FirestoreFunctions.Functions.UpdatePassword(model);
            task.Wait();
            bool isValid = task.Result;

            return Ok(Json(JsonConvert.SerializeObject(isValid)));
        }
    }
}
