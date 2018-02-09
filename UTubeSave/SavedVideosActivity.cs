using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Views;
using Android.Widget;
using UTubeSave.Droid.Adapters;
using UTubeSave.Droid.Helpers;
using UTubeSave.Droid.Model;

namespace UTubeSave.Droid
{
    [Activity(Theme = "@android:style/Theme.NoTitleBar.Fullscreen", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class SavedVideosActivity : Activity
    {
        SavedVideosAdapter _videoAdapter;
        View _noVideosView;
        View _activityView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SavedVideos);

            var mAdView = FindViewById<AdView>(Resource.Id.adView);
            Advertistment.Instance.LoadAd(mAdView);

            var savedListView = FindViewById<ListView>(Resource.Id.savedVideos);
            _noVideosView = FindViewById(Resource.Id.noVideoView);
            _activityView = FindViewById(Resource.Id.activityBar);

            var savedVideos = Storage.Instance.GetSavedVideos();
            CheckVideosCount(savedVideos?.Count);

            _videoAdapter = new SavedVideosAdapter(this, savedVideos);

            _videoAdapter.PlayVideo += SavedVideosAdapterPlayVideo;
            _videoAdapter.RemoveVideo += SavedVideosAdapterRemoveVideo;

            Storage.Instance.VideosUpdated += (sender, e) =>
            {
                _videoAdapter.DataSource = e;
                CheckVideosCount(e?.Count);
            };

            savedListView.Adapter = _videoAdapter;
        }

        void CheckVideosCount(int? count)
        {
            _noVideosView.Visibility = count > 0 ? ViewStates.Gone : ViewStates.Visible;
        }

        async void SavedVideosAdapterPlayVideo(object sender, Video e)
        {
            _activityView.Visibility = ViewStates.Visible;
            await Advertistment.Instance.ShowBetweenPagesAd(this);
            _activityView.Visibility = ViewStates.Gone;

            var intent = new Intent(this, typeof(PlayerActivity));
            intent.PutExtra(Constants.VideoPath, e.Path);
            StartActivity(intent);
        }

        void SavedVideosAdapterRemoveVideo(object sender, Video e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle(Resource.String.remove_video_title);
            alert.SetMessage(Resource.String.remove_video_message);
            alert.SetPositiveButton(Resource.String.yes, (senderAlert, args) => {
                RemoveVideo(e);
            });

            alert.SetNegativeButton(Resource.String.no, (senderAlert, args) => {});

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        void RemoveVideo(Video video)
        {
            if (!Storage.Instance.RemoveVideo(video))
            {
                Toast.MakeText(this, Resource.String.cannot_remove_video, ToastLength.Short).Show();
            }
        }
    }
}
