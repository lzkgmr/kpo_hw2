using FileStorageService.Application.DTO;
using FileStorageService.Domain.Entities;
using FileStorageService.Domain.Interfaces;

namespace FileStorageService.Application.Services;

public class FileStorageService(
    IFileStorageRepository repository,
    IWebHostEnvironment env)
    : IFileStorageService
{
    public FileHolder? GetFile(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Invalid file ID.");
        Console.WriteLine(repository.Get(id)?.FileDirectory);
        return repository.Get(id);
    }

    public bool HashExists(int hash)
    {
        var file = repository.GetByHash(hash);
        return file != null;
    }

    public Guid GetFileIdByHash(int hash)
    {
        var file = repository.GetByHash(hash);
        return file.Id;
    }

    public Guid AddFile(FileUploadDto fileDto, int hash)
    {
        var existingFile = repository.GetByHash(hash);
        if (existingFile != null)
        {
            return existingFile.Id;
        }
        
        var uploadsPath = Path.Combine(env.ContentRootPath, "uploads");
        Directory.CreateDirectory(uploadsPath);
        var filePath = Path.Combine(uploadsPath, $"{Guid.NewGuid()}.txt");
        
        File.WriteAllText(filePath, fileDto.Content);
        
        var newFile = new FileHolder(
            id: Guid.NewGuid(),
            fileName: fileDto.FileName!,
            hash: hash, 
            fileDirectory: filePath
        );
        repository.Add(newFile);
        return newFile.Id;
    }
    
    public async Task<byte[]> GetFileContentAsync(FileHolder fileHolder)
    {
        return await File.ReadAllBytesAsync(fileHolder.FileDirectory);
    }
}