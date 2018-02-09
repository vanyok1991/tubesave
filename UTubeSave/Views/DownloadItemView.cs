using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using UTubeSave.Droid.Extractor;
using UTubeSave.Droid.Model;

namespace UTubeSave.Droid.Views
{
    public class DownloadItemView : RelativeLayout
    {
        public event EventHandler<Video> DownloadFinished;
        public event EventHandler<Video> ProgressChanged;

        public DownloadItemView(Context context, VideoDownloader downloader, Video video) :
            base(context)
        {
            Initialize(downloader, video);
        }

        public DownloadItemView(Context context, IAttributeSet attrs, VideoDownloader downloader, Video video) :
            base(context, attrs)
        {
            Initialize(downloader, video);
        }

        public DownloadItemView(Context context, IAttributeSet attrs, int defStyle, VideoDownloader downloader, Video video) :
            base(context, attrs, defStyle)
        {
            Initialize(downloader, video);
        }

        void Initialize(VideoDownloader downloader, Video video)
        {
            var inflater = (LayoutInflater)this.Context.GetSystemService(Context.LayoutInflaterService);
            var view = inflater.Inflate(Resource.Layout.DownloadItem, this, true);

            var titleView = view.FindViewById<TextView>(Resource.Id.titleTextView);
            titleView.Text = downloader.Video.Title;

            var progressBar = view.FindViewById<ProgressBar>(Resource.Id.downloadProgressBar);

            downloader.DownloadProgressChanged += (sender, e) => 
            {
                progressBar.SetProgress((int)e.ProgressPercentage, true);
                ProgressChanged?.Invoke(this, video);
            };

            downloader.DownloadFinished += (sender, e) => 
            {
                DownloadFinished?.Invoke(this, video);
            };
        }
    }
}
