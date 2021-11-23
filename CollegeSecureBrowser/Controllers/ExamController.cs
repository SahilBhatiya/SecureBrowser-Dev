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
    public class ExamController : Controller
    {
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
        public IActionResult AddExam(Exam model)
        {
            model.CollegeEmail = User.Identity.Name;
            var task = FirestoreFunctions.Functions.CreateExam(model);
            task.Wait();
            return Ok(Json(JsonConvert.SerializeObject(task.Result)));
        }

        public IActionResult Edit(string Id)
        {
            if (User.Identity.IsAuthenticated)
            {
                Task<FirestoreCollege> user = FirestoreFunctions.Functions.GetCollege(User.Identity.Name);
                user.Wait();
                ViewBag.User = user.Result;

                Task<FirestoreExam> exam = FirestoreFunctions.Functions.GetExam(User.Identity.Name, Id);
                exam.Wait();
                ViewBag.exam = exam.Result;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult EditExam(Exam model)
        {
            model.CollegeEmail = User.Identity.Name;
            var task = FirestoreFunctions.Functions.UpdateExamAsync(model);
            task.Wait();
            return Ok(Json(JsonConvert.SerializeObject(task.Result)));
        }

        [HttpPost]
        public IActionResult Remove(String Id)
        {
            var task = FirestoreFunctions.Functions.DeleteExam(User.Identity.Name, Id);
            task.Wait();
            return Ok(Json(JsonConvert.SerializeObject(task.Result)));
        }

    }
}
