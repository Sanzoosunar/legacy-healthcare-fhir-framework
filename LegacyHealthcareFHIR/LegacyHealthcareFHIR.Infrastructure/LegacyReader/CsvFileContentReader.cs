using CsvHelper;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Legacy;
using System.Globalization;

namespace LegacyHealthcareFHIR.Infrastructure.LegacyReader;

public class CsvFileContentReader : IFileContentReader
{
    public string Extension => "CSV";
    public FileReadResult Read(Stream stream)
    {
        try
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            if (!csv.Read())
            {
                return new FileReadResult
                {
                    Success = false,
                    Errors = ["File is Empty"]              
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
                return new FileReadResult
                {
                    Success = false,
                    Errors = ["Invalid CSV File"]
                };

            return new FileReadResult
            {
                Success = true,
                Records = records,
                Headers = headers.ToList()
            };
        }
        catch (CsvHelperException ex)
        {
            return new FileReadResult
            {
                Success = false,
                Errors = [ex.Message.ToString()]
            };
        }
    }
}