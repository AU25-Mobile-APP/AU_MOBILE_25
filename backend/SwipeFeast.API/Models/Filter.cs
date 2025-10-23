using System.ComponentModel.DataAnnotations;

namespace SwipeFeast.API.Models
{
    /// <summary>
    /// Data model to represent a filter.
    /// </summary>
    public class Filter
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public bool Active { get; set; } = false;
    }
}
