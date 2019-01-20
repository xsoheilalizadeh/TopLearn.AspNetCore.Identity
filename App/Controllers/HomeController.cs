using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using App.Models;
using App.Services;
using Microsoft.AspNetCore.Authorization;

namespace App.Controllers
{
    [Authorize(nameof(ConstantPolicies.DynamicPermission))]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

      
        [Display(Name = "تماس باما")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [Display(Name = "حریم خصوصی")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}