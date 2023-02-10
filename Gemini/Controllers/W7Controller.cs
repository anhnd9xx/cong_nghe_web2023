using System;
using System.Linq;
using Gemini.Models;
using System.Web.Mvc;
using Gemini.Controllers.Common;
using System.Collections.Generic;

namespace Gemini.Controllers
{
    public class W7Controller : BaseController
    {
        #region GetRequestedFriend

        [HttpPost]
        public ActionResult GetRequestedFriend(GetRequestedFriendRequest req)
        {
            try
            {
                var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.Token == req.token && x.Active == TaiKhoanCaNhan_Active.Actived);

                var valid = ValidateGetRequestedFriend(req, acc);

                if (valid)
                {
                    int count = req.count.GetValueOrDefault(10);
                    var index = req.index.GetValueOrDefault(0);

                    var lstRequested = DataGemini.BanBes.Where(x => x.TaiKhoanCaNhanChapNhan == acc.Id && x.Accepted == BanBe_Accepted.Waiting).OrderByDescending(x => x.CreatedAt).Skip(index).Take(count).ToList();
                    var lstFriend = DataGemini.BanBes.Where(x => x.TaiKhoanCaNhanChapNhan == acc.Id && x.Accepted == BanBe_Accepted.Accepted).Select(x => x.TaiKhoanCaNhanYeuCau).ToList();

                    var lstRequestedId = lstRequested.Select(x => x.TaiKhoanCaNhanYeuCau);
                    var lstAccRequested = DataGemini.TaiKhoanCaNhans.Where(x => lstRequestedId.Contains(x.Id) && x.Active == TaiKhoanCaNhan_Active.Actived).ToList();
                    var lstFriendOfRequested = DataGemini.BanBes.Where(x => lstRequestedId.Contains(x.TaiKhoanCaNhanChapNhan) && x.Accepted == BanBe_Accepted.Accepted).Select(x => x.TaiKhoanCaNhanYeuCau).ToList();

                    DataReturn.code = ResponseCode.OK.ToString();
                    DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];

                    var res = new GetRequestedFriendResponseData()
                    {
                        request = new List<GetRequestedFriendResponseData_Request>(),
                        total = lstRequested.Count,
                    };

                    foreach (var request in lstRequested)
                    {
                        var accRequested = lstAccRequested.FirstOrDefault(x => x.Id == request.TaiKhoanCaNhanYeuCau);
                        var countFriendSame = lstFriendOfRequested.Count(x => lstFriend.Contains(x));
                        var requestResponseData = new GetRequestedFriendResponseData_Request();

                        requestResponseData.id = request.TaiKhoanCaNhanYeuCau.GetValueOrDefault(0);
                        requestResponseData.username = accRequested?.Ten;
                        requestResponseData.avatar = accRequested?.LinkAvatar;
                        requestResponseData.same_friends = countFriendSame;
                        requestResponseData.created = request.CreatedAt;

                        res.request.Add(requestResponseData);
                    }
                    DataReturn.data = res;
                }
            }
            catch (Exception ex)
            {
                DataReturn.code = ResponseCode.ExceptionError.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ExceptionError];
                DataReturn.data = ex.ToString();
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        private bool ValidateGetRequestedFriend(GetRequestedFriendRequest req, TaiKhoanCaNhan acc)
        {
            if (string.IsNullOrWhiteSpace(req.token))
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            if (req.index == null)
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            if (req.count == null)
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            if (acc == null)
            {
                DataReturn.code = ResponseCode.UserIsNotValidated.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.UserIsNotValidated];

                return false;
            }

            return true;
        }

        #endregion

        #region GetUserFriend

        [HttpPost]
        public ActionResult GetUserFriend(GetUserFriendRequest req)
        {
            try
            {
                var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.Token == req.token && x.Active == TaiKhoanCaNhan_Active.Actived);

                var valid = ValidateGetUserFriend(req, acc);

                if (valid)
                {
                    int count = req.count.GetValueOrDefault(10);
                    var index = req.index.GetValueOrDefault(0);

                    var lstFriend = DataGemini.BanBes.Where(x => x.TaiKhoanCaNhanChapNhan == acc.Id && x.Accepted == BanBe_Accepted.Accepted).OrderByDescending(x => x.CreatedAt).Skip(index).Take(count).ToList();

                    var lstFriendId = lstFriend.Select(x => x.TaiKhoanCaNhanYeuCau);
                    var lstAccRequested = DataGemini.TaiKhoanCaNhans.Where(x => lstFriendId.Contains(x.Id) && x.Active == TaiKhoanCaNhan_Active.Actived).ToList();
                    var lstFriendOfRequested = DataGemini.BanBes.Where(x => lstFriendId.Contains(x.TaiKhoanCaNhanChapNhan) && x.Accepted == BanBe_Accepted.Accepted).Select(x => x.TaiKhoanCaNhanYeuCau).ToList();

                    DataReturn.code = ResponseCode.OK.ToString();
                    DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];

                    var res = new GetUserFriendResponseData()
                    {
                        friends = new List<GetUserFriendResponseData_Request>(),
                        total = lstFriend.Count,
                    };

                    foreach (var friend in lstFriend)
                    {
                        var accRequested = lstAccRequested.FirstOrDefault(x => x.Id == friend.TaiKhoanCaNhanYeuCau);
                        var countFriendSame = lstFriendOfRequested.Count(x => lstFriend.Exists(c => c.TaiKhoanCaNhanYeuCau == x));
                        var requestResponseData = new GetUserFriendResponseData_Request();

                        requestResponseData.id = friend.TaiKhoanCaNhanYeuCau.GetValueOrDefault(0);
                        requestResponseData.username = accRequested?.Ten;
                        requestResponseData.avatar = accRequested?.LinkAvatar;
                        requestResponseData.same_friends = countFriendSame;
                        requestResponseData.created = friend.CreatedAt;

                        res.friends.Add(requestResponseData);
                    }
                    DataReturn.data = res;
                }
            }
            catch (Exception ex)
            {
                DataReturn.code = ResponseCode.ExceptionError.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ExceptionError];
                DataReturn.data = ex.ToString();
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        private bool ValidateGetUserFriend(GetUserFriendRequest req, TaiKhoanCaNhan acc)
        {
            if (string.IsNullOrWhiteSpace(req.token))
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            if (req.index == null)
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            if (req.count == null)
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            if (acc == null)
            {
                DataReturn.code = ResponseCode.UserIsNotValidated.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.UserIsNotValidated];

                return false;
            }

            return true;
        }

        #endregion
    }
}