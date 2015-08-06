using AttributeRouting.Web.Mvc;
using MySelfie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shared.Extensions;
using System.IO;
using System.Data.Entity.Validation;
using System.Net.Http.Formatting;

namespace MySelfie.Controllers
{
    public class WallController : Controller
    {
        [GET("/Admin", RouteName = "wall_index_g")]
        public ActionResult Index() // to later include an organization id.
        {
            var model = new WallListViewModel();

            try
            {
                using (var db = new MySelfieEntities())
                {
                    //https://instagram.com/oauth/authorize/?scope=likes+relationships&response_type=code&redirect_uri=https%3A%2F%2Fapigee.com%2Foauth_callback%2Finstagram%2Foauth2CodeCallback&client_id=1fb234f05ed1496a9eb35458be5d2c5c
                    var url_start = "";
                    url_start += "https://api.instagram.com/oath/authorize/";
                    url_start += "?client_id=";
                    var url_end = "&redirect_uri=" + Url.Encode("http://myselfiest.azurewebsites.net/Auth/Instagram/");
                    url_end += "&response_type=code";
                    //url_end += "&state=";
                    model.WallList = db.Walls
                        .Where(x => x.Status != "deleted")                        
                        .Select(x => new WallListItemViewModel()
                        {
                            Hashtag = x.Hashtag,
                            Name = x.Name,
                            TwitterUserName = x.TwitterUserName,
                            Scrape_TwitterUserName = x.Scrape_TwitterUserName,
                            WallId = x.WallId,
                            IsActive = x.IsActive,
                            Status = x.Status,
                            PendingApproval = x.Photos.Where(y => y.HasPhoto == true).Count(y => y.Status == "new"),
                            Instagram_Redirect_URL = url_start + x.Scrape_InstagramClientID + url_end     // the wallID here goes to instagram and then back to our endpoint when we get the token
                        })
                        .ToList();
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex);

                Logger.Log(ex.ToString(), "error", User.Identity.Name, "GET /Admin");
            }

            return View(model);
        }

        [GET("/Wall/Create", RouteName = "wall_create_g")]
        public ActionResult Create()
        {
            return View();
        }

        [POST("/Wall/Create", RouteName = "wall_create_p")]
        [ValidateInput(false)]
        public ActionResult Create(WallModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new MySelfieEntities())
                {
                    try
                        {
                            Wall wall;

                            wall = new Wall();

                            wall.MergeWithOtherType(model);

                            if (model.LogoPath.IsEmptyOrNull()) wall.LogoPath = "";
                            if (model.FrameTopColor.IsEmptyOrNull()) wall.FrameTopColor = "#DDDDDD";
                            if (model.FrameBottomColor.IsEmptyOrNull()) wall.FrameBottomColor = "#FFFFFF";
                            if (model.RetweetMessage.IsEmptyOrNull()) wall.RetweetMessage = "";
                                
                            if (model.CaptionText.IsEmptyOrNull()) wall.CaptionText = "";
                            if (model.DescriptionText.IsEmptyOrNull()) wall.DescriptionText = "";
                            if (model.RightText.IsEmptyOrNull()) wall.RightText = "";
                            if (model.Title.IsEmptyOrNull()) wall.Title = "";

                            if (model.LogoImageFile.HasFile())
                            {
                                wall.LogoImage = model.LogoImageFile.getFileBytes();    // extracts bytes from posted file
                                wall.LogoImageType = model.LogoImageFile.getFileType(); // extracts type from posted file
                            }
                            //Johnm - some mismatches on column naming, thus mergeWithOtherType not working
                            wall.PostingAccount_InstagramToken = model.Post_InstagramToken;

                            wall.IsActive = false;
                            wall.CreatedAt = DateTime.UtcNow;
                            wall.Status = "new";

                            db.Walls.Add(wall);

                            wall.Name = model.Name;

                            db.SaveChanges();
                        }
                    catch (DbEntityValidationException ex)
                    {
                        // Retrieve the error messages as a list of strings.
                        var errorMessages = ex.EntityValidationErrors
                                .SelectMany(x => x.ValidationErrors)
                                .Select(x => x.ErrorMessage);

                        // Join the list to a single string.
                        var fullErrorMessage = string.Join("; ", errorMessages);

                        // Combine the original exception message with the new one.
                        var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                        Logger.Log(exceptionMessage, "error", User.Identity.Name, "POST /Wall/Create");

                        ModelState.AddModelError("ex", ex);

                        return View(model);
                    }                       
                    catch (Exception ex)
                    {
                        Logger.Log(ex.ToString(), "error", User.Identity.Name, "POST /Wall/Create");

                        ModelState.AddModelError("ex", ex);

                        return View(model);
                    }
                }


                return RedirectToAction("Index", "Wall");
            }

            return View(model);
        }

        [GET("/Wall/Edit/", RouteName = "wall_edit_g")]
        public ActionResult Edit(int id)
        {
            var model = new WallModel();

            if (id.IsLessThanOne())
            {
                ModelState.AddModelError("missing_id", "Missing or Invalid ID");
            }

            try
            {
                using (var db = new MySelfieEntities())
                {
                    var entity = db.Walls.SingleOrDefault(x => x.WallId == id);

                    if (entity.IsNotNull())
                    {                        
                        model.MergeWithOtherType(entity);

                        model.LogoImage = entity.LogoImage;

                        //Johnm - some mismatches on column naming, thus mergeWithOtherType not working
                        model.Post_InstagramToken = entity.PostingAccount_InstagramToken;

                        return View(model);
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), "error", User.Identity.Name, "GET /Wall/Edit/");

                ModelState.AddModelError("ex", ex);

                Logger.Log(ex.ToString());
            }

            return View(model);
        }

        [POST("/Wall/TestInstagramComment")]
        public JsonResult TestInstagramComment(FormCollection form) {
        //public JsonResult TestInstagramComment() {
            string token = form["Post_InstagramToken"];
            string id = form["MediaId"];
            string template = form["RetweetMessage"];

            string response = "";
            response = PhotoStatusChangeModel.PublishInstagramComment("WallTestUser", DateTime.Now, template, token, id);
            //response = PhotoStatusChangeModel.PublishInstagramComment("WallTestUser", DateTime.Now, " Thanks {username} . Your selfie will be live in {time}. Be sure to follow @myselfielive!", "1561604955.91f8687.f0f677ef6d1e44c59ec6be50d4ea88e5", "938465188402119058_1545689559");
            var success = true;
            if (response != "") {
                success = false;
            }
            var stuff = new { success = success, response = response };

            //var stuff = new { success = true, response = response, token=token, id=id, template=template };

            return ( Json(stuff) );
        }

        [POST("/Wall/Edit", RouteName = "wall_edit_p")]
        [ValidateInput(false)]
        public ActionResult Edit(WallModel model)
        {
            if (ModelState.IsValid)
            {                
                try
                {
                    using (var db = new MySelfieEntities())
                    {
                        var wall = db.Walls.Where(x => x.WallId == model.WallId).FirstOrDefault();

                        wall.MergeWithOtherType(model);

                        if (model.CaptionText.IsEmptyOrNull()) wall.CaptionText = "";
                        if (model.DescriptionText.IsEmptyOrNull()) wall.DescriptionText = "";
                        if (model.RightText.IsEmptyOrNull()) wall.RightText = "";
                        if (model.Title.IsEmptyOrNull()) wall.Title = "";

                        if (model.LogoImageFile.HasFile())
                        {
                            wall.LogoImage = model.LogoImageFile.getFileBytes();    // extracts bytes from posted file
                            wall.LogoImageType = model.LogoImageFile.getFileType(); // extracts type from posted file
                        }

                        wall.IsActive = model.IsActive;

                        wall.Scrape_InstagramToken = model.Scrape_InstagramToken;

                        //Johnm - some mismatches on column naming, thus mergeWithOtherType not working
                        wall.PostingAccount_InstagramToken = model.Post_InstagramToken;

                        db.SaveChanges();
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ex", ex);

                    Logger.Log(ex.ToString(), "error", User.Identity.Name, "POST /Wall/Edit/");

                    return View(model);
                }

                return RedirectToAction("Index", "Wall");
            }

            return View(model);
        }

        [GET("/Wall/Image/", RouteName = "wall_image_g")]
        public FileResult Image(int id)
        {
            try
            {
                using (var db = new MySelfieEntities())
                {
                    var wall = db.Walls.Where(x => x.WallId == id).FirstOrDefault();

                    if (wall.IsNotNull())
                    {
                        return new FileStreamResult(new MemoryStream(wall.LogoImage), wall.LogoImageType);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), "error", User.Identity.Name, "GET /Wall/Image/");

                ModelState.AddModelError("ex", ex);

                Logger.Log(ex.ToString());
            }
            var fileName = "error.jpg";
            var path = Path.Combine(Server.MapPath("~/Images"), fileName);

            return new FileStreamResult(new FileStream(path, FileMode.Open), "image/jpeg");
        }

        [GET("/Wall/Delete/", RouteName = "wall_delete_g")]
        public ActionResult Delete(int id)
        {
            if (id.IsLessThanOne())
            {
                ModelState.AddModelError("missing_id", "Missing or Invalid ID");

                return RedirectToAction("Index", "Wall");
            }

            try
            {
                using (var db = new MySelfieEntities())
                {
                    var entity = db.Walls.SingleOrDefault(x => x.WallId == id);

                    if (entity != null)
                    {
                        entity.Status = "deleted";
                        entity.IsActive = false;

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex);

                Logger.Log(ex.ToString(), "error", User.Identity.Name, "GET /Wall/Delete/");

                return View();
            }
            
            return RedirectToAction("Index", "Wall");
        }
	}
}