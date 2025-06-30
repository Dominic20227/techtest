using UserManagement.Common.UserModels;

namespace UserManagement.Common.LogsModels;
public class LogsModel : BaseModel
{
    public string? LogType { get; set; }
    public string? LogMessage { get; set; }
    public DateTime DateAndTime { get; set; }
    public long UserId { get; set; }
    }
