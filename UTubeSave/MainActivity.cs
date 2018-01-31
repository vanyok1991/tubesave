using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Android.Gms.Ads;
using Android.OS;
using Android.Widget;
using UTubeSave.Droid.Extractor;
using YoutubeExtractor;

namespace UTubeSave.Droid
{
    [Activity]
    public class MainActivity : Activity
    {
        string appId = "ca-app-pub-6653353220256677~5913709845";
        //string bunnerId = "ca-app-pub-6653353220256677/9117978752";
        string showId = "ca-app-pub-6653353220256677/4139972485";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            MobileAds.Initialize(this, appId);

            string link = "https://www.youtube.com/watch?v=T8AgHAlwNeE";
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

            var mAdView = FindViewById<AdView>(Resource.Id.adView);
            var adRequest = new AdRequest.Builder().Build();
            mAdView.LoadAd(adRequest);

            var mInterstitialAd = new InterstitialAd(this);
            mInterstitialAd.AdUnitId = showId;
            mInterstitialAd.Rewarded += (sender, e) => 
            {
                Console.WriteLine("mInterstitialAd.Rewarded " + e.Reward.Amount);
            };

            mInterstitialAd.RewardedVideoAdClosed += (sender, e) => 
            {
                Console.WriteLine("mInterstitialAd.RewardedVideoAdClosed ");

                var videoView = FindViewById<VideoView>(Resource.Id.SampleVideoView);

                var uri = Android.Net.Uri.Parse(Path.Combine(ApplicationInfo.DataDir, video.Title + video.VideoExtension));

                videoView.SetVideoURI(uri);

                videoView.SetMediaController(new Android.Widget.MediaController(this));

                videoView.Start();
            };

            mInterstitialAd.RewardedVideoAdFailedToLoad += (sender, e) =>
            {
                 Console.WriteLine("mInterstitialAd.RewardedVideoAdFailedToLoad " + e.ErrorCode);
            };

            mInterstitialAd.RewardedVideoAdLeftApplication += (sender, e) =>
            {
                Console.WriteLine("mInterstitialAd.RewardedVideoAdLeftApplication ");
            };

            mInterstitialAd.RewardedVideoAdLoaded += (sender, e) =>
            {
                Console.WriteLine("mInterstitialAd.RewardedVideoAdLoaded ");
                mInterstitialAd.Show();
            };

            mInterstitialAd.RewardedVideoAdOpened += (sender, e) =>
            {
                Console.WriteLine("mInterstitialAd.RewardedVideoAdOpened ");
            };

            mInterstitialAd.RewardedVideoStarted += (sender, e) =>
            {
                Console.WriteLine("mInterstitialAd.RewardedVideoStarted ");
            };


            var videoDownloader = new VideoDownloader(video, Path.Combine(ApplicationInfo.DataDir, video.Title + video.VideoExtension));

            // Register the ProgressChanged event and print the current progress
            videoDownloader.DownloadProgressChanged += (sender, e) =>
            {
                Console.WriteLine(e.ProgressPercentage);

                if (e.ProgressPercentage == 100)
                {
                    mInterstitialAd.LoadAd(new AdRequest.Builder().Build());
                }
            };

            /*
             * Execute the video downloader.
             * For GUI applications note, that this method runs synchronously.
             */
            videoDownloader.Execute();
        }
    }
}