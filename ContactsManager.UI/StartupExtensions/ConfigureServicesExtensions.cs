using ContactsManager.Core.Domain.IdentityEntities;
using CRUDExample.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

            //enable identity in this project
            service.AddIdentity<ApplicationUser,ApplicationRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 3; //eg: AB23AB - here unique character is 4 is acceptable as per this line
                options.Password.RequireDigit =false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders() //otp for forget password
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

            service.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                                          .RequireAuthenticatedUser()
                                            .Build(); //enforces authorization policy(user must be authenticated) for all the action methods
            });

            service.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login"; //if user is not loggedin then redirect to this page

            });

            //Data Source=(localdb)\ProjectModels;Initial Catalog=PersonsDatabase;Integrated Security=True;
            //Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;
            //Multi Subnet Failover=False
            return service;

        }
    }
}
