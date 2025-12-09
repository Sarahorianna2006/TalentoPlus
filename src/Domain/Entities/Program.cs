namespace Domain.Entities;

public class Program
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Code { get; private set; }
    public string Name { get; private set; }

    //[JsonIgnore]
    public ICollection<Employee> Employees { get; private set; }

    public Program(string code, string name)
    {
        Code = code;
        Name = name;
        Employees = new List<Employee>();
    }
}