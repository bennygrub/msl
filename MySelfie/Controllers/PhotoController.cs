using AttributeRouting.Web.Mvc;
using MySelfie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Shared.Extensions;
using System.Threading;

namespace MySelfie.Controllers
{
    public class PhotoController : Controller
    {
        [GET("/Admin/Photo/Approve", RouteName = "photo_approval_g")]
        public ActionResult Approval(int? id)
        {
            var model = new PhotoApprovalViewModel();

            if (id.HasValue)
                model.WallId = id.Value;

            model.FetchLatestAmount = 10;
            model.FetchLatestSpeedInSeconds = 4;

            try
            {
                using (var db = new MySelfieEntities())
                {
                    var wall = db.Walls
                        .Where(x => x.WallId == model.WallId)
                        .SingleOrDefault();

                    if (wall.IsNotNull())
                    {
                        //model.FetchLatestAmount = wall.
                        //model.FetchLatestSpeedInSeconds = 
                    }

                    var query = db.Photos
                        .Where(x => x.HasPhoto == true)
                        .Where(x => x.Status == "new");

                    if (id.HasValue)
                        query = query.Where(x => x.WallId == id.Value);

                    var results = query
                        .OrderBy(x => x.CreatedAt)
                        .Select(x => new PhotoViewModel
                        {
                            FileName = x.Filename,
                            PhotoId = x.PhotoId,
                            Text = x.Text,
                            SocialId = x.SocialID,
                            SocialCreated = x.SocialCreatedAt,
                            UserScreenName = x.Username,
                            Source = x.Source
                        })
                        .Take(model.FetchLatestAmount)                        
                        .ToList();

                    var photosLeft = db.Photos
                            .Where(x => x.HasPhoto == true)
                            .Where(x => x.Status == "new")
                            .Where(x => x.WallId == model.WallId)
                            .Count();

                    var message = "Starting Approval page." + Environment.NewLine
                        + "WallID: " + model.WallId + Environment.NewLine
                        + "New photos left: " + photosLeft + Environment.NewLine
                        + "FetchLatestAmount: " + model.FetchLatestAmount + Environment.NewLine
                        + "FetchLatestSpeedInSeconds: " + model.FetchLatestSpeedInSeconds + Environment.NewLine;

                    Logger.Log(message, "update", User.Identity.Name, "/Photo/Latest");


                    if (results.Any())
                    {
                        model.ImageList = results;
                        model.LastPhotoTweetId = results.Select(x => x.PhotoId).Last();

                        return View(model);
                    }
                    else
                    {
                        model.ImageList = results;
                        model.LastPhotoTweetId = 0;

                        
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex);

                Logger.Log(ex.ToString(), "error", User.Identity.Name, "/Admin/Photo/Approve");
            }

            return View();
        } //Approve MVC5 endpoint

        //Method to dynamically-ajaxily get pages of comments awaiting approval
        [GET("/Photo/Latest", RouteName = "photo_latest_g")]
        public ActionResult Latest(int? id, PhotoApprovalViewModel model)
        {
            try
            {
                using (var db = new MySelfieEntities())
                {
                    if (model.IsNull())
                        model = new PhotoApprovalViewModel();

                    if (id.HasValue)
                        model.LastPhotoTweetId = id.Value;

                    if (model.FetchLatestAmount.IsLessThanOne())
                        model.FetchLatestAmount = 10;

                    if (model.FetchLatestSpeedInSeconds.IsLessThanOne())
                        model.FetchLatestSpeedInSeconds = 5;

                    var query = db.Photos
                        .Where(x => x.HasPhoto == true)
                        .Where(x => x.Status == "new")
                        //.Where(x => x.Source == "Twitter")
                        .Where(x => x.PhotoId > model.LastPhotoTweetId);

                    if (model.WallId > 0)
                        query = query.Where(x => x.WallId == model.WallId);

                    model.ImageList = query
                        .OrderBy(x => x.CreatedAt)
                        .Take(model.FetchLatestAmount)
                        .Select(x => new PhotoViewModel
                        {
                            FileName = x.Filename,
                            PhotoId = x.PhotoId,
                            Text = x.Text,
                            SocialId = x.SocialID,
                            SocialCreated = x.SocialCreatedAt,
                            UserScreenName = x.Username,
                            Source = x.Source
                        })
                        .ToList();

                    if (model.ImageList.Any())
                    {
                        model.LastPhotoTweetId = model.ImageList.Select(x => x.PhotoId).Last();
                    }
                    else
                    {
                        if (id.HasValue)
                        {
                            model.LastPhotoTweetId = id.Value;
                        }

                    }

                    var photosLeft = db.Photos
                        .Where(x => x.HasPhoto == true)
                        .Where(x => x.Status == "new")
                        .Where(x => x.Source == "Twitter")
                        .Where(x => x.WallId == model.WallId)
                        .Count();

                    var message = "Got more images for approval." + Environment.NewLine
                        + "WallID: " + model.WallId + Environment.NewLine
                        + "New photos left: " + photosLeft + Environment.NewLine;

                    //Logger.Log(message, "update", User.Identity.Name, "/Photo/Latest");

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex);

                Logger.Log(ex.ToString(), "error", User.Identity.Name, "/Photo/Latest");

                return View(new PhotoApprovalViewModel());
            }
        }


        //MVC5 comment approval endpoint
        [GET("/Admin/Photo/CommentApproval", RouteName = "photo_approval_comment")]
        public ActionResult CommentApproval(int? id)
        {
            var model = new PhotoApprovalViewModel();

            if (id.HasValue)
                model.WallId = id.Value;

            model.FetchLatestAmount = 10;
            model.FetchLatestSpeedInSeconds = 4;

            try
            {
                using (var db = new MySelfieEntities())
                {
                    var wall = db.Walls
                        .Where(x => x.WallId == model.WallId)
                        .SingleOrDefault();

                    if (wall.IsNotNull())
                    {
                        //model.FetchLatestAmount = wall.
                        //model.FetchLatestSpeedInSeconds = 
                    }

                    var query = db.Photos
                        .Where(x => x.HasPhoto == true)
                        .Where(x => x.Status == "packed");

                    if (id.HasValue)
                        query = query.Where(x => x.WallId == id.Value);

                    var results = query
                        .OrderBy(x => x.CreatedAt)
                        .Select(x => new PhotoViewModel
                        {
                            FileName = x.Filename,
                            PhotoId = x.PhotoId,
                            Text = x.Text,
                            SocialId = x.SocialID,
                            SocialCreated = x.SocialCreatedAt,
                            UserScreenName = x.Username,
                            Source = x.Source
                        })
                        .Take(model.FetchLatestAmount)
                        .ToList();

                    var photosLeft = db.Photos
                            .Where(x => x.HasPhoto == true)
                            .Where(x => x.Status == "packed")
                            .Where(x => x.WallId == model.WallId)
                            .Count();

                    var message = "Starting CommentApproval page." + Environment.NewLine
                        + "WallID: " + model.WallId + Environment.NewLine
                        + "New photos left: " + photosLeft + Environment.NewLine
                        + "FetchLatestAmount: " + model.FetchLatestAmount + Environment.NewLine
                        + "FetchLatestSpeedInSeconds: " + model.FetchLatestSpeedInSeconds + Environment.NewLine;

                    Logger.Log(message, "update", User.Identity.Name, "/Photo/Latest");


                    if (results.Any())
                    {
                        model.ImageList = results;
                        model.LastPhotoTweetId = results.Select(x => x.PhotoId).Last();

                        return View(model);
                    }
                    else
                    {
                        model.ImageList = results;
                        model.LastPhotoTweetId = 0;


                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex);

                Logger.Log(ex.ToString(), "error", User.Identity.Name, "/Admin/Photo/CommentApproval");
            }

            return View();
        }

        //Method to dynamically-ajaxily get pages of comments awaiting approval
        [GET("/Photo/LatestReadyForComments", RouteName = "photo_latest_for_comments")]
        public ActionResult LatestReadyForComments(int? id, PhotoApprovalViewModel model)
        {
            try
            {
                using (var db = new MySelfieEntities())
                {
                    if (model.IsNull())
                        model = new PhotoApprovalViewModel();

                    if (id.HasValue)
                        model.LastPhotoTweetId = id.Value;

                    if (model.FetchLatestAmount.IsLessThanOne())
                        model.FetchLatestAmount = 10;

                    if (model.FetchLatestSpeedInSeconds.IsLessThanOne())
                        model.FetchLatestSpeedInSeconds = 5;

                    var query = db.Photos
                        .Where(x => x.HasPhoto == true)
                        .Where(x => x.Status == "packed")
                        //.Where(x => x.Source == "Twitter")
                        .Where(x => x.PhotoId > model.LastPhotoTweetId);

                    if (model.WallId > 0)
                        query = query.Where(x => x.WallId == model.WallId);

                    model.ImageList = query
                        .OrderBy(x => x.CreatedAt)
                        .Take(model.FetchLatestAmount)
                        .Select(x => new PhotoViewModel
                        {
                            FileName = x.Filename,
                            PhotoId = x.PhotoId,
                            Text = x.Text,
                            SocialId = x.SocialID,
                            SocialCreated = x.SocialCreatedAt,
                            UserScreenName = x.Username,
                            Source = x.Source
                        })
                        .ToList();

                    if (model.ImageList.Any())
                    {
                        model.LastPhotoTweetId = model.ImageList.Select(x => x.PhotoId).Last();
                    }
                    else
                    {
                        if (id.HasValue)
                        {
                            model.LastPhotoTweetId = id.Value;
                        }

                    }

                    var photosLeft = db.Photos
                        .Where(x => x.HasPhoto == true)
                        .Where(x => x.Status == "packed")
                        .Where(x => x.Source == "Twitter")
                        .Where(x => x.WallId == model.WallId)
                        .Count();

                    var message = "Got more images for comments." + Environment.NewLine
                        + "WallID: " + model.WallId + Environment.NewLine
                        + "New photos left: " + photosLeft + Environment.NewLine;

                    //Logger.Log(message, "update", User.Identity.Name, "/Photo/Latest");

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex);

                Logger.Log(ex.ToString(), "error", User.Identity.Name, "/Photo/LatestForComments");

                return View(new PhotoApprovalViewModel());
            }
        }

        [POST("/Photo/SendComment", RouteName = "photo_sendcomment")]
        public async Task<JsonResult> SendComment(PhotoStatusChangeModel model)
        {
            try
            {
                using (var db = new MySelfieEntities())
                {
                    var photo = db.Photos.Single(x => x.PhotoId == model.PhotoTweetId);

                    var wall = photo.Wall;

                    string token = wall.PostingAccount_InstagramPassword;
                    string id = photo.SocialIDstring;
                    string template = wall.RetweetMessage;
                    string username = photo.Username;

                    //I should not have put all of this should not be in the controller, but the comment sending was really
                    //Couple to the batch creation
                    string response = "";
                    var success = true;


                    if (photo.Source == "Twitter")
                    {
                        var context = PhotoStatusChangeModel.GetTwitterContext(new PhotoStatusChangeModel.WallModel(wall));
                        response = await PhotoStatusChangeModel.PublishTweet(photo.Username, DateTime.UtcNow, wall.RetweetMessage, context);
                    }
                    if (photo.Source == "Instagram")
                    {
                        response = PhotoStatusChangeModel.PublishInstagramComment(photo.Username, DateTime.UtcNow, wall.RetweetMessage, wall.PostingAccount_InstagramToken, photo.SocialIDstring);
                    }
                    if (response == "")
                    {
                        photo.Status = "commentSent";
                    }
                    else 
                    {
                        success = false;
                    }
                    db.SaveChanges();

                    var stuff = new { success = success, response = response };
                    return (Json(stuff));
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), "error", User.Identity.Name, "/Photo/Status");

                return Json(new { error = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }



        [GET("/Photo/Stream", RouteName = "photo_stream_g")]
        public JsonResult Stream(int? id)
        {
            try
            {
                using (var db = new MySelfieEntities())
                {
                    var query = db.Photos
                        .Where(x => x.HasPhoto == true)
                        .Where(x => x.Source == "Twitter")
                        .Where(x => x.Status == "new");

                    if (id.HasValue)
                    {
                        query = query.Where(x => x.PhotoId > id);
                    }

                    var results = query
                        .OrderBy(x => x.CreatedAt)
                        .Take(10)
                        .Select(x => new PhotoViewModel
                        {
                            FileName = x.Filename,
                            PhotoId = x.PhotoId,
                            Text = x.Text,
                            SocialId = x.SocialID,
                            SocialCreated = x.SocialCreatedAt,
                            UserScreenName = x.Username
                        })
                        .ToList();

                    return Json(new { data = results }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), "error", User.Identity.Name, "/Photo/Stream");

                return Json(new { error = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [POST("/Photo/Status", RouteName = "photo_status_p")]
        public async Task<JsonResult> Status(PhotoStatusChangeModel model)
        {
            try
            {
                using (var db = new MySelfieEntities())
                {
                    var entity = db.Photos.Single(x => x.PhotoId == model.PhotoTweetId);

                    entity.Status = model.Status;
                    entity.Approved = model.Approved;
                    entity.ApprovedAt = DateTime.UtcNow;

                    db.SaveChanges();

                    if (model.Status == "approved")
                    {
                        //var t = new Thread(() => PhotoStatusChangeModel.CreateBatch(model.WallId));
                        //t.Start();
                        //await PhotoStatusChangeModel.CreateBatch(model.WallId);
                        await PhotoStatusChangeModel.CreateBatch(model.WallId);
                    }

                    Logger.Log("PhotoID " + model.PhotoTweetId.ToString() + " status changed to: " + model.Status, "info", User.Identity.Name, "/Photo/Status");

                    return Json(new { data = new PhotoViewModel(entity), success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), "error", User.Identity.Name, "/Photo/Status");

                return Json(new { error = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [GET("/Viewer", RouteName = "photo_viewer_g")]
        public ActionResult Viewer(int? id)
        {
            try
            {
                using (var db = new MySelfieEntities())
                {
                    var query = db.Packets
                        .Where(x => x.Status == "new" || x.Status == "sent");

                    if (id.HasValue)
                    {
                        ViewBag.WallId = id.Value;
                        var wall = db.Walls.Single(x => x.WallId == id.Value);
                        wall.Status = "running";

                        ViewBag.Caption = wall.CaptionText;
                        ViewBag.Title = wall.Title;
                        ViewBag.Left = wall.DescriptionText;
                        ViewBag.Right = wall.RightText;
                        ViewBag.TopColor = wall.FrameTopColor;
                        ViewBag.BottomColor = wall.FrameBottomColor;
                        ViewBag.Interval = wall.PhotoShownLengthSecond * 1000;
                        ViewBag.AnimationDuration = 800;
                        ViewBag.GridShowDuration = 500;
                        ViewBag.GridStagger = 170;
                        ViewBag.FinalInterval = wall.AdShownLengthSecond * 1000;

                        query = query.Where(x => x.WallId == id.Value);
                    }

                    query = query
                        .OrderBy(x => x.StartTime);

                    var entity = query.First();

                    entity.Status = "viewed";                                       

                    var packets = query.Select(x => new PhotoPacketModel
                    {
                        Data = x.JSONBody,
                        PacketId = x.PacketId,
                        Status = x.Status
                    })
                    .ToArray();

                    db.SaveChanges();

                    var message = "Viewer started!" + Environment.NewLine
                        + "WallID: " + ViewBag.WallId + Environment.NewLine
                        + "Caption: " + ViewBag.Caption + Environment.NewLine
                        + "Title: " + ViewBag.Title + Environment.NewLine
                        + "Left: " + ViewBag.Left + Environment.NewLine
                        + "Right: " + ViewBag.Right + Environment.NewLine
                        + "TopColor" + ViewBag.TopColor + Environment.NewLine
                        + "BottomColor" + ViewBag.BottomColor + Environment.NewLine
                        + "Interval" + ViewBag.Interval + Environment.NewLine
                        + "AnimationDuration: " + ViewBag.AnimationDuration + Environment.NewLine
                        + "GridShowDuration: " + ViewBag.GridShowDuration + Environment.NewLine
                        + "GridStagger: " + ViewBag.GridStagger + Environment.NewLine
                        + "FinalInterval: " + ViewBag.FinalInterval + Environment.NewLine;

                    Logger.Log(message, "info", User.Identity.Name, "/Viewer");

                    return View(packets);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex);

                Logger.Log(ex.ToString(), "error", User.Identity.Name, "/Viewer");
            }

            return View();
        }

        [GET("API/Photo/Packet", RouteName = "photo_packet_json_g")]
        public JsonResult Packet(int? id)
        {
            try
            {
                using (var db = new MySelfieEntities())
                {
                    var query = db.Packets.Where(x => x.Status == "new" || x.Status == "sent");

                    if (id.HasValue)
                        query = query.Where(x => x.WallId == id.Value);

                    query = query
                        .OrderBy(x => x.StartTime)
                        .Take(4);

                    foreach (var entity in query.ToList())
                    {
                        entity.Status = "sent";
                    }

                    db.SaveChanges();

                    var packets = query.Select(x => new PhotoPacketModel
                    {
                        Data = x.JSONBody,
                        PacketId = x.PacketId,
                        Status = x.Status
                    })
                    .ToArray();

                    var message = "Packets attempted to be pulled..." + Environment.NewLine
                        + "Count: " + packets.Count() + Environment.NewLine
                        + "Content-------------" + Environment.NewLine;

                    foreach(var p in packets)
                    {
                        message += "Data: " + p.Data + Environment.NewLine;
                    }

                    Logger.Log(message, "update", User.Identity.Name, "API/Photo/Packet");

                    return Json(new {
                        packets = packets                   
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), "error", User.Identity.Name, "API/Photo/Packet");

                return Json(new { error = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [POST("API/Photo/Packet/Status", RouteName = "photo_packet_status_json_p")]
        public JsonResult PacketStatus(PhotoPacketStatusChangeModel model)
        {
            try
            {
                var success = false;

                using (var db = new MySelfieEntities())
                {
                    var entity = db.Packets.SingleOrDefault(x => x.PacketId == model.PacketId);

                    if (entity.IsNotNull())
                    {
                        entity.Status = model.Status;

                        db.SaveChanges();

                        success = true;
                    }
                }

                return Json(new { success = success });
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), "error", User.Identity.Name, "API/Photo/Packet");

                return Json(new { error = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [GET("API/Photo/Packet/List", RouteName = "photo_packet_list_json_g")]
        public JsonResult PacketList(int? id)
        {
            try
            {
                using (var db = new MySelfieEntities())
                {
                    var query = db.Packets.Where(x => x.Status != "deleted");

                    if (id.HasValue)
                        query = query.Where(x => x.WallId == id.Value);

                    query = query
                        .OrderBy(x => x.StartTime);

                    var packets = query.ToList();
 
                    return Json(new
                    {
                        packets = packets
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), "error", User.Identity.Name, "API/Photo/Packet/List");

                return Json(new { error = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [GET("API/Photo/Count", RouteName = "photo_count_json_g")]
        public JsonResult Count(int? id, string status)
        {
            try
            {
                using (var db = new MySelfieEntities())
                {
                    var query = db.Packets.AsQueryable();
                    
                    if (status.IsNotEmptyOrNull())
                        query.Where(x => x.Status == status);

                    if (id.HasValue)
                        query = query.Where(x => x.WallId == id.Value);

                    var count = query.Count();

                    return Json(new
                    {
                        count = count
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), "error", User.Identity.Name, "API/Photo/Count");

                return Json(new { error = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}