public class LocalFileStorageService
{
    private readonly string _uploadDirectory;

    public LocalFileStorageService(string uploadDirectory)
    {
        _uploadDirectory = uploadDirectory;
    }

    public async Task SaveAsync(
        Stream fileStream,
        string fileName)
    {
        var filePath = Path.Combine(
            _uploadDirectory,
            fileName);

        await using var outputStream = File.Create(filePath);

        await fileStream.CopyToAsync(outputStream);
    }

    public Stream OpenRead(string fileName)
    {
        var filePath = Path.Combine(
            _uploadDirectory,
            fileName);

        return File.OpenRead(filePath);
    }

    public Task Remove(string fileName)
    {
        var filePath = Path.Combine(_uploadDirectory, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}