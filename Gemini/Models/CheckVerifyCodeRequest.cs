namespace Gemini.Models
{
    public class CheckVerifyCodeRequest
    {
        public string phonenumber { get; set; }

        public string code_verify { get; set; }
    }

    public class CheckVerifyCodeResponseData
    {
        public string token { get; set; }

        public int id { get; set; }

        public int active { get; set; }
    }
}