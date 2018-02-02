using System;
namespace UTubeSave.Droid.Model
{
    public class Video
    {
        public string Title { get; private set; }

        public string Path { get; private set; }

        public string Id { get; private set; }

        public Video(string title, string path)
        {
            Title = title;
            Path = path;

            Id = Guid.NewGuid().ToString();
        }
    }
}
