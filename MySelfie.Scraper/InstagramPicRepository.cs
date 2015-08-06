using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Extensions;

namespace MySelfie.Scraper
{
    public class InstagramPicRepository
    {
        private AzureBlobRepository _azureBlobRepo;
        private int _wallId;

        public InstagramPicRepository(int wallId)
        {
            this._wallId = wallId;

            this._azureBlobRepo = new AzureBlobRepository();
        }

        public void Add(MySelfie.Scraper.InstagramObject.Datum data)
        {
            this.SavePic(data, data.images.standard_resolution.url, CreateFileName(data.id));
        }

        public bool IsNew(long id)
        {
            using (var db = new MySelfieEntities())
            {
                var result = db.Photos.Any(x => x.SocialID == id);

                return !result;
            }
        }

        private void SavePic(MySelfie.Scraper.InstagramObject.Datum data, string originalURL, string fileName)
        {
            var azureURL = this.StorePicture(originalURL, fileName);
            var urls = data.images.standard_resolution.url;
            var hashTags = "";

            try
            {
                hashTags = String.Join("|", data.tags.Select(x => x.To<string>()));
            }
            catch
            {

            }

            using (var db = new MySelfieEntities())
            {
                var entity = new Photo();

                entity.Username = data.user.username;
                entity.Text = data.caption.text;
                //entity.SocialCreatedAt = new DateTime(long.Parse(data.created_time));
                entity.SocialCreatedAt = DateTime.UtcNow;
                entity.SocialID = long.Parse(data.caption.id);
                entity.SocialIDstring = data.id;

                entity.HashTags = hashTags;
                entity.Urls = urls;

                entity.Filename = azureURL;
                entity.OriginalURL = originalURL;                

                entity.Source = "Instagram";
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

        private string StorePicture(string originalURL, string filename)
        {
            // save to Azure
            var uri = this._azureBlobRepo.AddImageFromURL(originalURL, filename); // returns Azure URL                        
            // return URL of our picture on azure
            return uri.AbsoluteUri;
        }

        private string CreateFileName(string id)
        {
            return id + ".jpg";
        }
    }
}
