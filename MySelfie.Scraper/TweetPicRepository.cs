using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Interfaces;

namespace MySelfie.Scraper
{
    public class TweetPicRepository
    {
        private AzureBlobRepository _azureBlobRepo;
        private int _wallId;

        public TweetPicRepository(int wallId)
        {
            this._wallId = wallId;

            this._azureBlobRepo = new AzureBlobRepository();
        }

        public void Add(ITweet tweet)
        {
            if (tweet.Media != null)
            {
                foreach (var media in tweet.Media)
                {
                    this.SaveTweetPic(tweet, media.MediaURL, CreateFileName(tweet));
                }
            }
            else
            {
                // saves the tweet without a picture
                this.SaveTweet(tweet);
            }                      
        }
        public bool IsNew(long id)
        {
            using (var db = new MySelfieEntities())
            {
                var result = db.Photos.Any(x => x.SocialID == id);

                return !result;
            }
        }
        private void SaveTweetPic(ITweet tweet, string originalURL, string fileName)
        {
            var azureURL = this.StorePicture(originalURL, fileName);
            var urls = String.Join("|", tweet.Urls.Select(x => x.ExpandedURL));
            var hashTags = String.Join("|", tweet.Hashtags.Select(x => x.Text));

            using (var db = new MySelfieEntities())
            {
                var entity = new Photo();

                entity.Username = tweet.Creator.ScreenName;
                entity.Text = tweet.Text;
                entity.SocialCreatedAt = tweet.CreatedAt;
                entity.SocialID = tweet.Id;
                entity.SocialIDstring = tweet.IdStr;

                entity.HashTags = hashTags;
                entity.Urls = urls;

                entity.Filename = azureURL;
                entity.OriginalURL = originalURL;

                entity.Source = "Twitter";
                entity.HasPhoto = true;
                entity.CreatedAt = DateTime.UtcNow;
                entity.Approved = false;
                entity.Status = "new";
                entity.WallId = this._wallId;                

                db.Photos.Add(entity);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.Log("SaveTweetPic error: " + ex.ToString());
                }
            }
        }
        private string CreateFileName(ITweet tweet)
        {
            return tweet.IdStr + ".jpg";
        }
        private string StorePicture(string originalURL, string filename)
        {
            // save to Azure
            var uri = this._azureBlobRepo.AddImageFromURL(originalURL, filename); // returns Azure URL                        
            // return URL of our picture on azure
            return uri.AbsoluteUri;
        }

        private void SaveTweet(ITweet tweet)
        {
            var urls = String.Join("|", tweet.Urls.Select(x => x.ExpandedURL));
            var hashTags = String.Join("|", tweet.Hashtags.Select(x => x.Text));

            using (var db = new MySelfieEntities())
            {
                var entity = new Photo();
                entity.Username = tweet.Creator.ScreenName;
                entity.Filename = "";       // has no picture
                entity.OriginalURL = "";    // has no picture
                entity.Text = tweet.Text;
                entity.SocialCreatedAt = tweet.CreatedAt;
                entity.SocialID = tweet.Id;

                entity.Source = "Twitter";
                entity.HasPhoto = false;
                entity.CreatedAt = DateTime.UtcNow;
                entity.Approved = false;
                entity.Status = "no picture";
                entity.WallId = this._wallId;

                entity.HashTags = hashTags;
                entity.Urls = urls;

                db.Photos.Add(entity);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.Log("SaveTweet error: " + ex.ToString());
                }
            }
        }

        // source: http://stackoverflow.com/questions/11082804/detecting-image-url-in-c-net
        private bool IsImageUrl(string URL)
        {
            if (Uri.IsWellFormedUriString(URL, UriKind.Absolute))
            {
                try
                {
                    var req = (HttpWebRequest)HttpWebRequest.Create(URL);
                    req.Method = "HEAD";
                    using (var resp = req.GetResponse())
                    {
                        return resp.ContentType
                            .ToLower(CultureInfo.InvariantCulture)
                            .StartsWith("image/");
                    }
                }
                catch (WebException ex)
                {
                    Trace.WriteLine(ex);
                    Trace.WriteLine("URL: " + URL);
                }
                catch (UriFormatException ex)
                {
                    Trace.WriteLine(ex);
                    Trace.WriteLine("URL: " + URL);
                }
            }

            return false;
        }

        // source: http://stackoverflow.com/questions/3175062/long-urls-from-short-ones-using-c-sharp
        private string UrlLengthen(string url)
        {
            string newurl = url;

            bool redirecting = true;

            while (redirecting)
            {

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(newurl);
                    request.AllowAutoRedirect = false;
                    request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.3) Gecko/20090824 Firefox/3.5.3 (.NET CLR 4.0.20506)";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if ((int)response.StatusCode == 301 || (int)response.StatusCode == 302)
                    {
                        string uriString = response.Headers["Location"];
                        //Log.Debug("Redirecting " + newurl + " to " + uriString + " because " + response.StatusCode);
                        newurl = uriString;
                        // and keep going
                    }
                    else
                    {
                        //Log.Debug("Not redirecting " + url + " because " + response.StatusCode);
                        redirecting = false;
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add("url", newurl);
                    //Exceptions.ExceptionRecord.ReportWarning(ex); // change this to your own
                    redirecting = false;
                }
            }
            return newurl;
        }

        // source: http://stackoverflow.com/questions/10077219/download-image-from-url-in-c-sharp
        private Image GetImageFromUrl(string url)
        {
            using (var webClient = new WebClient())
            {
                return ByteArrayToImage(webClient.DownloadData(url));
            }
        }

        // source: http://stackoverflow.com/questions/10077219/download-image-from-url-in-c-sharp
        private Image ByteArrayToImage(byte[] fileBytes)
        {
            using (var stream = new MemoryStream(fileBytes))
            {
                return Image.FromStream(stream);
            }
        }
    }
}
