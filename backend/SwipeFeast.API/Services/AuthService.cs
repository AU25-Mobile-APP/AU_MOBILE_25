using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Grpc.Core;
using SwipeFeast.API.Models;
using SwipeFeast.API.Services.Exceptions;

namespace SwipeFeast.API.Services;

/// <summary>
/// Service to handle incoming authentication requests.
/// </summary>
public class AuthService : IAuthService
{
    private readonly FirestoreDb _firestore;
    private readonly FirebaseAuth _auth;
    private readonly CollectionReference _users;
    private readonly CollectionReference _sessions;
    /// <summary>
    /// Constructor of AuthService. Creates an AuthService and handles incoming requests.
    /// </summary>
    /// <param name="firestore"></param>
    /// <param name="auth"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AuthService(FirestoreDb firestore, FirebaseAuth auth)
    {
        _firestore = firestore ?? throw new ArgumentNullException(nameof(firestore));
        _auth = auth ?? throw new ArgumentNullException(nameof(auth));
        _users = _firestore.Collection("users");
        _sessions = _firestore.Collection("sessions");
    }

    /// <summary>
    /// Takes the register information checks if the user already exists in the firestore DB and creates a new entry if not.
    /// </summary>
    /// <param name="registerDto"></param>
    /// <returns></returns>
    /// <summary>
    /// Creates a new user document if and only if it does not exist yet.
    /// Uses Firestore's atomic CreateAsync (no race conditions).
    /// </summary>
    public async Task CreateUserIfNotExistsAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.UserUID))
            throw new ArgumentException("UserUID must be provided.", nameof(dto));

        DocumentReference docRef = _users.Document(dto.UserUID);
        

        
        try
        {
            // CreateAsync fails with AlreadyExists if the document is already there.
            await docRef.CreateAsync(dto, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new UserAlreadyExistsException();
        }
    }

    public async Task<RegisterDto> GetUserByUidAsync(string userUid)
    {
        try
        {
            DocumentReference docRef = _users.Document(userUid);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
                throw new UserNotExistingException();

            return snapshot.ConvertTo<RegisterDto>();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
    
    public async Task<RegisterDto> PatchUser(RegisterDto registerDto)
    {
        try
        {
            ValidateRegisterDto(registerDto);

            if (string.IsNullOrWhiteSpace(registerDto.UserUID))
                throw new ArgumentException("UserUID must be provided for patching.", nameof(registerDto));

            DocumentReference docRef = _users.Document(registerDto.UserUID);

            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                throw new UserNotExistingException();
            }

            // Patch semantics: merge incoming fields into existing document
            await docRef.SetAsync(registerDto, SetOptions.MergeAll);

            snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                var user = snapshot.ConvertTo<RegisterDto>();
                return user;
            }
            throw new UserLoadingException();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// Creates a GroupID entry in FirestoreDB if there isn't one for the current user.
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="UserAlreadyExistsException"></exception>
    public async Task CreateGroupIdIfNotExistsAsync(GroupIDDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.UserUID))
            throw new ArgumentException("SessionID must be provided.", nameof(dto));

        DocumentReference docRef = _users.Document(dto.UserUID);
        

        
        try
        {
            // CreateAsync fails with AlreadyExists if the document is already there.
            await docRef.CreateAsync(dto, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new UserAlreadyExistsException();
        }
    }
    
    /// <summary>
    /// Returns active GroupID for user from FirestoreDB
    /// </summary>
    /// <param name="userUid"></param>
    /// <returns></returns>
    /// <exception cref="UserNotExistingException"></exception>
    public async Task<GroupIDDto> GetGroupIdByUIDAsync(string userUid)
    {
        try
        {
            DocumentReference  docRef = _sessions.Document(userUid);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
                throw new UserNotExistingException();
            return snapshot.ConvertTo<GroupIDDto>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
    
    /// <summary>
    /// Patches current Group ID in firestore DB.
    /// </summary>
    /// <param name="groupIdDto"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="UserNotExistingException"></exception>
    /// <exception cref="UserLoadingException"></exception>
    public async Task<GroupIDDto> PatchGroupIdByUIDAsync(GroupIDDto groupIdDto)
    {
        try
        {
            if (!TryParseValidId(groupIdDto.GroupID, out Guid parsedId)) throw new ArgumentException("Ungültige Guid", nameof(groupIdDto.GroupID));

            if (string.IsNullOrWhiteSpace(groupIdDto.UserUID))
                throw new ArgumentException("UserUID must be provided for patching.", nameof(groupIdDto));

            DocumentReference docRef = _sessions.Document(groupIdDto.UserUID);

            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                throw new UserNotExistingException();
            }

            // Patch semantics: merge incoming fields into existing document
            await docRef.SetAsync(groupIdDto, SetOptions.MergeAll);

            snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                var user = snapshot.ConvertTo<GroupIDDto>();
                return user;
            }
            throw new UserLoadingException();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// Function that checks if all fields of RegisterDto are valid.
    /// </summary>
    /// <param name="dto"></param>
    /// <exception cref="InvalidDtoException"></exception>
    private void ValidateRegisterDto(RegisterDto dto)
    {
        if (dto is null)
            throw new InvalidDtoException("RegisterDto cannot be null.");
    
        ValidateName(dto.FirstName, "FirstName");
        ValidateName(dto.LastName, "LastName");
        
        if(dto.UserUID is null)
            throw new InvalidDtoException("Missing UserUID.");
    
        if (!IsValidEmail(dto.Email))
            throw new InvalidDtoException("Invalid mail address.");
    
        if (string.IsNullOrWhiteSpace(dto.FavoriteDish) || dto.FavoriteDish.Length > 50)
            throw new InvalidDtoException("FavoriteDish needs to be assigned and cannot exceed 50 characters.");
    }
    
    /// <summary>
    /// Validates Name 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fieldName"></param>
    /// <exception cref="InvalidDtoException"></exception>
    private void ValidateName(string name, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidDtoException($"{fieldName} cannot be empty.");
    
        if (name.Length > 50)
            throw new InvalidDtoException($"{fieldName} Cannot be longer than 50 characters.");
    
        var namePattern = new System.Text.RegularExpressions.Regex(@"^[\p{L}\-'\s]+$"); // Unicode letters, spaces, hyphens, apostrophes
        if (!namePattern.IsMatch(name))
            throw new InvalidDtoException($"{fieldName} can only contain letters, empty spaces, hyphens or apostrophes.");
    }
    
    /// <summary>
    /// Checks if email adress has valid format.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Length > 254)
            return false;
    
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return string.Equals(addr.Address, email, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
    
    private bool TryParseValidId(string input, out Guid id)
    {
        id = Guid.Empty;
        if (string.IsNullOrWhiteSpace(input)) return false;
        if (!Guid.TryParse(input, out id)) return false;
        return id != Guid.Empty;
    }
    
    
    
    
    

}

