namespace Application.Models;

public class ExcelEmployeeRow
{
    public string Document { get; set; } = "";
    public string Name { get; set; } = "";
    public string LastName { get; set; } = "";
    public string DateOfBirth { get; set; } = "";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Post { get; set; } = "";
    public double Salary { get; set; }
    public DateTime EntryDate { get; set; }
    public string State { get; set; } = "";
    public string EducationalLevel { get; set; } = "";
    public string ProfessionalProfile { get; set; } = "";
    public string Department { get; set; } = "";
    
    public string? Error { get; set; }
}