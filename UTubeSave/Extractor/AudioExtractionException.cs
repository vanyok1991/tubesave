using System;
namespace UTubeSave.Droid.Extractor
{
    public class AudioExtractionException : Exception
    {
        public AudioExtractionException(string message)
            : base(message)
        { }
    }
}
