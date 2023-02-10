using System;
using System.IO;
using System.Web;
using System.Linq;
using Gemini.Models;
using System.Web.Mvc;
using Gemini.Controllers.Common;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Gemini.Controllers
{
    public class W2Controller : BaseController
    {
        #region CheckVerifyCode

        [HttpPost]
        public ActionResult CheckVerifyCode(CheckVerifyCodeRequest req)
        {
            try
            {
                var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.SoDienThoai.Equals(req.phonenumber, StringComparison.OrdinalIgnoreCase));

                var valid = ValidateCheckVerifyCode(req, acc);

                if (valid)
                {
                    acc.Active = TaiKhoanCaNhan_Active.Actived;

                    VerifyCodeCacheManager.RefreshVerifyCode(req.phonenumber, req.code_verify);

                    DataReturn.code = ResponseCode.OK.ToString();
                    DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];
                    DataReturn.data = new CheckVerifyCodeResponseData()
                    {
                        token = acc.Token,
                        id = acc.Id,
                        active = acc.Active,
                    };
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

        private bool ValidateCheckVerifyCode(CheckVerifyCodeRequest req, TaiKhoanCaNhan acc)
        {
            if (string.IsNullOrWhiteSpace(req.phonenumber) || (!string.IsNullOrWhiteSpace(req.phonenumber)
                                                               && !(req.phonenumber.Length == 10 && req.phonenumber.StartsWith("0"))))
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            var regex = @"^[A-Z0-9_.-]*$";
            if (string.IsNullOrWhiteSpace(req.code_verify)
                || req.code_verify.Length != 6
                || !Regex.Match(req.code_verify, regex).Success)
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
            else
            {
                if (acc.Active == TaiKhoanCaNhan_Active.Actived)
                {
                    DataReturn.code = ResponseCode.UserExisted.ToString();
                    DataReturn.message = ResponseCode.dicDesc[ResponseCode.UserExisted];

                    return false;
                }

                if (!VerifyCodeCacheManager.VerifyCode.ContainsKey(acc.SoDienThoai)
                    || !VerifyCodeCacheManager.VerifyCode[acc.SoDienThoai].Equals(req.code_verify, StringComparison.OrdinalIgnoreCase))
                {
                    DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                    DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                    return false;
                }
            }

            return true;
        }

        #endregion

        #region ChangeInfoAfterSignUp

        [HttpPost]
        public ActionResult ChangeInfoAfterSignUp(ChangeInfoAfterSignUpRequest req)
        {
            try
            {
                var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.Token == req.token && x.Active == TaiKhoanCaNhan_Active.Actived);

                var valid = ValidateChangeInfoAfterSignUp(req, acc);

                if (valid)
                {
                    acc.Ten = req.username;

                    try
                    {
                        if (Request.Files != null)
                        {
                            var avatar = Request.Files[0];

                            string guidImage = Guid.NewGuid().ToString();
                            var nameFile = Path.GetFileName(avatar.FileName);

                            string serverPath = "/Content/UserFiles/Images/" + guidImage + "/";
                            string tmp = Server.MapPath("~" + serverPath);
                            if (System.IO.File.Exists(Path.Combine(tmp, nameFile)))
                            {
                                System.IO.File.Delete(Path.Combine(tmp, nameFile));
                            }
                            string physicalPath = Path.Combine(Server.MapPath("~" + serverPath), nameFile);
                            VerifyDir(physicalPath);
                            WriteFileFromStream(avatar.InputStream, physicalPath);

                            acc.LinkAvatar = serverPath + nameFile;
                        }

                        if (SaveData())
                        {
                            DataReturn.code = ResponseCode.OK.ToString();
                            DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];
                            DataReturn.data = new ChangeInfoAfterSignUpResponseData()
                            {
                                id = acc.Id,
                                username = acc.Ten,
                                phonenumber = acc.SoDienThoai,
                                created = acc.CreatedAt,
                                avatar = acc.LinkAvatar,
                            };
                        }
                        else
                        {
                            DataReturn.code = ResponseCode.UnknownError.ToString();
                            DataReturn.message = ResponseCode.dicDesc[ResponseCode.UnknownError];
                        }
                    }
                    catch (Exception ex)
                    {
                        DataReturn.code = ResponseCode.FileSizeIsToBig.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.FileSizeIsToBig];
                        DataReturn.data = ex.ToString();
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

        private bool ValidateChangeInfoAfterSignUp(ChangeInfoAfterSignUpRequest req, TaiKhoanCaNhan acc)
        {
            if (string.IsNullOrWhiteSpace(req.token))
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            var regex = @"^[a-zA-Z0-9_.-]*$";
            if (string.IsNullOrWhiteSpace(req.username)
                || !Regex.Match(req.username, regex).Success)
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

            //TODO: DuongNH: Confirm dữ liệu req.username thế nào là "đến mức bị khóa" ?

            return true;
        }

        #endregion

        #region AddPost

        [HttpPost]
        public ActionResult AddPost(AddPostRequest req)
        {
            try
            {
                var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.Token == req.token && x.Active == TaiKhoanCaNhan_Active.Actived);
                var lstImg = new List<HttpPostedFileBase>();

                if (Request.Files != null)
                    lstImg = Request.Files.GetMultiple("image").ToList();

                var valid = ValidateAddPost(req, acc, lstImg);

                if (valid)
                {
                    var post = new BaiVietCaNhan();
                    List<BaiVietCaNhanAnh> postImgs = new List<BaiVietCaNhanAnh>();

                    post.ChuBaiVietId = acc.Id;
                    post.NoiDungBaiViet = req.described;
                    post.Status = req.status;
                    post.CreatedAt = DateTime.Now;
                    post.CreatedBy = acc.Id;

                    try
                    {
                        if (Request.Files != null)
                        {
                            var video = Request.Files["video"];

                            if (video != null)
                            {
                                using (Stream inputStream = video.InputStream)
                                {
                                    MemoryStream memoryStream = inputStream as MemoryStream;
                                    if (memoryStream == null)
                                    {
                                        memoryStream = new MemoryStream();
                                        inputStream.CopyTo(memoryStream);
                                    }
                                    post.Media = memoryStream.ToArray();
                                }
                            }
                        }

                        DataGemini.BaiVietCaNhans.Add(post);

                        if (Request.Files != null)
                        {
                            if (lstImg != null && lstImg.Any())
                            {
                                postImgs = AddPostAnh(post.Id, lstImg);
                                DataGemini.BaiVietCaNhanAnhs.AddRange(postImgs);
                            }
                        }

                        if (SaveData())
                        {
                            DataReturn.code = ResponseCode.OK.ToString();
                            DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];
                            DataReturn.data = new AddPostResponseData()
                            {
                                id = post.Id,
                            };
                        }
                        else
                        {
                            DataGemini.BaiVietCaNhans.Remove(post);
                            if (postImgs.Any())
                                DataGemini.BaiVietCaNhanAnhs.RemoveRange(postImgs);

                            DataReturn.code = ResponseCode.UnknownError.ToString();
                            DataReturn.message = ResponseCode.dicDesc[ResponseCode.UnknownError];
                        }
                    }
                    catch (Exception ex)
                    {
                        DataReturn.code = ResponseCode.FileSizeIsToBig.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.FileSizeIsToBig];
                        DataReturn.data = ex.ToString();
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

        private bool ValidateAddPost(AddPostRequest req, TaiKhoanCaNhan acc, List<HttpPostedFileBase> lstImg)
        {
            if (string.IsNullOrWhiteSpace(req.token))
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

            if (lstImg.Count > 4)
            {
                DataReturn.code = ResponseCode.MaximumNumberOfImages.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.MaximumNumberOfImages];

                return false;
            }

            return true;
        }

        private List<BaiVietCaNhanAnh> AddPostAnh(int baiVietCaNhanId, List<HttpPostedFileBase> lstImg)
        {
            List<BaiVietCaNhanAnh> imgs = new List<BaiVietCaNhanAnh>();

            int sort = 0;
            foreach (var img in lstImg)
            {
                var newImg = new BaiVietCaNhanAnh()
                {
                    BaiVietId = baiVietCaNhanId,
                    Sort = sort,
                };

                using (Stream inputStream = img.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    newImg.Image = memoryStream.ToArray();
                }

                imgs.Add(newImg);
                sort++;
            }

            return imgs;
        }

        #endregion
    }
}