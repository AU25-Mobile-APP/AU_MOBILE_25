using Microsoft.AspNetCore.Identity;

namespace SwipeFeast.API.Models;

public class ApplicationUser : IdentityUser
{
    public string FirebaseUserId { get; set; }
    public string Name { get; set; }

}