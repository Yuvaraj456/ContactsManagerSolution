using CRUDExample.Controllers;
using Entities;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;


namespace CRUDExample.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesService _countryService;

        public PersonCreateAndEditPostActionFilter(ICountriesService countriesService)
        {
            _countryService = countriesService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(context.Controller is PersonsController personsController)
            {
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> country = await _countryService.GetAllCountry();

                    personsController.ViewBag.Country = country.Select(temp => new SelectListItem() { Text = temp.CountryName, Value=temp.CountryId.ToString() }); ;

                    personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(x => x.Errors).Select(y => y.ErrorMessage).ToList();

                    var arguments = context.ActionArguments["Request"];

                    context.Result =  personsController.View(arguments);//short-circuiting or skip the subsequent action filters & action methods
                }
                else
                {
                    await next(); //invokes the subsequent filter or action method
                }

            }
            else
            {
                await next();//invokes the subsequent filter or action method
            }
                       

            
        }
    }
}
