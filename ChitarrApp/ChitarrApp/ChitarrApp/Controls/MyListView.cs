using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ChitarrApp.Controls
{
    public class MyListView : ListView
    {

        public int FirstVisibleItem = -1;
        public int LastVisibleItem = -1;
        public int VisibleItemCount = -1;
        public void WasScrolled()
        {

        }


    }
   
}
