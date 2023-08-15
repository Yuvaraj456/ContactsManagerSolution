using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Resresent business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add</param>
        /// <returns>Returns the Country object after adding it (including newly generated country id)</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns All the countries from the list
        /// </summary>
        /// <returns>returns list of countries as CountryResponse object</returns>
        Task<List<CountryResponse>> GetAllCountry();

        /// <summary>
        /// Returns a object based on the given country id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns>Matching Country as CountryResponse object</returns>
        Task<CountryResponse?> GetCountryById(Guid? countryId);

        /// <summary>
        /// Uploads Countries from excel file into database
        /// </summary>
        /// <param name="formFiles">excel file with list of countries</param>
        /// <returns>Returns number of countries added</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFiles);
    }
}