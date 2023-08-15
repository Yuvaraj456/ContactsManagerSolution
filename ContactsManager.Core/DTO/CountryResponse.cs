using ContactsManager.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used to return type for most of CountriesService methods
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }

        //It compares the current object to another
        //object of CountryResponse type and returns true, if both values are same
        //otherwise return false
        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if(obj.GetType() != typeof(CountryResponse))
                return false; 

            CountryResponse country_res = (CountryResponse)obj;

            return this.CountryId == country_res.CountryId && this.CountryName == country_res.CountryName;

        }


    }
    public static class CountryExtensions
    {
        //Country object into countryresponse object
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse { CountryId = country.CountryId, CountryName = country.CountryName };
        }
    }
}
