using MySelfie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using AttributeRouting.Web.Mvc;
using System.Web.Security;

namespace MySelfie.Controllers
{


    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
           

            try
            {
                using (var db = new MySelfieEntities())
                {
                    var users = (from u in db.AspNetUsers
                                 select u).ToList().OrderBy(x => x.UserName);

                    var userList = users.Select(user => new UserCreateModel
                    {
                        Id = user.Id,
                        Name = user.UserName ?? "",
                        //Email = user.Email,
                        Role = GetRole(user.UserName).FirstOrDefault().ToString()
                    }).ToList();

                    return View(userList);
                }
            }
            catch (Exception)
            {


            }
            return View();
        }


        public static string GetRole(string username)
        {
            string userRole = "";

            try
            {
                userRole = Roles.GetRolesForUser(username).FirstOrDefault();

            }
            catch (Exception)
            {

            }


            return userRole;
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        // GET: User/Create
        public ActionResult Create(int id = 0)
        {

            UserCreateModel model = new UserCreateModel(); // only to store the roles

      
            string role = "";

            if (id != 0)
            {
                try
                {
                    using (var db = new MySelfieEntities())
                    {
                        //role = db.AspNetUserRoles.Where(x => x.Id == id).SingleOrDefault().Id;

                    }
                }
                catch (Exception)
                {

                }
            }

            try
            {
                using (var db = new MySelfieEntities())
                {
                    var roles = db.AspNetRoles.Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id,
                        Selected = (x.Id == role ? true : false)                     
                       
                    }).ToList();

                    model.Roles = new List<SelectListItem>(roles);
                }
            }
            catch (Exception)
            {
                ViewBag.Roles = "";
            }


            return View(model);
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(RegisterViewModel model)
        {

            AspNetUser theUser;

         
            try
            {
                if (ModelState.IsValid)
                {
                    //theUser = new ApplicationUser() { UserName = model.UserName };

                    theUser = new AspNetUser();


                    theUser.UserName = model.UserName;

                    using (var db = new MySelfieEntities())
                    {
                        db.AspNetUsers.Add(theUser);
                        db.SaveChanges();
                    }
                    
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }



        }





        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
