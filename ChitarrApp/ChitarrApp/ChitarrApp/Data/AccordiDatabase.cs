using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace ChitarrApp.Data
{
    public class AccordiDatabase
    {
        readonly SQLiteAsyncConnection database;

        public AccordiDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Models.Canzoni>().Wait();
        }
        public async Task<int> GetSoungCount()
        {
            var i= await database.Table<Models.Canzoni>().CountAsync();
            return i;
        }
        public async Task<int> GetAuthorsCount()
        {
            var i = await database.Table<Models.Autori>().CountAsync();
            return i;
        }
        public async Task<List<Models.Canzoni>> GetCanzoni(string name)
        {
            object[] sss = {  "%" + name + "%" };
            //SELECT Canzoni.ID,Canzoni.Titolo,Canzoni.IDAutore,Canzoni.DataBlob,Autori.Nome    FROM Canzoni INNER JOIN Autori ON Canzoni.IDAutore = Autori.ID;
            var data = await database.QueryAsync<Models.Canzoni>("SELECT Canzoni.ID,Canzoni.Titolo,Canzoni.IDAutore,Canzoni.DataBlob,Autori.Nome    FROM Canzoni INNER JOIN Autori ON Canzoni.IDAutore = Autori.ID WHERE Canzoni.Titolo LIKE ? ORDER BY Canzoni.Titolo", sss);
            return data;
        }
        public async Task<List<Models.Canzoni>> GetRandomCanzoni()
        {
            List<Models.Canzoni> vo = new List<Models.Canzoni>();
            var maxIdx = await GetSoungCount();
            Random rnd = new Random();
            for (int i=1; i<=10; i++)
            {
                int idx = rnd.Next(1, maxIdx);
                object[] sss = { idx };
                var data = await database.QueryAsync<Models.Canzoni>("SELECT Canzoni.ID,Canzoni.Titolo,Canzoni.IDAutore,Canzoni.DataBlob,Autori.Nome    FROM Canzoni INNER JOIN Autori ON Canzoni.IDAutore = Autori.ID WHERE Canzoni.Id = ?", sss);
                if (data != null) vo.AddRange(data);
            }
            
           
            return vo;
        }
        public async Task<List<Models.Canzoni>> GetCanzoniByAutore(int IdAutore)
        {
            object[] sss = { IdAutore };
            //SELECT Canzoni.ID,Canzoni.Titolo,Canzoni.IDAutore,Canzoni.DataBlob,Autori.Nome    FROM Canzoni INNER JOIN Autori ON Canzoni.IDAutore = Autori.ID;
            var data = await database.QueryAsync<Models.Canzoni>("SELECT Canzoni.ID,Canzoni.Titolo,Canzoni.IDAutore,Canzoni.DataBlob,Autori.Nome    FROM Canzoni INNER JOIN Autori ON Canzoni.IDAutore = Autori.ID WHERE Canzoni.IDAutore = ? ORDER BY Canzoni.Titolo", sss);
            return data;
        }
        public async Task<List<Models.Autori>> GetAutoriByCategoria(int catID)
        {
            object[] sss = { catID };
            //SELECT Canzoni.ID,Canzoni.Titolo,Canzoni.IDAutore,Canzoni.DataBlob,Autori.Nome    FROM Canzoni INNER JOIN Autori ON Canzoni.IDAutore = Autori.ID;
            var data = await database.QueryAsync<Models.Autori>("SELECT  Id,Nome From Autori Where IdCategoria = ? order by Nome", sss);
            return data;
        }
        public async Task<Models.Autori> GetAutore(int IdAutore)
        {
            object[] sss = { IdAutore };
            //SELECT Canzoni.ID,Canzoni.Titolo,Canzoni.IDAutore,Canzoni.DataBlob,Autori.Nome    FROM Canzoni INNER JOIN Autori ON Canzoni.IDAutore = Autori.ID;
            var data = await database.QueryAsync<Models.Autori>("SELECT  * from Autori Where ID =?", sss);
            if (data != null && data.Count >= 1) return data[0];
            return null;
        }
        public async Task<List<Models.Autori>> GetAutori(string name)
        {
            object[] sss = { "%" + name + "%" }; 

            if (name.Length==1)
            {
                sss[0] = name + "%";
            }
            //SELECT Canzoni.ID,Canzoni.Titolo,Canzoni.IDAutore,Canzoni.DataBlob,Autori.Nome    FROM Canzoni INNER JOIN Autori ON Canzoni.IDAutore = Autori.ID;
            var data = await database.QueryAsync<Models.Autori>("SELECT  * from Autori Where Nome like ?", sss);
            
            return data;
        }
        public async Task<List<Models.Categorie>> GetCategorie()
        {
      
            //SELECT Canzoni.ID,Canzoni.Titolo,Canzoni.IDAutore,Canzoni.DataBlob,Autori.Nome    FROM Canzoni INNER JOIN Autori ON Canzoni.IDAutore = Autori.ID;
            var data = await database.QueryAsync<Models.Categorie>("SELECT ID,Nome   FROM Categorie");
            return data;
        }
        public async Task<Models.Categorie> GetCategoria(int Id)
        {
            object[] sss = { Id };
          
            var data = await database.QueryAsync<Models.Categorie>("SELECT ID,Nome   FROM Categorie  Where ID =?", sss);
            if (data != null && data.Count >= 1) return data[0];
            return null;
        }


    }
}
