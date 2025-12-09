using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Application.Models;

namespace Application.UseCases;
public class ImportEmployeesUseCase
{
    private readonly IEmployeeRepository _repo;
    private readonly IExcelEmployeeReader _excel;

    public ImportEmployeesUseCase(
        IEmployeeRepository repo,
        IExcelEmployeeReader excel)
    {
        _repo = repo;
        _excel = excel;
    }

    public async Task<ImportResult> Execute(Stream excelFile)
    {
        var rows = await _excel.Read(excelFile);

        int created = 0;
        int updated = 0;
        List<string> errors = new();

        foreach (var row in rows)
        {
            // Si la fila tiene error del lector Excel
            if (!string.IsNullOrEmpty(row.Error))
            {
                errors.Add(row.Error);
                continue;
            }

            if (string.IsNullOrWhiteSpace(row.Document))
            {
                errors.Add("Documento vacío");
                continue;
            }

            var existing = await _repo.GetByDocumentAsync(row.Document);

            if (existing == null)
            {
                // Crear nuevo empleado
                var newEmployee = new Employee(
                    row.Document,
                    row.Name,
                    row.LastName,
                    row.DateOfBirth,
                    row.Address,
                    row.Phone,
                    row.Email,
                    row.Post,
                    row.Salary,
                    row.EntryDate,
                    row.State,
                    row.EducationalLevel,
                    row.ProfessionalProfile,
                    row.Department,
                    programId: null
                );

                await _repo.AddAsync(newEmployee);
                created++;
            }
            else
            {
                existing.UpdateContact(row.Email, row.Phone);
                updated++;
            }
        }

        await _repo.SaveChangesAsync();

        return new ImportResult(created, updated, errors);
    }
}

public record ImportResult(int Created, int Updated, List<string> Errors);