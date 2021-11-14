using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SecureBrowser.Functions;
using SecureBrowser.Models;
using SecureBrowser.OtherFunctions.Functions;
using SecureBrowser.OtherFunctions.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SecureBrowser.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Colleges = await FirestoreFunctions.Functions.GetAllCollege();
            ViewBag.IpAddress = GetIpAddress.Fetch();
            return View();
        }

        public async Task<IActionResult> PrintExams(string Email)
        {
            List<FirestoreExam> m = await FirestoreFunctions.Functions.GetAllExams(Email);
            string f = null;
            foreach (var s in m)
            {
                f += "<option value='" + s.Semester + "'> Sem " + s.Semester + "</option>";
            }
            return Ok(Json(JsonConvert.SerializeObject(f)));
        }

        [HttpPost]
        public async Task<IActionResult> LoginStudent(string Email, string StudentEmail, string Password)
        {
            //Password Hashing
            //string newPassword = Hashing.ComputeSha256Hash(Password);

            //Getting Data From Server
            var user = await FirestoreFunctions.Functions.VerifyStudent(Email, StudentEmail, Password);

            if (user)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, StudentEmail),
                    new Claim(ClaimTypes.Name, StudentEmail),
                    new Claim(ClaimTypes.Role, "Student"),

                };
                var identity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme
                    );

                var principal = new ClaimsPrincipal(identity);
                var props = new AuthenticationProperties();

                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, principal, props
                    ).Wait();

            return Ok(Json(JsonConvert.SerializeObject(true)));
            }
            else
            {
                return Ok(Json(JsonConvert.SerializeObject(false)));
            }
        }

        
        public async Task<IActionResult> SelectExam(string clg, string email, string pass)
        {
            var isvalid = await FirestoreFunctions.Functions.VerifyStudent(clg, email, pass);

            if (isvalid)
            {
                Task<FirestoreStudent> user = FirestoreFunctions.Functions.GetStudent(clg, email);
                user.Wait();
                ViewBag.User = user.Result;
                if (user.Result != null)
                {
                    ViewBag.Exams = await FirestoreFunctions.Functions.GetAllExamsInSem(clg, user.Result.Semester);
                    return View();
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoadExam(string clg, string email, string pass, string exam)
        {
            var isvalid = await FirestoreFunctions.Functions.VerifyStudent(clg, email, pass);

            if (isvalid)
            {
                Task<FirestoreStudent> user = FirestoreFunctions.Functions.GetStudent(clg, email);
                user.Wait();
                ViewBag.User = user.Result;
                if (user.Result != null)
                {
                    List<FirestoreExam> exams = await FirestoreFunctions.Functions.GetAllExamsInSem(clg, user.Result.Semester);
                    FirestoreExam SelectedExam = exams.Find(x => x.Id == exam);

                    DateTime Start = DateTime.ParseExact(SelectedExam.Start.ToString().Replace("Timestamp:", "").Trim(),
"yyyy-MM-ddTHH:mm:ffZ", null);
                    Start = Start.AddHours(-6).AddMinutes(30);

                    DateTime End = DateTime.ParseExact(SelectedExam.End.ToString().Replace("Timestamp:", "").Trim(),
        "yyyy-MM-ddTHH:mm:ffZ", null);
                    End = End.AddHours(-6).AddMinutes(30);

                    return Ok(Json(JsonConvert.SerializeObject(
                        new { Exam = SelectedExam, 
                            Start = Start, 
                            End = End}
                    )));
                }
                else
                {
                    return Ok(Json(JsonConvert.SerializeObject(false)));
                }
            }
            else
            {
                return Ok(Json(JsonConvert.SerializeObject(false)));
            }
        }

        public async Task<IActionResult> CurrentExam(string clgEmail, string studentEmail, string examId, string pass, string endTime)
        {
            var isvalid = await FirestoreFunctions.Functions.VerifyStudent(clgEmail, studentEmail, pass);

            if (isvalid)
            {
                Task<FirestoreStudent> user = FirestoreFunctions.Functions.GetStudent(clgEmail, studentEmail);
                user.Wait();
                ViewBag.User = user.Result;
                if (user.Result != null)
                {
                    List<FirestoreExam> exams = await FirestoreFunctions.Functions.GetAllExamsInSem(clgEmail, user.Result.Semester);
                    FirestoreExam SelectedExam = exams.Find(x => x.Id == examId);
                    ViewBag.SelectedExam = SelectedExam;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
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
