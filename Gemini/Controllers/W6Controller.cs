using System;
using System.Linq;
using Gemini.Models;
using System.Web.Mvc;
using Gemini.Controllers.Common;
using System.Collections.Generic;

namespace Gemini.Controllers
{
    public class W6Controller : BaseController
    {
        #region Search

        [HttpPost]
        public ActionResult Search(SearchRequest req)
        {
            try
            {
                var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.Token == req.token && x.Active == TaiKhoanCaNhan_Active.Actived);

                var valid = ValidateSearch(req, acc);

                if (valid)
                {
                    var keywordRemoveUnicode = RemoveSign4VietnameseString(req.keyword);
                    var lstKeyWord = req.keyword.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    var lstPost = new List<BaiVietCaNhan>();
                    var lstBlockedId = !string.IsNullOrWhiteSpace(acc.ChanIds) ? acc.ChanIds.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();
                    var lstPostQ = DataGemini.BaiVietCaNhans.Where(x => !lstBlockedId.Contains(x.CreatedBy)).ToList();

                    foreach (var post in lstPostQ)
                    {
                        if (!string.IsNullOrWhiteSpace(post.NoiDungBaiViet))
                        {
                            var postValid = false;

                            if (post.NoiDungBaiViet.Contains(keywordRemoveUnicode))
                                postValid = true;

                            var lenghValid = (int)Math.Ceiling(lstKeyWord.Count * 0.2);
                            for (int i = 0; i < lstKeyWord.Count; i++)
                            {
                                var newKeyWord = string.Join(" ", lstKeyWord.Skip(i).Take(lenghValid));
                                if (!string.IsNullOrWhiteSpace(newKeyWord) && post.NoiDungBaiViet.Contains(newKeyWord))
                                {
                                    postValid = true;
                                    break;
                                }
                            }

                            int validUnicodeIndex = 0;
                            foreach (var word in lstKeyWord)
                            {
                                if (post.NoiDungBaiViet.Contains(word))
                                    validUnicodeIndex++;
                            }
                            if (validUnicodeIndex == lstKeyWord.Count)
                                postValid = true;

                            if (post.NoiDungBaiViet.Contains(req.keyword))
                                postValid = true;

                            if (postValid)
                                lstPost.Add(post);
                        }
                    }

                    int count = req.count.GetValueOrDefault(10);
                    var index = req.index.GetValueOrDefault(0);
                    lstPost = lstPost.OrderByDescending(x => x.CreatedAt).Skip(index).Take(count).ToList();

                    var lstPostCreatedBy = lstPost.Select(x => x.CreatedBy);
                    var lstAuthor = DataGemini.TaiKhoanCaNhans.Where(x => lstPostCreatedBy.Contains(x.Id)).ToList();

                    var lstPostId = lstPost.Select(x => (int?)x.Id);
                    List<SearchResponseData_Image> lstPostImgs = DataGemini.BaiVietCaNhanAnhs.Where(x => lstPostId.Contains(x.BaiVietId))
                                                                                             .OrderBy(x => x.Sort)
                                                                                             .Select(x => new SearchResponseData_Image
                                                                                             {
                                                                                                 id = x.Id,
                                                                                                 baiVietId = x.BaiVietId,
                                                                                                 img = x.Image
                                                                                             }).ToList();

                    LichSuTimKiem hisSearch = new LichSuTimKiem()
                    {
                        TuKhoa = req.keyword,
                        CreatedAt = DateTime.Now,
                        CreatedBy = req.user_id.GetValueOrDefault(0),
                    };

                    DataGemini.LichSuTimKiems.Add(hisSearch);

                    if (SaveData())
                    {
                        if (lstPost.Any())
                        {
                            DataReturn.code = ResponseCode.OK.ToString();
                            DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];

                            var resData = new List<SearchResponseData>();
                            foreach (var post in lstPost)
                            {
                                var imgs = lstPostImgs.Where(x => x.baiVietId == post.Id).ToList();
                                var accCreatePost = lstAuthor.FirstOrDefault(x => x.Id == post.CreatedBy);
                                var lstBinhLuan = !string.IsNullOrWhiteSpace(post.BinhLuanIds) ? post.BinhLuanIds.Split(';').ToList() : new List<string>();
                                var lstNguoiThich = !string.IsNullOrWhiteSpace(post.NguoiThichIds) ? post.NguoiThichIds.Split(';').ToList() : new List<string>();

                                var cmtResData = new SearchResponseData()
                                {
                                    id = post.Id,
                                    described = post.NoiDungBaiViet,
                                    like = lstNguoiThich.Count(),
                                    comment = lstBinhLuan.Count(),
                                    is_liked = lstNguoiThich.Contains(acc.Id.ToString()),
                                    image = imgs,
                                    video = new SearchResponseData_Video()
                                    {
                                        vid = post.Media,
                                        thumb = post.MediaThumb
                                    },
                                    author = new SearchResponseData_Author()
                                    {
                                        id = accCreatePost.Id,
                                        name = accCreatePost.Ten,
                                        avatar = accCreatePost.LinkAvatar,
                                    },
                                };

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

        private bool ValidateSearch(SearchRequest req, TaiKhoanCaNhan acc)
        {
            if (string.IsNullOrWhiteSpace(req.token))
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            if (string.IsNullOrWhiteSpace(req.keyword))
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            if (req.user_id == null)
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

            if (acc != null && acc.Id != req.user_id)
            {
                DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                return false;
            }

            return true;
        }

        #endregion

        #region GetSavedSearch

        [HttpPost]
        public ActionResult GetSavedSearch(GetSavedSearchRequest req)
        {
            try
            {
                var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.Token == req.token && x.Active == TaiKhoanCaNhan_Active.Actived);

                var valid = ValidateGetSavedSearch(req, acc);

                if (valid)
                {
                    int count = req.count.GetValueOrDefault(10);
                    var index = req.index.GetValueOrDefault(0);
                    var lstHisSearch = DataGemini.LichSuTimKiems.Where(x => x.CreatedBy == acc.Id).OrderByDescending(x => x.CreatedAt).Skip(index).Take(count)
                                                                .Select(x => new GetSavedSearchResponseData()
                                                                {
                                                                    id = x.Id,
                                                                    keyword = x.TuKhoa,
                                                                    created = x.CreatedAt
                                                                }).ToList();

                    if (lstHisSearch != null && lstHisSearch.Any())
                    {
                        DataReturn.code = ResponseCode.OK.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];
                        DataReturn.data = lstHisSearch;
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

        private bool ValidateGetSavedSearch(GetSavedSearchRequest req, TaiKhoanCaNhan acc)
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

        #region DelSavedSearch

        [HttpPost]
        public ActionResult DelSavedSearch(DelSavedSearchRequest req)
        {
            try
            {
                var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.Token == req.token && x.Active == TaiKhoanCaNhan_Active.Actived);
                int idCreatedBy = (acc?.Id).GetValueOrDefault(0);
                var searchHis = DataGemini.LichSuTimKiems.FirstOrDefault(x => x.Id == req.search_id && x.CreatedBy == idCreatedBy);

                var valid = ValidateDelSavedSearch(req, acc, searchHis);

                if (valid)
                {
                    var lstSearchHis = DataGemini.LichSuTimKiems.Where(x => x.CreatedBy == idCreatedBy);

                    if (req.all.GetValueOrDefault(false))
                    {
                        DataGemini.LichSuTimKiems.RemoveRange(lstSearchHis);
                    }
                    else
                    {
                        DataGemini.LichSuTimKiems.Remove(searchHis);
                    }

                    if (SaveData())
                    {
                        DataReturn.code = ResponseCode.OK.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];
                    }
                    else
                    {
                        if (req.all.GetValueOrDefault(false))
                        {
                            DataGemini.LichSuTimKiems.AddRange(lstSearchHis);
                        }
                        else
                        {
                            DataGemini.LichSuTimKiems.Add(searchHis);
                        }

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

        private bool ValidateDelSavedSearch(DelSavedSearchRequest req, TaiKhoanCaNhan acc, LichSuTimKiem searchHis)
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

            if (!req.all.GetValueOrDefault(false))
            {
                if (req.search_id == null || searchHis == null)
                {
                    DataReturn.code = ResponseCode.ParameterValueIsInvalid.ToString();
                    DataReturn.message = ResponseCode.dicDesc[ResponseCode.ParameterValueIsInvalid];

                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}