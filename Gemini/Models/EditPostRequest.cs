using System.Collections.Generic;

namespace Gemini.Models
{
    public class EditPostRequest
    {
        public string token { get; set; }

        public int? id { get; set; }

        public string described { get; set; }

        public string status { get; set; }

        public List<int> image_del { get; set; }

        public List<int> image_sort { get; set; }

        public string auto_accept { get; set; }

        public string auto_block { get; set; }
    }
}