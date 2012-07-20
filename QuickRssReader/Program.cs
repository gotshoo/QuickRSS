using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Argotic.Syndication;
using Argotic.Extensions;
using Argotic.Extensions.Core;
using System.Web;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace QuickRssReader
{
    class Program
    {
        static void Main(string[] args)
        {
            //RssFeed feed = RssFeed.Create(new Uri("http://www.sj-r.com/entertainment/rss"));
            RssFeed feed = RssFeed.Create(new Uri("http://gotshoo.com/feed/"));
            //AtomFeed feed = AtomFeed.Create(new Uri("http://www.sj-r.com/entertainment/rss"));
            

            foreach (RssItem item in feed.Channel.Items)
            {

                Program p = new Program();
                // The items have been limited to the first 10 in the feed channel
                Console.WriteLine("Title:    " + item.Title);
                Console.WriteLine("Pub Date: " + item.PublicationDate.ToLongDateString());
                Console.WriteLine("Descript  " + item.Description);
                Console.WriteLine("URL       " + item.Link);
                Console.WriteLine("Source:   " + p.GetBlogPostContent(item,true));

                p.DownloadPostImages( p.GetPostImages(item));
            }


            Console.Write("Finished");
            Console.ReadKey();

        }

        public List<string> GetPostImages(RssItem i)
        {
            List<string> imagelist = new List<string>();
            foreach (ISyndicationExtension s in i.Extensions)
            {
                if (s is SiteSummaryContentSyndicationExtension)
                {
                    SiteSummaryContentSyndicationExtension ss = s as SiteSummaryContentSyndicationExtension;
                    var t = ss.Context.Encoded;
                    string regexImgSrc = "<img.+?src=[\"'](.+?)[\"'].+?>";
                    MatchCollection matches = Regex.Matches(t, regexImgSrc,RegexOptions.IgnoreCase | RegexOptions.Compiled);
                     
                   imagelist.AddRange(from Match m in matches
                                      select m.Groups[1].Value);
                                      //select Regex.Match(m.Value,"").Value);
       
                }
            }

            return imagelist;
        }

        public string GetBlogPostContent(RssItem i, bool removeHtml)
        {
            string content = null;
            foreach (ISyndicationExtension s in i.Extensions)
            {

                if (s is SiteSummaryContentSyndicationExtension)
                {

                    SiteSummaryContentSyndicationExtension ss = s as SiteSummaryContentSyndicationExtension;
                    var t = ss.Context.Encoded;
                    content = HttpUtility.HtmlDecode(t);
                    //content =

                    if (removeHtml)
                    {
                            
                        content = Regex.Replace(content, "<[^>]*>", "", RegexOptions.Compiled);
                    }
                }
            }
            return content;
        }

        public bool DownloadPostImages(List<string> imagelist)
        {
            imagelist.ForEach(delegate(String uri){
                WebClient wc = new WebClient();
                wc.DownloadFileAsync(new Uri(uri),"c:\\temp\\rss\\" + uri.Split('/').LastOrDefault());
            });
            
            return true;
        }
    }
}
