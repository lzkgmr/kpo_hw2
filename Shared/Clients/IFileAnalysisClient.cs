using Shared.DTOs;

namespace Shared.Clients;

public interface IFileAnalysisClient
{
    Task<int> AnalyzeHash(string content);
} 