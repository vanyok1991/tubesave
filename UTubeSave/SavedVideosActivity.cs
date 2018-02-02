
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using UTubeSave.Droid.Helpers;
using UTubeSave.Droid.Model;

namespace UTubeSave.Droid
{
    [Activity(Theme = "@android:style/Theme.NoTitleBar")]
    public class SavedVideosActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SavedVideos);

            var savedListView = FindViewById<ListView>(Resource.Id.savedVideos);

            var savedVideos = Storage.Instance.GetSavedVideos();

            var savedVideosAdapter = new ArrayAdapter<Video>(this, Android.Resource.Layout.SimpleListItem1, savedVideos);

            savedListView.Adapter = savedVideosAdapter;
        }
    }
}
