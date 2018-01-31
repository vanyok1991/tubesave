using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using YoutubeExtractor;

namespace UTubeSave.Droid.Views
{
    public class DownloadVideoView : RelativeLayout
    {
        IEnumerable<VideoInfo> _videos;
        Spinner _resolutionSpinner;
        Spinner _videoTypeSpinner;
        Spinner _audioBitradeSpinner;
        Spinner _audioTypeSpinner;
        Button _saveAudioButton;
        Button _saveVideoButton;

        List<int> _resolutions;
        List<VideoType> _videoTypes;
        List<int> _audioBitrades;
        List<AudioType> _audioTypes;

        public event EventHandler<VideoInfo> SaveAudioClicked;
        public event EventHandler<VideoInfo> SaveVideoClicked;

        public DownloadVideoView(Context context, IEnumerable<VideoInfo> videos) :
            base(context)
        {
            Initialize(videos);
        }

        public DownloadVideoView(Context context, IAttributeSet attrs, IEnumerable<VideoInfo> videos) :
            base(context, attrs)
        {
            Initialize(videos);
        }

        public DownloadVideoView(Context context, IAttributeSet attrs, int defStyle, IEnumerable<VideoInfo> videos) :
            base(context, attrs, defStyle)
        {
            Initialize(videos);
        }

        void Initialize(IEnumerable<VideoInfo> videos)
        {
            var inflater = (LayoutInflater)this.Context.GetSystemService(Context.LayoutInflaterService);
            var view = inflater.Inflate(Resource.Layout.DownloadVideoView, this, true);

            _videos = videos;
            _resolutionSpinner = view.FindViewById<Spinner>(Resource.Id.resolutionSpinner);
            _videoTypeSpinner = view.FindViewById<Spinner>(Resource.Id.videoTypeSpinner);
            _audioBitradeSpinner = view.FindViewById<Spinner>(Resource.Id.audioBitradeSpinner);
            _audioTypeSpinner = view.FindViewById<Spinner>(Resource.Id.audioTypeSpinner);
            _saveAudioButton = view.FindViewById<Button>(Resource.Id.saveAudioButton);
            _saveVideoButton = view.FindViewById<Button>(Resource.Id.saveVideoButton);

            _resolutions = _videos.Where(v => v.Resolution != 0).Select(v => v.Resolution).Distinct().ToList();
            var resolutionAdapter = new ArrayAdapter<int>(this.Context, Android.Resource.Layout.SimpleSpinnerItem, _resolutions);
            resolutionAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            _resolutionSpinner.ItemSelected += ResolutionSpinnerItemSelected;
            _resolutionSpinner.Adapter = resolutionAdapter;

            _saveAudioButton.Click += SaveAudioButtonClick;
            _saveVideoButton.Click += SaveVideoButtonClick;
        }

        void ResolutionSpinnerItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            _videoTypes = _videos.Where(v => v.Resolution == _resolutions[e.Position]).Select(v => v.VideoType).Distinct().ToList();
            var videoTypeAdapter = new ArrayAdapter<VideoType>(this.Context, Android.Resource.Layout.SimpleSpinnerItem, _videoTypes);
            videoTypeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            _videoTypeSpinner.ItemSelected += VideoTypeSpinnerItemSelected;
            _videoTypeSpinner.Adapter = videoTypeAdapter;
        }

        void VideoTypeSpinnerItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            _audioBitrades = _videos.Where(v => v.Resolution == _resolutions[_resolutionSpinner.SelectedItemPosition] && v.VideoType == _videoTypes[e.Position]).Select(v => v.AudioBitrate).Distinct().ToList();
            var audioBitradeAdapter = new ArrayAdapter<int>(this.Context, Android.Resource.Layout.SimpleSpinnerItem, _audioBitrades);
            audioBitradeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            _audioBitradeSpinner.ItemSelected += AudioBitradeSpinnerItemSelected;
            _audioBitradeSpinner.Adapter = audioBitradeAdapter;
        }

        void AudioBitradeSpinnerItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            _audioTypes = _videos.Where(v => v.Resolution == _resolutions[_resolutionSpinner.SelectedItemPosition] &&
                                        v.VideoType == _videoTypes[_videoTypeSpinner.SelectedItemPosition] && 
                                        v.AudioBitrate == _audioBitrades[e.Position])
                                        .Select(v => v.AudioType).Distinct().ToList();
            var audioTypeAdapter = new ArrayAdapter<AudioType>(this.Context, Android.Resource.Layout.SimpleSpinnerItem, _audioTypes);
            audioTypeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            _audioTypeSpinner.ItemSelected += AudioTypeSpinnerItemSelected;
            _audioTypeSpinner.Adapter = audioTypeAdapter;
        }

        void AudioTypeSpinnerItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var audioPosibilityCount = _videos.Count(v => v.CanExtractAudio);
            _saveAudioButton.Enabled = audioPosibilityCount > 0;
        }

        void SaveAudioButtonClick(object sender, EventArgs e)
        {
            var audioInfo = _videos.FirstOrDefault(v => v.CanExtractAudio);
            if (audioInfo != null)
            {
                SaveAudioClicked?.Invoke(this, audioInfo);
            }else
            {
                Toast.MakeText(this.Context, Context.GetString(Resource.String.cannot_download_audio), ToastLength.Short).Show();
            }
        }

        void SaveVideoButtonClick(object sender, EventArgs e)
        {
            var videoInfo = _videos.FirstOrDefault(v => v.Resolution == _resolutions[_resolutionSpinner.SelectedItemPosition] &&
                                                   v.VideoType == _videoTypes[_videoTypeSpinner.SelectedItemPosition] &&
                                                   v.AudioBitrate == _audioBitrades[_audioBitradeSpinner.SelectedItemPosition] &&
                                                   v.AudioType == _audioTypes[_audioBitradeSpinner.SelectedItemPosition]);

            if(videoInfo != null)
            {
                SaveVideoClicked?.Invoke(this, videoInfo);
            }else
            {
                Toast.MakeText(this.Context, Context.GetString(Resource.String.cannot_download_audio), ToastLength.Short).Show();
            }
        }
    }
}
