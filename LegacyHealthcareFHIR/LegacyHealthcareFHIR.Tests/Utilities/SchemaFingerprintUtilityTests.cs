using LegacyHealthcareFHIR.Core.Utilities;

namespace LegacyHealthcareFHIR.Tests.Utilities;

public class SchemaFingerprintUtilityTests
{
    [Fact]
    public void GenerateFromHeaders_SameHeadersDifferentOrder_ReturnsSameFingerprint()
    {
        var headers1 = new List<string>
        {
            "P_ID",
            "F_NAME",
            "L_NAME",
            "DOB",
            "SEX"
        };

        var headers2 = new List<string>
        {
            "SEX",
            "DOB",
            "L_NAME",
            "F_NAME",
            "P_ID"
        };

        var fingerprint1 = SchemaFingerprintUtility.GenerateFromHeaders(headers1);
        var fingerprint2 = SchemaFingerprintUtility.GenerateFromHeaders(headers2);

        Assert.Equal(fingerprint1, fingerprint2);
    }
}