using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Admin.Services;
using Web.Admin.ViewModels;

namespace Web.Admin.Controllers;

[Authorize]
public class EmployeesController : Controller
{
    private readonly ApiClient _api;

    public EmployeesController(ApiClient api)
    {
        _api = api;
    }

    // LISTADO
    public async Task<IActionResult> Index()
    {
        var employees = await _api.GetAsync<List<EmployeeVm>>("api/Employees");
        return View(employees ?? new List<EmployeeVm>()); // evita NullReference
    }
    
    // get create
    [HttpGet("Employees/Create")]
    public IActionResult Create()
    {
        return View();
    }
    
    // POST CREATE

    [HttpPost("Employees/Create")]
    public async Task<IActionResult> Create(EmployeeVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        // Separar FullName → Name + LastName
        var parts = vm.FullName?.Trim().Split(" ");

        var name = parts != null && parts.Length > 0 ? parts[0] : "";
        var lastName = parts != null && parts.Length > 1
            ? string.Join(" ", parts.Skip(1))
            : "";

        // Program REAL de tu BD
        var programId = Guid.Parse("9d44276f-ac17-43cb-bc67-df7dfabc5b59");

        // Construir DTO EXACTO para la API
        var dto = new
        {
            document = vm.Document,
            name = name,
            lastName = lastName,
            dateOfBirth = "2000-01-01", 
            address = "N/A",
            phone = vm.Phone,
            email = vm.Email,
            post = "Empleado",
            salary = vm.Salary,
            entryDate = vm.EntryDate,
            state = vm.State,
            educationalLevel = vm.EducationalLevel,
            professionalProfile = vm.ProfessionalProfile,
            department = vm.Department,

            programId = programId,

            program = new {
                code = "SIS",
                name = "Ingeniería de Sistemas"
            }
        };

        var ok = await _api.PostAsync("api/Employees", dto);

        if (!ok)
        {
            ModelState.AddModelError("", "No se pudo crear el empleado.");
            return View(vm);
        }

        return RedirectToAction(nameof(Index));
    }
    
    // GET EDIT
    [HttpGet("Employees/Edit/{document}")]
    public async Task<IActionResult> GetEmployeeByDocument(string document)
    {
        // Usar ApiClient para obtener el empleado por documento
        var employee = await _api.GetAsync<EmployeeVm>($"api/Employees/{document}");

        if (employee == null)
        {
            return NotFound();
        }

        return View("Create", employee);
    }

    // POST EDIT
    [HttpPost("Employees/Edit/{document}")]
    public async Task<IActionResult> Edit(string document, EmployeeVm vm)
    {
        if (!ModelState.IsValid)
            return View("Create", vm); // volver a la vista correcta

        var ok = await _api.PutAsync($"api/Employees/{document}", vm);

        if (!ok)
        {
            ModelState.AddModelError("", "No se pudo actualizar");
            return View("Create", vm);
        }

        return RedirectToAction(nameof(Index));
    }

    // DELETE
    [HttpPost("Employees/Delete/{document}")]
    public async Task<IActionResult> Delete(string document)
    {
        if (document == null)
            return NotFound();

        // Eliminar el empleado a través de la API
        await _api.DeleteAsync($"Employees/{document}");

        return RedirectToAction(nameof(Index));
    }

    // IMPORT
    public IActionResult Import()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ViewBag.Error = "Archivo inválido";
            return View();
        }

        // Importar empleados desde el archivo Excel
        await _api.ImportEmployees(file);

        return RedirectToAction(nameof(Index));
    }
}
