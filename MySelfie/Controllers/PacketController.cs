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
    public class PacketController : Controller
    {
        [GET("API/Packet", RouteName = "packet_json_g")]
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

        [POST("API/Packet/Status", RouteName = "packet_status_json_p")]
        public JsonResult Status(PhotoPacketStatusChangeModel model)
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

        [GET("API/Packet/List", RouteName = "packet_list_json_g")]
        public JsonResult List(int? id)
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

        [GET("API/Packet/Count", RouteName = "packet_count_json_g")]
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
                Logger.Log(ex.ToString(), "error", User.Identity.Name, "API/Photo/Packet/List");

                return Json(new { error = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}