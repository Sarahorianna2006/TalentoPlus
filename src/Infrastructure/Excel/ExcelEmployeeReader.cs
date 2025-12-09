using Application.Interfaces;
using Application.Models;
using ClosedXML.Excel;

namespace Infrastructure.Excel;

public class ExcelEmployeeReader : IExcelEmployeeReader
{
    public async Task<List<ExcelEmployeeRow>> Read(Stream stream)
    {
        return await Task.Run(() =>
        {
            var rows = new List<ExcelEmployeeRow>();

            using var workbook = new XLWorkbook(stream);
            var sheet = workbook.Worksheet(1);

            var lastRow = sheet.LastRowUsed().RowNumber();

            for (int row = 2; row <= lastRow; row++)
            {
                var r = new ExcelEmployeeRow();

                try
                {
                    r.Document  = sheet.Cell(row, 1).GetString().Trim();
                    r.Name      = sheet.Cell(row, 2).GetString().Trim();
                    r.LastName  = sheet.Cell(row, 3).GetString().Trim();
                    r.DateOfBirth = sheet.Cell(row, 4).GetString().Trim();
                    r.Address   = sheet.Cell(row, 5).GetString().Trim();
                    r.Phone     = sheet.Cell(row, 6).GetString().Trim();
                    r.Email     = sheet.Cell(row, 7).GetString().Trim();
                    r.Post      = sheet.Cell(row, 8).GetString().Trim();

                    // SALARIO (puede venir como texto)
                    double salaryParsed;
                    r.Salary = double.TryParse(sheet.Cell(row, 9).GetString(), out salaryParsed)
                        ? salaryParsed
                        : 0;

                    // FECHA INGRESO (puede venir como texto o numérico)
                    DateTime entryDateParsed;
                    if (sheet.Cell(row, 10).DataType == XLDataType.DateTime)
                    {
                        r.EntryDate = sheet.Cell(row, 10).GetDateTime();
                    }
                    else if (DateTime.TryParse(sheet.Cell(row, 10).GetString(), out entryDateParsed))
                    {
                        r.EntryDate = entryDateParsed;
                    }
                    else
                    {
                        r.EntryDate = DateTime.MinValue;
                        r.Error = "FechaIngreso inválida";
                    }

                    r.State               = sheet.Cell(row,11).GetString().Trim();
                    r.EducationalLevel    = sheet.Cell(row,12).GetString().Trim();
                    r.ProfessionalProfile = sheet.Cell(row,13).GetString().Trim();
                    r.Department          = sheet.Cell(row,14).GetString().Trim();

                    // Validaciones
                    if (string.IsNullOrWhiteSpace(r.Document))
                        r.Error = "Documento vacío";
                    else if (string.IsNullOrWhiteSpace(r.Name))
                        r.Error = "Nombres vacío";
                    else if (string.IsNullOrWhiteSpace(r.LastName))
                        r.Error = "Apellidos vacío";
                    else if (string.IsNullOrWhiteSpace(r.Email))
                        r.Error = "Email vacío";
                }
                catch (Exception ex)
                {
                    r.Error = $"Error leyendo fila {row}: {ex.Message}";
                }

                rows.Add(r);
            }

            return rows;
        });
    }
}
