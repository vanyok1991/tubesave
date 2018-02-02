using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Gms.Ads;

namespace UTubeSave.Droid.Helpers.Advert
{
    public class BetweenPagesAd
    {
        const string _showId = "ca-app-pub-6653353220256677/4139972485";
        const int _loadPosibilityCount = 3;

        int _currentLoadsCount;
        InterstitialAd _mInterstitialAd;

        readonly TaskCompletionSource<bool> _completionSource;

        public BetweenPagesAd(Context context)
        {
            _mInterstitialAd = new InterstitialAd(context);
            _mInterstitialAd.AdUnitId = _showId;
            _mInterstitialAd.RewardedVideoAdClosed += InterstitialAdRewardedVideoAdClosed;
            _mInterstitialAd.RewardedVideoAdLoaded += InterstitialAdRewardedVideoAdLoaded;
            _mInterstitialAd.RewardedVideoAdFailedToLoad += InterstitialAdRewardedVideoAdFailedToLoad;

            _completionSource = new TaskCompletionSource<bool>();
        }

        public Task<bool> LoadAd()
        {
            _mInterstitialAd.LoadAd(new AdRequest.Builder().Build());
            return _completionSource.Task;
        }

        void InterstitialAdRewardedVideoAdClosed(object sender, EventArgs e)
        {
            _completionSource.TrySetResult(true);
        }

        void InterstitialAdRewardedVideoAdLoaded(object sender, EventArgs e)
        {
            _mInterstitialAd.Show();
        }

        void InterstitialAdRewardedVideoAdFailedToLoad(object sender, Android.Gms.Ads.Reward.RewardedVideoAdFailedToLoadEventArgs e)
        {
            if (_currentLoadsCount < _loadPosibilityCount)
            {
                _currentLoadsCount++;
                _mInterstitialAd.LoadAd(new AdRequest.Builder().Build());
            }
            else
            {
                _completionSource.TrySetResult(false);
            }
        }
    }
}
