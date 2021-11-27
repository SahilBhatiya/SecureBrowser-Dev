using AdminSecureBrowser.OtherFunctions.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AdminSecureBrowser.Controllers
{
    public class CollegesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }


        public async Task<IActionResult> Edit(string email)
        {
            FirestoreCollege user = await FirestoreFunctions.Functions.GetCollege(email);
            ViewBag.User = user;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(College model)
        {
            bool result = await FirestoreFunctions.Functions.UpdateCollege(model);
            return Ok(Json(JsonConvert.SerializeObject(result)));
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email)
        {
            bool result = await FirestoreFunctions.Functions.ResetPassword(email);
            return Ok(Json(JsonConvert.SerializeObject(result)));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCollege(string email)
        {
            bool result = await FirestoreFunctions.Functions.DeleteCollege(email);
            return Ok(Json(JsonConvert.SerializeObject(result)));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCollege(College model)
        {
            string result = FirestoreFunctions.Functions.CreateCollege(model);
            return Ok(Json(JsonConvert.SerializeObject(result)));
        }
    }
}
