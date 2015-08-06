using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MySelfie
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
            //JohnM re-enabled DB logging for web
            //If from web, log to DB, if command line log to console
            if (HttpRuntime.AppDomainAppId != null) {
                using (var db = new MySelfieEntities())
                {
                    var entity = new WebLog();

                    entity.Message = message;
                    entity.TimeStamp = DateTime.UtcNow;
                    entity.Type = type;
                    entity.UserName = username;
                    entity.Context = context;
                    entity.Status = "new";
                    //entity.Header = header;

                    db.WebLogs.Add(entity);
                    db.SaveChanges();
                }
            }
            else {
                Console.WriteLine(message);
            }
        }

        public static void Log(string message, string type, string username, string context, string header, MySelfieEntities db, bool saveChanges)
        {
            var entity = new WebLog();

            entity.Message = message;
            entity.TimeStamp = DateTime.UtcNow;
            entity.Type = type;
            entity.UserName = username;
            entity.Context = context;
            entity.Status = "new";
            //entity.Header = header;

            db.WebLogs.Add(entity);

            if (saveChanges)
                db.SaveChanges();            
        }
    }
}
