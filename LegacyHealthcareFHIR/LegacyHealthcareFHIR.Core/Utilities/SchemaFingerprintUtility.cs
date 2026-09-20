using System.Security.Cryptography;
using System.Text;

namespace LegacyHealthcareFHIR.Core.Utilities;

public static class SchemaFingerprintUtility
{
    public static string GenerateFromHeaders(List<string> headers)
    {
        var normalizedHeaders = headers.Select(x => x.Trim().ToLowerInvariant()).OrderBy(x => x);
        var schema = string.Join("|", normalizedHeaders);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(schema));
        return Convert.ToHexString(bytes);
    }
}