using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PersonalLifeManager.Models;

public class AppUser : IdentityUser
{
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
}