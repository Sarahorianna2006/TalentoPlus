namespace Web.Admin.ViewModels;

public class DashboardMetricsVm
{
    public int Total { get; set; }
    public int Activos { get; set; }
    public List<ProgramCountVm> PorPrograma { get; set; } = new();
}

public class ProgramCountVm
{
    public string Program { get; set; } = "";
    public int Total { get; set; }
}