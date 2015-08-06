using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySelfie.Scraper
{
    public interface ISocialData
    {
        string Url { get; }
        string Username { get; }
        string Text { get; }
        long Id { get; }
        DateTime CreatedAt { get; }
        string[] Tags { get; }
    }
}
