using System;
using System.Linq;
using Gemini.Models;
using System.Web.Mvc;
using Gemini.Controllers.Common;
using System.Collections.Generic;

namespace Gemini.Controllers
{
    public class W4Controller : BaseController
    {
        #region DeletePost

        [HttpPost]
        public ActionResult DeletePost(DeletePostRequest req)
        {
            try
            {
                var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.Token == req.token && x.Active == TaiKhoanCaNhan_Active.Actived);
                var post = DataGemini.BaiVietCaNhans.FirstOrDefault(x => x.Id == req.id);

                var valid = ValidateDeletePost(req, acc, post);

                if (valid)
                {
                    DataGemini.BaiVietCaNhans.Remove(post);

                    if (SaveData())
                    {
                        DataReturn.code = ResponseCode.OK.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];
                    }
                    else
                    {
                        DataGemini.BaiVietCaNhans.Add(post);

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

        private bool ValidateDeletePost(DeletePostRequest req, TaiKhoanCaNhan acc, BaiVietCaNhan post)
        {
            if (string.IsNullOrWhiteSpace(req.token))
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            if (req.id == null)
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

            if (post == null)
            {
                DataReturn.code = ResponseCode.PostIsNotExisted.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.PostIsNotExisted];

                return false;
            }

            return true;
        }

        #endregion

        #region ReportPost

        [HttpPost]
        public ActionResult ReportPost(ReportPostRequest req)
        {
            try
            {
                var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.Token == req.token && x.Active == TaiKhoanCaNhan_Active.Actived);
                var post = DataGemini.BaiVietCaNhans.FirstOrDefault(x => x.Id == req.id);

                var valid = ValidateReportPost(req, acc, post);

                if (valid)
                {
                    var reportPost = new BaiVietCaNhanBaoCao()
                    {
                        BaiVietId = post.Id,
                        Subject = req.subject,
                        Details = req.details,
                        CreatedAt = DateTime.Now,
                        CreatedBy = acc.Id
                    };

                    DataGemini.BaiVietCaNhanBaoCaos.Add(reportPost);

                    if (SaveData())
                    {
                        DataReturn.code = ResponseCode.OK.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];
                    }
                    else
                    {
                        DataGemini.BaiVietCaNhanBaoCaos.Remove(reportPost);

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

        private bool ValidateReportPost(ReportPostRequest req, TaiKhoanCaNhan acc, BaiVietCaNhan post)
        {
            if (string.IsNullOrWhiteSpace(req.token))
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            if (req.id == null)
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            if (string.IsNullOrWhiteSpace(req.subject))
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            if (string.IsNullOrWhiteSpace(req.details))
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

            if (post == null)
            {
                DataReturn.code = ResponseCode.PostIsNotExisted.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.PostIsNotExisted];

                return false;
            }

            return true;
        }

        #endregion

        #region LikePost

        [HttpPost]
        public ActionResult LikePost(LikePostRequest req)
        {
            try
            {
                var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.Token == req.token && x.Active == TaiKhoanCaNhan_Active.Actived);
                var post = DataGemini.BaiVietCaNhans.FirstOrDefault(x => x.Id == req.id);

                var valid = ValidateLikePost(req, acc, post);

                if (valid)
                {
                    var lstLike = !string.IsNullOrWhiteSpace(post.NguoiThichIds) ? post.NguoiThichIds.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();
                    if (lstLike.Contains(acc.Id))
                    {
                        lstLike.Remove(acc.Id);
                    }
                    else
                    {
                        lstLike.Add(acc.Id);
                    }
                    post.NguoiThichIds = string.Join(";", lstLike.Select(x => x.ToString()));

                    if (SaveData())
                    {
                        DataReturn.code = ResponseCode.OK.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];
                        DataReturn.data = new LikePostResponseData()
                        {
                            like = lstLike.Count,
                        };
                    }
                    else
                    {
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

        private bool ValidateLikePost(LikePostRequest req, TaiKhoanCaNhan acc, BaiVietCaNhan post)
        {
            if (string.IsNullOrWhiteSpace(req.token))
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            if (req.id == null)
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

            if (post == null)
            {
                DataReturn.code = ResponseCode.PostIsNotExisted.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.PostIsNotExisted];

                return false;
            }

            return true;
        }

        #endregion

        #region GetComment

        [HttpPost]
        public ActionResult GetComment(GetCommentRequest req)
        {
            try
            {
                var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.Token == req.token && x.Active == TaiKhoanCaNhan_Active.Actived);
                var post = DataGemini.BaiVietCaNhans.FirstOrDefault(x => x.Id == req.id);

                var valid = ValidateGetComment(req, acc, post);

                if (valid)
                {
                    var lstComment = new List<BinhLuan>();
                    var lstBlockedId = !string.IsNullOrWhiteSpace(acc.ChanIds) ? acc.ChanIds.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();
                    var lstCommentQ = DataGemini.BinhLuans.Where(x => x.BaiVietId == req.id && !lstBlockedId.Contains(x.CreatedBy));

                    int count = req.count.GetValueOrDefault(10);
                    var index = req.index.GetValueOrDefault(0);
                    lstComment = lstCommentQ.OrderByDescending(x => x.CreatedAt).Skip(index).Take(count).ToList();

                    var lstCommentCreatedBy = lstComment.Select(x => x.CreatedBy);
                    var lstPoster = DataGemini.TaiKhoanCaNhans.Where(x => lstCommentCreatedBy.Contains(x.Id)).ToList();

                    if (lstComment.Any())
                    {
                        DataReturn.code = ResponseCode.OK.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];
                        var resData = new List<GetCommentResponseData>();
                        foreach (var cmt in lstComment)
                        {
                            var cmtResData = new GetCommentResponseData()
                            {
                                id = cmt.Id,
                                comment = cmt.NoiDungBinhLuan,
                                created = cmt.CreatedAt,
                            };

                            var poster = lstPoster.FirstOrDefault(x => x.Id == cmt.CreatedBy);
                            if (poster != null)
                            {
                                cmtResData.poster = new GetCommentResponseData_Poster()
                                {
                                    id = poster.Id,
                                    name = poster.Ten,
                                    avatar = poster.LinkAvatar,
                                };
                            }

                            resData.Add(cmtResData);
                        }
                        DataReturn.data = resData;
                    }
                    else
                    {
                        DataReturn.code = ResponseCode.NoDataOrEndOfListData.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.NoDataOrEndOfListData];
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

        private bool ValidateGetComment(GetCommentRequest req, TaiKhoanCaNhan acc, BaiVietCaNhan post)
        {
            if (string.IsNullOrWhiteSpace(req.token))
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            if (req.id == null)
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

            if (post == null)
            {
                DataReturn.code = ResponseCode.PostIsNotExisted.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.PostIsNotExisted];

                return false;
            }

            return true;
        }

        #endregion

        #region SetComment

        [HttpPost]
        public ActionResult SetComment(SetCommentRequest req)
        {
            try
            {
                var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.Token == req.token && x.Active == TaiKhoanCaNhan_Active.Actived);
                var post = DataGemini.BaiVietCaNhans.FirstOrDefault(x => x.Id == req.id);

                var valid = ValidateSetComment(req, acc, post);

                if (valid)
                {
                    BinhLuan cmt = new BinhLuan()
                    {
                        BaiVietId = post.Id,
                        NguoiVietBinhLuanId = acc.Id,
                        NoiDungBinhLuan = req.comment,
                        CreatedAt = DateTime.Now,
                        CreatedBy = acc.Id
                    };

                    DataGemini.BinhLuans.Add(cmt);

                    if (SaveData())
                    {
                        DataReturn.code = ResponseCode.OK.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];

                        #region GetComment

                        var lstComment = new List<BinhLuan>();
                        var lstBlockedId = !string.IsNullOrWhiteSpace(acc.ChanIds) ? acc.ChanIds.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();
                        var lstCommentQ = DataGemini.BinhLuans.Where(x => x.BaiVietId == req.id && !lstBlockedId.Contains(x.CreatedBy));

                        int count = req.count != null ? req.count.Value : 50;
                        var index = req.index.GetValueOrDefault(0);
                        lstComment = lstCommentQ.OrderByDescending(x => x.CreatedAt).Skip(index).Take(count).ToList();

                        var lstCommentCreatedBy = lstComment.Select(x => x.CreatedBy);
                        var lstPoster = DataGemini.TaiKhoanCaNhans.Where(x => lstCommentCreatedBy.Contains(x.Id)).ToList();

                        var resData = new List<SetCommentResponseData>();
                        foreach (var comment in lstComment)
                        {
                            var cmtResData = new SetCommentResponseData()
                            {
                                id = comment.Id,
                                comment = comment.NoiDungBinhLuan,
                                created = comment.CreatedAt,
                            };

                            var poster = lstPoster.FirstOrDefault(x => x.Id == comment.CreatedBy);
                            if (poster != null)
                            {
                                cmtResData.poster = new SetCommentResponseData_Poster()
                                {
                                    id = poster.Id,
                                    name = poster.Ten,
                                    avatar = poster.LinkAvatar,
                                };
                            }

                            resData.Add(cmtResData);
                        }
                        DataReturn.data = resData;

                        #endregion
                    }
                    else
                    {
                        DataGemini.BinhLuans.Remove(cmt);

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

        private bool ValidateSetComment(SetCommentRequest req, TaiKhoanCaNhan acc, BaiVietCaNhan post)
        {
            if (string.IsNullOrWhiteSpace(req.token))
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            if (req.id == null)
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

            if (post == null)
            {
                DataReturn.code = ResponseCode.PostIsNotExisted.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.PostIsNotExisted];

                return false;
            }

            return true;
        }

        #endregion
    }
}