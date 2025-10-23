namespace SwipeFeast.API.Services.Exceptions
{
    /// <summary>
    /// Exception that is thrown when a ranking is not found.
    /// </summary>
    public class RankingNotFoundException : Exception
    {
        /// <summary>
        /// Message of the exception.
        /// </summary>
        public static readonly string CustomMessage = "Restaurant Ranking not found.";

        /// <summary>
        /// Constructor for the exception.
        /// </summary>
        public RankingNotFoundException() : base(CustomMessage) { }
    }
}
