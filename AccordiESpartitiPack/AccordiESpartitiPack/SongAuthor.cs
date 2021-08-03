using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccordiESpartitiPack
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
        public string NameClean()
        {
            string vo = Name;
            //\n\t\n\t\tTAB\n\t\n\t
            vo = vo.Replace("\n", "");
            vo = vo.Replace("\t", "");
            vo = vo.Replace("&amp;", "&");
           
            return vo;
        }
    }
    public class SongAuthors : System.Collections.Generic.List<SongAuthor>
    {

        public static string[] CATEGORIES = { "italiani", "internazionali", "cartoni", "popolari", "disney" };

    }

    public class SongsList :   System.Collections.Generic.List<Song>
    {
       

    }
}
