using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DotNetDay_201119.Models;
using Microsoft.FeatureManagement;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNetDay_201119.Controllers
{
    public class HomeController : Controller
    {
        // using IFeatureManager could cause the 'IsEnabled' status of a feature to change during the course of a request
        //private readonly IFeatureManager _featureManager;

        // using IFeatureManagerSnapshot preserves the 'IsEnabled' status of a feature during the course of a request
        private readonly IFeatureManagerSnapshot _featureManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IFeatureManagerSnapshot featureManager)
        {
            _logger = logger;
            _featureManager = featureManager;
        }

        public IActionResult Index()
        {
            ViewData["OutputString"] = "Output from le controller: ";

            if (_featureManager.IsEnabledAsync("FeatureA").Result)
            {
                ViewData["OutputString"] += " | Feature A is Enabled!";
            }

            if (_featureManager.IsEnabledAsync("FeatureB").Result)
            {
                ViewData["OutputString"] += " | Feature B is Enabled!";
            }

            if (_featureManager.IsEnabledAsync("FeatureC").Result)
            {
                ViewData["OutputString"] += " | Feature C is Enabled!";
            }

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

        public async Task<string> Features()
        {
            IAsyncEnumerable<string> features = this._featureManager.GetFeatureNamesAsync();
            Dictionary<string, bool> frontendFeatures = new Dictionary<string, bool>();

            await foreach (string feature in features)
            {
                frontendFeatures.Add(feature, this._featureManager.IsEnabledAsync(feature).Result);
            }

            return JsonSerializer.Serialize(frontendFeatures);
        }
    }
}
