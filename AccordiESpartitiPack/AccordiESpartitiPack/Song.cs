
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccordiESpartitiPack
{
    public class SongToken
    {
        public static int noteToIndex(string noteName)
        {
            int rv = -1;
            rv = Array.IndexOf(tones, noteName);
            return rv;

        }
        static string[] tones = { "DO", "REb", "RE", "MIb", "MI", "FA", "FA#", "SOL", "LAb", "LA", "SIb", "SI" };
        public enum SongTokenEnum
        {
            songTSpace,
            songTNote,
            songTText
        }
        public SongTokenEnum sType = SongTokenEnum.songTText;
        public int numValue = -1;
        public string txtValue = "";
        public bool isNote()
        {
            return sType == SongTokenEnum.songTNote;
        }
        public override string ToString()
        {
            switch (sType)
            {
                case SongTokenEnum.songTSpace: return new String(' ',  numValue);
                case SongTokenEnum.songTNote: if (numValue != -1) return tones[numValue] + txtValue;
                    else return "";
                case SongTokenEnum.songTText: return txtValue.Trim();
            }
            return "";
        }
    }
    public class SongRow : System.Collections.Generic.List<SongToken>
    {
        public enum SongRowEnum
        {
            chord,
            lyrics
           
        }
       // public SongRowEnum sType = SongRowEnum.lyrics;
       public bool isAccord()
        {
            foreach (SongToken st in this)
            {
                if (st.isNote()) return true;
            }
            return false;
        }
        public override string ToString()
        {
            String oo = "";
            bool acc = isAccord();
            foreach (SongToken st  in this)
            {
                oo = oo + st.ToString();
            }

            if (isAccord()) return oo + "     @@@ACCORD";
            return oo;
        }
    }


    public class Song
    {
        public System.Collections.Generic.List<SongRow> Rows= new System.Collections.Generic.List<SongRow>();
        public string Name = "";
        public string Link = "";
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("===" + Name + "===");
            foreach (SongRow R in Rows)
            {
                sb.AppendLine(R.ToString());
            }
            return sb.ToString();
        }
        public string pathName()
        {
            if (string.IsNullOrEmpty(Link)) return "";
            var T = Link.Split('/');
            return T[T.Length - 2];
        }
        public string NameClean()
        {
            string vo = Name;
            //\n\t\n\t\tTAB\n\t\n\t
            vo = vo.Replace("\n", "");
            vo = vo.Replace("\t", "");
            vo = vo.Replace("&amp;", "&");
            if (vo.EndsWith("TAB")) vo = vo.Replace("TAB", "");
            return vo;
        }


    }
}
