using System;
namespace UTubeSave
{
    internal interface IAudioExtractor : IDisposable
    {
        string VideoPath { get; }

        /// <exception cref="AudioExtractionException">An error occured while writing the chunk.</exception>
        void WriteChunk(byte[] chunk, uint timeStamp);
    }
}
