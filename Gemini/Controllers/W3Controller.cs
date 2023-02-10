using System;
using System.IO;
using System.Web;
using System.Linq;
using Gemini.Models;
using System.Web.Mvc;
using Gemini.Controllers.Common;
using System.Collections.Generic;

namespace Gemini.Controllers
{
    public class W3Controller : BaseController
    {
        #region GetPost

        [HttpPost]
        public ActionResult GetPost(GetPostRequest req)
        {
            try
            {
                var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.Token == req.token && x.Active == TaiKhoanCaNhan_Active.Actived);
                var post = DataGemini.BaiVietCaNhans.FirstOrDefault(x => x.Id == req.id);

                var valid = ValidateGetPost(req, acc, post);

                if (valid)
                {
                    TaiKhoanCaNhan accCreatePost = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.Id == post.ChuBaiVietId && x.Active == TaiKhoanCaNhan_Active.Actived);

                    if (accCreatePost != null && accCreatePost.ChanIds != null && accCreatePost.ChanIds.Split(';').Contains(acc.Id.ToString()))
                    {
                        DataReturn.code = ResponseCode.OK.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];
                        DataReturn.data = new GetPostResponseData()
                        {
                            is_blocked = true,
                        };
                    }
                    else
                    {
                        var lstNguoiThich = !string.IsNullOrWhiteSpace(post.NguoiThichIds) ? post.NguoiThichIds.Split(';').ToList() : new List<string>();
                        var lstBinhLuan = !string.IsNullOrWhiteSpace(post.BinhLuanIds) ? post.BinhLuanIds.Split(';').ToList() : new List<string>();
                        List<GetPostResponseData_Image> imgs = DataGemini.BaiVietCaNhanAnhs.Where(x => x.BaiVietId == post.Id)
                                                                                           .OrderBy(x => x.Sort)
                                                                                           .Select(x => new GetPostResponseData_Image
                                                                                           {
                                                                                               id = x.Id,
                                                                                               img = x.Image
                                                                                           }).ToList();

                        DataReturn.code = ResponseCode.OK.ToString();
                        DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];
                        DataReturn.data = new GetPostResponseData()
                        {
                            id = post.Id,
                            described = post.NoiDungBaiViet,
                            created = post.CreatedAt,
                            modified = post.UpdatedAt,
                            like = lstNguoiThich.Count(),
                            comment = lstBinhLuan.Count(),
                            is_liked = lstNguoiThich.Contains(acc.Id.ToString()),
                            image = imgs,
                            video = new GetPostResponseData_Video()
                            {
                                vid = post.Media,
                                thumb = post.MediaThumb
                            },
                            author = new GetPostResponseData_Author()
                            {
                                id = accCreatePost.Id,
                                name = accCreatePost.Ten,
                                avatar = accCreatePost.LinkAvatar,
                            },
                            state = post.Status,
                            is_blocked = false,
                            can_edit = acc.Token == accCreatePost.Token,
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                DataReturn.code = ResponseCode.ExceptionError.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.ExceptionError];
                DataReturn.data = ex.ToString();
            }

            var jsonResult = Json(DataReturn, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        private bool ValidateGetPost(GetPostRequest req, TaiKhoanCaNhan acc, BaiVietCaNhan post)
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

            // TODO: DuongNH: Confirm thế nào là bài viết vi phạm tiêu chuẩn cộng đồng hoặc bị hạn chế tại quốc gia ?
            //                Lấy và lưu trạng thái vi phạm ở đâu ?

            if (post == null)
            {
                DataReturn.code = ResponseCode.PostIsNotExisted.ToString();
                DataReturn.message = ResponseCode.dicDesc[ResponseCode.PostIsNotExisted];

                return false;
            }

            return true;
        }

        #endregion

        #region EditPost

        [HttpPost]
        public ActionResult EditPost(EditPostRequest req)
        {
            try
            {
                var acc = DataGemini.TaiKhoanCaNhans.FirstOrDefault(x => x.Token == req.token && x.Active == TaiKhoanCaNhan_Active.Actived);
                var post = DataGemini.BaiVietCaNhans.FirstOrDefault(x => x.Id == req.id);

                var valid = ValidateEditPost(req, acc, post);

                if (valid)
                {
                    post.NoiDungBaiViet = !string.IsNullOrWhiteSpace(req.described) ? req.described : post.NoiDungBaiViet;
                    post.Status = !string.IsNullOrWhiteSpace(req.status) ? req.status : post.Status;

                    List<BaiVietCaNhanAnh> imgsNew = new List<BaiVietCaNhanAnh>();
                    List<BaiVietCaNhanAnh> imgsToDel = new List<BaiVietCaNhanAnh>();
                    List<BaiVietCaNhanAnh> imgsInDB = DataGemini.BaiVietCaNhanAnhs.Where(x => x.BaiVietId == post.Id).ToList();

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

                            var thumb = Request.Files["thumb"];

                            if (thumb != null)
                            {
                                using (Stream inputStream = thumb.InputStream)
                                {
                                    MemoryStream memoryStream = inputStream as MemoryStream;
                                    if (memoryStream == null)
                                    {
                                        memoryStream = new MemoryStream();
                                        inputStream.CopyTo(memoryStream);
                                    }
                                    post.MediaThumb = memoryStream.ToArray();
                                }
                            }

                            var lstImg = Request.Files.GetMultiple("image").ToList();
                            if (lstImg != null && lstImg.Any())
                            {
                                imgsNew = AddPostAnh(post.Id, lstImg, req.image_sort, (imgsInDB.Max(x => x.Sort)).GetValueOrDefault(0));
                            }
                        }

                        if (req.image_del != null && req.image_del.Any())
                        {
                            imgsToDel = imgsInDB.Where(x => req.image_del.Contains(x.Id)).ToList();
                        }

                        if (imgsInDB.Count + imgsNew.Count - imgsToDel.Count > 4)
                        {
                            DataReturn.code = ResponseCode.MaximumNumberOfImages.ToString();
                            DataReturn.message = ResponseCode.dicDesc[ResponseCode.MaximumNumberOfImages];
                        }
                        else
                        {
                            DataGemini.BaiVietCaNhanAnhs.AddRange(imgsNew);
                            DataGemini.BaiVietCaNhanAnhs.RemoveRange(imgsToDel);

                            if (SaveData())
                            {
                                DataReturn.code = ResponseCode.OK.ToString();
                                DataReturn.message = ResponseCode.dicDesc[ResponseCode.OK];
                            }
                            else
                            {
                                if (imgsNew.Any())
                                    DataGemini.BaiVietCaNhanAnhs.RemoveRange(imgsNew);
                                if (imgsToDel.Any())
                                    DataGemini.BaiVietCaNhanAnhs.AddRange(imgsToDel);

                                DataReturn.code = ResponseCode.UnknownError.ToString();
                                DataReturn.message = ResponseCode.dicDesc[ResponseCode.UnknownError];
                            }
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

        private bool ValidateEditPost(EditPostRequest req, TaiKhoanCaNhan acc, BaiVietCaNhan post)
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

        private List<BaiVietCaNhanAnh> AddPostAnh(int baiVietCaNhanId, List<HttpPostedFileBase> lstImg, List<int> image_sort, int maxSort)
        {
            List<BaiVietCaNhanAnh> imgs = new List<BaiVietCaNhanAnh>();

            int index = 0;
            foreach (var img in lstImg)
            {
                int sort = maxSort;
                try
                {
                    sort = image_sort[index];
                }
                catch
                {
                    maxSort++;
                    sort = maxSort;
                }

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
                index++;
            }

            return imgs;
        }

        #endregion
    }
}