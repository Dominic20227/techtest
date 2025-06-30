using System;

namespace UserManagement.Data.Entities;
public class Logs : BaseEntity
{
    public User User { get; set; } = default!;
    public long UserId { get; set; }
    public string? LogType { get; set; }
    public string? LogMessage { get; set; }
    public DateTime DateAndTime { get; set; }
}
