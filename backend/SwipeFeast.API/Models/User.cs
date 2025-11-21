using System.Net.Mail;

namespace SwipeFeast.API.Models;

public class User
{
    public string uuid  { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public MailAddress Email { get; set; }
    
    public string FavoriteDish { get; set; }
}