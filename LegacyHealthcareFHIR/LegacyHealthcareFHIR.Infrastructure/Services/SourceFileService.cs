using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LegacyHealthcareFHIR.Infrastructure.Services;

public class SourceFileService
{
    private FileReadingService _fileReadingService;
    private readonly AppDbContext _dbContext;

    public SourceFileService(FileReadingService fileReadingService,
        AppDbContext dbContext)
    {
        _fileReadingService = fileReadingService;
        _dbContext = dbContext;
    }

    public SourceFileData Read(string storedFileName,string extension)
    {
        var result = _fileReadingService.Read(storedFileName, extension);
        if (!result.Success)
            throw new Exception("Failed to read source file.");

        return new SourceFileData
        {
            Headers = result.Headers,
            SampleRecords = result.Records.Take(5).ToList()
        };
    }
    public async Task<SourceFileData> ReadAndSaveAsync(Guid importJobId, string storedFileName,string extension)
    {
        var sourceFileData = Read(storedFileName, extension);
        sourceFileData.ImportJobId = importJobId;

        _dbContext.SourceFileData.Add(sourceFileData);
        await _dbContext.SaveChangesAsync();

        return sourceFileData;
    }

    public async Task<SourceFileData> GetOrCreate(Guid importJobId, string storedFileName, string fileExtension)
    {
        var sourceFileData = await _dbContext.SourceFileData.FirstOrDefaultAsync(x => x.ImportJobId == importJobId);

        if (sourceFileData == null)
        {
            sourceFileData = await ReadAndSaveAsync(importJobId, storedFileName, fileExtension);
        }

        return sourceFileData;
    }

}