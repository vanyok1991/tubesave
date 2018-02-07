using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using UTubeSave.Droid.Model;

namespace UTubeSave.Droid.Views
{
    public class VideoItemView : LinearLayout
    {
        public event EventHandler<Video> PlayVideo;
        public event EventHandler<Video> RemoveVideo;

        public VideoItemView(Context context, Video video) :
            base(context)
        {
            Initialize(video);
        }

        public VideoItemView(Context context, IAttributeSet attrs, Video video) :
            base(context, attrs)
        {
            Initialize(video);
        }

        public VideoItemView(Context context, IAttributeSet attrs, int defStyle, Video video) :
            base(context, attrs, defStyle)
        {
            Initialize(video);
        }

        void Initialize(Video video)
        {
            var inflater = (LayoutInflater)this.Context.GetSystemService(Context.LayoutInflaterService);
            var view = inflater.Inflate(Resource.Layout.VideoItem, this, true);

            var title = view.FindViewById<TextView>(Resource.Id.title);
            title.Text = video.Title;

            var playButton = view.FindViewById<ImageButton>(Resource.Id.playButton);
            playButton.Click += (sender, e) => 
            {
                PlayVideo?.Invoke(this, video);
            };

            var removeButton = view.FindViewById<ImageButton>(Resource.Id.removeButton);
            removeButton.Click += (sender, e) => 
            {
                RemoveVideo?.Invoke(this, video);
            };
        }
    }
}
