using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Application.Interfaces;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Infrastructure.AI;

public class OpenAiService : IAiAnalysisService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public OpenAiService(IConfiguration config)
    {
        _apiKey = config["OpenAI:ApiKey"] ?? "";

        _http = new HttpClient();
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    // ============================================================
    // 1. IA interpreta la pregunta
    // ============================================================
    public async Task<string> AnalyzeQuestion(string question)
    {
        var prompt = $@"
Eres un asistente que SOLO devuelve una instrucción para ejecutar en base de datos.
Pregunta: {question}

Devuelve SOLO uno de los siguientes comandos:

- count_all
- count_active
- count_inactive
- count_auxiliares
- count_by_department:<nombre>
- count_by_program:<nombre>

Si no sabes cuál usar, devuelve: unknown
";

        var body = new
        {
            model = "gpt-4.1-mini",
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var jsonBody = JsonSerializer.Serialize(body);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync(
            "https://api.openai.com/v1/chat/completions",
            content
        );

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var text = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return text?.Trim() ?? "unknown";
    }

    // ============================================================
    // 2. Resolver instrucción en la base de datos real
    // ============================================================
    public async Task<string> ResolveInstruction(string instruction, IEmployeeRepository repo)
    {
        instruction = instruction.ToLower();

        // Convertimos SIEMPRE a List()
        var all = (await repo.GetAllAsync()).ToList();
    
        // COUNT ALL
        if (instruction == "count_all")
        {
            return $"Hay {all.Count} empleados registrados.";
        }

        // COUNT ACTIVE
        if (instruction == "count_active")
        {
            var total = all.Count(e =>
                (e.State ?? "").Equals("Activo", StringComparison.OrdinalIgnoreCase));

            return $"Hay {total} empleados activos.";
        }

        // COUNT INACTIVE
        if (instruction == "count_inactive")
        {
            var total = all.Count(e =>
                (e.State ?? "").Equals("Inactivo", StringComparison.OrdinalIgnoreCase));

            return $"Hay {total} empleados inactivos.";
        }

        // AUXILIARES
        if (instruction == "count_auxiliares")
        {
            var total = all.Count(e =>
                e.Post != null &&
                e.Post.Contains("Auxiliar", StringComparison.OrdinalIgnoreCase));

            return $"Hay {total} auxiliares en la plataforma.";
        }

        // POR DEPARTAMENTO
        if (instruction.StartsWith("count_by_department:"))
        {
            var dept = instruction.Replace("count_by_department:", "").Trim();

            var total = all.Count(e =>
                e.Department != null &&
                e.Department.Equals(dept, StringComparison.OrdinalIgnoreCase));

            return $"En el departamento '{dept}' hay {total} empleados.";
        }

        // POR PROGRAMA
        if (instruction.StartsWith("count_by_program:"))
        {
            var prog = instruction.Replace("count_by_program:", "").Trim();

            var total = all.Count(e =>
                e.Program != null &&
                e.Program.Name.Equals(prog, StringComparison.OrdinalIgnoreCase));

            return $"En el programa '{prog}' hay {total} empleados.";
        }

        return "No entendí tu pregunta. Intenta reformularla.";
    }


    // 3. Método general AskAsync usado por el controlador
    public async Task<string> AskAsync(string question, IEmployeeRepository repo)
    {
        var instruction = await AnalyzeQuestion(question);  // ✔ correcto
        var result = await ResolveInstruction(instruction, repo); // ✔ correcto
        return result;
    }
}
