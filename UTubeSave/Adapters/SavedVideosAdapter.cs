using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using UTubeSave.Droid.Model;
using UTubeSave.Droid.Views;

namespace UTubeSave.Droid.Adapters
{
    public class SavedVideosAdapter : BaseAdapter<Video>
    {
        public event EventHandler<Video> PlayVideo;
        public event EventHandler<Video> RemoveVideo;

        List<Video> _dataSource;
        Context _context;

        public SavedVideosAdapter(Context context, List<Video> source)
        {
            _context = context;
            _dataSource = source;
        }

        public List<Video> DataSource
        {
            get => _dataSource;
            set
            {
                if (value != null)
                {
                    _dataSource = value;
                    NotifyDataSetChanged();
                }
            }
        }

        public override Video this[int position] => _dataSource[position];

        public override int Count => _dataSource.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var video = _dataSource[position];
            var videoItem = new VideoItemView(_context, video);

            videoItem.PlayVideo += (sender, e) => 
            {
                PlayVideo?.Invoke(this, e);
            };

            videoItem.RemoveVideo += (sender, e) => 
            {
                RemoveVideo?.Invoke(this, e);
            };

            return videoItem;
        }
    }
}
