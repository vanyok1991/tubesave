using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;

namespace UTubeSave.Droid
{
    [Activity(Theme = "@android:style/Theme.NoTitleBar.Fullscreen", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class PlayerActivity : Activity
    {
        VideoView _videoView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Player);

            _videoView = FindViewById<VideoView>(Resource.Id.videoView);
        }

        protected override void OnStart()
        {
            base.OnStart();

            var path = Intent.GetStringExtra(Constants.VideoPath);
            var uri = Android.Net.Uri.Parse(path);

            _videoView.SetVideoURI(uri);
            _videoView.SetMediaController(new MediaController(this));
            _videoView.Start();
        }

        protected override void OnPause()
        {
            base.OnPause();

            _videoView.Pause();
        }
    }
}
