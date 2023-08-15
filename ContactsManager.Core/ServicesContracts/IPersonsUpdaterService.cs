using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;

namespace ServiceContracts
{
    public interface IPersonsUpdaterService
    {
       
        /// <summary>
        ///  Updates the specified person details based on the given personId
        /// </summary>
        /// <param name="personUpdateRequest">Person details to update, including personid</param>
        /// <returns>Updated Person object</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

       
    }
}
