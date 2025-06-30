using System.ComponentModel.DataAnnotations;
using UserManagement.Common.LogsModels;

namespace UserManagement.Common.UserModels;
public class UserLogsModel : BaseModel
{
    public long Id { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? Forename { get; set; }
    public string? Surname { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
    public bool IsActive { get; set; }

    public List<LogsModel>? Logs { get; set; } = new List<LogsModel>();

    public string FullName => $"{Forename} {Surname}";
    public int TotalLogs => Logs?.Count ?? 0;
}
