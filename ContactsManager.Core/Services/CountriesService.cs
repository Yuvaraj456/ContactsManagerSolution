using ContactsManager.Core.Domain.Entities;  
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using System.Security.Cryptography.X509Certificates;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesrepository;

        public CountriesService(ICountriesRepository countriesrepository)
        {
            _countriesrepository = countriesrepository;                         
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            //when CountryAddRequest is null, it should throw ArgumentNullException
            if(countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //When the CountryName is null, it should throw ArgumentNullException
            if(countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            
            //When the CountryName is Duplicate, it should throw ArgumentNullException
            if (await _countriesrepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
               
            {
                throw new ArgumentException("Specified Country Name is Exist Already");
            }


            Country country = countryAddRequest!.ToCountry();

            //new Guid
            country.CountryId = Guid.NewGuid();

            //adding to country domain object
            await _countriesrepository.AddCountry(country);
           
            return country.ToCountryResponse();

        }

        public async Task<List<CountryResponse>> GetAllCountry()
        {
            List<Country> countries = await _countriesrepository.GetAllCountries();
            return countries.Select(x => x.ToCountryResponse()).ToList();


        }

        public async Task<CountryResponse?> GetCountryById(Guid? countryId)
        {
            if(countryId == null)
            {
                return null;
            }

            Country? response = await _countriesrepository.GetCountryById(countryId);


            if (response == null)
                return null;

            return response.ToCountryResponse();
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFiles)
        {
            MemoryStream memoryStream = new MemoryStream();

            await formFiles.CopyToAsync(memoryStream);
            int insertedcount = 0;
            using(ExcelPackage  package = new ExcelPackage(memoryStream))
            {
               ExcelWorksheet worksheet = package.Workbook.Worksheets["Countries"];

                int rowCount = worksheet.Dimension.Rows;

                for(int row = 2; row <= rowCount; row++)
                {
                   string? cellvalue = Convert.ToString(worksheet.Cells[row, 1].Value);

                    if(!string.IsNullOrEmpty(cellvalue) )
                    {
                        string? countryName = cellvalue;

                        if(await _countriesrepository.GetCountryByCountryName(countryName) == null)
                        {
                            Country country = new Country() { CountryName = countryName };

                            await _countriesrepository.AddCountry(country);                           

                            insertedcount++;
                        }
                    }
                }

            }

            return insertedcount;
            
        }
    }
}