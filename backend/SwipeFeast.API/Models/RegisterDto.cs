using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace SwipeFeast.API.Models;

/// <summary>
/// Represents an input model for registering a user, including first- lastname, mailaddress
/// </summary>
public class RegisterDto
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }
    
    [Required]
    [StringLength(50)]
    public string LastName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [StringLength(50)]
    public string FavoriteDish { get; set; }
    
}