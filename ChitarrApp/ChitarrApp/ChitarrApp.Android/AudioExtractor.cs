using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Nio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChitarrApp.Droid
{
    class AudioExtractor
    {
        private static  int DEFAULT_BUFFER_SIZE = 1 * 1024 * 1024;
        private static  String TAG = "AudioExtractorDecoder";

        /**
         * @param srcPath  the path of source video file.
         * @param dstPath  the path of destination video file.
         * @param startMs  starting time in milliseconds for trimming. Set to
         *                 negative if starting from beginning.
         * @param endMs    end time for trimming in milliseconds. Set to negative if
         *                 no trimming at the end.
         * @param useAudio true if keep the audio track from the source.
         * @param useVideo true if keep the video track from the source.
         * @throws IOException
          */
        public static string GetRealPathFromURI(Context context, Android.Net.Uri uri)
        {

            bool isKitKat = Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat;

            // DocumentProvider
            if (isKitKat && Android.Provider.DocumentsContract.IsDocumentUri(context, uri))
            {
                // ExternalStorageProvider
                if (isExternalStorageDocument(uri))
                {
                    string docId = Android.Provider.DocumentsContract.GetDocumentId(uri);
                    string[] split = docId.Split(':');
                    string type = split[0];

                    if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        return Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                    }

                    // TODO handle non-primary volumes
                }
                // DownloadsProvider
                else if (isDownloadsDocument(uri))
                {

                    string id = Android.Provider.DocumentsContract.GetDocumentId(uri);
                    Android.Net.Uri contentUri = ContentUris.WithAppendedId(Android.Net.Uri.Parse("content://downloads/public_downloads"), Convert.ToInt64(id));

                    return getDataColumn(context, contentUri, null, null);
                }
                // MediaProvider
                else if (isMediaDocument(uri))
                {
                    string docId = Android.Provider.DocumentsContract.GetDocumentId(uri);
                    string[] split = docId.Split(':');
                    string type = split[0];

                    Android.Net.Uri contentUri = null;
                    if ("image".Equals(type))
                    {
                        contentUri = Android.Provider.MediaStore.Images.Media.ExternalContentUri;
                    }
                    else if ("video".Equals(type))
                    {
                        contentUri = Android.Provider.MediaStore.Video.Media.ExternalContentUri;
                    }
                    else if ("audio".Equals(type))
                    {
                        contentUri = Android.Provider.MediaStore.Audio.Media.ExternalContentUri;
                    }

                    string selection = "_id=?";
                    string[] selectionArgs = new string[] {
                    split[1]
            };

                    return getDataColumn(context, contentUri, selection, selectionArgs);
                }
            }
            // MediaStore (and general)
            else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return getDataColumn(context, uri, null, null);
            }
            // File
            else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return uri.Path;
            }

            return null;
        }

        /**
         * Get the value of the data column for this Uri. This is useful for
         * MediaStore Uris, and other file-based ContentProviders.
         *
         * @param context The context.
         * @param uri The Uri to query.
         * @param selection (Optional) Filter used in the query.
         * @param selectionArgs (Optional) Selection arguments used in the query.
         * @return The value of the _data column, which is typically a file path.
         */
        public static String getDataColumn(Context context, Android.Net.Uri uri, String selection,
                String[] selectionArgs)
        {

            Android.Database.ICursor cursor = null;
            string column = "_data";
            string[] projection = {
                column
            };

            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs,
                        null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int column_index = cursor.GetColumnIndexOrThrow(column);
                    return cursor.GetString(column_index);
                }
            }
            finally
            {
                if (cursor != null)
                    cursor.Close();
            }
            return null;
        }


        /**
         * @param uri The Uri to check.
         * @return Whether the Uri authority is ExternalStorageProvider.
         */
        public static bool isExternalStorageDocument(Android.Net.Uri uri)
        {
            return "com.android.externalstorage.documents".Equals(uri.Authority);
        }

        /**
         * @param uri The Uri to check.
         * @return Whether the Uri authority is DownloadsProvider.
         */
        public static bool isDownloadsDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.downloads.documents".Equals(uri.Authority);
        }

        /**
         * @param uri The Uri to check.
         * @return Whether the Uri authority is MediaProvider.
         */
        public static bool isMediaDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.media.documents".Equals(uri.Authority);
        }
        public void genVideoUsingMuxer(String srcPath, String dstPath, int startMs, int endMs, bool useAudio, bool useVideo) 
        {
            // Set up MediaExtractor to read from the source.
            MediaExtractor extractor = new MediaExtractor();
        extractor.SetDataSource(srcPath);
            int trackCount = extractor.TrackCount;
        // Set up MediaMuxer for the destination.
        MediaMuxer muxer;
        muxer = new MediaMuxer(dstPath,  MuxerOutputType.Mpeg4);

            // HashMap<Integer, Integer> indexMap = new HashMap<Integer, Integer>(trackCount);
            System.Collections.Generic.Dictionary<int, int> indexMap = new System.Collections.Generic.Dictionary<int, int>();
        int bufferSize = -1;
        for (int i = 0; i<trackCount; i++) {
            MediaFormat format = extractor.GetTrackFormat(i);
        String mime = format.GetString(MediaFormat.KeyMime);
        bool selectCurrentTrack = false;
            if (mime.StartsWith("audio/") && useAudio) {
                selectCurrentTrack = true;
            } else if (mime.StartsWith("video/") && useVideo) {
                selectCurrentTrack = true;
            }
if (selectCurrentTrack)
{
    extractor.SelectTrack(i);
    int dstIndex = muxer.AddTrack(format);
    indexMap.Add(i, dstIndex);
    if (format.ContainsKey(MediaFormat.KeyMaxInputSize))
    {
        int newSize = format.GetInteger(MediaFormat.KeyMaxInputSize);
        bufferSize = newSize > bufferSize ? newSize : bufferSize;
    }
}
        }
        if (bufferSize < 0)
{
    bufferSize = DEFAULT_BUFFER_SIZE;
}
// Set up the orientation and starting time for extractor.
MediaMetadataRetriever retrieverSrc = new MediaMetadataRetriever();
retrieverSrc.SetDataSource(srcPath);
String degreesString = retrieverSrc.ExtractMetadata( MetadataKey.VideoRotation);
if (degreesString != null)
{
    int degrees = int.Parse(degreesString);
    if (degrees >= 0)
    {
        muxer.SetOrientationHint(degrees);
    }
}
if (startMs > 0)
{
    extractor.SeekTo(startMs * 1000,  MediaExtractorSeekTo.ClosestSync);
}
// Copy the samples from MediaExtractor to MediaMuxer. We will loop
// for copying each sample and stop when we get to the end of the source
// file or exceed the end time of the trimming.
int offset = 0;
int trackIndex = -1;
ByteBuffer dstBuf = ByteBuffer.Allocate(bufferSize);
MediaCodec.BufferInfo bufferInfo = new MediaCodec.BufferInfo();
muxer.Start();
while (true)
{
    bufferInfo.Offset = offset;
    bufferInfo.Size = extractor.ReadSampleData(dstBuf, offset);
    if (bufferInfo.Size < 0)
    {
        //Log.d(TAG, "Saw input EOS.");
        bufferInfo.Size = 0;
        break;
    }
    else
    {
                    bufferInfo.PresentationTimeUs = extractor.SampleTime;
        if (endMs > 0 && bufferInfo.PresentationTimeUs > (endMs * 1000))
        {
            Log.Debug(TAG, "The current sample is over the trim end time.");
            break;
        }
        else
        {
                        bufferInfo.Flags = (MediaCodecBufferFlags) extractor.SampleFlags;
                        trackIndex = extractor.SampleTrackIndex;
            muxer.WriteSampleData(indexMap[trackIndex], dstBuf, bufferInfo);
            extractor.Advance();
        }
    }
}
muxer.Stop();
muxer.Release();
            muxer.Dispose();

            //Java.IO.File FF = new Java.IO.File(dstPath);
            //var URIF =Android.Net.Uri.FromFile(FF);
           

            //var FF1 = GetRealPathFromURI(MainActivity.Instance.ApplicationContext, URIF);


            //int fileSize = (int)new System.IO.FileInfo(dstPath).Length;
            //byte[] buffer = null;

            //ContentValues values = new ContentValues();
            //ContentResolver contentResolver = MainActivity.Instance.ContentResolver;

            //var appName = "ChitarrApp";

            //values.Put(Android.Provider.MediaStore.IMediaColumns.Title, System.IO.Path.GetFileNameWithoutExtension(dstPath));
            //values.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, "audio/mpeg");
            //values.Put(Android.Provider.MediaStore.IMediaColumns.Size, fileSize);
            //values.Put(Android.Provider.MediaStore.Downloads.InterfaceConsts.DisplayName, System.IO.Path.GetFileNameWithoutExtension(dstPath));

            //Android.Net.Uri newUri = contentResolver.Insert(Android.Provider.MediaStore.Downloads.ExternalContentUri, values);

            //System.IO.Stream save;

            //try
            //{
            //    buffer = new byte[1024];
            //    save = contentResolver.OpenOutputStream(newUri);

            //    Android.Content.Res.AssetManager assets = MainActivity.Instance.Assets;
            //    Android.Content.Res.AssetFileDescriptor descriptor = Android.App.Application.Context.Assets.OpenFd(System.IO.Path.GetFileName(dstPath));
            //    System.IO.Stream inputstream = descriptor.CreateInputStream();
            //    inputstream.CopyTo(save);

            //    inputstream.Close();
            //    save.Close();

            //    inputstream.Dispose();
            //    save.Dispose();

            //}

            //catch (Exception ex)
            //{
            //    System.Console.WriteLine(ex.Message);

            //}


            return;
    }
    }
}