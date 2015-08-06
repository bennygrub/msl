using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySelfie.Scraper
{
    public static class Logger
    {
        public static void Log(string message)
        {
            Logger.Log(message, "default", "no user", "no context", "");
        }
        public static void Log(string message, string type)
        {
            Logger.Log(message, type, "no user", "no context", "");
        }
        public static void Log(string message, string type, string username)
        {
            Logger.Log(message, type, username, "no context", "");
        }
        public static void Log(string message, string type, string username, string context)
        {
            Logger.Log(message, type, username, "no context", "");
        }
        public static void Log(string message, string type, string username, string context, string header)
        {
            using (var db = new MySelfieEntities())
            {
                var entity = new WorkerStatus();

                entity.Message = message;
                entity.TimeStamp = DateTime.UtcNow;
                entity.Type = type;
                entity.UserName = username;
                entity.Context = context;
                entity.Status = "new";
                entity.Header = header;

                db.WorkerStatus.Add(entity);
                db.SaveChanges();
            }
        }
    }
}
