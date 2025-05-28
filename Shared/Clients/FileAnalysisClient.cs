using System.Net;
using System.Net.Http.Json;
using Shared.DTOs;

namespace Shared.Clients;

public class FileAnalysisClient(HttpClient httpClient) : IFileAnalysisClient
{
    public async Task<FileResult?> GetStatistics(Guid fileId)
    {
        var resp = await httpClient.GetAsync($"/api/analysis/{fileId}");
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<FileResult>();
    }

    public async Task<int> AnalyzeHash(string content)
    {
        var response = await httpClient.GetAsync($"api/analysis/GetHash/{content}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }
}