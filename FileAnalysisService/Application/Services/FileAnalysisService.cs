using System.Net;
using FileAnalysisService.Domain.Entities;
using FileAnalysisService.Domain.Interfaces;
using Shared.Clients;

namespace FileAnalysisService.Application.Services;

public class FileAnalysisService(IFileAnalysisRepository repository, IFileStorageClient client)
    : IFileAnalysisService
{
    public FileDataHolder AnalyzeFile(string content)
    {
        var wordCount = content.Split([' ', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries).Length;
        var paragraphCount = content.Split("\n\n").Length;
        var symbolCount = content.Length;
        var hash = wordCount * paragraphCount + symbolCount;
        
        var fileData = new FileDataHolder(
            fileId: Guid.NewGuid(),
            hash: hash,
            wordCount: wordCount,
            paragraphCount: paragraphCount,
            symbolCount: symbolCount
        );
        return fileData;
    }
    
    public async Task<FileDataHolder?> GetFileStatistics(Guid id)
    {
        if (repository.Exists(id))
            return repository.Get(id);
        
        string content;
        try
        {
            content = await client.GetFileContent(id);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        var stats = AnalyzeFile(content);
        var fileData = new FileDataHolder(id, stats.Hash, stats.WordCount, stats.ParagraphCount, stats.SymbolCount);

        repository.Add(fileData);
        return fileData;
    }
    public FileDataHolder? GetAnalysisResult(Guid fileId)
    {
        return repository.Get(fileId);
    }

    public bool DeleteAnalysisResult(Guid fileId)
    {
        return repository.Delete(fileId) != null;
    }
}
