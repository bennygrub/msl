using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Shared.Extensions;

namespace MySelfie.Models
{
    public class WallListViewModel
    {
        public WallListViewModel()
        {
            this.WallList = new List<WallListItemViewModel>();
        }
        public IList<WallListItemViewModel> WallList { get; set; }        
    }
    public class WallListItemViewModel
    {
        public int WallId { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }
        [Display(Name = "Active?")]
        public bool IsActive { get; set; }        
        [Display(Name = "Scraping?")]
        public bool IsScraping { get; set; }
        [Display(Name = "Hashtag")]
        public string Hashtag { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Twitter Account")]
        public string TwitterUserName { get; set; }
        [Display(Name = "Scraper Account")]
        public string Scrape_TwitterUserName { get; set; }
        public string Instagram_Redirect_URL { get; set; }        
        public int PendingApproval { get; set; }
    }

    //public class WallCreateModel
    //{
    //    public int WallId { get; set; }
    //    public DateTime CreatedAt { get; set; }
    //    [Required]
    //    public string Name { get; set; }        
    //    public string Status { get; set; }
    //    [Display(Name = "Is Active?")]
    //    public bool IsActive { get; set; }
        
    //    public int PhotoShownLengthMillisecond { get; set; }
    //    public int AdShownLengthMillisecond { get; set; }
    //    [Required]
    //    [Display(Name = "Photo Shown Length in Seconds")]
    //    public int PhotoShownLengthSecond { get; set; }
    //    [Required]
    //    [Display(Name = "Ad Shown Length in Seconds")]
    //    public int AdShownLengthSecond { get; set; }

    //    [Required]
    //    public string Hashtag { get; set; }
    //    [Display(Name = "Top / Title text")]
    //    public string Title { get; set; }
    //    [Display(Name = "Top (lower) / Caption text")]
    //    public string CaptionText { get; set; }
    //    [Display(Name = "Left / Call to Action text")]
    //    public string DescriptionText { get; set; }
    //    [Display(Name = "Right text")]
    //    public string RightText { get; set; }
    //    [Display(Name = "Re-tweet Message")]
    //    public string RetweetMessage { get; set; }
    //    [Display(Name = "Frame Top Color")]
    //    public string FrameTopColor { get; set; }
    //    [Display(Name = "Frame Bottom Color")]
    //    public string FrameBottomColor { get; set; }
    //    [Display(Name = "Logo Picture")]
    //    public string LogoPath { get; set; }
    //    public byte[] LogoImage { get; set; }
    //    public string LogoImageType { get; set; }
    //    public HttpPostedFileBase LogoImageFile { get; set; }

    //    [Required]
    //    [Display(Name = "Twitter UserName")]
    //    public string TwitterUserName { get; set; }
    //    [Required]
    //    [Display(Name = "Twitter App Name")]
    //    public string TwitterAppName { get; set; }
    //    [Required]
    //    [Display(Name = "API Key")]
    //    public string ConsumerKey { get; set; }
    //    [Required]
    //    [Display(Name = "API Secret")]
    //    public string ConsumerSecret { get; set; }
    //    [Required]
    //    [Display(Name = "Access Token")]
    //    public string UserTokenKey { get; set; }
    //    [Required]
    //    [Display(Name = "Access Token Secret")]
    //    public string UserTokenSecret { get; set; }

    //    [Required]
    //    [Display(Name = "Scrape Twitter UserName")]
    //    public string Scrape_TwitterUserName { get; set; }
    //    [Required]
    //    [Display(Name = "Scrape Twitter App Name")]
    //    public string Scrape_TwitterAppName { get; set; }
    //    [Required]
    //    [Display(Name = "Scrape API Key")]
    //    public string Scrape_ConsumerKey { get; set; }
    //    [Required]
    //    [Display(Name = "Scrape API Secret")]
    //    public string Scrape_ConsumerSecret { get; set; }
    //    [Required]
    //    [Display(Name = "Scrape Access Token")]
    //    public string Scrape_UserTokenKey { get; set; }
    //    [Required]
    //    [Display(Name = "Scrape Access Token Secret")]
    //    public string Scrape_UserTokenSecret { get; set; }
    //}

    public class WallModel
    {
        #region Wall parameters
        public int WallId { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Hashtag { get; set; }
        public string Status { get; set; }
        [Required]
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
        [Display(Name = "Top / Title text")]
        public string Title { get; set; }
        [Display(Name = "Top (lower) / Caption text")]
        public string CaptionText { get; set; }
        [Display(Name = "Left / Call to Action text")]
        public string DescriptionText { get; set; }
        [Display(Name = "Right text")]
        public string RightText { get; set; }
        public int PhotoShownLengthMillisecond { get; set; }
        public int AdShownLengthMillisecond { get; set; }
        [Required]
        [Display(Name = "Photo Shown Length in Seconds")]
        public int PhotoShownLengthSecond { get; set; }
        [Required]
        [Display(Name = "Ad Shown Length in Seconds")]
        public int AdShownLengthSecond { get; set; }       
        [Display(Name = "Re-tweet Message")]
        public string RetweetMessage { get; set; }
        [Display(Name = "Frame Top Color")]
        public string FrameTopColor { get; set; }
        [Display(Name = "Frame Bottom Color")]
        public string FrameBottomColor { get; set; }
        [Display(Name = "Logo Picture")]
        public string LogoPath { get; set; }
        public byte[] LogoImage { get; set; }
        public string LogoImageType { get; set; }
        public HttpPostedFileBase LogoImageFile { get; set; }
        #endregion
        #region Twitter fields
        [Required]
        [Display(Name = "Twitter UserName")]
        public string TwitterUserName { get; set; }
        [Required]
        [Display(Name = "Twitter App Name")]
        public string TwitterAppName { get; set; }
        [Required]
        [Display(Name = "API Key")]
        public string ConsumerKey { get; set; }
        [Required]
        [Display(Name = "API Secret")]
        public string ConsumerSecret { get; set; }
        [Required]
        [Display(Name = "Access Token")]
        public string UserTokenKey { get; set; }
        [Required]
        [Display(Name = "Access Token Secret")]
        public string UserTokenSecret { get; set; }

        [Required]
        [Display(Name = "Scrape Twitter UserName")]
        public string Scrape_TwitterUserName { get; set; }
        [Required]
        [Display(Name = "Scrape Twitter App Name")]
        public string Scrape_TwitterAppName { get; set; }
        [Required]
        [Display(Name = "Scrape API Key")]
        public string Scrape_ConsumerKey { get; set; }
        [Required]
        [Display(Name = "Scrape API Secret")]
        public string Scrape_ConsumerSecret { get; set; }
        [Required]
        [Display(Name = "Scrape Access Token")]
        public string Scrape_UserTokenKey { get; set; }
        [Required]
        [Display(Name = "Scrape Access Token Secret")]
        public string Scrape_UserTokenSecret { get; set; }
        #endregion
        #region Instagram fields        
        //[Required]
        [Display(Name = "Post Instagram UserName")]
        public string Post_InstagramUserName { get; set; }
        //[Required]
        [Display(Name = "Post Instagram Password")]
        public string Post_InstagramPassword { get; set; }
        //[Required]
        [Display(Name = "Post Client ID")]
        public string Post_InstagramClientID { get; set; }
        //[Required]
        [Display(Name = "Post Client Secret")]
        public string Post_InstagramClientSecret { get; set; }        
        [Display(Name = "Post Instagram Token")]
        public string Post_InstagramToken { get; set; }

        //[Required]
        [Display(Name = "Scrape Instagram UserName")]
        public string Scrape_InstagramUserName { get; set; }
        //[Required]
        [Display(Name = "Scrape Instagram Password")]
        public string Scrape_InstagramPassword { get; set; }
        [Required]
        [Display(Name = "Scrape Client ID")]
        public string Scrape_InstagramClientID { get; set; }
        [Required]
        [Display(Name = "Scrape Client Secret")]
        public string Scrape_InstagramClientSecret { get; set; }
        [Display(Name = "Scrape Access Token")]
        public string Scrape_InstagramToken { get; set; }
        #endregion
    }
}
