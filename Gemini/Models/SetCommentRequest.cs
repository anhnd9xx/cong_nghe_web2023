using System;

namespace Gemini.Models
{
    public class SetCommentRequest
    {
        public string token { get; set; }

        public int? id { get; set; }

        public string comment { get; set; }

        public int? index { get; set; }

        public int? count { get; set; }
    }

    public class SetCommentResponseData
    {
        public int id { get; set; }

        public string comment { get; set; }

        public DateTime created { get; set; }

        public SetCommentResponseData_Poster poster { get; set; }
    }

    public class SetCommentResponseData_Poster
    {
        public int id { get; set; }

        public string name { get; set; }

        public string avatar { get; set; }
    }
}