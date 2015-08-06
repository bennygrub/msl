using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using LinqToTwitter;
using System.Threading;
using System.Net;
using System.Threading.Tasks;
using System.IO;

namespace MySelfie.Models
{

    public class PhotoApprovalViewModel
    {
        public List<PhotoViewModel> ImageList { get; set; }
        public int WallId { get; set; }

        public int LastPhotoTweetId { get; set; }

        public int FetchLatestSpeedInSeconds { get; set; }

        public int FetchLatestAmount { get; set; }
    }

    public class PhotoViewModel
    {
        public string FileName { get; set; }

        public int PhotoId { get; set; }

        public string Text { get; set; }

        public long SocialId { get; set; }

        public DateTime SocialCreated { get; set; }

        public string UserScreenName { get; set; }

        public string Source { get; set; }

        public PhotoViewModel()
        {

        }

        public PhotoViewModel(Photo entity)
        {
            this.MergeWithOtherType(entity);
        }

    }

    public class PhotoStatusChangeModel
    {
        public int WallId { get; set; }
        public int PhotoTweetId { get; set; }
        public string Status { get; set; }

        public bool Approved { get; set; }

        public class WallModel
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

                            //Johnm -- comment sending now a separate process from batching
                            /*
                            if (photo.Source == "Twitter")
                            {
                                await PhotoStatusChangeModel.PublishTweet(photo.Username, entity.StartTime, wall.RetweetMessage, context);
                            }
                            if (photo.Source == "Instagram")
                            {
                                PhotoStatusChangeModel.PublishInstagramComment(photo.Username, entity.StartTime, wall.RetweetMessage, wall.PostingAccount_InstagramToken, photo.SocialIDstring);
                            }
                            */
                        }

                        db.SaveChanges();    
                    }
                    catch(Exception ex)
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

        public static TwitterContext GetTwitterContext(WallModel model)
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

        public static async Task<string> PublishTweet(string userName, DateTime time, string template, TwitterContext context)
        {
            try
            {
                string usernamePH = "{username}";
                string timePH = "{time}";
                var message = template
                    .Replace(usernamePH, "@" + userName)
                    .Replace(timePH, DateTime.UtcNow.ToLongTimeString());
                    //.Replace(timePH, time.AddHours(-6).ToLongTimeString());

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
                return (ex.ToString());
            }
            return ("");
        }

        public static string PublishInstagramComment(string userName, DateTime time, string template, string token, string id)
        {
            try
            {
                string usernamePH = "{username}";
                string timePH = "{time}";
                var message = template
                    .Replace(usernamePH, "@" + userName)
                    .Replace(timePH, DateTime.UtcNow.ToLongTimeString());
                    //.Replace(timePH, time.AddHours(-5).ToLongTimeString());

                if (message.Length < 140)
                {
                    var url = "https://api.instagram.com/v1/media/" + id + "/comments";

                    string postString = string.Format("access_token={0}&text={1}", HttpUtility.UrlEncode(token), HttpUtility.UrlEncode(message));
                    const string contentType = "application/x-www-form-urlencoded";
                    var request = WebRequest.Create(url) as HttpWebRequest;
                    request.Method = "POST";
                    request.ContentType = contentType;
                    request.ContentLength = postString.Length;
                    request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.0.1) Gecko/2008070208 Firefox/3.0.1";
                    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream());
                    requestWriter.Write(postString);
                    requestWriter.Close();
                    StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream());
                    string responseString = responseReader.ReadToEnd();
                    responseReader.Close();
                    request.GetResponse().Close();

                    Logger.Log("Published instagram comment " + message, "info");

                    Thread.Sleep(1000);
                    return ("");
                }
                else
                {
                    string error = "comment too long";
                    Logger.Log(error, "warning");
                    return ("");
                }
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        Logger.Log(text, "error");
                        return (text);
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                Logger.Log(error, "error");
                return (error);
            }
            //Should not get here
            return("");
        }

        //Johnm - Was previously being used by Instagram post.  I don't think it's being used any longer
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

                Logger.Log("Instagram processWebRequest exception: " + ex.ToString());

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

    public class PhotoPacketModel
    {
        public int PacketId { get; set; }
        public string Data { get; set; }
        public string Status { get; set; }
    }

    public class PhotoPacketStatusChangeModel
    {
        public int PacketId { get; set; }
        public string Status { get; set; }
    }
}
