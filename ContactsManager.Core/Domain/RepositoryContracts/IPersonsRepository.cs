using ContactsManager.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts
{
    /// <summary>
    /// managing data access logic for managing person entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// add a new person to the datastore
        /// </summary>
        /// <param name="person">receiving object of new person</param>
        /// <returns>return the person object after added in to data store</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// return all the persons in the data store
        /// </summary>      
        /// <returns>return all the persons in the data store</returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// Returns a person object based on the given person id
        /// </summary>
        /// <param name="personId">Personid to filter the person in the table</param>
        /// <returns>return person filtered by personid</returns>
        Task<Person?> GetPersonById(Guid? personId);

        /// <summary>
        /// return all the person object based on the given expression
        /// </summary>
        /// <param name="predicate">Linq expression to check</param>
        /// <returns>All matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person,bool>> predicate); //Linq expression

        /// <summary>
        /// Delete specific person based on the personid
        /// </summary>
        /// <param name="personId">personid to search person</param>
        /// <returns>return true if person deleted, otherwise false</returns>
        Task<bool> DeletePersonById(Guid? personId);

        /// <summary>
        /// Updates a person object based on the person object
        /// </summary>
        /// <param name="person">person obejct for update</param>
        /// <returns>return the updated person object </returns>
        Task<Person> UpdatePerson(Person person);

    }
}
