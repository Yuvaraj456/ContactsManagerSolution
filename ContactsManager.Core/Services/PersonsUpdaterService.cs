
using CsvHelper;
using CsvHelper.Configuration;
using ContactsManager.Core.Domain.Entities;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SerilogTimings;
using Exceptions;

namespace Services
{
    public class PersonsUpdaterService : IPersonsUpdaterService
    {
        private readonly IPersonsRepository _personRepository;
  
        private readonly ILogger<PersonsUpdaterService> _logger;

        private readonly IDiagnosticContext _diagnosticContext;
                     
        public PersonsUpdaterService(IPersonsRepository personRepository, ILogger<PersonsUpdaterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personRepository = personRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
       

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest));


            //Validation
            ValidationHelper.ModelValidation(personUpdateRequest);
          
            //get matching person object to update
            Person? matchingperson = await _personRepository.GetPersonById(personUpdateRequest.PersonId);

            if (matchingperson == null)
                throw new InvalidPersonIdException("Given person id doesnt valid");

            Person editreq = personUpdateRequest.ToPerson();
            ////update all details

            //matchingperson.PersonName = personUpdateRequest.PersonName;
            //matchingperson.Email = personUpdateRequest.Email;
            //matchingperson.DateOfBirth = personUpdateRequest.DateOfBirth;
            //matchingperson.Gender = personUpdateRequest.Gender.ToString();
            //matchingperson.CountryId = personUpdateRequest.CountryId;
            //matchingperson.Address = personUpdateRequest.Address;
            //matchingperson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            Person updatedPerson = await _personRepository.UpdatePerson(editreq);

            return updatedPerson.ToPersonResponse();
        }                

        
    }
}
