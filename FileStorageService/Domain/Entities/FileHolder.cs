namespace FileStorageService.Domain.Entities;

public class FileHolder(Guid id, string fileName, int hash, string fileDirectory)
{
    public Guid Id { get; init; } = id;
    public string FileName { get; init; } = fileName;
    public int Hash { get; init; } = hash;
    public string FileDirectory { get; init; } = fileDirectory;
}