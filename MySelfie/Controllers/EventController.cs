using MySelfie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shared.Extensions;

namespace MySelfie.Controllers
{
    public class EventController : Controller
    {
       
        // GET: Event
        public ActionResult Index()
        {
            IEnumerable<EventCreateModel> model;

            try
            {
                using (var db = new MySelfieEntities())
                {
                    model = db.Events.Select( x => new EventCreateModel()
                        {
                            EventId = x.EventId,
                            Name = x.Name,
                            StartDate = x.StartDate,
                            EndDate = x.EndDate
                        }).ToList();
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex);

              
            }

            return View();
        }

        // GET: Event/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }


        // POST: Event/Create
        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            EventCreateModel model = new EventCreateModel();

            try
            {
                using (var db = new MySelfieEntities())
                {
                    Event theEvent;
                    theEvent = db.Events.Where(x => x.EventId == id).FirstOrDefault();
                    model.MergeWithOtherType(theEvent);
                    return View(model);
                }
            }
            catch (Exception)
            {
                
               // throw;
            }


            return View();
        }

        [HttpPost]
        public ActionResult Create(EventCreateModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new MySelfieEntities())
                    {
                        Event theEvent;

                        if (model.EventId == 0)
                        {
                            theEvent = new Event();

                            theEvent.MergeWithOtherType(model);

                            db.Events.Add(theEvent);
                        }
                        else
                        {
                            theEvent = db.Events.Where(x => x.EventId == model.EventId).FirstOrDefault();

                            theEvent.MergeWithOtherType(model);

                        }

                        db.SaveChanges();
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ex", ex);

                    return View(model);
                }

                return RedirectToAction("Index", "Event");
            }

            return View(model);
        }

        


        // GET: Event/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Event/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

         [HttpGet]
        public ActionResult Delete(int id)
        {
            EventCreateModel model = new EventCreateModel();

            try
            {
                using (var db = new MySelfieEntities())
                {
                    Event theEvent;
                    theEvent = db.Events.Where(x => x.EventId == id).FirstOrDefault();
                    db.Events.Remove(theEvent);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {

                // throw;
            }


            return RedirectToAction("Index");
        }

     
    }
}
