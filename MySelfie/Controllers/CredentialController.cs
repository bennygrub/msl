using AttributeRouting.Web.Mvc;
using MySelfie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shared.Extensions;

namespace MySelfie.Controllers
{
    public class CredentialController : Controller
    {
        [GET("/Credential", RouteName = "credential_index_g")]
        public ActionResult Index() // to later include an organization id.
        {
            var model = new CredentialListViewModel();

            try
            {
                using (var db = new MySelfieEntities())
                {
                    model.CredentialList = db.Credentials
                        .Select(x => new CredentialListItemViewModel()
                        {
                            CredentialId = x.CredentialId,
                            UserName = x.UserName,
                            AppName = x.AppName,
                            UsageToday = x.UsageToday
                        })
                        .ToList();
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex);

                Logger.Log(ex.ToString());
            }

            return View(model);
        }

        [GET("/Credential/Create", RouteName = "credential_create_g")]
        public ActionResult Create()
        {
            return View();
        }

        [POST("/Credential/Create", RouteName = "credential_create_p")]
        public ActionResult Create(CredentialCreateModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new MySelfieEntities())
                    {

                        var entity = new Credential();

                        entity.MergeWithOtherType(model);

                        db.Credentials.Add(entity);

                        db.SaveChanges();
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ex", ex);

                    return View(model);
                }

                return RedirectToRoute("credential_index_g");
            }

            return View(model);
        }

        [GET("/Credential/Edit/", RouteName = "credential_edit_g")]
        public ActionResult Edit(int id)
        {
            var model = new CredentialEditModel();

            if (id.IsLessThanOne())
            {
                ModelState.AddModelError("missing_id", "Missing or Invalid ID");
            }

            try
            {
                using (var db = new MySelfieEntities())
                {
                    var entity = db.Credentials.SingleOrDefault(x => x.CredentialId == id);

                    if (entity.IsNotNull())
                    {                        
                        model.MergeWithOtherType(entity);

                        return View(model);
                    }

                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex);

                Logger.Log(ex.ToString());
            }

            return View(model);
        }

        [POST("/Credential/Edit", RouteName = "credential_edit_p")]
        public ActionResult Edit(CredentialEditModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new MySelfieEntities())
                    {
                        var wall = db.Credentials.Where(x => x.CredentialId == model.CredentialId).FirstOrDefault();

                        wall.MergeWithOtherType(model);

                        db.SaveChanges();
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ex", ex);

                    Logger.Log(ex.ToString());

                    return View(model);
                }

                return RedirectToRoute("credential_index_g");
            }

            return View(model);
        }

        [GET("/Credential/Delete/", RouteName = "credential_delete_g")]
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
                    var entity = db.Credentials.SingleOrDefault(x => x.CredentialId == id);

                    if (entity.IsNotNull())
                    {
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex);

                Logger.Log(ex.ToString());

                return View();
            }

            return RedirectToRoute("credential_index_g");
        }

        public void InstagramOAuth()
        {

        }
	}
}