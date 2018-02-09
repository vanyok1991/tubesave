using System;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Text.Format;

namespace UTubeSave.Droid.Helpers
{
    public class SystemInfo
    {
        public static bool IsNetworkAvailable(Context context) {
            var connectivityManager = (ConnectivityManager)context?.GetSystemService(Context.ConnectivityService);
            NetworkInfo activeNetworkInfo = connectivityManager?.ActiveNetworkInfo;
            return activeNetworkInfo != null && activeNetworkInfo.IsConnected;
        }

        public static string GetFreeSpace(Context context)
        {
            if (context == null)
                return string.Empty;
            
            var stat = new StatFs(context.ApplicationInfo.DataDir);
            var blockSize = stat.BlockSizeLong;
            var availableBlocks = stat.AvailableBlocksLong;
            return Formatter.FormatFileSize(context, availableBlocks * blockSize);
        }
    }
}
