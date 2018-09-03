using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using WordSpy.Interfaces;

namespace WordSpy.Services
{
    public class DownloadService : IDownload
    {
        public string GetHTML(string url)
        {
            string html;

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.Credentials = CredentialCache.DefaultNetworkCredentials;
                    html = client.DownloadString(url);
                }
                catch(Exception e)
                {
                    html = null;
                }
            }

            return html;
        }

        public IEnumerable<string> GetUrls(string html)
        {
            List<string> Links = new List<string>();
            // string HRefPattern = @"(?i)<\s*?a\s+[\S\s\x22\x27\x3d]*?href=[\x22\x27]?([^\s\x22\x27<>]+)[\x22\x27]?.*?>";
            string HRefPattern = "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";
            Match m = Regex.Match(html, HRefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            while (m.Success)
            {
                Links.Add(m.Groups[1].Value);
                m = m.NextMatch();
            }
            return Links.Distinct();
        }
    }
}
