using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Extensions;

namespace MySelfie.Scraper
{
    class InstagramModel : ISocialData
    {
        public InstagramModel(MySelfie.Scraper.InstagramObject.Datum data)
        {
            this.data = data;
        }

        private MySelfie.Scraper.InstagramObject.Datum data;

        public string Url
        {
            get
            {
                return this.data.images.standard_resolution.url;
            }
        }

        public string Username
        {
            get
            {
                return this.data.user.full_name;
            }
        }

        public string Text
        {
            get
            {
                return this.data.caption.text;
            }
        }

        public long Id
        {
            get
            {
                long output = 0;

                if (long.TryParse(this.data.caption.id, out output))
                {
                    return output;
                } else
                {
                    return -1;
                }                
            }
        }

        public DateTime CreatedAt
        {
            get
            {
                var raw = this.data.created_time;

                return DateTime.UtcNow;
            }
        }

        public string[] Tags
        {
            get
            {
                return this.data.tags.Select(x => x.To<string>()).ToArray();
            }
        }
    }
}
