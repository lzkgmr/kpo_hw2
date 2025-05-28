using FileAnalysisService.Domain.Entities;

namespace FileAnalysisService.Domain.Interfaces;

public interface IFileAnalysisService
{
    FileDataHolder AnalyzeFile(string content);
    Task<FileDataHolder?> GetFileStatistics(Guid fileId);
}