using System;
using System.Collections.Generic;

namespace Gemini.Models
{
    public class GetPostRequest
    {
        public string token { get; set; }

        public int? id { get; set; }
    }

    public class GetPostResponseData
    {
        public int id { get; set; }

        public string described { get; set; }

        public DateTime created { get; set; }

        public DateTime? modified { get; set; }

        public int like { get; set; }

        public int comment { get; set; }

        public bool is_liked { get; set; }

        public List<GetPostResponseData_Image> image { get; set; }

        public GetPostResponseData_Video video { get; set; }

        public GetPostResponseData_Author author { get; set; }

        public string state { get; set; }

        public bool is_blocked { get; set; }

        public bool can_edit { get; set; }

        public string banned { get; set; }

        public bool? can_comment { get; set; }

        public string url { get; set; }

        public string messages { get; set; }
    }

    public class GetPostResponseData_Image
    {
        public int id { get; set; }

        public byte[] img { get; set; }
    }

    public class GetPostResponseData_Video
    {
        public byte[] vid { get; set; }

        public byte[] thumb { get; set; }
    }

    public class GetPostResponseData_Author
    {
        public int id { get; set; }

        public string name { get; set; }

        public string avatar { get; set; }

        public bool? online { get; set; }
    }
}