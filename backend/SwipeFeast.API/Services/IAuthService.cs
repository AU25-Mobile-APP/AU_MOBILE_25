using Google.Cloud.Firestore;
using SwipeFeast.API.Models;

namespace SwipeFeast.API.Services.Exceptions;

public interface IAuthService
{
    public Task<DocumentSnapshot> Register(RegisterDto registerDto);
}