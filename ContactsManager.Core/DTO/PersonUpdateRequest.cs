using ContactsManager.Core.Domain.Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// represents the dto class that contains the person details to update
    /// </summary>
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage = "PersonId Cannot be blank")]
        public Guid PersonId { get; set; }

        [Required(ErrorMessage = "PersonName Cannot be blank")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Email Cannot be blank")]
        [EmailAddress(ErrorMessage = "Email should be valid")]
        public string? Email { get; set; }


        public DateTime? DateOfBirth { get; set; }

        public GenderOptions? Gender { get; set; }

        public Guid? CountryId { get; set; }

        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        /// <summary>
        /// Converting PersonAddRequest object to Person Object
        /// </summary>
        /// <returns>Returns the person object</returns>
        public Person ToPerson()
        {
            return new Person()
            {
                 PersonId = this.PersonId,
                Address = this.Address,
                DateOfBirth = this.DateOfBirth,
                Email = this.Email,
                Gender = this.Gender.ToString(),
                CountryId = this.CountryId,
                PersonName = this.PersonName,
                ReceiveNewsLetters = this.ReceiveNewsLetters

            };
        }
    }
}
