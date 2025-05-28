using System.Net.Http.Json;

namespace Shared.Clients;

public class FileStorageClient(HttpClient httpClient) : IFileStorageClient
{
    public async Task<string> GetFileContent(Guid fileId)
    {
        var response = await httpClient.GetAsync($"/api/files/storage-request/{fileId}");
        
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<Guid> Upload(Stream content, string fileName)
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StreamContent(content), "file", fileName);
        
        var response = await httpClient.PostAsync("/api/files", form);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<Guid>();
    }
}