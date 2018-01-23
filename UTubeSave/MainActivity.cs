using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using YoutubeExtractor;
using System.Linq;
using System.Net;
using System;

namespace UTubeSave
{
    [Activity(Label = "UTubeSave", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            string link = "https://www.youtube.com/watch?v=DHHY8m3rEzU";
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link);

            VideoInfo video = videoInfos
                .First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);

            /*
             * If the video has a decrypted signature, decipher it
             */
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            var webClient = new WebClient();

            var url = new Uri(video.DownloadUrl);

            webClient.DownloadFileCompleted += (s, e) => {
                System.Diagnostics.Debug.WriteLine("DownloadFileCompleted");
                var sd = e.UserState;
            };

            webClient.DownloadProgressChanged += (sender, e) => 
            {
                System.Diagnostics.Debug.WriteLine("DownloadProgressChanged");
                var i = e.ProgressPercentage;
            };

            webClient.DownloadFile(url, Android.OS.Environment.ExternalStorageDirectory.Path + "/video.mp4");
        }
    }
}

