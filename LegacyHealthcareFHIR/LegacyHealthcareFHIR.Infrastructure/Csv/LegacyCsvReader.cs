using System.Globalization;
using CsvHelper;
using LegacyHealthcareFHIR.Core.Models.Legacy;

namespace LegacyHealthcareFHIR.Infrastructure.Csv;

public class LegacyCsvReader
{
    public CsvReadResult Read(Stream stream)
    {
        try
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            if (!csv.Read())
            {
                return new CsvReadResult
                {
                    IsSuccess = false,
                    Errors =
                    [
                        new CsvReadError
                        {
                            ErrorType = CsvReadErrorType.EmptyFile
                        }
                    ]
                };
            }

            csv.ReadHeader();

            var headers = csv.HeaderRecord!;

            var records = new List<LegacyRecord>();

            while (csv.Read())
            {
                var record = new LegacyRecord();

                foreach (var header in headers)
                {
                    record.Fields[header] = csv.GetField(header);
                }

                records.Add(record);
            }

            if (records.Count == 0)
                throw new InvalidOperationException("Invalid CSV");

            return new CsvReadResult
            {
                IsSuccess = true,
                Records = records
            };
        }
        catch (CsvHelperException ex)
        {
            return new CsvReadResult
            {
                IsSuccess = false,
                Errors =
                [
                    new CsvReadError
                    {
                        ErrorType = CsvReadErrorType.InvalidCsv,
                        Details = ex.Message
                    }
                ]
            };
        }
    }
}