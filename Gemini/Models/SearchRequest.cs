using System.Collections.Generic;

namespace Gemini.Models
{
    public class SearchRequest
    {
        public string token { get; set; }

        public string keyword { get; set; }

        public int? user_id { get; set; }

        public int? index { get; set; }

        public int? count { get; set; }
    }

    public class SearchResponseData
    {
        public int id { get; set; }

        public List<SearchResponseData_Image> image { get; set; }

        public SearchResponseData_Video video { get; set; }

        public int like { get; set; }

        public int comment { get; set; }

        public bool is_liked { get; set; }

        public SearchResponseData_Author author { get; set; }

        public string described { get; set; }
    }

    public class SearchResponseData_Image
    {
        public int id { get; set; }

        public int? baiVietId { get; set; }

        public byte[] img { get; set; }
    }

    public class SearchResponseData_Video
    {
        public byte[] vid { get; set; }

        public byte[] thumb { get; set; }
    }

    public class SearchResponseData_Author
    {
        public int id { get; set; }

        public string name { get; set; }

        public string avatar { get; set; }
    }
}