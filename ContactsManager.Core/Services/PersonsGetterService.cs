
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
    public class PersonsGetterService : IPersonsGetterService
    {
        private readonly IPersonsRepository _personRepository;
  
        private readonly ILogger<PersonsGetterService> _logger;

        private readonly IDiagnosticContext _diagnosticContext;
                     
        public PersonsGetterService(IPersonsRepository personRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personRepository = personRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }     

        public virtual async Task<List<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonService");
            //return _db.Persons.ToList() //first fetch data from db then convert the result to person response
            //    .Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
            List<Person> people = await _personRepository.GetAllPersons();

            return people.Select(temp => temp.ToPersonResponse()).ToList();

        }

        public virtual async Task<PersonResponse?> GetPersonById(Guid? guid)
        {
            if (guid == null)
                return null;

            Person? person = await _personRepository.GetPersonById(guid);

            if (person == null)
                return null;

            return person.ToPersonResponse();


        }

        public virtual async Task<List<PersonResponse>> GetFilteredPersons(string searchby, string? searchString)
        {
            List<Person> Persons;
            _logger.LogInformation("GetFilteredPersons of PersonService");
            using (Operation.Time("Time for filtered Persons from database"))
            { 
                 Persons = searchby switch
                {
                    nameof(PersonResponse.PersonName) =>
                     await _personRepository.GetFilteredPersons(x => x.PersonName.Contains(searchString)),


                    nameof(PersonResponse.Email) =>
                      await _personRepository.GetFilteredPersons(x => x.Email.Contains(searchString)),

                    nameof(PersonResponse.DateOfBirth) =>
                        await _personRepository.GetFilteredPersons(x => x.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),

                    nameof(PersonResponse.Gender) =>
                        await _personRepository.GetFilteredPersons(x => x.Gender.Contains(searchString)),

                    nameof(PersonResponse.CountryId) =>
                        await _personRepository.GetFilteredPersons(x => x.Country.CountryName.Contains(searchString)),

                    nameof(PersonResponse.Address) =>
                         await _personRepository.GetFilteredPersons(x => x.Address.Contains(searchString)),

                    _ => await _personRepository.GetAllPersons()

                };
            }
            _diagnosticContext.Set("Persons", Persons);
            return Persons.Select(x=>x.ToPersonResponse()).ToList();
        }
             

        public virtual async Task<MemoryStream> GetAllPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();

            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);

            CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);

            //PersonName, Email,DateOfBirth,...
            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.Country));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
            csvWriter.NextRecord();

            List<Person> persons = await _personRepository.GetAllPersons();

            List<PersonResponse> personlist = persons.Select(temp=>temp.ToPersonResponse()).ToList();

            foreach(PersonResponse person in personlist)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                if(person.DateOfBirth.HasValue)
                {
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
                }     
                else
                {
                    csvWriter.WriteField("");
                }
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Gender);
                csvWriter.WriteField(person.Country);
                csvWriter.WriteField(person.Address);
                csvWriter.WriteField(person.ReceiveNewsLetters);
                csvWriter.NextRecord();
                csvWriter.Flush();
            }
            

            memoryStream.Position = 0;

            return memoryStream;

        }

        public virtual async Task<MemoryStream> GetPersonsExcel()
        {
                MemoryStream memoryStream = new MemoryStream();

                using(ExcelPackage excelpackage = new ExcelPackage(memoryStream))
                {
                    ExcelWorksheet worksheet = excelpackage.Workbook.Worksheets.Add("PersonsList");

                    worksheet.Cells["A1"].Value= "PersonName";
                    worksheet.Cells["B1"].Value = "Email";
                    worksheet.Cells["C1"].Value = "Date Of Birth";
                    worksheet.Cells["D1"].Value = "Age";
                    worksheet.Cells["E1"].Value = "Gender";
                    worksheet.Cells["F1"].Value = "Country";
                    worksheet.Cells["G1"].Value = "Address";
                    worksheet.Cells["H1"].Value = "Receive News Letter";

                    using(ExcelRange headercells = worksheet.Cells["A1:H1"])
                    {
                        headercells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        headercells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        headercells.Style.Font.Bold = true;
                    }

                    int row = 2;
                    List<Person> persons = await _personRepository.GetAllPersons();

                    List<PersonResponse> personslist = persons.Select(temp => temp.ToPersonResponse()).ToList();

                    foreach(PersonResponse person in personslist)
                    {
                        worksheet.Cells[row,1].Value = person.PersonName;
                        worksheet.Cells[row,2].Value = person.Email;
                        if(person.DateOfBirth.HasValue)
                        {
                            worksheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                        }                    
                        worksheet.Cells[row,4].Value = person.Age;
                        worksheet.Cells[row,5].Value = person.Gender;
                        worksheet.Cells[row,6].Value = person.Country;
                        worksheet.Cells[row,7].Value = person.Address;
                        worksheet.Cells[row,8].Value = person.ReceiveNewsLetters;
                        row++;
                    }

                    worksheet.Cells[$"A1:H{row}"].AutoFitColumns();

                    await excelpackage.SaveAsync();
                }

                memoryStream.Position = 0;

                return memoryStream;
        }
    }
}
