using System;
namespace UTubeSave
{
    public class AudioExtractionException : Exception
    {
        public AudioExtractionException(string message)
            : base(message)
        { }
    }
}
