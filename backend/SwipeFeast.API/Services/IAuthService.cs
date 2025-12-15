using Google.Cloud.Firestore;
using SwipeFeast.API.Models;

namespace SwipeFeast.API.Services.Exceptions;

public interface IAuthService
{
    public Task CreateUserIfNotExistsAsync(RegisterDto authDto, CancellationToken cancellationToken);
    public Task<RegisterDto> GetUserByUidAsync(string userUid);
    public Task<RegisterDto> PatchUser(RegisterDto authDto);
    public Task<GroupIDDto> GetGroupIdByUIDAsync(string userUid);
    public Task<GroupIDDto> PatchGroupIdByUIDAsync(GroupIDDto authDto);
    public Task CreateGroupIdIfNotExistsAsync(GroupIDDto dto, CancellationToken cancellationToken);
}