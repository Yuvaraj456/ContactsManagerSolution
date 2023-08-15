using CRUDExample.Filters.ActionFilters;
using CRUDExample.Filters.AuthorizationFilter;
using CRUDExample.Filters.ExceptionFilter;
using CRUDExample.Filters.ResourceFilter;
using CRUDExample.Filters.ResultFilters;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers
{

    [Route("[Controller]")]    
    [ResponseHeaderFilterFactory("MyKeyController", "MyValueController", 3)]
    //[TypeFilter(typeof(HandleExceptionFilter))]
    public class PersonsController : Controller
    {
        private readonly ICountriesService _countriesService;

        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsSorterService _personsSorterService;

        private ILogger<PersonsController> _logger;

        public PersonsController(IPersonsGetterService personsGetterService,IPersonsDeleterService personsDeleterService,IPersonsAdderService personsAdderService,IPersonsSorterService personsSorterService,IPersonsUpdaterService personsUpdaterService , ICountriesService countriesService, ILogger<PersonsController> logger)
        {

            _personsGetterService = personsGetterService;
            _personsAdderService = personsAdderService;
            _personsDeleterService = personsDeleterService;
            _personsUpdaterService = personsUpdaterService;
            _personsSorterService = personsSorterService;
            _countriesService = countriesService;
            _logger = logger;
        }


        [Route("[Action]")]
        [Route("/")]
        [TypeFilter(typeof(PersonsListActionFilter),Order =4)]
      
        [TypeFilter(typeof(PersonsListResultFilter))]
        [ResponseHeaderFilterFactory("MyKeyAction", "MyValueAction", 1)]
        public async Task<IActionResult> Index(string searchBy, string searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderoptions sortOrder = SortOrderoptions.ASC)

        {
            _logger.LogInformation("Index action method of PersonsController");

            _logger.LogDebug($"searchby:{searchBy},searchString :{searchString}, " +
                $"sortBy: {sortBy}, sortOrder : {sortOrder}");
            //Searching
            //    ViewBag.Search = new Dictionary<string, string>()
            //    {
            //    { nameof(PersonResponse.PersonName), "Person Name" },
            //    { nameof(PersonResponse.Email), "Email" },
            //    { nameof(PersonResponse.DateOfBirth), "DOB" },
            //    { nameof(PersonResponse.Gender), "Gender" },
            //    { nameof(PersonResponse.CountryId), "Country" },
            //    { nameof(PersonResponse.Address), "Address" },
                
            //};

            List<PersonResponse> responseList = await _personsGetterService.GetFilteredPersons(searchBy, searchString);

            //ViewBag.CurrentSearchBy = searchBy;
            //ViewBag.CurrentSearchString = searchString;

            // List<PersonResponse> persons =  _personsService.GetAllPersons().ToList();
            //Sorting
            List<PersonResponse> sortedList = await _personsSorterService.GetSortedPersons(responseList, sortBy, sortOrder);

            //ViewBag.CurrentsortBy = sortBy;
            //ViewBag.CurrentsortOrder = sortOrder.ToString();

            return View(sortedList);

        }

        //Executes when the user clicks on "Create Person" hyperlink(While opening the create view)
        [Route("[Action]")]
        [HttpGet]     
        [ResponseHeaderFilterFactory("MyKeyAction", "MyValueAction", 4)]
        public async Task<IActionResult> Create()
         {
           List<CountryResponse> country = await _countriesService.GetAllCountry();


            ViewBag.Country = country.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString()});
            return View();
        }


        //Executes when the user clicks on "Submit" button(after submission of create form)
        [Route("[Action]")]
        [HttpPost]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFilter), Arguments = new object[] {false})]
        public async Task<IActionResult> Create(PersonAddRequest Request)
        {
           
           //call the service method
            PersonResponse personResponse = await _personsAdderService.AddPerson(Request);

            //navigate to Index() action method (it makes another get request to "persons/index")
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personId}")] //Eg : /Persons/Edit/1
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid personId)
        {
            List<CountryResponse> country = await _countriesService.GetAllCountry();

            ViewBag.Country = country.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });

            PersonResponse? personResponse = await _personsGetterService.GetPersonById(personId);

            if(personResponse == null)
            {
                return RedirectToAction("index");  
            }

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            return View(personUpdateRequest);

        }

        [HttpPost]
        [Route("[action]/{personId}")] //Eg : /Persons/Edit/1
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(TokenAuthorizaionFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest Request)
        {
           
           PersonResponse? person = await _personsGetterService.GetPersonById(Request.PersonId);

            if(person == null)
            {
                return RedirectToAction("Index");
            }

           
              PersonResponse updatedperson =  await _personsUpdaterService.UpdatePerson(Request);
                return RedirectToAction("Index");
        
            //else
            //{
            //    List<CountryResponse> country = await _countriesService.GetAllCountry();

            //    ViewBag.Country = country.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });

            //    ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(y => y.ErrorMessage).ToList(); //all model validation errors will be taken here
            //    
            //}
           

        }


        [HttpGet]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(Guid personId)
        {
           PersonResponse? personResponse = await _personsGetterService.GetPersonById(personId);

            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personId}")]
        public async Task <IActionResult> Delete(PersonResponse person)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonById(person.PersonId);

            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }
            await _personsDeleterService.DeletePerson(person.PersonId);

      

            return RedirectToAction("Index");
        }

        [Route("[Action]")]
        public async Task<IActionResult> GeneratePDF()
        {
            //get list of persons
          List<PersonResponse> Persons = await _personsGetterService.GetAllPersons();

            //return View as pdf
            return new ViewAsPdf("GeneratePDF", Persons, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins() { Right = 20, Left=20, Top = 20, Bottom=20 },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }

        [Route("[Action]")]
        public async Task<IActionResult> GenerateCSV()
        {
            MemoryStream memoryStream = await _personsGetterService.GetAllPersonsCSV();

            return File(memoryStream, "application/octet-stream", "persons.csv");
        }

        [Route("[Action]")]
        public async Task<IActionResult> GenerateExcel()
        {
            MemoryStream memoryStream = await _personsGetterService.GetPersonsExcel();

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }
    }
}
