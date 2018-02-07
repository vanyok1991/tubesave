using System;
using System.Collections.Generic;
using Java.IO;
using Newtonsoft.Json;
using Plugin.SecureStorage;
using UTubeSave.Droid.Model;

namespace UTubeSave.Droid.Helpers
{
    public class Storage
    {
        public static Storage Instance;

        private Storage(){
            var videosExist = CrossSecureStorage.Current.HasKey(_videosKey);

            if (videosExist)
            {
                var videosString = CrossSecureStorage.Current.GetValue(_videosKey);
                _videos = JsonConvert.DeserializeObject<List<Video>>(videosString);
            }
            else
            {
                _videos = new List<Video>();
            }
        }

        static Storage()
        {
            Instance = new Storage();
        }

        const string _videosKey = "SavedVideos";
        List<Video> _videos;

        public void SaveVideo(Video video)
        {
            _videos.Add(video);

            var videosJson = JsonConvert.SerializeObject(_videos);
            CrossSecureStorage.Current.SetValue(_videosKey, videosJson);
        }

        public List<Video> GetSavedVideos()
        {
            return _videos;
        }

        public bool RemoveVideo(Video video)
        {
            if (_videos.Contains(video))
            {
                if (RemoveLocalFile(video.Path))
                {
                    _videos.Remove(video);

                    var videosJson = JsonConvert.SerializeObject(_videos);
                    CrossSecureStorage.Current.SetValue(_videosKey, videosJson);

                    return true;
                }
            }

            return false;
        }

        bool RemoveLocalFile(string path)
        {
            var file = new File(path);
            return file.Delete();
        }
    }
}
