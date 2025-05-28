namespace Shared.Clients;

public interface IFileStorageClient
{
    Task<string> GetFileContent(Guid fileId);
} 