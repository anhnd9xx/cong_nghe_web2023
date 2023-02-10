namespace Gemini.Models
{
    public class LikePostRequest
    {
        public string token { get; set; }

        public int? id { get; set; }
    }

    public class LikePostResponseData
    {
        public int like { get; set; }
    }
}