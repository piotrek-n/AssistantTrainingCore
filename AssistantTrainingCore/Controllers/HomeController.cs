using AssistantTrainingCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AssistantTrainingCore.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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

        public IActionResult ChangeLanguage(string culture)
        {
            var isCulture = CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault(c => c.Name == culture);
            var uri = new Uri(Request.Headers["Referer"]);

            if (isCulture != null && uri.Segments.Length == 1 && uri.Segments[0] == "/")
            {
                return Redirect(string.Format("{0}/{1}", uri.Authority, culture));
            }

            if (isCulture != null && uri.Segments.Length > 1 && CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault(c => c.Name == uri.Segments[1].Replace("/", "")) != null)
            {
                return Redirect(uri.ToString().Replace(uri.Segments[1].Replace("/", ""), culture));
            }

            return Redirect(Request.Headers["Host"].ToString());
        }
    }
}
