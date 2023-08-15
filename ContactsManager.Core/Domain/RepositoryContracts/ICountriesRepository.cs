using ContactsManager.Core.Domain.Entities;


namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing country Entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Adds a new Country object to the data store
        /// </summary>
        /// <param name="country">Country object to add</param>
        /// <returns>Returns  the country object after adding into the data store</returns>
        Task<Country> AddCountry(Country country); 

        /// <summary>
        /// return all countries in the database
        /// </summary>      
        /// <returns>return all countries from table</returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// return country based on the country id, otherwise return null.
        /// </summary>
        /// <param name="countryId">based on the country id filter the country in data store</param>
        /// <returns>return country based on the country id or null</returns>
        Task<Country?> GetCountryById(Guid? countryId);

        /// <summary>
        /// get country by using country name
        /// </summary>
        /// <param name="countryName">based on the country name return country object</param>
        /// <returns>return country or null</returns>
        Task<Country?> GetCountryByCountryName(string countryName);

    }
}