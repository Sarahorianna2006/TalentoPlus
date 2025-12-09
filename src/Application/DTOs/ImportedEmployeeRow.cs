namespace Application.DTOs;

public class ImportedEmployeeRow
{
    public string DocumentNumber { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string ProgramCode { get; set; } = ""; // ?
    public string Error { get; set; } = "";
}