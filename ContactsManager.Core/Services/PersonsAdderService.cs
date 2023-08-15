
using CsvHelper;
using CsvHelper.Configuration;
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
using ContactsManager.Core.Domain.Entities;

namespace Services
{
    public class PersonsAdderService : IPersonsAdderService
    {
        private readonly IPersonsRepository _personRepository;
  
        private readonly ILogger<PersonsAdderService> _logger;

        private readonly IDiagnosticContext _diagnosticContext;
                     
        public PersonsAdderService(IPersonsRepository personRepository, ILogger<PersonsAdderService> logger, IDiagnosticContext diagnosticContext)
        {
            _personRepository = personRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null) throw new ArgumentNullException(nameof(personAddRequest));

            //Model Validation
            ValidationHelper.ModelValidation(personAddRequest);

            //Convert Personaddrequest into person
            Person person = personAddRequest.ToPerson();

            //Generate new guidid
            person.PersonId = Guid.NewGuid();

            await _personRepository.AddPerson(person);
          
            //_db.Sp_InsertPerson(person);

            //convert person to PersonResponse type

            return  person.ToPersonResponse();

        }

       
    }
}
