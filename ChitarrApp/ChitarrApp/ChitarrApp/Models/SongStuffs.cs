using System;
using System.Collections.Generic;
using System.Text;

namespace ChitarrApp.Models
{
    public class SongToken
    {
        public static int noteToIndex(string noteName)
        {
            int rv = -1;
            rv = Array.IndexOf(tones, noteName);
            return rv;

        }
        static string[] tones = { "DO", "DO#", "RE", "MIb", "MI", "FA", "FA#", "SOL", "LAb", "LA", "SIb", "SI" };
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
                case SongTokenEnum.songTSpace: return new String(' ', numValue);
                case SongTokenEnum.songTNote:
                    if (numValue != -1) return tones[numValue] + txtValue;
                    else return "";
                case SongTokenEnum.songTText: return txtValue.Trim();
            }
            return "";
        }

        public void Transpose (int relative)
        {
            if (sType== SongTokenEnum.songTNote)
            {
                int maxIdx = tones.Length - 1;
                numValue = numValue + relative;
                if (numValue < 0) numValue = maxIdx;
                if (numValue > maxIdx) numValue = 0;
            }
          
        }
    }
    public class SongRow : System.Collections.Generic.List<SongToken>
    {
        private int fsize = 14;
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
            foreach (SongToken st in this)
            {
                oo = oo + st.ToString();
            }

            //if (isAccord()) return oo + "     @@@ACCORD";
            return oo;
        }
        public void Transpose(int relative)
        {
            if (isAccord())
            {
                foreach (SongToken tk in this)
                {
                    tk.Transpose(relative);
                }
            }
        }
        public string songRowString
        {
            get
            {
                return this.ToString();
            }
        }
        public Xamarin.Forms.Color songRowColor
        {
            get
            {
                if (isAccord()) return Xamarin.Forms.Color.Red;
                return Xamarin.Forms.Color.White;

            }
        }

             public Xamarin.Forms.FontAttributes songRowFontAtt
        {
            get
            {
                if (isAccord()) return Xamarin.Forms.FontAttributes.Bold;
                return Xamarin.Forms.FontAttributes.None;

            }
        }
        public int songRowFontSz
        {
            get
            {

                return fsize;

            } 
            set
            {
                fsize = value;
            }
        }
    }


    public class Song
    {
        public System.Collections.Generic.List<SongRow> Rows = new System.Collections.Generic.List<SongRow>();
        public string Name = "";
        public string Link = "";
        public int ID = 0;
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

        public void Transpose(int relative)
        {
            foreach (SongRow R in Rows) R.Transpose(relative);
        }
        public void ChangeFont(int relative)
        {
            foreach (SongRow R in Rows) R.songRowFontSz = R.songRowFontSz + relative;
        }
        public int currentFontSize()
        {

            if (Rows.Count > 0) return Rows[0].songRowFontSz;
            return 0;
        }


    }
}
