using System;
namespace UTubeSave.Droid.Extractor
{
    public class VideoNotAvailableException : Exception
    {
        public VideoNotAvailableException()
        { }

        public VideoNotAvailableException(string message)
            : base(message)
        { }
    }
}
