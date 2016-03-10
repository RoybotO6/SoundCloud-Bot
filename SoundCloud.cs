using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SoundCloudBot
{
    class SoundCloud
    {
        private string auth = " OAuth ", ua = UA();
        public SoundCloud(string auth)
        {
            this.auth += auth;
        }

        public void Follow(string user)
        {
            var web = (HttpWebRequest)WebRequest.Create("https://api.soundcloud.com/me/followings/" + user);
            web.Method = "PUT";
            doRequest(web);
        }

        public void UnFollow(string user)
        {
            var web = (HttpWebRequest)WebRequest.Create("https://api.soundcloud.com/me/followings/" + user);
            web.Method = "DELETE";
            doRequest(web);
        }

        public void LikeTrack(string id)
        {
            var web = (HttpWebRequest)WebRequest.Create("https://api.soundcloud.com/e1/me/track_likes/" + id);
            web.Method = "PUT";
            doRequest(web);
        }

        public void UnLikeTrack(string id)
        {
            var web = (HttpWebRequest)WebRequest.Create("https://api.soundcloud.com/e1/me/track_likes/" + id);
            web.Method = "DELETE";
            doRequest(web);
        }

        public void CommentTrack(string id, string text)
        {
            try
            {
                var web = (HttpWebRequest)WebRequest.Create("https://api.soundcloud.com/tracks/" + id + "/comments");
                web.Method = "POST";
                byte[] data = Encoding.ASCII.GetBytes("comment[body]=" + HttpUtility.UrlEncode(text));
                web.ContentLength = data.Length;
                web.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                web.Headers["Authorization"] = auth;
                web.UserAgent = ua;
                web.Timeout = 1000;
                web.Proxy = null;
                web.GetRequestStream().Write(data, 0, data.Length);
                web.GetResponse();
            }
            catch { }
        }

        private void doRequest(HttpWebRequest web)
        {
            try
            {
                web.Headers["Authorization"] = auth;
                web.UserAgent = ua;
                web.Timeout = 1000;
                web.Proxy = null;
                web.GetResponse();
            }
            catch { }
        }

        public List<Tuple<string, string>> Search(string text)
        {
            var web = (HttpWebRequest)WebRequest.Create("https://api-v2.soundcloud.com/search/autocomplete?q=" + HttpUtility.UrlEncode(text) + "&queries_limit=0&results_limit=100");
            web.Method = "GET";
            web.Headers["Authorization"] = auth;
            web.UserAgent = ua;
            web.Timeout = 1000;
            web.Proxy = null;
            try
            {
                using (var q = new StreamReader(web.GetResponse().GetResponseStream()))
                {
                    var data = q.ReadToEnd().Split(new[] { "id\":" }, StringSplitOptions.None);
                    var ret = new List<Tuple<string, string>>();
                    foreach (string d in data)
                    {
                        var res = d.Split(',')[0];
                        int o;
                        if (int.TryParse(res, out o))
                            ret.Add(new Tuple<string, string>(
                                res,
                                d.Split(new[] { "kind\":\"" }, StringSplitOptions.None)[1].Split('"')[0]));
                    }
                    return ret;
                }
            }
            catch { }
            return null;
        }

        public string GetOAuth()
        {
            return auth;
        }

        public static string UA()
        {
            return "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.116 Safari/537.36";
        }
    }
}
