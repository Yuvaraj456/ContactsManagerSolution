using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;

namespace ServiceContracts
{
    public interface IPersonsDeleterService
    {
        

        /// <summary>
        /// Delete a person based on the given person id
        /// </summary>
        /// <param name="PersonId">PersonId to delete</param>
        /// <returns>Return true, if the deletion is successfull, otherwise false</returns>
        Task<bool> DeletePerson(Guid? PersonId);
      
    }
}
