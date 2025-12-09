using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Web.Admin.ViewModels;

namespace Web.Admin.Services;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    private JsonSerializerOptions JsonOptions => new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    // GET genérico
    public async Task<T?> GetAsync<T>(string url)
    {
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return default;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    // POST genérico
    public async Task<bool> PostAsync<T>(string url, T data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync(url, content);
        return response.IsSuccessStatusCode;
    }

    // POST que devuelve un objeto (para IA: ai/query)
    public async Task<TResponse?> PostForResultAsync<TResponse>(string url, object data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(result, JsonOptions);
    }

    // PUT genérico
    public async Task<bool> PutAsync<T>(string url, T data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PutAsync(url, content);
        return response.IsSuccessStatusCode;
    }

    // DELETE genérico
    public async Task<bool> DeleteAsync(string url)
    {
        var response = await _http.DeleteAsync(url);
        return response.IsSuccessStatusCode;
    }

    // IMPORTACIÓN DE EMPLEADOS (Excel)
    public async Task<bool> ImportEmployees(IFormFile file)
    {
        using var form = new MultipartFormDataContent();
        using var stream = file.OpenReadStream();

        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        form.Add(fileContent, "file", file.FileName);

        var response = await _http.PostAsync("admin/import", form);
        return response.IsSuccessStatusCode;
    }

    // LISTA DE EMPLEADOS
    public async Task<List<EmployeeVm>?> GetEmployees()
    {
        return await GetAsync<List<EmployeeVm>>("api/Employees");
    }

    // DASHBOARD MÉTRICAS
    public async Task<DashboardMetricsVm?> GetDashboardMetricsAsync()
    {
        return await GetAsync<DashboardMetricsVm>("api/dashboard/metrics");
    }
}
