using System.ComponentModel.DataAnnotations;

namespace UserManagement.Common.UserModels;

public class UserModel : BaseModel
{
    public long Id { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? Forename { get; set; }
    public string? Surname { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public List<string>? Logs { get; set; } = new List<string>();
}
