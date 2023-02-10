using System;

namespace Gemini.Models
{
    public class GetSavedSearchRequest
    {
        public string token { get; set; }

        public int? index { get; set; }

        public int? count { get; set; }
    }

    public class GetSavedSearchResponseData
    {
        public int id { get; set; }

        public string keyword { get; set; }

        public DateTime created { get; set; }
    }
}