using System.Collections.Generic;
using System.IO;
using Chronos.DTOs;
using Chronos.Interfaces;
using Chronos.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;

namespace Chronos.Services
{
    public class FileService : IFileService
    {
        public IEnumerable<EmployeeModel> ReadFile(IFormFile file)
        {
            var listEmployees = new List<EmployeeModel>();

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed();

                    int registration = 0;
                    string role = string.Empty;
                    string employeeName = string.Empty;
                    string totals = string.Empty;

                    foreach (var row in rows)
                    {
                        foreach (var cell in row.Cells())
                        {
                            var value = cell.GetString().Trim();

                            if (value.Contains("Matrícula"))
                            {
                                var nextRow = worksheet.Row(row.RowNumber() + 1);
                                var registrationStr = nextRow.Cell(cell.Address.ColumnNumber).GetString().Trim();
                                int.TryParse(registrationStr, out registration);
                            }

                            if (value.Contains("Colaborador"))
                            {
                                employeeName = row.Cell(2).GetString().Trim();
                            }

                            if (value.Contains("Cargo"))
                            {
                                var nextRow = worksheet.Row(row.RowNumber() + 1);
                                role = nextRow.Cell(cell.Address.ColumnNumber).GetString().Trim();
                            }

                            if (value.Contains("TOTAIS"))
                            {
                                totals = row.Cell(3).GetString().Trim();

                                if (!string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(employeeName))
                                {
                                    listEmployees.Add(new EmployeeModel
                                    {
                                        Registration = registration,
                                        Name = employeeName,
                                        Role = role,
                                        TotalHours = totals
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return listEmployees;
        }
    }
}
