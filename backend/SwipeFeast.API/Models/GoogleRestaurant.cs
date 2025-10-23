using System.Text.Json.Serialization;

namespace SwipeFeast.API.Models
{
	public class GooglePlaces
	{
		public GoogleRestaurant[] places { get; set; } = [];
	}

	public class GoogleRestaurant
	{
		public string id { get; set; }
		public GoogleLocation location { get; set; }
		public List<string> types {  get; set; }
        public float rating { get; set; }
        public GoogleDisplayName displayName { get; set; }
        public string googleMapsUri { get; set; }
		public EditorialSummary editorialSummary { get; set; }
		public List<GooglePhotoId> photos { get; set; }
    }

	public class GoogleLocation
	{
		public double latitude { get; set; }
		public double longitude { get; set; }
	}

    public class  GoogleDisplayName
    {
        public string text { get; set; }
        public string languageCode { get; set; }
    }

	public class EditorialSummary {
		public string text { get; set; }
		public string languageCode { get; set; }
	}

	public class GooglePhotoId
	{
        public string name { get; set; }
        public int widthPx { get; set; }
        public int heightPx { get; set; }
    }
}
