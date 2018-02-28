using System;
namespace AutoService.Core.Services
{
    public class DataFailedArgs : EventArgs
    {
        public DataError Error { get; private set; }

        public DateTime LastUpdated { get; private set; }

        public DataFailedArgs(DataError error, DateTime lastUpdated)
        {
            Error = error;
            LastUpdated = lastUpdated;
        }
    }
}
