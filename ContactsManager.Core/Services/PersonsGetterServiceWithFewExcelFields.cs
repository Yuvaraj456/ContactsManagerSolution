using ContactsManager.Core.Domain.Entities;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PersonsGetterServiceWithFewExcelFields : IPersonsGetterService
    {
        private readonly PersonsGetterService _personsGetterService;

        private readonly IPersonsRepository _personsRepository;

        public PersonsGetterServiceWithFewExcelFields(PersonsGetterService personsGetterService, IPersonsRepository personsRepository)
        {
            _personsGetterService = personsGetterService;
            _personsRepository = personsRepository;
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
           return await _personsGetterService.GetAllPersons();
        }

        public async Task<MemoryStream> GetAllPersonsCSV()
        {
            return await _personsGetterService.GetAllPersonsCSV();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchby, string? searchString)
        {
            return await _personsGetterService.GetFilteredPersons(searchby, searchString);
        }       

        public async Task<PersonResponse?> GetPersonById(Guid? guid)
        {
            return await _personsGetterService.GetPersonById(guid);
        }

        public async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();

            using (ExcelPackage excelpackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelpackage.Workbook.Worksheets.Add("PersonsList");

                worksheet.Cells["A1"].Value = "PersonName";          
                worksheet.Cells["B1"].Value = "Age";
                worksheet.Cells["C1"].Value = "Gender";              
                using (ExcelRange headercells = worksheet.Cells["A1:C1"])
                {
                    headercells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headercells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headercells.Style.Font.Bold = true;
                }

                int row = 2;               

                List<PersonResponse> personslist = await GetAllPersons();

                foreach (PersonResponse person in personslist)
                {
                    worksheet.Cells[row, 1].Value = person.PersonName;                   
                    worksheet.Cells[row, 2].Value = person.Age;
                    worksheet.Cells[row, 3].Value = person.Gender;                 
                    row++;
                }

                worksheet.Cells[$"A1:C{row}"].AutoFitColumns();

                await excelpackage.SaveAsync();
            }

            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
