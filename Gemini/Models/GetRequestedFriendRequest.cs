using System;
using System.Collections.Generic;

namespace Gemini.Models
{
    public class GetRequestedFriendRequest
    {
        public string token { get; set; }

        public int? index { get; set; }

        public int? count { get; set; }
    }

    public class GetRequestedFriendResponseData
    {
        public List<GetRequestedFriendResponseData_Request> request { get; set; }

        public int? total { get; set; }
    }

    public class GetRequestedFriendResponseData_Request
    {
        public int id { get; set; }

        public string username { get; set; }

        public string avatar { get; set; }

        public int same_friends { get; set; }

        public DateTime created { get; set; }
    }
}