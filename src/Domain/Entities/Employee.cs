using System.Runtime.InteropServices.JavaScript;

namespace Domain.Entities;

public class Employee
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Document { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string DateOfBirth { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Post { get; set; }
    public double Salary { get; set; }
    public DateTime EntryDate { get; set; }
    public string State { get; set; }
    public string EducationalLevel { get; set; }
    public string ProfessionalProfile { get; set; }
    public string Department { get; set; }
    
    public Guid? ProgramId { get; private set; }
    public Program? Program { get; private set; }
    
    public Employee(
        string document,
        string name,
        string lastName,
        string dateOfBirth,
        string address,
        string phone,
        string email,
        string post,
        double salary,
        DateTime entryDate,
        string state,
        string educationalLevel,
        string professionalProfile,
        string department,
        Guid? programId)
    {
        Document = document;
        Name = name;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Address = address;
        Phone = phone;
        Email = email;
        Post = post;
        Salary = salary;
        EntryDate = entryDate;
        State = state;
        EducationalLevel = educationalLevel;
        ProfessionalProfile = professionalProfile;
        Department = department;
        ProgramId = programId;
    }
    
    // Método útil para actualizar datos de contacto (excel actualizaciones)
    public void UpdateContact(string email, string phone = "")
    {
        Email = email;
        if (!string.IsNullOrWhiteSpace(phone))
            Phone = phone;
    }
}