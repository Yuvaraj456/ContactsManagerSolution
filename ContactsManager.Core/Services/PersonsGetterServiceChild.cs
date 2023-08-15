using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using Serilog;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PersonsGetterServiceChild : PersonsGetterService
    {        

        public PersonsGetterServiceChild(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext) : base(personsRepository,logger,diagnosticContext)
        {   
            
        }

        public override async Task<MemoryStream> GetPersonsExcel()
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
