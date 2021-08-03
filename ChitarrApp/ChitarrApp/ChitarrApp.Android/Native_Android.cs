using Xamarin.Forms;

using Android.App;
using System.Threading.Tasks;
using System;
using Xamarin.Essentials;
using Android.Content;


using Android.OS;

[assembly: UsesPermission(Android.Manifest.Permission.Vibrate)]
[assembly: Dependency(typeof(ChitarrApp.Droid.Native_Android))]
namespace ChitarrApp.Droid
{
    public class Native_Android :  ChitarrApp.Services.INative
    {


        public void SendLink(string newUrl)
        {
            

          
                Intent shareIntent = new Intent();
                StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
                StrictMode.SetVmPolicy(builder.Build());
                Android.Net.Uri uriToImage = Android.Net.Uri.Parse($"file://{newUrl}");

                shareIntent.SetAction(Intent.ActionSend);
                shareIntent.SetType("image/*");
                shareIntent.PutExtra(Intent.ExtraStream, uriToImage);
                shareIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
            
            MainActivity.Instance.StartActivity(Intent.CreateChooser(shareIntent, "Share image"));
            

        }
        public void OpenPDFLink(string newUrl)
        {

            Java.IO.File JavaFile = new Java.IO.File(newUrl);
            Android.Net.Uri Path = FileProvider.GetUriForFile(global::Android.App.Application.Context, global::Android.App.Application.Context.PackageName + ".fileprovider", JavaFile);

            string Extension = global::Android.Webkit.MimeTypeMap.GetFileExtensionFromUrl(Path.ToString());
            string MimeType = global::Android.Webkit.MimeTypeMap.Singleton.GetMimeTypeFromExtension(Extension);


            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());
            Intent Intent = new Intent(Intent.ActionView);

            Intent.SetDataAndType(Path, MimeType);
            Intent.AddFlags(ActivityFlags.GrantReadUriPermission);
            //Intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);


      
            try
            {
                MainActivity.Instance.StartActivity(Intent.CreateChooser(Intent, "PDF"));
            }
            catch (System.Exception E1)
            {
                Android.Widget.Toast.MakeText(MainActivity.Instance,E1.Message, Android.Widget.ToastLength.Short).Show();
            }

        }



        public string getPath(string fname)
        {
            var javafile = Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads);
            //var javafile = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);

            //var javafile = (Android.OS.Environment.GetExternalStoragePublicDirectory( Android.OS.Environment.DirectoryPictures));
            string javafile1 = System.IO.Path.Combine(javafile.AbsolutePath, fname);
            return javafile1;
        }

        public string getDownloadPath()
        {

            //var dir1 = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).AbsolutePath;
            //return dir1;

            var javafile = Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryMusic);
            return javafile.AbsolutePath;
        }

        public string convertToMp3(string fname)
        {

            try
            {
                var ae = new AudioExtractor();
                var fnamed = System.IO.Path.ChangeExtension(fname, ".mp3");
                ae.genVideoUsingMuxer(fname, fnamed, -1, -1, true, false);


            }
            catch (Exception E1) {
                return E1.Message;
            }
           
            return "";

        }

        public void OpenFolder(string pathbase)
        {
            Intent shareIntent = new Intent(Intent.ActionView);
            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());
           

            Java.IO.File JavaFile = new Java.IO.File(pathbase);
            
            Android.Net.Uri Path = FileProvider.GetUriForFile(global::Android.App.Application.Context, global::Android.App.Application.Context.PackageName + ".fileprovider", JavaFile);

            Android.Net.Uri mydir =  Android.Net.Uri.Parse("file://" + pathbase);
            //shareIntent.SetDataAndType(mydir, Android.Provider.DocumentsContract.Document.MimeTypeDir);
            shareIntent.SetDataAndType(mydir, "audio/*");
            shareIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
            shareIntent.AddFlags(ActivityFlags.NewTask);
           // shareIntent.AddCategory(Intent.CategoryOpenable);
            //shareIntent.SetType("audio/*");
           MainActivity.Instance.StartActivity(shareIntent);

            //MainActivity.Instance.StartActivity(Intent.CreateChooser(shareIntent, "Folder"));
        }
    }
}
