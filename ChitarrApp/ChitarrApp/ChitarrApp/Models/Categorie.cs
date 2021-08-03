using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChitarrApp.Models
{
    public class Categorie
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Nome { get; set; }

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
                return "";
            }
        }
    }
}
