//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MySelfie
{
    using System;
    using System.Collections.Generic;
    
    public partial class Packet
    {
        public Packet()
        {
            this.PhotoTweets = new HashSet<PhotoTweet>();
            this.Photos = new HashSet<Photo>();
        }
    
        public int PacketId { get; set; }
        public int WallId { get; set; }
        public string JSONBody { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public int DurationMillisecond { get; set; }
        public string Status { get; set; }
        public System.DateTime CreatedAt { get; set; }
    
        public virtual Wall Wall { get; set; }
        public virtual ICollection<PhotoTweet> PhotoTweets { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
    }
}
