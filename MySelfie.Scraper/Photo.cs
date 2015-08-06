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
    
    public partial class Photo
    {
        public int PhotoId { get; set; }
        public string Username { get; set; }
        public string Filename { get; set; }
        public string Text { get; set; }
        public string OriginalURL { get; set; }
        public long SocialID { get; set; }
        public System.DateTime SocialCreatedAt { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public bool Approved { get; set; }
        public Nullable<System.DateTime> EstimatedFinalizedDateTime { get; set; }
        public Nullable<int> PacketId { get; set; }
        public string Status { get; set; }
        public string HashTags { get; set; }
        public string Urls { get; set; }
        public Nullable<System.DateTime> ApprovedAt { get; set; }
        public Nullable<int> WallId { get; set; }
        public Nullable<bool> HasPhoto { get; set; }
        public Nullable<System.DateTime> DisplayedAt { get; set; }
        public string Source { get; set; }
        public string SocialIDstring { get; set; }
    
        public virtual Packet Packet { get; set; }
        public virtual Wall Wall { get; set; }
    }
}
