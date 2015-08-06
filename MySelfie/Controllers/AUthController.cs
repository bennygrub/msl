using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Shared.Extensions;
using System.IO;
using Newtonsoft.Json;

namespace MySelfie.Controllers
{
    public class AuthController : ApiController
    {
        [Route("Instagram")]
        [HttpGet]
        public void Instagram(string code, int wallID)
        {
            using (var db = new MySelfieEntities())
            {
                var wall = db.Walls.SingleOrDefault(x => x.WallId == wallID);

                if (wall.IsNotNull())
                {
                    var result = "";

                    try
                    {
                        var url = "https://api.instagram.com/oauth/access_token";
                        url += "?client_id=" + wall.Scrape_InstagramClientID;
                        url += "&client_secret=" + wall.Scrape_InstagramClientSecret;
                        url += "&grabt_type=authorization_code";
                        url += "&code=" + code;

                        var uri = new Uri(url);

                        var request = (HttpWebRequest)HttpWebRequest.Create(uri);
                        request.Method = "POST";
                        

                        using (var response = (HttpWebResponse)request.GetResponse())
                        {
                            var stream = response.GetResponseStream();
                            var reader = new StreamReader(stream);
                            result = reader.ReadToEnd();
                            reader.Close();
                            stream.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                    var json = JsonConvert.DeserializeObject<dynamic>(result);

                    wall.Scrape_InstagramToken = json.access_token;
                    //wall.Scrape_InstagramUserName = json.user.username;

                }
            }
        }
    }
}
