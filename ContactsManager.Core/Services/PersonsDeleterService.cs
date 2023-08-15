
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
    public class PersonsDeleterService : IPersonsDeleterService
    {
        private readonly IPersonsRepository _personRepository;
  
        private readonly ILogger<PersonsDeleterService> _logger;

        private readonly IDiagnosticContext _diagnosticContext;
                     
        public PersonsDeleterService(IPersonsRepository personRepository, ILogger<PersonsDeleterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personRepository = personRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
       
        public async Task<bool> DeletePerson(Guid? PersonId)
        {
            if (PersonId == null)
                throw new ArgumentNullException(nameof(PersonId));

            Person? person = await _personRepository.GetPersonById(PersonId);

            if (person == null)
                return false ;


            await _personRepository.DeletePersonById(PersonId);           

            return true;
        }

       
    }
}
