using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Infrastructure.Csv;

namespace LegacyHealthcareFHIR.Infrastructure.Services;

public class SourceFileService
{
    private readonly LocalFileStorageService _fileStorage;
    private readonly LegacyCsvReader _csvReader;

    public SourceFileService(
        LocalFileStorageService fileStorage,
        LegacyCsvReader csvReader)
    {
        _fileStorage = fileStorage;
        _csvReader = csvReader;
    }

    public SourceFileData Read(string storedFileName)
    {
        using var stream =
            _fileStorage.OpenRead(storedFileName);

        var result =
            _csvReader.Read(stream);

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(
                "Failed to read source file.");
        }

        var sampleRecords = result.Records
            .Take(10)
            .ToList();

        var headers = result.Records
            .FirstOrDefault()?
            .Fields.Keys
            .ToList() ?? new List<string>();

        return new SourceFileData
        {
            Headers = headers,
            SampleRecords = sampleRecords
        };
    }
}