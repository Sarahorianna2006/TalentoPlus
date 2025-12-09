using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Admin.Services;
using Web.Admin.ViewModels;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApiClient _api;

    public DashboardController(ApiClient api)
    {
        _api = api;
    }

    public async Task<IActionResult> Index()
    {
        var metrics = await _api.GetDashboardMetricsAsync();
        return View(metrics ?? new DashboardMetricsVm());
    }

    [HttpPost]
    public async Task<IActionResult> Ask(string question)
    {
        var response = await _api.PostForResultAsync<AiResultVm>(
            "ai/query",
            new AiQuestionVm { Question = question }
        );

        ViewBag.Answer = response?.Answer ?? "No se pudo obtener respuesta.";

        var metrics = await _api.GetDashboardMetricsAsync();
        return View("Index", metrics);
    }
}
