using LegacyHealthcareFHIR.Core.Models;

namespace LegacyHealthcareFHIR.Infrastructure.Services;
public class FileReadingService
{
    private readonly LocalFileStorageService _fileStorage;
    private ServiceFactory _factory;
    public FileReadingService(LocalFileStorageService fileStorage,
        ServiceFactory factory)
    {
        _fileStorage = fileStorage;
        _factory = factory;
    }

    public FileReadResult Read(string fileName, string extension)
    {
        using var stream = _fileStorage.OpenRead(fileName);
        var _fileReader = _factory.GetFileContentReader(extension);
        var result = _fileReader.Read(stream);
        return result;
    }
    public async Task SaveFile(Stream stream,string fileName)
    {
        await _fileStorage.SaveAsync(stream, fileName);
    }
}