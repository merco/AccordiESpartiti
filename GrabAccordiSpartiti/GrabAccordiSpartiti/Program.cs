
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrabAccordiSpartiti
{
    class Program
    {
        public static string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        public static string AppPathCache = System.IO.Path.Combine(AppPath, "cache");
        public static string AppPathCacheLastID = System.IO.Path.Combine(AppPathCache, "LastID.txt");
        static void creaCartella (string path)
        {
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
        }
        public static IEnumerable<List<T>> SplitList<T>(List<T> bigList, int nSize = 3)
        {
            for (int i = 0; i < bigList.Count; i += nSize)
            {
                yield return bigList.GetRange(i, Math.Min(nSize, bigList.Count - i));
            }
        }
        static async Task DownloadLista(SongAuthors ListaAutori)
        {
            int startFrom = 0;
            if (System.IO.File.Exists(AppPathCacheLastID))
            {
                var txt = System.IO.File.ReadAllText(AppPathCacheLastID);
                if (!string.IsNullOrEmpty(txt))
                {
                    if (!int.TryParse(txt, out startFrom)) startFrom=0;
                }
            }

            int conteggio = 0;
            foreach (SongAuthor Autore in ListaAutori)
            {
                conteggio = conteggio + 1;
                if (conteggio <= startFrom) continue;


                string basefolder = Autore.pathName();
                if (string.IsNullOrEmpty(basefolder)) continue;
                Console.WriteLine(DateTime.Now.ToString() + "  Controllo per autore : " + Autore.Name + " " + conteggio.ToString() + "/" + ListaAutori.Count);
                int retry = 2;
                while (retry>0)
                {
                    Autore.songs = await SongsList.GetSongsList(Autore.Url);
                    if (Autore.songs.Count > 0) retry = 0;
                    retry = retry - 1;
                }
               

                string AppPathCacheAutore = System.IO.Path.Combine(AppPathCache, basefolder);
                creaCartella(AppPathCacheAutore);
                int conteggioCanzone = 0;
                foreach (Song CurrentSong in Autore.songs)
                {
                    conteggioCanzone = conteggioCanzone + 1;
                    Console.WriteLine("\t"+ DateTime.Now.ToString()+ "\t" + CurrentSong.Name + " " + conteggioCanzone.ToString() + "/" + Autore.songs.Count);
                    basefolder = CurrentSong.pathName();
                    string AppPathCacheAutoreSong = System.IO.Path.Combine(AppPathCacheAutore, basefolder + ".TXT");
                    if (System.IO.File.Exists(AppPathCacheAutoreSong)) continue;
                    var tmpSong = await Song.getSongRawSong(CurrentSong.Link);
                    CurrentSong.Rows = tmpSong.Rows;

                    //Console.WriteLine(CurrentSong.ToString());
                    if (CurrentSong.Rows.Count>0)
                    {
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(CurrentSong);
                        System.IO.File.WriteAllText(AppPathCacheAutoreSong, json);
                    }
           

                }


                if (System.IO.File.Exists(AppPathCacheLastID)) System.IO.File.Delete(AppPathCacheLastID);
                System.IO.File.WriteAllText(AppPathCacheLastID, conteggio.ToString());
            }
        }
        static async Task Main(string[] args)
        {

       
            creaCartella(AppPathCache);

           //var r = await Song.getSongRawSong("https://www.accordiespartiti.it/accordi/italiani/ligabue-luciano/ce-sempre-una-canzone/");
            // Console.WriteLine(r);
           // var ss= await SongsList.GetSongsList("https://www.accordiespartiti.it/cat/accordi/italiani/ligabue-luciano/");






            Console.WriteLine("Lettura lista autori...");
            var ListaAutori=await SongAuthors.GetAuthorList();

            string ListaAutoriFile = System.IO.Path.Combine(AppPathCache, "Autori.txt");
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(ListaAutori);
            if (System.IO.File.Exists(ListaAutoriFile)) System.IO.File.Delete(ListaAutoriFile);
            System.IO.File.WriteAllText(ListaAutoriFile, json);



            System.Collections.Generic.List<SongAuthors> multiListe = new System.Collections.Generic.List<SongAuthors>();
            int nSize = ListaAutori.Count / 1;
            for (int i = 0; i < ListaAutori.Count; i += nSize)
            {
                var ll = ListaAutori.GetRange(i, Math.Min(nSize, ListaAutori.Count - i));
                SongAuthors LA = new SongAuthors();
                LA.AddRange(ll);
                multiListe.Add(LA);
            }

            foreach (SongAuthors MiniLista in multiListe)
            {
                var t1 = DownloadLista(MiniLista);
            }



            Console.WriteLine("DONE!");
            Console.ReadLine();
        }
    }



}
