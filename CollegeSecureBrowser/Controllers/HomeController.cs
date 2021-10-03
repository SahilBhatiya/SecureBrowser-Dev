using CollegeSecureBrowser.Models;
using CollegeSecureBrowser.OtherFunctions.Functions;
using CollegeSecureBrowser.OtherFunctions.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CollegeSecureBrowser.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.isConnect = FirestoreFunctions.Functions.Connect();
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                return View();
            }
            
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginCollege(College college)
        {
            //Password Hashing
            college.Password = Hashing.ComputeSha256Hash(college.Password);

            //Getting Data From Server
            var task = FirestoreFunctions.Functions.VerifyCollege(college);
            task.Wait();
            bool isValid = task.Result;

            Task<FirestoreCollege> user = FirestoreFunctions.Functions.GetCollege(college.Email);
            user.Wait();

            if (isValid)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, college.Email),
                    new Claim(ClaimTypes.Role, user.Result.Role),
                    new Claim(ClaimTypes.Name, college.Email)

                };
                var identity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme
                    );

                var principal = new ClaimsPrincipal(identity);
                var props = new AuthenticationProperties();
                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, principal, props
                    ).Wait();
            }

            await Task.CompletedTask;
            return Ok(Json(JsonConvert.SerializeObject(isValid)));
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok(Json(JsonConvert.SerializeObject(true)));
        }


        [HttpPost]
        public ActionResult CreateCollege( College college)
        {
            //Password Hashing
            college.Password = Hashing.ComputeSha256Hash(college.Password);
        
            return Ok(Json(Newtonsoft.Json.JsonConvert.SerializeObject(FirestoreFunctions.Functions.CreateCollege(college))));
        }

        [HttpPost]
        public ActionResult Connect()
        {
            
            return Ok(Json("Connected To DataBase"));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
