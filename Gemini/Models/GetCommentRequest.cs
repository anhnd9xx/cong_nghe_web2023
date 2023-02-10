using System;

namespace Gemini.Models
{
    public class GetCommentRequest
    {
        public string token { get; set; }

        public int? id { get; set; }

        public int? index { get; set; }

        public int? count { get; set; }
    }

    public class GetCommentResponseData
    {
        public int id { get; set; }

        public string comment { get; set; }

        public DateTime created { get; set; }

        public GetCommentResponseData_Poster poster { get; set; }
    }

    public class GetCommentResponseData_Poster
    {
        public int id { get; set; }

        public string name { get; set; }

        public string avatar { get; set; }
    }
}