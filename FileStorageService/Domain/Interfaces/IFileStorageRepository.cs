using FileStorageService.Domain.Entities;

namespace FileStorageService.Domain.Interfaces;

public interface IFileStorageRepository
{
    FileHolder? Get(Guid id);
    FileHolder? GetByHash(int hash);
    Guid Add(FileHolder fileHolder);
}