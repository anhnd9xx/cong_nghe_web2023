namespace Gemini.Models
{
    public class ReportPostRequest
    {
        public string token { get; set; }

        public int? id { get; set; }

        public string subject { get; set; }

        public string details { get; set; }
    }
}