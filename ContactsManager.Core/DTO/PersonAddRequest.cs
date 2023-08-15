using ContactsManager.Core.Domain.Entities;
using ServiceContracts.Enums;   
using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    public class PersonAddRequest
    {
        [Required( ErrorMessage ="PersonName Cannot be blank")]
        public string? PersonName { get; set; }
   
        [Required(ErrorMessage = "Email Cannot be blank")]
        [EmailAddress(ErrorMessage ="Email should be valid")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required(ErrorMessage ="Date Of Birth is Required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is Required")]
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage = "Country is Required")]
        public Guid? CountryId { get; set; }

        [Required(ErrorMessage = "Address is Required")]
        public string? Address { get; set; }

  
        public bool ReceiveNewsLetters { get; set; }

        /// <summary>
        /// Converting PersonAddRequest object to Person Object
        /// </summary>
        /// <returns></returns>
        public Person ToPerson()
        {
            return new Person()
            {
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
