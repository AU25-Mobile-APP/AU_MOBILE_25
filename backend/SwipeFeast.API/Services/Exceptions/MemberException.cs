namespace SwipeFeast.API.Services.Exceptions
{
    /// <summary>
    /// Exception that is thrown when member in group is not found.
    /// </summary>
    public class MemberNotFoundException : Exception
    {
        /// <summary>
        /// Message of the exception.
        /// </summary>
        public static readonly string CustomMessage = "Member not found.";

        /// <summary>
        /// Constructor for the exception.
        /// </summary>
        public MemberNotFoundException() : base(CustomMessage) { }
    }
}
