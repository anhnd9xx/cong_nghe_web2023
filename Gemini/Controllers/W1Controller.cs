using System;
using System.Web;
using System.Linq;
using Gemini.Models;
using System.Web.Mvc;
using System.Web.Security;
using Gemini.Controllers.Common;

namespace Gemini.Controllers
{
    public class W1Controller : BaseController
    {
        #region SignUp

        [HttpPost]
        public ActionResult SignUp(SignUpRequest req)
        {
            try
            {
                var valid = ValidateSignUp(req);

                if (valid)
                {
                    TaiKhoanCaNhan newAcc = new TaiKhoanCaNhan()
                    {
                        MatKhau = Encrypt(req.password),
                        SoDienThoai = req.phonenumber,
                        Active = TaiKhoanCaNhan_Active.Raw,
                        CreatedAt = DateTime.Now,
                    };

                    DataGemini.TaiKhoanCaNhans.Add(newAcc);

                    if (SaveData())
                    {
                        var code = GenerateCode(6);

                        VerifyCodeCacheManager.RefreshVerifyCode(newAcc.SoDienThoai, code);

                        DataReturn.code = ResponseCode.OK.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];
                        DataReturn.data = code;
                    }
                    else
                    {
                        DataGemini.TaiKhoanCaNhans.Remove(newAcc);

                        DataReturn.code = ResponseCode.UnknownError.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.UnknownError];
                    }
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

        private bool ValidateSignUp(SignUpRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.phonenumber))
            {
                DataReturn.code = ResponseCode.ParameterIsNotEnought.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterIsNotEnought];

                return false;
            }

            if (string.IsNullOrWhiteSpace(req.password))
            {
                DataReturn.code = ResponseCode.ParameterIsNotEnought.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterIsNotEnought];

                return false;
            }

            var userExists = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.SoDienThoai.Equals(req.phonenumber, StringComparison.OrdinalIgnoreCase));

            if (userExists != null)
            {
                DataReturn.code = ResponseCode.UserExisted.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.UserExisted];

                return false;
            }

            return true;
        }

        #endregion

        #region Login

        [HttpPost]
        public ActionResult Login(LoginRequest req)
        {
            try
            {
                var valid = ValidateLogin(req);

                if (valid)
                {
                    string passWordEnc = Encrypt(req.password);
                    string token = Encrypt(req.uuid);
                    var acc = DataGemini.TaiKhoanCaNhans.Where(s => s.SoDienThoai.ToUpper().Equals(req.phonenumber.ToUpper().Trim()));

                    if (acc != null && acc.Any())
                    {
                        var accValid = acc.FirstOrDefault(s => s.MatKhau.ToUpper().Equals(passWordEnc.ToUpper()));

                        var authTicket = new FormsAuthenticationTicket(1, accValid.SoDienThoai, DateTime.Now, DateTime.Now.AddDays(1), true, accValid.Token);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                        if (authTicket.IsPersistent)
                        {
                            cookie.Expires = authTicket.Expiration;
                        }
                        cookie.HttpOnly = true;
                        HttpContext.Response.Cookies.Add(cookie);

                        // Store token to CacheServer
                        TokenCacheManager.RefreshTokenCached(accValid.SoDienThoai, token);

                        accValid.Token = token;
                        SaveData();

                        DataReturn.code = ResponseCode.OK.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];
                        DataReturn.data = new LoginResponseData()
                        {
                            id = accValid.Id.ToString(),
                            username = accValid.Ten,
                            token = accValid.Token,
                            avatar = accValid.LinkAvatar,
                            active = accValid.Active,
                        };
                    }
                    else
                    {
                        DataReturn.code = ResponseCode.UserIsNotValidated.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.UserIsNotValidated];
                    }
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

        private bool ValidateLogin(LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.phonenumber))
            {
                DataReturn.code = ResponseCode.ParameterIsNotEnought.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterIsNotEnought];

                return false;
            }

            if (string.IsNullOrWhiteSpace(req.password))
            {
                DataReturn.code = ResponseCode.ParameterIsNotEnought.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterIsNotEnought];

                return false;
            }

            if (string.IsNullOrWhiteSpace(req.uuid))
            {
                DataReturn.code = ResponseCode.ParameterIsNotEnought.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterIsNotEnought];

                return false;
            }

            return true;
        }

        #endregion

        #region Logout

        [HttpPost]
        public ActionResult Logout(LogoutRequest req)
        {
            try
            {
                var phoneNumber = GetKeyFromValue_Token(req.token);
                if (!string.IsNullOrWhiteSpace(phoneNumber))
                {
                    TokenCacheManager.RefreshTokenCached(phoneNumber, string.Empty);

                    var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(s => s.SoDienThoai.ToUpper().Equals(phoneNumber.ToUpper().Trim()));
                    if (acc != null)
                    {
                        acc.Token = string.Empty;

                        SaveData();
                    }
                }

                Session.RemoveAll();
                FormsAuthentication.SignOut();

                DataReturn.code = ResponseCode.OK.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];
            }
            catch (Exception ex)
            {
                DataReturn.code = ResponseCode.ExceptionError.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ExceptionError];
                DataReturn.data = ex.ToString();
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region GetVerifyCode

        [HttpPost]
        public ActionResult GetVerifyCode(GetVerifyCodeRequest req)
        {
            try
            {
                var valid = ValidateGetVerifyCode(req);

                if (valid)
                {
                    var code = GenerateCode(6);

                    VerifyCodeCacheManager.RefreshVerifyCode(req.phonenumber, code);
                    VerifyCodeLatestCallCacheManager.RefreshVerifyCodeLatestCall(req.phonenumber);

                    DataReturn.code = ResponseCode.OK.ToString();
                    DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];
                    DataReturn.data = code;
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

        private bool ValidateGetVerifyCode(GetVerifyCodeRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.phonenumber) || (!string.IsNullOrWhiteSpace(req.phonenumber)
                                                               && !(req.phonenumber.Length == 10 && req.phonenumber.StartsWith("0"))))
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }
            else
            {
                if (VerifyCodeLatestCallCacheManager.VerifyCodeLatestCall.ContainsKey(req.phonenumber))
                {
                    var latestCall = VerifyCodeLatestCallCacheManager.VerifyCodeLatestCall[req.phonenumber];

                    if ((DateTime.Now - latestCall).TotalSeconds < 120)
                    {
                        DataReturn.code = ResponseCode.NotAccess.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.NotAccess];

                        return false;
                    }
                }

                var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.SoDienThoai.Equals(req.phonenumber, StringComparison.OrdinalIgnoreCase));

                if (acc == null)
                {
                    DataReturn.code = ResponseCode.UserIsNotValidated.ToString();
                    DataReturn.message = ResponseCode.dicDesc[ResponseCode.UserIsNotValidated];

                    return false;
                }
                else if (acc.Active == TaiKhoanCaNhan_Active.Actived)
                {
                    DataReturn.code = ResponseCode.ActionHasBeenDonePreviouslyByThisUser.ToString();
                    DataReturn.message = ResponseCode.dicDesc[ResponseCode.ActionHasBeenDonePreviouslyByThisUser];

                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}