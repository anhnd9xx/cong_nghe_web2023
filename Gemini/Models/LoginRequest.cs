namespace Gemini.Models
{
    public class LoginRequest
    {
        public string phonenumber { get; set; }

        public string password { get; set; }

        public string uuid { get; set; }
    }

    public class LoginResponseData
    {
        public string id { get; set; }

        public string username { get; set; }

        public string token { get; set; }

        public string avatar { get; set; }

        public int active { get; set; }
    }
}