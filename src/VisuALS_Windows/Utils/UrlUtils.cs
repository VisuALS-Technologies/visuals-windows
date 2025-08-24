using System;
using System.Collections.Generic;
using System.Linq;

namespace VisuALS_WPF_App
{
    public static class UrlUtils
    {
        public static string GetAnglicizedUrl(string url)
        {
            string trimurl = url;
            trimurl = trimurl.Replace("https://", "");
            trimurl = trimurl.Replace("http://", "");
            trimurl = trimurl.Replace("www.", "");
            List<string> urlsplit = trimurl.Split('/').ToList();
            string domain = urlsplit[0];
            string urlparams = urlsplit.Last();
            urlsplit.RemoveAt(0);
            urlsplit.RemoveAt(urlsplit.Count - 1);
            urlsplit.Add(urlparams.Split('?')[0]);
            urlparams = String.Join("?", urlparams.Split('?').Skip(1));
            List<string> partwords = urlsplit.Where(wordlike).ToList();
            List<string> paramwords = urlparams.Split('&').Where(wordlike).ToList();
            return domain + "/" + String.Join("/…/", partwords) + "?" + String.Join("&…&", paramwords) + "…";
        }

        private static bool wordlike(string word)
        {
            float vowels = word.Count((x) => "aeiouAEIOU".IndexOf(x) >= 0);
            float consonants = word.Length - vowels;
            float ratio = consonants / vowels;
            return ratio >= 0.25 && ratio <= 2.5;
        }
    }
}
