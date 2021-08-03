using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrabAccordiSpartiti
{
    public class SongAuthor
    {
        public string Name = "";
        public string Comment = "";
        public string Url = "";
        public string Category = "";
        public override string ToString()
        {
            return Name;
        }
        public SongsList songs = new SongsList();
        public string pathName ()
        {
            if (string.IsNullOrEmpty(Url)) return "";
            var T = Url.Split('/');
            return T[T.Length - 2];
        }
    }
    public class SongAuthors : System.Collections.Generic.List<SongAuthor>
    {
        public const string SITEURL = "https://www.accordiespartiti.it/accordi-chitarra/";
        static string[] CATEGORIES = { "italiani", "internazionali", "cartoni", "popolari", "disney" };
        private static void decodeHtml(SongAuthors list,string page, string category)
        {
     
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(page);
            //("//div[contains(@class, 'hello')]")


            var nodoCat = doc.DocumentNode.SelectSingleNode("//ul[@id='" + category + "']");
            if (nodoCat != null)
            {
                var Ancore = nodoCat.SelectNodes("a");

                if (Ancore==null)
                {
                    Ancore = nodoCat.SelectNodes("ul/li/a");
                }
                if (Ancore!=null)
                {
                    foreach (HtmlAgilityPack.HtmlNode NDA in Ancore)
                    {

                        SongAuthor sa = new SongAuthor();
                        sa.Name = NDA.InnerText;
                        sa.Url = NDA.GetAttributeValue("href", "");
                        if (!sa.Url.StartsWith("#"))
                        {
                            sa.Comment = NDA.GetAttributeValue("title", "");
                            sa.Category = category;
                            list.Add(sa);
                        }
                       
                    }
                }
            

            }

            return ;
        }
        public async static Task<SongAuthors> GetAuthorList()
        {
            var list = new SongAuthors();
            string Data = await UrlGrabber.GetURLChrome(SITEURL);


            foreach (string currentCat in CATEGORIES)
            {
                 decodeHtml(list,Data, currentCat);
            }

            return list;

        }
    }

    public class SongsList :   System.Collections.Generic.List<Song>
    {
       
        private static String decodeHtml(SongsList list, string page)
        {

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(page);
            //("//div[contains(@class, 'hello')]")


            var nodiCat = doc.DocumentNode.SelectNodes("//div[@class='archives']/a");
            if (nodiCat == null) return "";
            foreach (HtmlAgilityPack.HtmlNode Nd in nodiCat)
            {
                var L = Nd.GetAttributeValue("href", "");
                var T = Nd.InnerText;
                Song SS = new Song();
                SS.Link = L;

                SS.Name = T;
                list.Add(SS);
            }
          
           

            return "";
        }
        public async static Task<SongsList> GetSongsList(string URL)
        {
            var list = new SongsList();
            string Data = await UrlGrabber.GetURLChrome(URL);


   
          
                decodeHtml(list, Data);
           

            return list;

        }
    }
}
