using System.ComponentModel.DataAnnotations;

namespace Gemini.Models
{
    public class SignUpRequest
    {
        public string phonenumber { get; set; }

        public string password { get; set; }

        public string uuid { get; set; }
    }
}