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

        private Storage(){}

        static Storage()
        {
            Instance = new Storage();
        }

        const string _videosKey = "SavedVideos";

        public void SaveVideo(Video video)
        {
            List<Video> videos;
            var videosExist = CrossSecureStorage.Current.HasKey(_videosKey);

            if(videosExist)
            {
                var videosString = CrossSecureStorage.Current.GetValue(_videosKey);
                videos = JsonConvert.DeserializeObject<List<Video>>(videosString);
            }else
            {
                videos = new List<Video>();
            }

            videos.Add(video);

            var videosJson = JsonConvert.SerializeObject(videos);
            CrossSecureStorage.Current.SetValue(_videosKey, videosJson);
        }

        public List<Video> GetSavedVideos()
        {
            var videosExist = CrossSecureStorage.Current.HasKey(_videosKey);

            if (videosExist)
            {
                var videosString = CrossSecureStorage.Current.GetValue(_videosKey);
                return JsonConvert.DeserializeObject<List<Video>>(videosString);
            }
            else
            {
                return new List<Video>();
            }
        }

        public void RemoveVideo(Video video)
        {
            List<Video> videos;
            var videosExist = CrossSecureStorage.Current.HasKey(_videosKey);

            if (videosExist)
            {
                var videosString = CrossSecureStorage.Current.GetValue(_videosKey);
                videos = JsonConvert.DeserializeObject<List<Video>>(videosString);

                if(videos.Contains(video))
                {
                    if (RemoveLocalFile(video.Path))
                    {
                        videos.Remove(video);

                        var videosJson = JsonConvert.SerializeObject(videos);
                        CrossSecureStorage.Current.SetValue(_videosKey, videosJson);
                    }else
                    {
                        throw new Exception("Can not remove local file");
                    }
                }
            }
        }

        bool RemoveLocalFile(string path)
        {
            var file = new File(path);
            return file.Delete();
        }
    }
}
