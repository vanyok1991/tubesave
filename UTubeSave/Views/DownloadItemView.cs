using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using UTubeSave.Droid.Extractor;

namespace UTubeSave.Droid.Views
{
    public class DownloadItemView : RelativeLayout
    {
        public event EventHandler<VideoDownloader> DownloadFinished;

        public DownloadItemView(Context context, VideoDownloader downloader) :
            base(context)
        {
            Initialize(downloader);
        }

        public DownloadItemView(Context context, IAttributeSet attrs, VideoDownloader downloader) :
            base(context, attrs)
        {
            Initialize(downloader);
        }

        public DownloadItemView(Context context, IAttributeSet attrs, int defStyle, VideoDownloader downloader) :
            base(context, attrs, defStyle)
        {
            Initialize(downloader);
        }

        void Initialize(VideoDownloader downloader)
        {
            var inflater = (LayoutInflater)this.Context.GetSystemService(Context.LayoutInflaterService);
            var view = inflater.Inflate(Resource.Layout.DownloadItem, this, true);

            var titleView = view.FindViewById<TextView>(Resource.Id.titleTextView);
            titleView.Text = downloader.Video.Title;

            var progressBar = view.FindViewById<ProgressBar>(Resource.Id.downloadProgressBar);

            downloader.DownloadProgressChanged += (sender, e) => 
            {
                progressBar.SetProgress((int)e.ProgressPercentage, true);
            };

            downloader.DownloadFinished += (sender, e) => 
            {
                DownloadFinished?.Invoke(this, downloader);
            };
        }
    }
}
