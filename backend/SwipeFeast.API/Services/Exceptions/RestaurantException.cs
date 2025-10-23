namespace SwipeFeast.API.Services.Exceptions
{
    /// <summary>
    /// Exception that is thrown when a restaurant is not found.
    /// </summary>
    public class RestaurantNotFoundException : Exception
    {
        /// <summary>
        /// Message of the exception.
        /// </summary>
        public static readonly string CustomMessage = "Restaurant not found.";

        /// <summary>
        /// Constructor for the exception.
        /// </summary>
        public RestaurantNotFoundException() : base(CustomMessage) { }
    }

    /// <summary>
    /// Exception that is thrown when a group is not found.
    /// </summary>
    public class NoRestaurantsFoundException : Exception
    {
        /// <summary>
        /// Message of the exception.
        /// </summary>
        public static readonly string CustomMessage = "No restaurants found.";

        /// <summary>
        /// Constructor for the exception.
        /// </summary>
		public NoRestaurantsFoundException() : base(CustomMessage) { }
	}
}
