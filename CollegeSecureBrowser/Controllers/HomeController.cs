using CollegeSecureBrowser.Models;
using CollegeSecureBrowser.OtherFunctions.Functions;
using CollegeSecureBrowser.OtherFunctions.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            ViewBag.DataValue = Hashing.ComputeSha256Hash("Sahil");
            return View();
        }


        public IActionResult Create()
        {
            return View();
        } 
        

        [HttpPost]
        public ActionResult CreateCollege( College college)
        {
            FirestoreFunctions.Functions.CreateCollege(college);
            return Ok(Json(Newtonsoft.Json.JsonConvert.SerializeObject(college)));
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
