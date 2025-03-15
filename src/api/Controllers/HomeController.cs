using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using api.Models;

namespace api.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _config;

    private readonly MyAppSettings _myAppSettings;

    public HomeController(ILogger<HomeController> logger, IConfiguration config, MyAppSettings myAppSettings)
    {
        _logger = logger;
        // Inject the IConfiguration to get values from appsettings.json
        _config = config;
        // Inject the MyAppSettings object to get values from appsettings.json (strong typed)
        _myAppSettings = myAppSettings;

    }

    public IActionResult Index()
    {
        // Get the secret and variable by refering to name of the section in appsettings.json
        ViewBag.mySecret = _config["MyAppSettings:MySecret"];
        ViewBag.myVariable = _config["MyAppSettings:Variable1"];

        // Get the secret and variable by using the injected MyAppSettings object (strong typed)
        ViewBag.myAppSettingsSecret = _myAppSettings.MySecret;
        ViewBag.myAppSettingsVariable = _myAppSettings.Variable1;

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
}
