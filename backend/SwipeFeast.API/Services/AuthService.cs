using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using SwipeFeast.API.Models;
using SwipeFeast.API.Services.Exceptions;

namespace SwipeFeast.API.Services;

public class AuthService : IAuthService
{
    private readonly FirestoreDb _firestore;
    private readonly FirebaseAuth _auth;
    public AuthService(FirestoreDb firestore, FirebaseAuth auth)
    {
        _firestore = firestore ?? throw new ArgumentNullException(nameof(firestore));
        _auth = auth ?? throw new ArgumentNullException(nameof(auth));
    }

    public async Task<DocumentSnapshot> Register(RegisterDto registerDto)
    {
        var test = await _firestore.Collection("users").Document().SetAsync(new
        {
            FirstName = "Fritz", LastName = "Bühler", MailAddress = "hallo@welt.com", uuid = "079",
            FavoriteDish = "Omeletten."
        });
        return await _firestore.Collection("users").Document("XPfs5HqPprvN5EPVwrc1").GetSnapshotAsync();


    }

}