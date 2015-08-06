using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Extensions;

namespace MySelfie.Scraper
{
    class WallModel
    {
        public int WallId { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public string Hashtag { get; set; }

        public string Twitter_ConsumerKey { get; set; }
        public string Twitter_ConsumerSecret { get; set; }
        public string Twitter_UserTokenKey { get; set; }
        public string Twitter_UserTokenSecret { get; set; }
        public string Instagram_AccessToken { get; set; }


        #region From wall table mapped data model made by entity framework (Wall.cs)
        // Twitter credentials
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string UserTokenKey { get; set; }
        public string UserTokenSecret { get; set; }
        // Instagram crentials for the account used to pull data from API
        public string Scrape_InstagramUserName { get; set; }
        public string Scrape_InstagramPassword { get; set; }
        public string Scrape_InstagramClientID { get; set; }
        public string Scrape_InstagramClientSecret { get; set; }
        public string Scrape_InstagramToken { get; set; }
        // Twitte crentials for the account used to pull data from API
        public string Scrape_ConsumerKey { get; set; }
        public string Scrape_ConsumerSecret { get; set; }
        public string Scrape_UserTokenKey { get; set; }
        public string Scrape_UserTokenSecret { get; set; }
        public string Scrape_TwitterUserName { get; set; }
        // Twitter other info
        public string TwitterUserName { get; set; }
        public string TwitterAppName { get; set; }
        public string Scrape_TwitterAppName { get; set; }
        // Instagram credentials for the account used to post from the API
        public string Post_InstagramUserName { get; set; }
        public string Post_InstagramPassword { get; set; }
        public string Post_InstagramClientID { get; set; }
        public string Post_InstagramClientSecret { get; set; }
        public string PostingAccount_InstagramUserName { get; set; }
        public string PostingAccount_InstagramPassword { get; set; }
        public string PostingAccount_InstagramToken { get; set; }
        #endregion

        public WallModel(Wall entity)
        {
            this.WallId = entity.WallId;
            this.Hashtag = entity.Hashtag;
            this.Status = entity.Status;
            this.IsActive = entity.IsActive;
            this.Twitter_ConsumerKey = entity.Scrape_ConsumerKey;
            this.Twitter_ConsumerSecret = entity.Scrape_ConsumerSecret;
            this.Twitter_UserTokenKey = entity.Scrape_UserTokenKey;
            this.Twitter_UserTokenSecret = entity.Scrape_UserTokenSecret;
            this.Instagram_AccessToken = entity.Scrape_InstagramToken;

            this.MergeWithOtherType(entity);    // copies all fields with same name/type from entity to thia
           
        }
    }
}
