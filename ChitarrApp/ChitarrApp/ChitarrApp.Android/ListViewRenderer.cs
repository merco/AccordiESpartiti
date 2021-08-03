
using Android.Content;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ChitarrApp.Controls;
using ChitarrApp.Droid;
using System.Collections.Generic;

[assembly: ExportRenderer(typeof(MyListView), typeof(MyListViewRenderer))]
namespace ChitarrApp.Droid
{
    class MyListViewRenderer : ListViewRenderer
    {
        private bool IsInit = false;
        private Dictionary<int, int> listViewItemHeights = new Dictionary<int, int>();
        public MyListViewRenderer(Context context) : base(context)
        {

        }
       
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            var source = this.Element as MyListView;
            if (Control != null)
            {
                //Control.SetBackgroundColor(global::Android.Graphics.Color.LightGreen);
                int r1 = Control.FirstVisiblePosition;
                int r2 = Control.LastVisiblePosition;
      

            }
            if (source != null && this.Control != null)
            {
                if (this.IsInit == false )
                {
                    this.Control.Scroll += ListViewScrolled;
                    this.IsInit = true;
                }
            }
        }
        private void ListViewScrolled(object sender, Android.Widget.AbsListView.ScrollEventArgs e)
            {
            var item = this.Element as MyListView;
          

                if (item != null)
                {

                item.FirstVisibleItem = e.FirstVisibleItem;
                item.LastVisibleItem= e.FirstVisibleItem+e.VisibleItemCount;
                item.VisibleItemCount = e.VisibleItemCount;
                item.WasScrolled();
            }
            }

            private int GetYPosition()
            {
                if (this.Control != null)
                {
                    var c = this.Control.GetChildAt(0); //this is the first visible row
                    int scrollY = -c.Top;
                    if (listViewItemHeights.ContainsKey(this.Control.FirstVisiblePosition) == false)
                    {
                        listViewItemHeights.Add(this.Control.FirstVisiblePosition, c.Height);
                    }
                    for (int i = 0; i < this.Control.FirstVisiblePosition; ++i)
                    {
                        if (listViewItemHeights.ContainsKey(i) && listViewItemHeights[i] != 0)
                            scrollY += listViewItemHeights[i];
                    }
                    return scrollY;
                }
                return 0;
            }
     
    }
}