using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrabAccordiSpartiti
{
    public class SongToken
    {
        public static int noteToIndex(string noteName)
        {
            int rv = -1;
            rv = Array.IndexOf(tones, noteName);
            if (rv==-1)
            {
                if (noteName.Contains("#")) noteName = noteName.Split('#')[0]+"#";
                rv = Array.IndexOf(tones, noteName);
            }
            return rv;

        }
        public static string indexToNote(int idx)
        {
            if (idx < 0) return "";
            return tones[idx];

        }
        static string[] tones = { "DO", "DO#", "RE", "RE#", "MI", "FA", "FA#", "SOL", "SOL#", "LA", "SIb", "SI" };
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


        private static Song decodeHtml(string page)
        {
            Song currentSong = new Song();

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(page);
            var nodeSongKeys = doc.DocumentNode.SelectNodes("//div[@class='chiave']");
            if (nodeSongKeys==null)
            {

                 var nPre = doc.DocumentNode.SelectNodes("//pre");
                nodeSongKeys = new HtmlNodeCollection(doc.DocumentNode);
                if (nPre != null && nPre.Count>=1)
                {
                    nodeSongKeys.Add(nPre[0]);
                } else return currentSong;


            }
            SongRow currentRow=null;
            SongToken currentToken=null;

            foreach (HtmlNode sk in nodeSongKeys)
            {
                foreach (HtmlNode nd in sk.ChildNodes)
                {
                    var nt = nd.NodeType.ToString();
                    if (nt == "Text")
                    {
                        string nodeValue = nd.InnerText;
                        if (nodeValue.Trim(new Char[] { ' ' ,'\t'}).StartsWith("\n"))
                        {
                            if (currentRow != null) currentSong.Rows.Add(currentRow);
                            currentRow = new SongRow();
                            nodeValue = nodeValue.Substring(1);
                        }
                        currentToken = new SongToken();
     
                        if (nodeValue.Trim() == "")
                        {

                           
                            currentToken.sType = SongToken.SongTokenEnum.songTSpace;
                            currentToken.numValue = nodeValue.Length;
                            currentRow.Add(currentToken);
                        }
                        else
                        {
                            
                            if (nodeValue.Contains("\n"))
                            {
                                //ci soo diverse righe di testo
                                if (currentRow != null  && currentRow.Count>0) currentSong.Rows.Add(currentRow);
                                currentRow = new SongRow();
                                var textElements=nodeValue.Split('\n');
                                int ce = 0;
                                foreach (string te in textElements)
                                {
                                    ce = ce + 1;
                                    currentToken = new SongToken();
                                    currentToken.sType = SongToken.SongTokenEnum.songTText;
                                    currentToken.txtValue = te;
                                    if (te.Trim()=="" && te!="")
                                    {
                                        currentToken.sType = SongToken.SongTokenEnum.songTSpace;
                                        currentToken.numValue = te.Length;
                                    }
                                    currentRow.Add(currentToken);

                                    if (ce<textElements.Length)
                                    {
                                        if (currentRow != null) currentSong.Rows.Add(currentRow);
                                        currentRow = new SongRow();
                                    } else
                                    {
                                        if (nodeValue.EndsWith("\n")) { 
                                            if (currentRow != null) currentSong.Rows.Add(currentRow);
                                            currentRow = new SongRow();
                                        }
                                    }
                                    

                                }


                            } else
                            {
                                currentToken.sType = SongToken.SongTokenEnum.songTText;
                                currentToken.txtValue = nodeValue.Replace("\n", "");

                                currentRow.Add(currentToken);

                                if (nodeValue.Trim(new Char[] { ' ' }).EndsWith("\n"))
                                {
                                    if (currentRow != null) currentSong.Rows.Add(currentRow);
                                    currentRow = new SongRow();
                                }
                            }
                            
                            
                            

                        }


                    }
                    if (nt == "Element")
                    {
           
                        var nota = nd.SelectSingleNode("span[@class='note']");
                        if (nota!=null)
                        {
                            if (nd.InnerText.Contains("/"))
                            {

                                var notine = nd.InnerText.Split('/');
                                int cntnotine = 0;
                                foreach (string notina in notine)
                                {
                                    cntnotine = cntnotine + 1;
                                    var strNota = notina;
                                    var alterazione = "";
                                    currentToken = new SongToken();
                                    currentToken.sType = SongToken.SongTokenEnum.songTNote;
                                    currentToken.numValue = SongToken.noteToIndex(notina);
                                   if (currentToken.numValue!=-1)
                                    {
                                        string lanota = SongToken.indexToNote(currentToken.numValue);
                                        if (lanota != "") alterazione = notina.Replace(lanota, "");
                                        currentToken.txtValue = alterazione;
                                        if (currentRow == null) currentRow = new SongRow();
                                        currentRow.Add(currentToken);
                                    } else
                                    {
                                        currentToken.sType = SongToken.SongTokenEnum.songTText;
                                        currentToken.txtValue = strNota;
                                        if (currentRow == null) currentRow = new SongRow();
                                        currentRow.Add(currentToken);
                                    }
                                    

                                    if (cntnotine<notine.Length) { 
                                        currentToken = new SongToken();
                                        currentToken.sType = SongToken.SongTokenEnum.songTText;
                                        currentToken.txtValue = "/";
                                        currentRow.Add(currentToken);
                                    }
                                }



                            } else
                            {
                                var strNota = nota.InnerText;
                                var alterazione = nd.InnerText.Replace(strNota, "");
                                currentToken = new SongToken();
                                currentToken.sType = SongToken.SongTokenEnum.songTNote;
                                currentToken.numValue = SongToken.noteToIndex(strNota);
                                currentToken.txtValue = alterazione;
                                if (currentRow == null) currentRow = new SongRow();
                                currentRow.Add(currentToken);
                            }
                            
                            
                            

                        } else
                        {
                            currentToken.sType = SongToken.SongTokenEnum.songTText;
                            currentToken.txtValue = nd.InnerText.Replace("\n", "");

                            currentRow.Add(currentToken);
                        }
                        
                    }
                }
            }
            
            return currentSong;
        }
        public async static Task<Song> getSongRawSong(String URL)
        {
            string Data = "";
            int a = 3;
            while (a>0) { 
                 Data=await UrlGrabber.GetURLChrome(URL);
                a = a - 1;
                if (!string.IsNullOrEmpty(Data)) a = 0;

            }
            return decodeHtml(Data);


        }
    }
}
