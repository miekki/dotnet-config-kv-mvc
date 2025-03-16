using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Azure;
using Azure.Identity;

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


        var tableServiceClient = new TableServiceClient(new Uri(_myAppSettings.TableUri), new DefaultAzureCredential());
        var tableName = "TestTable";
        TableItem table = tableServiceClient.CreateTableIfNotExists(tableName);
        ViewBag.myTableName = tableName;

        var partitionKey = "Stationery";

        var tableClient = tableServiceClient.GetTableClient(tableName);
        // var tableClient = new TableClient(
        //     new Uri(_myAppSettings.TableUri),
        //     tableName,
        //     new DefaultAzureCredential()
        // );
        // var tableClient = new TableClient(new Uri(_myAppSettings.TableUri), tableName, new DefaultAzureCredential());

        Pageable<OfficeSuplyEntity> queryResultsFilter = tableClient.Query<OfficeSuplyEntity>(filter: $"PartitionKey eq '{partitionKey}'");

        if (queryResultsFilter.Count() < 2)
        {
            var rowKey = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            var officeProduct1 = new OfficeSuplyEntity
            {
                Product = "Pen Set - colors",
                Price = 7.00,
                Quantity = 21,
                PartitionKey = partitionKey,
                RowKey = rowKey
            };
            var rowKey2 = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            var officeProduct2 = new OfficeSuplyEntity
            {
                Product = "Pen Set",
                Price = 3.00,
                Quantity = 10,
                PartitionKey = partitionKey,
                RowKey = rowKey2
            };


            tableClient.AddEntity(officeProduct1);
            tableClient.AddEntity(officeProduct2);
        }

        ViewBag.queryTableResults = queryResultsFilter;

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
