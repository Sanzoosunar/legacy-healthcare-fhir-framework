using LegacyHealthcareFHIR.Core.Models.Legacy;
using LegacyHealthcareFHIR.Infrastructure.Csv;
using LegacyHealthcareFHIR.Tests.Helpers;

namespace LegacyHealthcareFHIR.Tests.Csv;

public class LegacyCsvReaderTests
{
    [Fact]
    public void Read_ValidCsv_ReturnsDynamicLegacyRecords()
    {
        // Arrange
        var csvContent =
            """
            PAT_ID,FIRST_NM,LAST_NM,DOB_DT,SEX_CD
            1001,John,Smith,1985-04-12,M
            1002,Mary,Jones,1990-08-25,F
            """;

        using var stream = TestStreamHelper.CreateStream(csvContent);

        var csvReader = new LegacyCsvReader();

        // Act
        var result = csvReader.Read(stream);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);

        Assert.Equal(2, result.Records.Count);

        Assert.Equal("1001", result.Records[0].Fields["PAT_ID"]);
        Assert.Equal("John", result.Records[0].Fields["FIRST_NM"]);
        Assert.Equal("Smith", result.Records[0].Fields["LAST_NM"]);

        Assert.Equal("1002", result.Records[1].Fields["PAT_ID"]);
        Assert.Equal("Mary", result.Records[1].Fields["FIRST_NM"]);
    }

    [Fact]
    public void Read_EmptyCsv_ReturnsEmptyFileError()
    {
        // Arrange
        using var stream = TestStreamHelper.CreateStream("");

        var csvReader = new LegacyCsvReader();

        // Act
        var result = csvReader.Read(stream);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Empty(result.Records);

        Assert.Single(result.Errors);

        Assert.Equal(
            CsvReadErrorType.EmptyFile,
            result.Errors[0].ErrorType);
    }

    [Fact]
    public void Read_MalformedCsv_ReturnsInvalidCsvError()
    {
        // Arrange
        var csvContent =
            """
        PAT_ID,FIRST_NM,LAST_NM
        1001,"John,Smith
        """;

        using var stream = TestStreamHelper.CreateStream(csvContent);

        var csvReader = new LegacyCsvReader();

        // Act
        var result = csvReader.Read(stream);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Empty(result.Records);

        Assert.Single(result.Errors);

        Assert.Equal(
            CsvReadErrorType.InvalidCsv,
            result.Errors[0].ErrorType);
    }
}