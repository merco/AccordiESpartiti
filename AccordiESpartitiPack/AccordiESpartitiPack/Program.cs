
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccordiESpartitiPack
{
    class Program
    {
        public static string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        public static string AppPathDB = System.IO.Path.Combine(AppPath, "Accordi.db");

        public static byte[] Compress(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory,
                CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }
                return memory.ToArray();
            }
        }
        static void Main(string[] args)
        {

            //string testoCanzone1 = System.IO.File.ReadAllText(@"C:\DAVIDE\GrabAccordiESpartiti\GrabAccordiSpartiti\GrabAccordiSpartiti\bin\Debug\cache\883\aeroplano.txt");
            //Song theSong1 = Newtonsoft.Json.JsonConvert.DeserializeObject<Song>(testoCanzone1);

            //SimplePDFWriter pdfWriter = new SimplePDFWriter(842.0f, 1190.0f, 10.0f, 10.0f);
    
            //pdfWriter.Write(theSong1.ToString());
            //pdfWriter.Save(@"F:\pippo2.pdf");

            string baseDir = Properties.Settings.Default.HomeDir;
            if (!System.IO.Directory.Exists(baseDir))
            {
                Console.WriteLine("Cartella non valida " + baseDir);
                Console.ReadLine();
                return;
            }

            string fileAutori = System.IO.Path.Combine(baseDir, "Autori.txt");
            string fileDB = System.IO.Path.Combine(baseDir, "Accordi.db");
            if (System.IO.File.Exists(fileDB)) System.IO.File.Delete(fileDB);
            System.IO.File.Copy(AppPathDB, fileDB);



                if (!System.IO.File.Exists(fileAutori))
            {
                Console.WriteLine("fileAutori non valida " + fileAutori);
                Console.ReadLine();
                return;
            }

            fileAutori = System.IO.File.ReadAllText(fileAutori);
            int contaAutori = 0;
            int contaCanzoni = 0;
            using (var connection = new SQLiteConnection("Data Source="+ fileDB + ""))
            {
                connection.Open();
                SongAuthors ListaAutori = Newtonsoft.Json.JsonConvert.DeserializeObject<SongAuthors>(fileAutori);
                foreach (SongAuthor SA in ListaAutori)
                {
                    string subDir = System.IO.Path.Combine(baseDir, SA.pathName());
                    if (!System.IO.Directory.Exists(subDir)) continue;
                    var canzoni = System.IO.Directory.GetFiles(subDir, "*.txt");
                    if (canzoni.Length <= 0) continue;


                    contaAutori = contaAutori + 1;
                    var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO Autori (ID,Nome,IdCategoria,Commento) VALUES ($ID,$Nome,$IdCategoria,$Commento)";
                    command.Parameters.AddWithValue("$ID", contaAutori);
                    command.Parameters.AddWithValue("$Nome", SA.NameClean());

                    command.Parameters.AddWithValue("$IdCategoria", Array.IndexOf(SongAuthors.CATEGORIES, SA.Category));
                    command.Parameters.AddWithValue("$Commento", SA.Comment);
                    command.ExecuteNonQuery();
                    command.Dispose();

                    foreach (string canzone in canzoni)
                    {
                        contaCanzoni = contaCanzoni + 1;
                        string testoCanzone = System.IO.File.ReadAllText(canzone);
                        byte[] arrayCanzone = System.IO.File.ReadAllBytes(canzone);
                        byte[] compressedCanzone = Compress(arrayCanzone);

                         Song theSong = Newtonsoft.Json.JsonConvert.DeserializeObject<Song>(testoCanzone);
                        //SA.songs.Add(theSong);

                        var commandCanzone = connection.CreateCommand();
                        commandCanzone.CommandText = "INSERT INTO Canzoni (ID,Titolo,IdAutore,DataBlob) VALUES ($ID,$Titolo,$IdAutore,$DataBlob)";
                        commandCanzone.Parameters.AddWithValue("$ID", contaCanzoni);
                        commandCanzone.Parameters.AddWithValue("$Titolo", theSong.NameClean());

                        commandCanzone.Parameters.AddWithValue("$IdAutore", contaAutori);
                        // commandCanzone.Parameters.AddWithValue("$Data", testoCanzone);

                        var P1 = commandCanzone.Parameters.Add("$DataBlob", System.Data.DbType.Binary, 20);
                            P1.Value = compressedCanzone;
                        commandCanzone.ExecuteNonQuery();
                        commandCanzone.Dispose();

                    }

                }


                connection.Close();
            } //using (var connection = new SqliteConnection("Data Source=hello.db"))


           
        }
    }
}
