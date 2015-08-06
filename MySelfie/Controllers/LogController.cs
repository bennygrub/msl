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
    public class LogController : Controller
    {
        // GET: Log
        public ActionResult Index()
        {
            var model = new LogListViewModel();

            model.FetchAmount = 5;
            model.FetchSpeedInSeconds = 10;

            using (var db = new MySelfieEntities())
            {
                var combined = new List<LogViewModel>();

                var web = db.WebLogs
                    .Where(x => x.Status == "new")
                    .OrderByDescending(x => x.TimeStamp)
                    .Take(100)
                    .Select(x => new LogViewModel
                    {
                        Id = x.WebLogId,
                        Message = x.Message,
                        TimeStamp = x.TimeStamp,
                        Type = x.Type,
                        UserName = x.UserName,
                        Context = x.Context,
                        Source = "web",
                        Header = "Web Log Data"
                    }).ToList();

                foreach (var log in web)
                {
                    combined.Add(log);
                }

                var workerStatus = db.WorkerStatus
                    .Where(x => x.Status == "new")
                    .OrderByDescending(x => x.TimeStamp)
                    .Take(100)
                    .Select(x => new LogViewModel
                    {
                        Id = x.WorkerStatusID,
                        Message = x.Message,
                        TimeStamp = x.TimeStamp,
                        Type = "update",
                        UserName = "Scraper",
                        Source = "scraper",
                        Header = "Scraper Log Data"
                    })
                    .ToList();

                foreach (var log in workerStatus)
                {
                    combined.Add(log);
                }

                model.LogList = combined;

                if (web.Any()) model.LastWebLogId = web.Select(x => x.Id).First();
                if (workerStatus.Any()) model.LastWorkerLogId = workerStatus.Select(x => x.Id).First();
            }

            return View(model);
        }

        [POST("/Log/Delete", RouteName = "log_delete_p")]
        public JsonResult Remove(int id, string source)
        {
            if (source == "web")
            {
                using (var db = new MySelfieEntities())
                {
                    var entity = db.WebLogs.SingleOrDefault(x => x.WebLogId == id);

                    if (entity.IsNotNull())
                    {
                        entity.Status = "deleted";

                        db.SaveChanges();

                        return Json(new { id = id, success = true, source = source });
                    }
                    else
                    {
                        return Json(new { id = id, success = false, error = "record not found" });
                    }
                }
                
            }
            if (source == "scraper")
            {
                using (var db = new MySelfieEntities())
                {
                    var entity = db.WorkerStatus.SingleOrDefault(x => x.WorkerStatusID == id);

                    if (entity.IsNotNull())
                    {
                        entity.Status = "deleted";

                        db.SaveChanges();

                        return Json(new { id = id, success = true, source = source });
                    }
                    else
                    {
                        return Json(new { id = id, success = false, error = "record not found" });
                    }
                }
            }

            return Json(new { id = id, success = false, error = "invalid type" });
        }

        [GET("/Log/Latest", RouteName = "log_latest_g")]
        public ActionResult Latest(LogListViewModel model)
        {
            if (model.FetchAmount.IsLessThanOne())
                model.FetchAmount = 10;

            if (model.FetchSpeedInSeconds.IsLessThanOne())
                model.FetchSpeedInSeconds = 5;

            model.LogList = new List<LogViewModel>();

            using (var db = new MySelfieEntities())
            {
                var combined = new List<LogViewModel>();

                var query = db.WebLogs
                    .Where(x => x.WebLogId > model.LastWebLogId)
                    .Where(x => x.Status == "new")
                    .OrderByDescending(x => x.TimeStamp)
                    .Take(model.FetchAmount);

                var web = query.Select(x => new LogViewModel
                {
                    Id = x.WebLogId,
                    Message = x.Message,
                    TimeStamp = x.TimeStamp,
                    Type = x.Type,
                    UserName = x.UserName,
                    Context = x.Context,
                    Source = "web",
                    Header = "Web Log Data"
                }).ToList();

                foreach (var log in web)
                {
                    combined.Add(log);
                }

                var workerStatus = db.WorkerStatus
                    .Where(x => x.WorkerStatusID > model.LastWorkerLogId)
                    .Where(x => x.Status == "new")
                    .OrderByDescending(x => x.TimeStamp)
                    .Take(model.FetchAmount)
                    .Select(x => new LogViewModel
                    {
                        Id = x.WorkerStatusID,
                        Message = x.Message,
                        TimeStamp = x.TimeStamp,
                        Type = "update",
                        UserName = "Scraper",
                        Source = "scraper",
                        Header = "Scraper Log Data"
                    })
                    .ToList();

                foreach(var log in workerStatus)
                {
                    combined.Add(log);
                }

                model.LogList = combined;

                if (web.Any()) model.LastWebLogId = web.Select(x => x.Id).Last();
                if (workerStatus.Any()) model.LastWorkerLogId = workerStatus.Select(x => x.Id).Last();

                return View(model);
            }
        }
    }
}