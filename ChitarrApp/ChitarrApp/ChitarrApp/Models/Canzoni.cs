using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChitarrApp.Models
{
    public class Canzoni
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Titolo { get; set; }
        public int IDAutore { get; set; }
        public byte[] DataBlob { get; set; }
        public string Nome { get; set; }
        public string UiMainText
        {
            get
            {
                return Titolo;
            }
        }
        public string UiDetailText
        {
            get
            {
                return Nome;
            }
        }

        public Task<Song> GetSong()
        {


            var f1 = Uty.Decompress(DataBlob);
            string asciiString = System.Text.Encoding.UTF8.GetString(f1, 0, f1.Length);
            Song SS = Newtonsoft.Json.JsonConvert.DeserializeObject<Song>(asciiString);
            SS.ID = this.ID;
            return Task.FromResult<Song>(SS) ;

        }
    }
}
