
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesService _countriesservice;

        public CountriesController(ICountriesService countriesService) 
        {
            _countriesservice = countriesService;

        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelfile)
        {
            if (excelfile == null || excelfile.Length == 0)
            {
                ViewBag.ErrorMessage = "Please select an xlsx file";
                return View();
            }

            if(!Path.GetExtension(excelfile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Unsupported file. '.xlsx' file is expected";
                return View();
            }

           int countinserted =  await _countriesservice.UploadCountriesFromExcelFile(excelfile);

            ViewBag.Message = $"{countinserted} Countries updated";

            return View();
        }
    }
      
}
