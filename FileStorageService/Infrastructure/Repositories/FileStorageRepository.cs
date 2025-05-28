using FileStorageService.Domain.Entities;
using FileStorageService.Domain.Interfaces;
using FileStorageService.Infrastructure.Repositories;

namespace FileStorageService.Infrastructure.Repos;

public class FileStorageRepository(FileStorageDbContext context) : IFileStorageRepository
{
    public FileHolder? Get(Guid id) => context.FileHolders.FirstOrDefault(f => f.Id == id);

    public FileHolder? GetByHash(int hash) => context.FileHolders.FirstOrDefault(f => f.Hash == hash);

    public IReadOnlyList<FileHolder>? GetAll() => context.FileHolders.ToList();

    public Guid Add(FileHolder fileHolder)
    {
        context.FileHolders.Add(fileHolder);
        context.SaveChanges();
        return fileHolder.Id;
    }

    public bool Delete(Guid id)
    {
        var file = Get(id);
        if (file == null) return false;
        
        context.FileHolders.Remove(file);
        context.SaveChanges();
        return true;
    }
}