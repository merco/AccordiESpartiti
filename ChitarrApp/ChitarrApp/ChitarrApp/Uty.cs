using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace ChitarrApp
{
    public class Uty
    {
        static bool setupDone = false;
        public static string SearchType = "";
        public static Models.SongPreferences SongPref;
        static Data.AccordiDatabase _database;
        public static Models.Canzoni CurrentSong=null;
        public static Models.Autori CurrentAutore = null;
        // Create the database connection as a singleton.
        public static Data.AccordiDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    _database = new Data.AccordiDatabase(dbPath);
                }
                return _database;
            }
        }
        // Create the database connection as a singleton.

        public static string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Accordi.db");
        public static string dbVersionPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ver.txt");
        public static System.Reflection.Assembly LocalAssembly;
        public static  String AssemblyName = "ChitarrApp";
        public static byte[] getFile(string Name)
        {

 

            System.IO.Stream tStream = LocalAssembly.GetManifestResourceStream(AssemblyName + ".intres." + Name);
            byte[] imgByteArray = null;
            var length = tStream.Length;
            imgByteArray = new byte[length];
            tStream.Read(imgByteArray, 0, (int)length);

            tStream.Dispose();
            return imgByteArray;

        }
        public static string getFileAsString(string Name)
        {
            var bb = getFile(Name);
            string asciiString = Encoding.UTF8.GetString(bb, 0, bb.Length);
            return asciiString;
        }
        public static List<Models.Canzoni> RandomPlays;

        public static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (System.IO.Compression.GZipStream stream = new GZipStream(new MemoryStream(gzip),
                CompressionMode.Decompress))
            {
                const int size = 8192;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
 
    }
        public static string getInstalledVer()
        {
            if (!File.Exists(dbVersionPath)) return "00000000";

            string sVer = System.IO.File.ReadAllText(dbVersionPath);
           

            return sVer;

        }
    public static System.Threading.Tasks.Task<bool> AutoSetup()
        {
            
            
            if (setupDone) return Task.FromResult(true);
            SongPref = new Models.SongPreferences();
            SongPref = SongPref.Load();


            string StrversioneDistribuita = getFileAsString("ver.txt");
      

            string versioneInstallata = getInstalledVer();
            int rv = string.Compare(versioneInstallata, StrversioneDistribuita);
            if (rv <0) System.IO.File.Delete(dbPath);
            
            
            if (!File.Exists(dbPath))
            {
                System.IO.File.WriteAllText(dbVersionPath, StrversioneDistribuita);
                var embeddedResourceDbStream = LocalAssembly.GetManifestResourceStream(AssemblyName + ".intres.Accordi.db");
                using (var br = new BinaryReader(embeddedResourceDbStream))
                {
                    using (var bw = new BinaryWriter(new FileStream(dbPath, FileMode.Create)))
                    {
                        var buffer = new byte[8192*8];
                        int len;
                        while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bw.Write(buffer, 0, len);
                        }
                    }
                }

            }
            setupDone = true;
            return Task.FromResult(true);
        }

        public static void SendLink(string newUrl)
        {
            Xamarin.Forms.DependencyService.Get<Services.INative>().SendLink(newUrl);
        }
        public static void OpenFolder(string newUrl)
        {
            Xamarin.Forms.DependencyService.Get<Services.INative>().OpenFolder(newUrl);
        }
        public static void OpenPDFLink(string newUrl)
        {
            Xamarin.Forms.DependencyService.Get<Services.INative>().OpenPDFLink(newUrl);
        }
        public static string GetPath(string newUrl)
        {
            return Xamarin.Forms.DependencyService.Get<Services.INative>().getPath(newUrl);
        }
        public static string GetDownloadPath(string fileName)
        {
            var appo= Xamarin.Forms.DependencyService.Get<Services.INative>().getDownloadPath();
            if (!string.IsNullOrEmpty(fileName)) return System.IO.Path.Combine(appo, fileName);
            return appo;
        }
        public static string ToMp3(string fileName)
        {
            var appo = Xamarin.Forms.DependencyService.Get<Services.INative>().convertToMp3(fileName);
            return appo;
        }

        public static async Task<string> DownloadPageAsync(string Url)
        {
            var _httpClient = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(150)};

            try
            {
                using (var httpResponse = await _httpClient.GetAsync(Url))
                {
                    if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return await httpResponse.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        //Url is Invalid
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                //Handle Exception
                return null;
            }
        }

        public static async Task<string> GetVideoID(string query)
        {
            //string KURL = "/watch?v=";
            //Uri U = new Uri("https://www.youtube.com/results?search_query=" + query);
            //var spage = await DownloadPageAsync(U.AbsoluteUri);
            //if (string.IsNullOrEmpty(spage)) return "";

            //var idx = spage.IndexOf(KURL);
            //if (idx == -1) return "";

            //var sstr = spage.Substring(idx, 50);
            //idx = sstr.IndexOf('"');
            //if (idx == -1) return "";
            //sstr = sstr.Substring(0, idx).Replace(KURL, "");
            Uri U = new Uri("https://davidemercanti.altervista.org/chitarrapp/query.php?q=" + query);
            var spage = await DownloadPageAsync(U.AbsoluteUri);
            return spage.Trim() ;
        }

        
        public static async Task<string> DoMP3(string query, string songName)
        {
            string thisErr = "";
         
            try
            {
                var VideoID = await GetVideoID(query);
                if (String.IsNullOrEmpty(VideoID)) return "Non trovato su YouTube";

                var pathVideo = Uty.GetDownloadPath(songName + ".mp4");
                if (System.IO.File.Exists(pathVideo)) System.IO.File.Delete(pathVideo);
                var youtube = VideoLibrary.YouTube.Default;
              
                var vid = youtube.GetVideo("https://www.youtube.com/watch?v=" + VideoID);
                System.IO.File.WriteAllBytes(pathVideo, vid.GetBytes());
                thisErr = Uty.ToMp3(pathVideo);
                var outPath = System.IO.Path.ChangeExtension(pathVideo, ".mp3");
                if (System.IO.File.Exists(pathVideo)) System.IO.File.Delete(pathVideo);
                return "@" + outPath;
            }
          catch (Exception E1)
            {
                thisErr = E1.Message;
            }
           




            return thisErr;
        }

    }

 


}
