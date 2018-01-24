using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using YoutubeExtractor;
using System.Linq;
using System.IO;
using Android.Gms.Ads;

namespace UTubeSave
{
    [Activity(Label = "UTubeSave", MainLauncher = true)]
    public class MainActivity : Activity
    {
        string appId = "ca-app-pub-6653353220256677~5913709845";
        string bunnerId = "ca-app-pub-6653353220256677/9117978752";
        string showId = "ca-app-pub-6653353220256677/4139972485";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            MobileAds.Initialize(this, appId);

            var mAdView = FindViewById<AdView>(Resource.Id.adView);
            var adRequest = new AdRequest.Builder().Build();
            mAdView.LoadAd(adRequest);

            //var mInterstitialAd = new InterstitialAd(this);
            //mInterstitialAd.AdUnitId = GetString(Resource.String.test_interstitial_ad_unit_id);

            //mInterstitialAd.AdListener = new AdListener();



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

            //var videoDownloader = new VideoDownloader(video, Path.Combine(ApplicationInfo.DataDir, video.Title + video.VideoExtension));

            //// Register the ProgressChanged event and print the current progress
            //videoDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage);

            ///*
             //* Execute the video downloader.
             //* For GUI applications note, that this method runs synchronously.
             //*/
            //videoDownloader.Execute();

            var videoView = FindViewById<VideoView>(Resource.Id.SampleVideoView);

            var uri = Android.Net.Uri.Parse(Path.Combine(ApplicationInfo.DataDir, video.Title + video.VideoExtension));

            videoView.SetVideoURI(uri);

            //videoView.Start();
        }
    }
}

