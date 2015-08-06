using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Interfaces;
using Shared.Extensions;

namespace MySelfie.Scraper
{
    class TweetModel : ISocialData
    {
        public TweetModel(ITweet data)
        {
            this.data = data;
        }

        private ITweet data;

        public string Url
        {
            get
            {
                return this.data.Media.IfNotNull(x => x.First().MediaURL, "");
            }
        }

        public string Username
        {
            get
            {
                return this.data.Creator.ScreenName;
            }
        }

        public string Text
        {
            get
            {
                return this.data.Text;
            }
        }

        public long Id
        {
            get
            {
                return this.data.Id;
            }
        }

        public DateTime CreatedAt
        {
            get
            {
                return this.data.CreatedAt;
            }
        }

        public string[] Tags
        {
            get
            {
                return this.data.Hashtags.Select(x => x.Text).ToArray();
            }
        }
    }
}
