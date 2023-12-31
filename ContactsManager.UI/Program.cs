using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using Services;
using Repositories;
using RepositoryContracts;
using Serilog;
using CRUDExample.Filters.ActionFilters;
using CRUDExample.StartupExtensions;
using CRUDExample.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Services.ConfigureServices(builder.Configuration);

//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration Loggerconf) =>
{
    //read configuration settings from built-in Iconfiguration
    Loggerconf.ReadFrom.Configuration(context.Configuration)
    //read out current app's services and make them available to serilog
    .ReadFrom.Services(services);
});

var app = builder.Build();
app.UseSerilogRequestLogging();

if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandlingMiddleware();
}

app.UseHsts();
app.UseHttpsRedirection();
app.Logger.LogDebug("Debug-message");
app.Logger.LogWarning("Warning-message");
app.Logger.LogInformation("Information-message");
app.Logger.LogError("Error-message");
app.Logger.LogCritical("Critical-message");

if (builder.Environment.IsEnvironment("Test") == false)
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseHttpLogging();
app.UseStaticFiles();

app.UseRouting(); //Identtifying action method based route
app.UseAuthentication(); //reading identity cookie eg: login and logout
app.UseAuthorization(); //Validates access permissions of the user
app.MapControllers();//Execute the filter pipeline (action + filters)

app.UseEndpoints(options =>
{
    //conventional routing for areas
    options.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}"

        );//Admin/Home/Index/

    //conventional routing, if we apply attribute route in controller then this code is overrided by Route Attribute
    options.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action}/{id?}"
 
        );
});

app.Run();

public partial class Program { } //Make the auto-generated Program accessible programatically