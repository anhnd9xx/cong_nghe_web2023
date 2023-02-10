namespace Gemini.Models
{
    public class DelSavedSearchRequest
    {
        public string token { get; set; }

        public int? search_id { get; set; }

        public bool? all { get; set; }
    }
}