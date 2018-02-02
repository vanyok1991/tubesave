using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Gms.Ads;
using Android.Gms.Ads.Reward;

namespace UTubeSave.Droid.Helpers.Advert
{
    public class RewardAd : Java.Lang.Object, IRewardedVideoAdListener
    {
        const string _rewardAdId = "ca-app-pub-6653353220256677/7893516677";
        const int _loadPosibilityCount = 3;

        int _currentLoadsCount;
        bool _rewarded;
        IRewardedVideoAd _rewardVideoAd;

        readonly TaskCompletionSource<RewardAdStatus> _completionSource;

        public RewardAd(Context context)
        {
            _rewardVideoAd = MobileAds.GetRewardedVideoAdInstance(context);
            _rewardVideoAd.RewardedVideoAdListener = this;

            _currentLoadsCount = 0;
            _completionSource = new TaskCompletionSource<RewardAdStatus>();
        }

        public Task<RewardAdStatus> LoadAd()
        {
            _rewardVideoAd.LoadAd(_rewardAdId, new AdRequest.Builder().Build());
            return _completionSource.Task;
        }

        public void OnRewarded(IRewardItem reward)
        {
            Console.WriteLine($"OnRewardedVideoAdOpened {reward.Amount}");
            _rewarded = true;
        }

        public void OnRewardedVideoAdClosed()
        {
            Console.WriteLine("OnRewardedVideoAdClosed");
            if (_rewarded)
            {
                _completionSource.TrySetResult(RewardAdStatus.Rewarded);
            }
            else
            {
                _completionSource.TrySetResult(RewardAdStatus.Canceled);
            }
        }

        public void OnRewardedVideoAdFailedToLoad(int errorCode)
        {
            Console.WriteLine($"OnRewardedVideoAdFailedToLoad ErrorCode: {errorCode}");

            if(_currentLoadsCount < _loadPosibilityCount)
            {
                _currentLoadsCount++;
                _rewardVideoAd.LoadAd(_rewardAdId, new AdRequest.Builder().Build());
            }else
            {
                _completionSource.TrySetResult(RewardAdStatus.NotLoaded);
            }
        }

        public void OnRewardedVideoAdLeftApplication()
        {
            Console.WriteLine("OnRewardedVideoAdLeftApplication");
        }

        public void OnRewardedVideoAdLoaded()
        {
            Console.WriteLine("OnRewardedVideoAdLoaded");
            _rewardVideoAd.Show();
        }

        public void OnRewardedVideoAdOpened()
        {
            Console.WriteLine("OnRewardedVideoAdOpened");
        }

        public void OnRewardedVideoStarted()
        {
            Console.WriteLine("OnRewardedVideoStarted");
        }
    }
}
