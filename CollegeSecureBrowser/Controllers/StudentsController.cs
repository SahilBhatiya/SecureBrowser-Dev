using CollegeSecureBrowser.OtherFunctions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeSecureBrowser.Controllers
{
    [Authorize(Roles = "College")]
    public class StudentsController : Controller
    {
        public IActionResult Verify()
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

        public IActionResult Add()
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


        [HttpPost]
        public IActionResult AddStudent(Student model)
        {
            model.CollegeEmail = User.Identity.Name;
            var task = FirestoreFunctions.Functions.CreateStudent(model);
            
            return Ok(Json(JsonConvert.SerializeObject(task)));
        }
       


        public IActionResult Edit(string Email)
        {
            if (User.Identity.IsAuthenticated)
            {
                Task<FirestoreCollege> user = FirestoreFunctions.Functions.GetCollege(User.Identity.Name);
                user.Wait();
                ViewBag.User = user.Result;

                Task<FirestoreStudent> student = FirestoreFunctions.Functions.GetStudent(User.Identity.Name, Email);
                student.Wait();
                ViewBag.student = student.Result;
                return View();
            }
            else
            {
                return RedirectToAction("Add");
            }
        }

        [HttpPost]
        public IActionResult UpdateStudent(Student model)
        {
            model.CollegeEmail = User.Identity.Name;
            var task = FirestoreFunctions.Functions.UpdateStudent(model);
            return Ok(Json(JsonConvert.SerializeObject(task)));
        }

        [HttpPost]
        public IActionResult Remove(String Email)
        {
            var task = FirestoreFunctions.Functions.DeleteStudent(User.Identity.Name, Email);
            task.Wait();
            return Ok(Json(JsonConvert.SerializeObject(task.Result)));
        }

    }
}
