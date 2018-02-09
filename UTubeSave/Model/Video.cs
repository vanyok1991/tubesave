using System;
namespace UTubeSave.Droid.Model
{
    public class Video
    {
        public string Title { get; private set; }

        public string Path { get; private set; }

        public string Id { get; private set; }

        public VideoStatus Status { get; private set; }

        public Video(string title, string path)
        {
            Title = title;
            Path = path;
            Status = VideoStatus.Open;

            Id = Guid.NewGuid().ToString();
        }

        public override string ToString()
        {
            return Title;
        }

        public bool TrySetInProgressStatus()
        {
            if(Status == VideoStatus.Open)
            {
                Status = VideoStatus.InProgress;

                return true;
            }

            return false;
        }

        public bool TrySetFinishedStatus()
        {
            if (Status == VideoStatus.InProgress)
            {
                Status = VideoStatus.Finished;
                return true;
            }

            return false;
        }
    }
}
