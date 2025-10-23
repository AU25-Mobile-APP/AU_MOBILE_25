using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SwipeFeast.API.Models
{
    /// <summary>
    /// Represents an input model for creating a group, including location data and optional filters.
    /// </summary>
    public class GroupCreationDto
    {
        [Required]
        [StringLength(50)] //TODO: What is the max length of the name?
        public string Name { get; set; }

        [Required]
        [Range(-90.00000, 90.00000)]
        public double Latitude { get; set; }

		[Required]
        [Range(-180.0, 180.0)]
        public double Longitude { get; set; }

		[Required]
        [Range(100, 10000)] // Value in meters
        public int LocationRange { get; set; }

        public List<Filter> Filters { get; set; }
    }

    /// <summary>
    /// Group DTO for existing groups.
    /// </summary>
    public class GroupExistingDto : GroupCreationDto
    {
		public DateTime ExpirationDate { get; set; }

        public int JoinCode { get; set; }
	}
}
