using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Gms.Ads;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using UTubeSave.Droid.Extractor;
using UTubeSave.Droid.Helpers;
using UTubeSave.Droid.Model;
using UTubeSave.Droid.Views;

namespace UTubeSave.Droid
{
    [Activity(MainLauncher = true, Theme = "@android:style/Theme.NoTitleBar")]
    public class HomeActivity : Activity
    {
        const string _youtubeHomeUrl = "https://www.youtube.com/?app=desktop&persist_app=1&noapp=1";

        DownloadVideoView _downloadVideoView;
        ViewGroup _contentView;
        ViewGroup _downloadsView;
        WebView _webView;
        View _activityView;
        ImageButton _saveButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Home);

            _contentView = FindViewById<ViewGroup>(Resource.Id.contentView);
            _downloadsView = FindViewById<ViewGroup>(Resource.Id.currentDownloads);
            _activityView = FindViewById(Resource.Id.activityBar);

            var updateButton = FindViewById<ImageButton>(Resource.Id.updateButton);
            updateButton.Click += (sender, e) =>
            {
                _webView?.Reload();
            };

            var savedButton = FindViewById<ImageButton>(Resource.Id.savedButton);
            savedButton.Click += async (sender, e) =>
            {
                await ShowSavedAd();
            };

            Advertistment.Instance.InitApp(this);

            var mAdView = FindViewById<AdView>(Resource.Id.adView);
            Advertistment.Instance.LoadAd(mAdView);

            _saveButton = FindViewById<ImageButton>(Resource.Id.saveButton);
            _saveButton.Click += SaveButtonClick;

            _webView = FindViewById<WebView>(Resource.Id.webView);
            var webClient = new YouTubeWebViewClient();

            _webView.SetWebViewClient(webClient);
            _webView.Settings.JavaScriptEnabled = true;
            _webView.LoadUrl(_youtubeHomeUrl);
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

        async void SaveButtonClick(object sender, EventArgs e)
        {
            await ShowDownloadVideoView();
        }

        async Task ShowRewardAdAndDownload(VideoInfo videoInfo)
        {
            _activityView.Visibility = ViewStates.Visible;
            var result = await Advertistment.Instance.ShowRewardAd(this);
            _activityView.Visibility = ViewStates.Gone;

            if(result)
            {
                HideDownloadVideoView();
                DownloadVideo(videoInfo);
            }else
            {
                Toast.MakeText(this, GetString(Resource.String.cannot_download), ToastLength.Short).Show();
            }
        }

        async Task ShowSavedAd()
        {
            _activityView.Visibility = ViewStates.Visible;
            await Advertistment.Instance.ShowBetweenPagesAd(this);
            _activityView.Visibility = ViewStates.Gone;

            StartActivity(typeof(SavedVideosActivity));
        }

        async Task ShowDownloadVideoView()
        {
            _activityView.Visibility = ViewStates.Visible;
            var videos = await CheckVideoAvailability(_webView.OriginalUrl);
            _activityView.Visibility = ViewStates.Gone;

            if (videos?.Count() > 0)
            {
                _downloadVideoView = new DownloadVideoView(this, videos);
                _downloadVideoView.SaveVideoClicked += DownloadVideoViewSaveVideoClicked;
                _contentView.AddView(_downloadVideoView);
            }
        }

        void HideDownloadVideoView()
        {
            _contentView.RemoveView(_downloadVideoView);
            _downloadVideoView = null;
        }

        async Task<IEnumerable<VideoInfo>> CheckVideoAvailability(string url)
        {
            try
            {
                IEnumerable<VideoInfo> videoInfos = await DownloadUrlResolver.GetDownloadUrlsAsync(url);
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

        async void DownloadVideoViewSaveVideoClicked(object sender, VideoInfo e)
        {
            await ShowRewardAdAndDownload(e);
        }
    }
}
