using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Configuration;
using SmartphoneApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartphoneApp.Controllers
{
    public class CiotolaController : Controller
    {
        private IConfiguration _configuration;

        public CiotolaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var cm = new CiotolaModel();
            cm.Id = "CiotolaManual";

            return View(cm);
        }

        public async Task<IActionResult> AskCiotola(string id, [FromBody] AskCiotolaRequest request)
        {
            return Json(new { Success = true });
        }

        public async Task<IActionResult> FillCiotola(string id, [FromBody] FillCiotolaRequest request)
        {
            var rm = RegistryManager.CreateFromConnectionString(_configuration["IoTHubConnectionString"]);
            var twin = await rm.GetTwinAsync(id);
            twin.Properties.Desired["Cmd"] = request.Cmd;
            await rm.UpdateTwinAsync(id, twin, twin.ETag);
            return Json(new { Success = true });
        }
    }
}
