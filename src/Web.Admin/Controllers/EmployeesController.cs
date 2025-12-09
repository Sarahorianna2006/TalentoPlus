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

    // GET EDIT
    [HttpGet("Employees/{document}")]
    public async Task<IActionResult> GetEmployeeByDocument(string document)
    {
        // Usar ApiClient para obtener el empleado por documento
        var employee = await _api.GetAsync<EmployeeVm>($"api/Employees/{document}");

        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // POST EDIT
    [HttpPost]
    public async Task<IActionResult> Edit(string document, EmployeeVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        // Enviar el objeto EmployeeVm para actualizar a través de la API
        await _api.PutAsync($"Employees/{document}", vm);

        return RedirectToAction(nameof(Index));
    }

    // DELETE
    [HttpPost]
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
