using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Infrastructure.Csv;
using LegacyHealthcareFHIR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LegacyHealthcareFHIR.Infrastructure.Services;

public class SourceFileService
{
    private readonly LocalFileStorageService _fileStorage;
    private readonly LegacyCsvReader _csvReader;
    private readonly AppDbContext _dbContext;

    public SourceFileService(
        LocalFileStorageService fileStorage,
        LegacyCsvReader csvReader,
        AppDbContext dbContext)
    {
        _fileStorage = fileStorage;
        _csvReader = csvReader;
        _dbContext = dbContext;
    }

    public SourceFileData Read(string storedFileName)
    {
        using var stream = _fileStorage.OpenRead(storedFileName);

        var result = _csvReader.Read(stream);

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException("Failed to read source file.");
        }      

        var sampleRecords = result.Records.Take(5).ToList();
        var headers = result.Records[0].Fields.Keys.ToList();

        return new SourceFileData
        {
            Headers = headers,
            SampleRecords = sampleRecords
        };
    }

    public async Task<SourceFileData> ReadAndSaveAsync(Guid importJobId, string storedFileName)
    {
        var sourceFileData = Read(storedFileName);
        sourceFileData.ImportJobId = importJobId;

        _dbContext.SourceFileData.Add(sourceFileData);
        await _dbContext.SaveChangesAsync();

        return sourceFileData;
    }

    public async Task<SourceFileData> GetOrCreate(Guid importJobId, string storedFileName)
    {
        var sourceFileData = await _dbContext.SourceFileData.FirstOrDefaultAsync(x => x.ImportJobId == importJobId);

        if (sourceFileData == null)
        {
            sourceFileData = await ReadAndSaveAsync(importJobId, storedFileName);
        }

        return sourceFileData;
    }

}