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
using UTubeSave.AdMob;
using UTubeSave.Droid.Extractor;
using UTubeSave.Droid.Helpers;
using UTubeSave.Droid.Model;
using UTubeSave.Droid.Views;
using YoutubeExtractor;

namespace UTubeSave.Droid
{
    [Activity(MainLauncher = true, Theme = "@android:style/Theme.NoTitleBar")]
    public class HomeActivity : Activity, IRewardedVideoAdListener
    {
        const string _appId = "ca-app-pub-6653353220256677~5913709845";
        const string _showId = "ca-app-pub-6653353220256677/4139972485";
        const string _downloadAdId = "ca-app-pub-6653353220256677/7893516677";
        const string _youtubeHomeUrl = "https://www.youtube.com/?app=desktop&persist_app=1&noapp=1";

        DownloadVideoView _downloadVideoView;
        ViewGroup _contentView;
        ViewGroup _downloadsView;
        WebView _webView;
        ImageButton _saveButton;
        InterstitialAd _mInterstitialAd;
        IRewardedVideoAd _downloadVideoAd;
        VideoInfo _currentVideoInfo;
        VideoInfo _currentAudioInfo;
        bool _isAudioDownloading;
        bool _useShowAdForDownloading;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Home);

            _contentView = FindViewById<ViewGroup>(Resource.Id.contentView);
            _downloadsView = FindViewById<ViewGroup>(Resource.Id.currentDownloads);

            var updateButton = FindViewById<ImageButton>(Resource.Id.updateButton);
            updateButton.Click += (sender, e) =>
            {
                _webView?.Reload();
            };

            var savedButton = FindViewById<ImageButton>(Resource.Id.savedButton);
            savedButton.Click += (sender, e) => 
            {
                ShowSavedAd();
            };

            MobileAds.Initialize(this, _appId);
            var mAdView = FindViewById<AdView>(Resource.Id.adView);
            var adRequest = new AdRequest.Builder().Build();
            mAdView.LoadAd(adRequest);

            _saveButton = FindViewById<ImageButton>(Resource.Id.saveButton);
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
            if(_useShowAdForDownloading && _mInterstitialAd.IsLoaded)
            {
                _mInterstitialAd.Show();
                return;
            }

            if(_downloadVideoAd.IsLoaded)
            {
                _downloadVideoAd.Show();
            }else
            {
                Toast.MakeText(this, GetString(Resource.String.cannot_download), ToastLength.Short).Show();
            }
        }

        void ShowSavedAd()
        {
            if (_mInterstitialAd.IsLoaded)
            {
                _mInterstitialAd.Show();
            }else
            {
                StartActivity(typeof(SavedVideosActivity));
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

                var downloadView = new DownloadItemView(this, videoDownloader);

                downloadView.DownloadFinished += (sender, e) => 
                {
                    RunOnUiThread(() =>
                    {
                        _downloadsView.RemoveView(sender as View);
                    });
                    var video = new Video(e.Video.Title, e.SavePath);
                    Storage.Instance.SaveVideo(video);
                };

                _downloadsView.AddView(downloadView);

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
            _mInterstitialAd.LoadAd(new AdRequest.Builder().Build());
            if(_useShowAdForDownloading)
            {
                DownloadCurrentContent();
            }
            else
            {
                StartActivity(typeof(SavedVideosActivity));
            }
        }

        void DownloadCurrentContent()
        {
            if (_isAudioDownloading)
            {
                DownloadAudio(_currentAudioInfo);
            }
            else
            {
                DownloadVideo(_currentVideoInfo);
            }
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
            DownloadCurrentContent();
        }

        public void OnRewardedVideoAdClosed()
        {
            _downloadVideoAd.LoadAd(_downloadAdId, new AdRequest.Builder().Build());
            Console.WriteLine("OnRewardedVideoAdOpened");
        }

        public void OnRewardedVideoAdFailedToLoad(int errorCode)
        {
            Console.WriteLine($"OnRewardedVideoAdFailedToLoad ErrorCode: {errorCode}");

            if(errorCode == ErrorCode.ERROR_CODE_NO_FILL)
            {
                _useShowAdForDownloading = true;
            }
            _downloadVideoAd.LoadAd(_downloadAdId, new AdRequest.Builder().Build());
        }

        public void OnRewardedVideoAdLeftApplication()
        {
            Console.WriteLine("OnRewardedVideoAdLeftApplication");
        }

        public void OnRewardedVideoAdLoaded()
        {
            Console.WriteLine("OnRewardedVideoAdLoaded");
            _useShowAdForDownloading = false;
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
