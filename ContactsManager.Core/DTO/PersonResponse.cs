using ContactsManager.Core.Domain.Entities;
using ServiceContracts.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTO class that is used as return type of most methods of Persons Service
    /// </summary>    
    public class PersonResponse
    {
        public Guid PersonId { get; set; }

        [Required]
        public string? PersonName { get; set; }

        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public Guid? CountryId { get; set; }

        public string? Country { get; set; }

        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        public double? Age { get; set; }

        /// <summary>
        /// Compares the current object data with the parameter object
        /// </summary>
        /// <param name="obj">the personresponse object to compare</param>
        /// <returns>True or False, indicating whether all person details are 
        /// matched with the specified parameter object</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != typeof(PersonResponse))
                return false;

            PersonResponse personResponse = (PersonResponse)obj;

            return this.PersonId == personResponse.PersonId && this.PersonName == personResponse.PersonName &&
                this.Email == personResponse.Email && this.DateOfBirth == personResponse.DateOfBirth &&
                this.Gender == personResponse.Gender && this.CountryId == personResponse.CountryId &&
                this.Country == personResponse.Country && this.ReceiveNewsLetters == personResponse.ReceiveNewsLetters;


        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Person Id:{PersonId}, Person Name:{PersonName}, Email : {Email}, " +
                $"DateOfBirth : {DateOfBirth}, Gender : {Gender}, CountryId : {CountryId}, Country : {Country}," +
                $"RecivesNewsLetters : {ReceiveNewsLetters}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonId = this.PersonId,
                PersonName = this.PersonName,
                Address = this.Address,
                CountryId = this.CountryId,
                DateOfBirth = this.DateOfBirth,
                Email = this.Email,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender!, true),
                ReceiveNewsLetters = this.ReceiveNewsLetters,
            };
        }


    }

    public static class PersonExtensions
    {
        /// <summary>
        /// An extension method to convert an object of person 
        /// class into PersonResponse class
        /// </summary>
        /// <param name="person">The person object to convert</param>
        /// <returns>returns the converted PersonResponse object</returns>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonId = person.PersonId,
                PersonName = person.PersonName,
                Address = person.Address,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryId = person.CountryId,               
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
                Country = person.Country?.CountryName

            };
        }

       



    }


}
