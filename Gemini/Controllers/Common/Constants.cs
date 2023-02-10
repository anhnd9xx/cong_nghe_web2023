using System.Collections.Generic;

namespace Gemini.Controllers.Common
{
    public class ResponseCode
    {
        public const int OK = 1000;
        public const int PostIsNotExisted = 9992;
        public const int CodeVerifyIsIncorrect = 9993;
        public const int NoDataOrEndOfListData = 9994;
        public const int UserIsNotValidated = 9995;
        public const int UserExisted = 9996;
        public const int MethodIsInvalid = 9997;
        public const int TokenIsInvalid = 9998;
        public const int ExceptionError = 9999;
        public const int CanNotConnectToDB = 1001;
        public const int ParameterIsNotEnought = 1002;
        public const int ParameterTypeIsInvalid = 1003;
        public const int ParameterValueIsInvalid = 1004;
        public const int UnknownError = 1005;
        public const int FileSizeIsToBig = 1006;
        public const int UploadFileFailed = 1007;
        public const int MaximumNumberOfImages = 1008;
        public const int NotAccess = 1009;
        public const int ActionHasBeenDonePreviouslyByThisUser = 1010;
        public const int CouldNotPublishThisPost = 1011;
        public const int LimitedAccess = 1012;

        public static Dictionary<int, string> dicDesc = new Dictionary<int, string>()
        {
            {OK,                                        "OK"},
            {PostIsNotExisted,                          "Bài viết không tồn tại"},
            {CodeVerifyIsIncorrect,                     "Mã xác thực không đúng"},
            {NoDataOrEndOfListData,                     "Không có hoặc không còn dữ liệu"},
            {UserIsNotValidated,                        "Không có người dùng này"},
            {UserExisted,                               "Người dùng đã tồn tại"},
            {MethodIsInvalid,                           "Phương thức không đúng"},
            {TokenIsInvalid,                            "Sai token"},
            {ExceptionError,                            "Lỗi exception"},
            {CanNotConnectToDB,                         "Lỗi mất kết nối DB/hoặc thực thi câu SQL"},
            {ParameterIsNotEnought,                     "Số lượng Parameter không đầy đủ"},
            {ParameterTypeIsInvalid,                    "Kiểu tham số không đúng đắn"},
            {ParameterValueIsInvalid,                   "Giá trị của tham số không hợp lệ"},
            {UnknownError,                              "Unknown Error"},
            {FileSizeIsToBig,                           "Cỡ file vượt mức cho phép"},
            {UploadFileFailed,                          "Upload thất bại"},
            {MaximumNumberOfImages,                     "Số lượng images vượt quá quy định"},
            {NotAccess,                                 "Không có quyền truy cập tài nguyên"},
            {ActionHasBeenDonePreviouslyByThisUser,     "Hành động đã được người dùng thực hiện trước đây"},
            {CouldNotPublishThisPost,                   "Bài đăng vi phạm tiêu chuẩn cộng đồng"},
            {LimitedAccess,                             "Bài đăng bị giới hạn ở một số quốc gia"},
        };
    }

    public class TaiKhoanCaNhan_Active
    {
        public const int Raw = -1;
        public const int Actived = 1;

        public static Dictionary<int, string> dicDesc = new Dictionary<int, string>()
        {
            {Raw,                               "Đợi đổi Tên và Ava"},
            {Actived,                           "Actived"},
        };
    }

    public class BanBe_Accepted
    {
        public const int Waiting = 0;
        public const int Accepted = 1;
        public const int Rejected = 2;

        public static Dictionary<int, string> dicDesc = new Dictionary<int, string>()
        {
            {Waiting,                           "Waiting"},
            {Accepted,                          "Accepted"},
            {Rejected,                          "Rejected"},
        };
    }
}