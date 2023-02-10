using System;

namespace Gemini.Models
{
    public class ChangeInfoAfterSignUpRequest
    {
        public string token { get; set; }

        public string username { get; set; }
    }

    public class ChangeInfoAfterSignUpResponseData
    {
        public int id { get; set; }

        public string username { get; set; }

        public string phonenumber { get; set; }

        public DateTime created { get; set; }

        public string avatar { get; set; }

        public bool? is_blocked { get; set; }

        public bool? online { get; set; }
    }
}