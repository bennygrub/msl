using LinqToTwitter;
using MySelfie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Shared.Extensions;

namespace MySelfie.Services
{
    public static class PhotoService
    {
        internal class WallModel
        {
            public int WallId { get; set; }
            public string Hashtag { get; set; }
            public string ConsumerKey { get; set; }
            public string ConsumerSecret { get; set; }
            public string UserTokenKey { get; set; }
            public string UserTokenSecret { get; set; }

            public WallModel(Wall entity)
            {
                this.MergeWithOtherType(entity);
            }
        }

        public static async Task<bool> CreateBatch(int wallId)
        {
            bool result = false;

            //IList<Task> taskList = new List<Task>();

            Wall wall = null;

            using (var db = new MySelfieEntities())
            {
                var query = db.Photos
                    .Where(x => x.WallId == wallId)
                    .Where(x => x.Status == "approved")
                    .OrderBy(x => x.ApprovedAt)
                    .Take(9);

                var results = query.Select(x => x.Filename).ToList();

                if (results.Count == 9)
                {
                    wall = db.Walls.Single(x => x.WallId == wallId);

                    var existingPacketCount = db.Packets
                        .Where(x => x.WallId == wallId)
                        .Where(x => x.Status == "new")
                        .Count();

                    var wallTotalSeconds = (wall.PhotoShownLengthSecond.Value * 9) + wall.AdShownLengthSecond;

                    var secondsUntilStart = wallTotalSeconds * existingPacketCount;

                    var packet = new[] {
                        new {
                            slides = results,
                            sponsor = "Wall/Image/" + wall.WallId
                        }
                    };

                    try
                    {
                        var jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                        var json = jss.Serialize(packet);

                        var entity = new Packet
                        {
                            WallId = wallId,
                            JSONBody = json,
                            StartTime = DateTime.UtcNow.AddSeconds(secondsUntilStart.IfNotNull(x => x.Value)),
                            EndTime = DateTime.UtcNow.AddSeconds(secondsUntilStart.IfNotNull(x => x.Value) + wallTotalSeconds.IfNotNull(x => x.Value)),
                            DurationMillisecond = wallTotalSeconds.IfNotNull(x => x.Value) * 1000,
                            Status = "new",
                            CreatedAt = DateTime.UtcNow
                        };

                        db.Packets.Add(entity);

                        string message = "Created new packet: " + Environment.NewLine
                            + "WallId: " + entity.WallId + Environment.NewLine
                            + "JSONBody: " + entity.JSONBody + Environment.NewLine
                            + "StartTime: " + entity.StartTime + Environment.NewLine
                            + "EndTime: " + entity.EndTime + Environment.NewLine
                            + "DurationMillisecond: " + entity.DurationMillisecond + Environment.NewLine
                            + "CreatedAt: " + entity.CreatedAt + Environment.NewLine;

                        Logger.Log(message, "info", "system", "creating a packet", "Packet Created", db, false);

                        var context = GetTwitterContext(new WallModel(wall));

                        foreach (var photo in query)
                        {
                            photo.Status = "packed";
                            photo.Packet = entity;

                            if (photo.Source == "Twitter")
                            {
                                await PhotoStatusChangeModel.PublishTweet(photo.Username, entity.StartTime, wall.RetweetMessage, context);
                            }
                            if (photo.Source == "Instagram")
                            {
                                PhotoStatusChangeModel.PublishInstagramComment(photo.Username, entity.StartTime, wall.RetweetMessage, wall.PostingAccount_InstagramToken, photo.SocialIDstring);
                            }
                        }

                        db.SaveChanges();

                        //foreach(var task in taskList)
                        //{
                        //    await task;
                        //}
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex.ToString(), "error", "system", "creating a packet", "Error creating packet", db, true);
                        result = false;
                    }

                    result = true;
                }
                else
                {
                    result = false;
                }
            }   // end using db

            //if (result) // result is true when a packet was created
            //{
            //    var context = GetTwitterContext(new WallModel(wall));

            //    foreach (var photo in packetPhotos)
            //    {
            //        if (photo.Source == "Twitter")
            //        {
            //            var t = new Thread(() => PhotoStatusChangeModel.PublishTweet(photo.Username, newPacket.StartTime, wall.RetweetMessage, context));
            //            t.Start();
            //        }
            //        if (photo.Source == "Instagram")
            //        {
            //            var t = new Thread(() => PhotoStatusChangeModel.PublishInstagramComment(photo.Username, newPacket.StartTime, wall.RetweetMessage, wall.PostingAccount_InstagramToken, photo.SocialIDstring));
            //            t.Start();
            //        }

            //    }
            //}

            return result;
        }

        private static TwitterContext GetTwitterContext(WallModel model)
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = model.ConsumerKey,
                    ConsumerSecret = model.ConsumerSecret,
                    OAuthToken = model.UserTokenKey,
                    OAuthTokenSecret = model.UserTokenSecret,
                }
            };

            auth.AuthorizeAsync();

            auth.AccessType = AuthAccessType.Write;

            return new TwitterContext(auth);
        }

        public static async Task PublishTweet(string userName, DateTime time, string template, TwitterContext context)
        {
            try
            {
                string usernamePH = "{username}";
                string timePH = "{time}";
                var message = template
                    .Replace(usernamePH, "@" + userName)
                    .Replace(timePH, time.AddHours(-6).ToShortTimeString());

                if (message.Length <= 140)
                {
                    var tweet = await context.TweetAsync(message);

                    Logger.Log("Published tweet: " + message, "info");

                    Thread.Sleep(100);
                    //Task.Delay(1000);
                }
                else
                {
                    Logger.Log("tweet too long", "warning");

                    message = message.Remove(140);

                    var tweet = await context.TweetAsync(message);

                    Thread.Sleep(100);
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), "error");
            }
        }

        public static void PublishInstagramComment(string userName, DateTime time, string template, string token, string id)
        {
            try
            {
                string usernamePH = "{username}";
                string timePH = "{time}";
                var message = template
                    .Replace(usernamePH, "@" + userName)
                    .Replace(timePH, time.AddHours(-5).ToShortTimeString());

                if (message.Length < 140)
                {
                    var url = "https://api.instagram.com/v1/media/";
                    var url_end = "/comments";
                    var url_token = "?access_token=" + token;

                    url = url + id + url_end + url_token;

                    WebResponse response = processWebRequest(url);

                    //Logger.Log("Published tweet: " + message, "info");

                    Thread.Sleep(1000);
                }
                else
                {
                    Logger.Log("tweet too long", "warning");
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), "error");
            }
        }

        private static WebResponse processWebRequest(string url, bool continueOnError = true)
        {
            try
            {
                var request = WebRequest.Create(url);
                request.Method = "POST";

                var response = request.GetResponse();

                return response;
            }
            catch (Exception ex)
            {
                //this._errorCount++;

                //Logger.Log("Instagram processWebRequest exception: " + ex.ToString());

                if (continueOnError)
                {
                    return processWebRequest(url, false);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}