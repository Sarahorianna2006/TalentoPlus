using Application.Models;

namespace Application.Interfaces;

public interface IExcelEmployeeReader
{
    Task<List<ExcelEmployeeRow>> Read(Stream excelStream);
    //IEnumerable<ExcelEmployeeRow> Read(Stream stream);
}