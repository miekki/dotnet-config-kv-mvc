using Azure.Identity;
using api.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add configuration sources based on environment
if (builder.Environment.IsProduction())
{
    var keyVaultUrl = builder.Configuration["KeyVaultConfig:KeyVaultUrl"];
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl!),
        new DefaultAzureCredential());
}

// Add configuration binding for MyAppSettings
builder.Services.Configure<MyAppSettings>(builder.Configuration.GetSection("MyAppSettings"));

// Register MyAppSettings as a service
builder.Services.AddScoped<MyAppSettings>(sp =>
    sp.GetRequiredService<IOptions<MyAppSettings>>().Value);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
