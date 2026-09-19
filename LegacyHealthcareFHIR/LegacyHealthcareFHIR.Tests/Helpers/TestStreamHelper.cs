using System.Text;

namespace LegacyHealthcareFHIR.Tests.Helpers;

public static class TestStreamHelper
{
    public static MemoryStream CreateStream(string content)
    {
        return new MemoryStream(
            Encoding.UTF8.GetBytes(content));
    }
}