using CRUDExample.Filters.ActionFilters;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using Services;
using System.Collections;

namespace CRUDExample.StartupExtensions
{
    public static class ConfigureServicesExtensions 
    {
        
        public static IServiceCollection ConfigureServices(this IServiceCollection service, IConfiguration configuration)
        {
            

            //Logging only in Console, Debug, Event Log
            //builder.Host.ConfigureLogging(loggingprovider =>
            //{
            //    loggingprovider.ClearProviders();
            //    loggingprovider.AddConsole();
            //    loggingprovider.AddDebug();
            //    loggingprovider.AddEventLog();
            //});

            service.AddTransient<ResponseHeaderActionFilter>();
            service.AddControllersWithViews(options =>
            {
                //Adding Global FIlters
                //options.Filters.Add<ResponseHeaderActionFilter>();

                var logger = service.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

                //Adding Global filter with Arguments
                options.Filters.Add(new ResponseHeaderActionFilter(logger) { _key = "MyKeyGlobal", _value = "MyValueGlobal", Order = 2 });

            });

            //add services into IOC container
            service.AddScoped<ICountriesService, CountriesService>();

            //service.AddScoped<IPersonsGetterService, PersonsGetterService>();
            service.AddScoped<IPersonsGetterService, PersonsGetterServiceWithFewExcelFields>();
            service.AddScoped<PersonsGetterService, PersonsGetterService>();


            service.AddScoped<IPersonsAdderService, PersonsAdderService>();
            service.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
            service.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
            service.AddScoped<IPersonsSorterService, PersonsSorterService>();

            service.AddScoped<ICountriesRepository, CountriesRepository>();

            service.AddScoped<IPersonsRepository, PersonsRepository>();

            // Service for DbContext
            service.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging();
            });



            //Data Source=(localdb)\ProjectModels;Initial Catalog=PersonsDatabase;Integrated Security=True;
            //Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;
            //Multi Subnet Failover=False
            return service;

        }
    }
}
