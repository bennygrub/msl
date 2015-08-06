using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MySelfie.Models
{
    public class UserCreateModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }

        public List<SelectListItem> Roles { get; set; }
    }

    public class UserListModel
    {
        public List<UserCreateModel> Users { get; set; }
    }
}