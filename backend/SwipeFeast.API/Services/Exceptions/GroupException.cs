namespace SwipeFeast.API.Services.Exceptions
{
    /// <summary>
    /// Exception that is thrown when a group is not found.
    /// </summary>
    public class GroupNotFoundException : Exception
    {
        /// <summary>
        /// Message of the exception.
        /// </summary>
        public static readonly string CustomMessage = "Group not found.";

        /// <summary>
        /// Constructor for the exception.
        /// </summary>
        public GroupNotFoundException() : base(CustomMessage) { }
    }
}
