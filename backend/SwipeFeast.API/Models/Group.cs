using System.Security.Cryptography.Xml;

namespace SwipeFeast.API.Models
{
    /// <summary>
    /// Model of group that holds the pre set settings on creaton, the list of matching restaurants and of likes (in the Rankings property).
    /// </summary>
    public class Group
    {
        public Guid Id { get; set; }

        public int JoinCode { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int LocationRange { get; set; }

        public List<Filter> Filters { get; set; } = new List<Filter>();

        public List<Ranking> Rankings { get; set; } = new List<Ranking>();

        public List<Restaurant> Restaurants { get; set; } = new List<Restaurant>();

        public List<Guid> Members { get; set; } = new List<Guid>();

        public DateTime ExpirationDate { get; set; }
    }

    /// <summary>
    /// Model to hold the numbers of received likes by a restaurant.
    /// </summary>
    public class Ranking
    {
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public int LikeCount { get; set; }
        public List<Guid> Members { get; set; } = [];
    }
}



