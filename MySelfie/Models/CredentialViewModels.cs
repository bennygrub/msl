using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MySelfie.Models
{
    public class CredentialListViewModel
    {
        public CredentialListViewModel()
        {
            this.CredentialList = new List<CredentialListItemViewModel>();
        }

        public IList<CredentialListItemViewModel> CredentialList { get; set; }
    }
    public class CredentialListItemViewModel
    {
        public int CredentialId { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "App Name")]
        public string AppName { get; set; }
        [Display(Name = "Usage Today")]
        public int UsageToday { get; set; }
    }
    public class CredentialCreateModel
    {
        public int CredentialId { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "App Name")]
        public string AppName { get; set; }
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

        [Display(Name = "Usage Today")]
        public int UsageToday { get; set; }
    }
    public class CredentialEditModel
    {
        public int CredentialId { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "App Name")]
        public string AppName { get; set; }
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

        [Display(Name = "Usage Today")]
        public int UsageToday { get; set; }
    }
}