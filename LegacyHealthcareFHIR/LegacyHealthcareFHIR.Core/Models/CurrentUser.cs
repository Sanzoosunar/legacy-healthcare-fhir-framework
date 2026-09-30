namespace LegacyHealthcareFHIR.Core.Models;
public class CurrentUser
{
    private int? _hospitalId;
    private int? _userId;

    public string Username { get; private set; } = string.Empty;
    public string HospitalName { get; private set; } = string.Empty;

    public void SetHospitalId(int hospitalId)
    {
        _hospitalId = hospitalId;
    }

    public void SetUserId(int userId)
    {
        _userId = userId;
    }

    public void SetUsername(string username)
    {
        Username = username;
    }

    public void SetHospitalName(string hospitalName)
    {
        HospitalName = hospitalName;
    }

    public int UserId => _userId ?? throw new UnauthorizedAccessException("No authenticated user.");
    public int HospitalId => _hospitalId ?? throw new UnauthorizedAccessException("No authenticated user.");
}
