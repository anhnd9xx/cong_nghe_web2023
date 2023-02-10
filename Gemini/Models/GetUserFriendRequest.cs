using System;
using System.Collections.Generic;

namespace Gemini.Models
{
    public class GetUserFriendRequest
    {
        public string token { get; set; }

        public int? index { get; set; }

        public int? count { get; set; }
    }

    public class GetUserFriendResponseData
    {
        public List<GetUserFriendResponseData_Request> friends { get; set; }

        public int? total { get; set; }
    }

    public class GetUserFriendResponseData_Request
    {
        public int id { get; set; }

        public string username { get; set; }

        public string avatar { get; set; }

        public int same_friends { get; set; }

        public DateTime created { get; set; }
    }
}