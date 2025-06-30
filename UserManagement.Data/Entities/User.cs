using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Data.Entities;

public class User : BaseEntity
{
    //[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateOnly DateOfBirth { get; set; }
    public string Forename { get; set; } = default!;
    public string Surname { get; set; } = default!;
    [EmailAddress]
    public string Email { get; set; } = default!;
    public bool IsActive { get; set; }
    public List<Logs> Logs { get; set; } = new List<Logs>();
}
