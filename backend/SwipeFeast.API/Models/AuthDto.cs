using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace SwipeFeast.API.Models;

/// <summary>
/// Represents an input model for registering a user, including first- lastname, mailaddress
/// </summary>
using Google.Cloud.Firestore;

[FirestoreData]
public class RegisterDto
{
    [FirestoreProperty]
    public string UserUID { get; set; }

    [FirestoreProperty]
    public string FirstName { get; set; }

    [FirestoreProperty]
    public string LastName { get; set; }

    [FirestoreProperty]
    public string Email { get; set; }

    [FirestoreProperty]
    public string FavoriteDish { get; set; }
}


public class GroupIDDto
{
    [FirestoreProperty]
    public string UserUID { get; set; }
    [FirestoreProperty]
    public string GroupID { get; set; }
}