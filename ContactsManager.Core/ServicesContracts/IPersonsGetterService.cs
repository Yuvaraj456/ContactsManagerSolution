using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;

namespace ServiceContracts
{
    public interface IPersonsGetterService
    {       

        /// <summary>
        /// Returns all person in the list
        /// </summary>
        /// <returns>Returns all person in the list</returns>
        Task<List<PersonResponse>> GetAllPersons ();

        /// <summary>
        /// Returns the person object based on the given person id
        /// </summary>
        /// <param name="guid">Person id to search</param>
        /// <returns>Return matching person object</returns>
        Task<PersonResponse?> GetPersonById(Guid? guid);

        /// <summary>
        /// Get All Persons matching with Filter using parameter value
        /// </summary>
        /// <param name="searchby">search field to search</param>
        /// <param name="searchString">search string to search</param>
        /// <returns>shows only filtered Persons</returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchby, string? searchString);
        

        /// <summary>
        /// Return Persons as CSV
        /// </summary>
        /// <returns>returns the memorystream with csv of persons</returns>
        Task<MemoryStream> GetAllPersonsCSV();

        /// <summary>
        /// Returns persons as Excel
        /// </summary>
        /// <returns>returns the memorystream with excel data of persons</returns>
        Task<MemoryStream> GetPersonsExcel();
    }
}
