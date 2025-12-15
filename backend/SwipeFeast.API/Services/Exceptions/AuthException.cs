namespace SwipeFeast.API.Services.Exceptions
{
    /// <summary>
    /// Exception that is thrown when a register dto is invalid.
    /// </summary>
    public class InvalidDtoException : Exception
    {
        /// <summary>
        /// Message prefix of the exception
        /// </summary>
        public static readonly string CustomMessage = "Error with auth: ";

        /// <summary>
        /// Additional detail about the error
        /// </summary>
        public string Detail { get; }

        /// <summary>
        /// Constructor for the exception with custom detail.
        /// </summary>
        public InvalidDtoException(string detail)
            : base(CustomMessage + detail)
        {
            Detail = detail;
        }

        /// <summary>
        /// Constructor that accepts an inner exception.
        /// </summary>
        public InvalidDtoException(string detail, Exception innerException)
            : base(CustomMessage + detail, innerException)
        {
            Detail = detail;
        }

        public override string ToString()
        {
            return $"{base.ToString()}\nDetail: {Detail}";
        }
    }
}

/// <summary>
/// Exception that is thrown if a user already exists in database.
/// </summary>
public class UserAlreadyExistsException : Exception
{
    public static readonly string CustomMessage = "Error during registration: User already exists.";

    /// <summary>
    /// Public constructor for Exception.
    /// </summary>
    public UserAlreadyExistsException() : base(CustomMessage) { }
}

/// <summary>
/// Exception that is thrown if a user already exists in database.
/// </summary>
public class UserCreationException : Exception
{
    public static readonly string CustomMessage = "Error during registration: User couldn't be created. Please try again.";

    /// <summary>
    /// Public constructor for Exception.
    /// </summary>
    public UserCreationException() : base(CustomMessage) { }
}

/// <summary>
/// Exception that is thrown if a user doesnt exist in the DB.
/// </summary>
public class UserNotExistingException : Exception
{
    public static readonly string CustomMessage = "Error during data retrival: User wasn't found. Contact support.";

    /// <summary>
    /// Public constructor for Exception.
    /// </summary>
    public UserNotExistingException() : base(CustomMessage) { }
}

/// <summary>
/// Exception that is thrown if a user doesn't exist in the DB.
/// </summary>
public class UserLoadingException : Exception
{
    public static readonly string CustomMessage = "Error during data retrival: User couldn't be updated.";

    /// <summary>
    /// Public constructor for Exception.
    /// </summary>
    public UserLoadingException() : base(CustomMessage) { }
}