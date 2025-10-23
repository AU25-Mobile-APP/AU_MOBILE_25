namespace SwipeFeast.API.Services.Exceptions
{
	/// <summary>
	/// Exception that is thrown when a filter is invalid.
	/// </summary>
	public class FilterInvalidException : Exception
	{
		/// <summary>
		/// Message of the exception.
		/// </summary>
		public static readonly string CustomMessage = "Provided invalid filter(s).";

		/// <summary>
		/// Constructor for the exception.
		/// </summary>
		public FilterInvalidException() : base(CustomMessage) { }
	}
}
