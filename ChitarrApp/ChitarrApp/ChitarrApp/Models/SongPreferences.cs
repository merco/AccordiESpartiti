using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitarrApp.Models
{
    public class SongPreferences
    {
        public const string SongPreferences_KEY = "SongPreferencesKey";
        public System.Collections.Generic.List<Models.Canzoni> Pref = new System.Collections.Generic.List<Models.Canzoni>();
        public System.Collections.Generic.List<Models.Canzoni> Last = new System.Collections.Generic.List<Models.Canzoni>();
        public Task<bool> Save()
        {
            string ss = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            Xamarin.Essentials.Preferences.Set(SongPreferences_KEY, ss);
            return Task.FromResult(true);

        }
        public System.Collections.Generic.List<Models.Canzoni> LastREV()
        {
            var c = new System.Collections.Generic.List<Models.Canzoni>();
            c.AddRange(Last);

            c.Reverse();
            return c;
        }
        public SongPreferences Load()
        {
            SongPreferences sp = new SongPreferences();
            if (Xamarin.Essentials.Preferences.ContainsKey(SongPreferences_KEY))
            {
                string ss = Xamarin.Essentials.Preferences.Get(SongPreferences_KEY, "");
                sp = Newtonsoft.Json.JsonConvert.DeserializeObject<SongPreferences>(ss);
            }
           
          return sp;

        }
        public void addLast(Models.Canzoni C)
        {
            //if (!Last.Contains(C)) Last.Add(C);
            if (!songContains(Last, C)) Last.Add(C);

            if (Last.Count > 10) Last.RemoveAt(0);
        }
        public void addPref(Models.Canzoni C)
        {
            //if (!Pref.Contains(C)) Pref.Add(C);

            if (!songContains(Pref,C)) Pref.Add(C);

        }
        public void remPref(Models.Canzoni C)
        {
            bool vo = false;
            var cc = Pref.FirstOrDefault<Models.Canzoni>(X => X.Titolo == C.Titolo && X.DataBlob.Length == C.DataBlob.Length);
            if (cc != null) Pref.Remove(cc);

        }
        public bool songContains(System.Collections.Generic.List<Models.Canzoni> List, Models.Canzoni C)
        {
            bool vo = false;
           var cc= List.FirstOrDefault<Models.Canzoni>(X => X.Titolo == C.Titolo && X.DataBlob.Length==C.DataBlob.Length);

            vo=(cc!=null);
            return vo;
        }

    }
}
