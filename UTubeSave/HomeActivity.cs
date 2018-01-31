using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.Gms.Ads.Reward;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using UTubeSave.View;
using YoutubeExtractor;

namespace UTubeSave
{
    [Activity(MainLauncher = true, Theme = "@android:style/Theme.NoTitleBar")]
    public class HomeActivity : Activity, IRewardedVideoAdListener
    {
        const string _appId = "ca-app-pub-6653353220256677~5913709845";
        const string _showId = "ca-app-pub-6653353220256677/4139972485";
        const string _downloadAdId = "ca-app-pub-6653353220256677/7893516677";
        const string _youtubeHomeUrl = "https://www.youtube.com/?app=desktop&persist_app=1&noapp=1";

        DownloadVideoView _downloadVideoView;
        Android.Views.ViewGroup _contentView;
        WebView _webView;
        Button _saveButton;
        InterstitialAd _mInterstitialAd;
        IRewardedVideoAd _downloadVideoAd;
        VideoInfo _currentVideoInfo;
        VideoInfo _currentAudioInfo;
        bool _isAudioDownloading;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Home);

            _contentView = FindViewById<ViewGroup>(Resource.Id.contentView);

            MobileAds.Initialize(this, _appId);
            var mAdView = FindViewById<AdView>(Resource.Id.adView);
            var adRequest = new AdRequest.Builder().Build();
            mAdView.LoadAd(adRequest);

            _saveButton = FindViewById<Button>(Resource.Id.saveButton);
            _saveButton.Click += SaveButtonClick;

            _webView = FindViewById<WebView>(Resource.Id.webView);
            var webClient = new YouTubeWebViewClient();

            _webView.SetWebViewClient(webClient);
            _webView.Settings.JavaScriptEnabled = true;
            _webView.LoadUrl(_youtubeHomeUrl);

            LoadInterstitialAd();
            LoadRewardedVideoAd();
        }

        void LoadInterstitialAd()
        {
            _mInterstitialAd = new InterstitialAd(this);
            _mInterstitialAd.AdUnitId = _showId;
            _mInterstitialAd.RewardedVideoAdClosed += InterstitialAdRewardedVideoAdClosed;
            _mInterstitialAd.LoadAd(new AdRequest.Builder().Build());
        }

        public override void OnBackPressed()
        {
            if(_downloadVideoView != null)
            {
                HideDownloadVideoView();
                return;
            }

            if (_webView.CanGoBack())
            {
                _webView.GoBack();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        void SaveButtonClick(object sender, EventArgs e)
        {
            ShowDownloadVideoView();
        }

        void ShowRewardAd()
        {
            if(_downloadVideoAd.IsLoaded)
            {
                _downloadVideoAd.Show();
            }else
            {
                Toast.MakeText(this, GetString(Resource.String.cannot_download), ToastLength.Short).Show();
            }
        }

        void ShowDownloadVideoView()
        {
            var videos = CheckVideoAvailability(_webView.OriginalUrl);
            if (videos?.Count() > 0)
            {
                _downloadVideoView = new DownloadVideoView(this, videos);
                _downloadVideoView.SaveAudioClicked += DownloadVideoViewSaveAudioClicked;
                _downloadVideoView.SaveVideoClicked += DownloadVideoViewSaveVideoClicked;
                _contentView.AddView(_downloadVideoView);
            }
        }

        void HideDownloadVideoView()
        {
            _contentView.RemoveView(_downloadVideoView);
            _downloadVideoView = null;
        }

        IEnumerable<VideoInfo> CheckVideoAvailability(string url)
        {
            try
            {
                IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(url);
                return videoInfos;
            }catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return null;
            }
        }

        void DownloadVideo(VideoInfo videoInfo)
        {
            try
            {
                if (videoInfo.RequiresDecryption)
                {
                    DownloadUrlResolver.DecryptDownloadUrl(videoInfo);
                }

                var videoDownloader = new VideoDownloader(videoInfo, Path.Combine(ApplicationInfo.DataDir, videoInfo.Title + videoInfo.VideoExtension));

                videoDownloader.DownloadProgressChanged += (sender, e) =>
                {
                    Console.WriteLine(e.ProgressPercentage);
                };

                var thread = new Thread(() =>
                {
                    try
                    {
                        videoDownloader.Execute();
                    }catch(Exception ex)
                    {
                        RunOnUiThread(() =>
                        {
                            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        });
                    }
                });

                thread.Start();

            }catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                Console.WriteLine($"DownloadVideo {ex.Message}");
            }
        }

        void DownloadAudio(VideoInfo audioInfo)
        {
            try
            {
                if (audioInfo.RequiresDecryption)
                {
                    DownloadUrlResolver.DecryptDownloadUrl(audioInfo);
                }

                var audioDownloader = new AudioDownloader(audioInfo, Path.Combine(ApplicationInfo.DataDir, audioInfo.Title + audioInfo.AudioExtension));

                audioDownloader.DownloadProgressChanged += (sender, e) =>
                {
                    Console.WriteLine(e.ProgressPercentage);
                };

                audioDownloader.AudioExtractionProgressChanged += (sender, e) => 
                {
                    Console.WriteLine(e.ProgressPercentage);
                };

                var thread = new Thread(() =>
                {
                    try
                    {
                        audioDownloader.Execute();
                    }
                    catch (Exception ex)
                    {
                        RunOnUiThread(() =>
                        {
                            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        });
                    }
                });

                thread.Start();

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                Console.WriteLine($"DownloadAudio {ex.Message}");
            }
        }

        void InterstitialAdRewardedVideoAdClosed(object sender, EventArgs e)
        {
            LoadInterstitialAd();
        }

        void LoadRewardedVideoAd()
        {
            _downloadVideoAd = MobileAds.GetRewardedVideoAdInstance(this);
            _downloadVideoAd.RewardedVideoAdListener = this;
            _downloadVideoAd.LoadAd(_downloadAdId, new AdRequest.Builder().Build());
        }

        public void OnRewarded(IRewardItem reward)
        {
            Console.WriteLine($"OnRewardedVideoAdOpened {reward.Amount}");
            if (_isAudioDownloading)
            {
                DownloadAudio(_currentAudioInfo);
            }
            else
            {
                DownloadVideo(_currentVideoInfo);
            }
        }

        public void OnRewardedVideoAdClosed()
        {
            LoadRewardedVideoAd();
            Console.WriteLine("OnRewardedVideoAdOpened");
        }

        public void OnRewardedVideoAdFailedToLoad(int errorCode)
        {
            Console.WriteLine($"OnRewardedVideoAdFailedToLoad ErrorCode: {errorCode}");
        }

        public void OnRewardedVideoAdLeftApplication()
        {
            Console.WriteLine("OnRewardedVideoAdLeftApplication");
        }

        public void OnRewardedVideoAdLoaded()
        {
            Console.WriteLine("OnRewardedVideoAdLoaded");
        }

        public void OnRewardedVideoAdOpened()
        {
            Console.WriteLine("OnRewardedVideoAdOpened");
        }

        public void OnRewardedVideoStarted()
        {
            Console.WriteLine("OnRewardedVideoStarted");
        }

        void DownloadVideoViewSaveAudioClicked(object sender, VideoInfo e)
        {
            _isAudioDownloading = true;
            _currentAudioInfo = e;
            HideDownloadVideoView();
            ShowRewardAd();
        }

        void DownloadVideoViewSaveVideoClicked(object sender, VideoInfo e)
        {
            _isAudioDownloading = false;
            _currentVideoInfo = e;
            HideDownloadVideoView();
            ShowRewardAd();
        }
    }
}
