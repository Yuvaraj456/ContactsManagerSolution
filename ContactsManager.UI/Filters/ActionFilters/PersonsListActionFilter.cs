using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {

            _logger = logger;

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("OnExecuted Method from ActionFilter method for Index action method");

            PersonsController personsController = (PersonsController)context.Controller;

            IDictionary<string,object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];

            if(parameters != null)
            {
                if(parameters.ContainsKey("searchBy"))
                {
                    personsController.ViewData["CurrentSearchBy"] = Convert.ToString(parameters["searchBy"]);
                }

                if (parameters.ContainsKey("searchString"))
                {
                    personsController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["searchString"]);
                }

                if (parameters.ContainsKey("sortBy"))
                {
                    personsController.ViewData["CurrentsortBy"] = Convert.ToString(parameters["sortBy"]);
                }
                else
                {
                    personsController.ViewData["CurrentsortBy"] = nameof(PersonResponse.PersonName);

                }

                if (parameters.ContainsKey("sortOrder"))
                {
                    personsController.ViewData["CurrentsortOrder"] = Convert.ToString(parameters["sortOrder"]);
                }
                else
                {
                    personsController.ViewData["CurrentsortOrder"] = nameof(SortOrderoptions.ASC);

                }
            }

            personsController.ViewBag.Search = new Dictionary<string, string>()
                {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "DOB" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryId), "Country" },
                { nameof(PersonResponse.Address), "Address" },

            };



        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["arguments"] = context.ActionArguments;

            _logger.LogInformation("OnExecuting Method from ActionFilter method for Index action method");

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchby = Convert.ToString(context.ActionArguments["searchBy"]);

                if (!string.IsNullOrEmpty(searchby))
                {
                    var searchlist = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.Address),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.ReceiveNewsLetters),
                        nameof(PersonResponse.Country),
                    };

                    if(searchlist.Any(temp=>temp == searchby))
                    {
                        _logger.LogInformation("searchby actual value is {searchby}", searchby);
                    }
                    else
                    {
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);

                        _logger.LogInformation("searchby Updated value is {searchby}", context.ActionArguments["searchBy"]);

                    }


                }
            }


        }
    }
}
