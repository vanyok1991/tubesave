using System;
using HockeyApp;

namespace UTubeSave.Droid.Helpers
{
    public class Tracker
    {
        static string PlayVideoEvent = "PlayVideoEvent";
        static string RewardAdEvent = "RewardAdEvent";
        static string BetweenAdEvent = "BetweenAdEvent";
        static string VideoNotAvailableEvent = "VideoNotAvailableEvent";
        static string VideoDownloadingFailedEvent = "VideoDownloadingFailedEvent";
        static string StartDownloadVideoFailedEvent = "StartDownloadVideoFailedEvent";
        
        public static void TrackPlayVideo()
        {
            MetricsManager.TrackEvent(PlayVideoEvent);
        }

        public static void TrackRewardAdShown()
        {
            MetricsManager.TrackEvent(RewardAdEvent);
        }

        public static void TrackBetweenAdShown()
        {
            MetricsManager.TrackEvent(BetweenAdEvent);
        }

        public static void TrackVideoNotAvailable()
        {
            MetricsManager.TrackEvent(VideoNotAvailableEvent);
        }

        public static void TrackVideoDownloadingFailed()
        {
            MetricsManager.TrackEvent(VideoDownloadingFailedEvent);
        }

        public static void TrackStartDownloadVideoFailed()
        {
            MetricsManager.TrackEvent(StartDownloadVideoFailedEvent);
        }
    }
}
