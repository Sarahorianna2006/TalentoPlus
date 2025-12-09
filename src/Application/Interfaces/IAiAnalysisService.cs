using Domain.Repositories;

namespace Application.Interfaces;

public interface IAiAnalysisService
{
    // IA interpreta la pregunta del usuario
    Task<string> AnalyzeQuestion(string question);

    // Ejecuta la instrucción sobre la base de datos real
    Task<string> ResolveInstruction(string instruction, IEmployeeRepository repo);
    Task<string> AskAsync(string question, IEmployeeRepository repo);
}