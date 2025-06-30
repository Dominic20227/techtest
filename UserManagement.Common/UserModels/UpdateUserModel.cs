using System.ComponentModel.DataAnnotations;

namespace UserManagement.Common.UserModels;

public class UpdateUserModel : BaseModel
{
    [Required]
    public long Id { get; set; }

    [Required(ErrorMessage = "Date of birth is required.")]
    [Display(Name = "Date of Birth")]
    public DateOnly DateOfBirth { get; set; } = new DateOnly();
    
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
    [Display(Name = "First Name")]
    public string Forename { get; set; } = default!;
    
    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
    [Display(Name = "Last Name")]
    public string Surname { get; set; } = default!;
    
    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(100, ErrorMessage = "Email address cannot exceed 100 characters.")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = default!;
    
    [Display(Name = "Active User")]
    public bool IsActive { get; set; }
}
