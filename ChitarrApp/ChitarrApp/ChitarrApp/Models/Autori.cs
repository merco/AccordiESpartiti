using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChitarrApp.Models
{
    public class Autori
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Nome { get; set; }
        public int IDCategoria { get; set; }
        public string Commento { get; set; }
        public string UiMainText
        {
            get
            {
                return Nome;
            }
        }
        public string UiDetailText
        {
            get
            {
                if (String.IsNullOrEmpty(Commento)) return "";
                return Commento.Replace("&quot;","'");
            }
        }
    }
}
