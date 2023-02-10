using System;

namespace Gemini.Models
{
    public class AddPostRequest
    {
        public string token { get; set; }

        public string described { get; set; }

        public string status { get; set; }
    }

    public class AddPostResponseData
    {
        public int id { get; set; }

        public string url { get; set; }
    }
}