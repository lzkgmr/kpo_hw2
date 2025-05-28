using FileAnalysisService.Domain.Entities;
using FileAnalysisService.Domain.Interfaces;

namespace FileAnalysisService.Infrastructure.Repositories;

public class FileAnalysisRepository(FileAnalysisDbContext context) : IFileAnalysisRepository
{
    public FileDataHolder? Get(Guid id)
    {
        return context.FilesData.FirstOrDefault(f => f.FileId == id);
    }
    public bool Exists(Guid id)
    {
        return context.FilesData.Any(f => f.FileId == id);
    }

    public FileDataHolder? Delete(Guid id)
    {
        var file = Get(id);
        context.FilesData.Remove(file);
        context.SaveChanges();
        return file;
    }

    public void Add(FileDataHolder? file)
    {
        context.FilesData.Add(file);
        context.SaveChanges();
    }
}