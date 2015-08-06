//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MySelfie.Scraper
{
    using System;
    using System.Collections.Generic;
    
    public partial class Wall
    {
        public Wall()
        {
            this.Packets = new HashSet<Packet>();
            this.PhotoTweets = new HashSet<PhotoTweet>();
            this.WorkerCommands = new HashSet<WorkerCommand>();
            this.Photos = new HashSet<Photo>();
        }
    
        public int WallId { get; set; }
        public string Hashtag { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string DescriptionText { get; set; }
        public int PhotoShownLengthMillisecond { get; set; }
        public int AdShownLengthMillisecond { get; set; }
        public string Title { get; set; }
        public Nullable<int> PhotoShownLengthSecond { get; set; }
        public Nullable<int> AdShownLengthSecond { get; set; }
        public string RetweetMessage { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string UserTokenKey { get; set; }
        public string UserTokenSecret { get; set; }
        public string FrameTopColor { get; set; }
        public string FrameBottomColor { get; set; }
        public string LogoPath { get; set; }
        public string Scrape_ConsumerKey { get; set; }
        public string Scrape_ConsumerSecret { get; set; }
        public string Scrape_UserTokenKey { get; set; }
        public string Scrape_UserTokenSecret { get; set; }
        public string Scrape_TwitterUserName { get; set; }
        public string TwitterUserName { get; set; }
        public string TwitterAppName { get; set; }
        public string Scrape_TwitterAppName { get; set; }
        public byte[] LogoImage { get; set; }
        public string LogoImageType { get; set; }
        public string RightText { get; set; }
        public string CaptionText { get; set; }
        public string Scrape_InstagramUserName { get; set; }
        public string Scrape_InstagramPassword { get; set; }
        public string Scrape_InstagramClientID { get; set; }
        public string Scrape_InstagramClientSecret { get; set; }
        public string Scrape_InstagramToken { get; set; }
        public string Post_InstagramUserName { get; set; }
        public string Post_InstagramPassword { get; set; }
        public string Post_InstagramClientID { get; set; }
        public string Post_InstagramClientSecret { get; set; }
        public string PostingAccount_InstagramUserName { get; set; }
        public string PostingAccount_InstagramPassword { get; set; }
        public string PostingAccount_InstagramToken { get; set; }
    
        public virtual ICollection<Packet> Packets { get; set; }
        public virtual ICollection<PhotoTweet> PhotoTweets { get; set; }
        public virtual ICollection<WorkerCommand> WorkerCommands { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
    }
}
