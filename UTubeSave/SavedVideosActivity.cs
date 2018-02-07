
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using UTubeSave.Droid.Adapters;
using UTubeSave.Droid.Helpers;
using UTubeSave.Droid.Model;

namespace UTubeSave.Droid
{
    [Activity(Theme = "@android:style/Theme.NoTitleBar")]
    public class SavedVideosActivity : Activity
    {
        SavedVideosAdapter _videoAdapter;
        VideoView _videoView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SavedVideos);

            var mAdView = FindViewById<AdView>(Resource.Id.adView);
            Advertistment.Instance.LoadAd(mAdView);

            var savedListView = FindViewById<ListView>(Resource.Id.savedVideos);

            var savedVideos = Storage.Instance.GetSavedVideos();

            _videoAdapter = new SavedVideosAdapter(this, savedVideos);

            _videoAdapter.PlayVideo += SavedVideosAdapterPlayVideo;
            _videoAdapter.RemoveVideo += SavedVideosAdapterRemoveVideo;

            savedListView.Adapter = _videoAdapter;

            _videoView = FindViewById<VideoView>(Resource.Id.videoView);
        }

        async void SavedVideosAdapterPlayVideo(object sender, Video e)
        {
            await Advertistment.Instance.ShowBetweenPagesAd(this);
            PlayVideo(e);
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
            if (Storage.Instance.RemoveVideo(video))
            {
                _videoAdapter.DataSource = Storage.Instance.GetSavedVideos();
            }
            else
            {
                Toast.MakeText(this, Resource.String.cannot_remove_video, ToastLength.Short).Show();
            }
        }

        void PlayVideo(Video video)
        {
            var uri = Android.Net.Uri.Parse(video.Path);

            _videoView.SetVideoURI(uri);

            _videoView.SetMediaController(new MediaController(this));

            _videoView.Visibility = ViewStates.Visible;
            _videoView.Start();
        }
    }
}
