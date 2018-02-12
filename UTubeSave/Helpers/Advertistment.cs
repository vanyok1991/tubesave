using System.Threading.Tasks;
using Android.Content;
using Android.Gms.Ads;
using UTubeSave.Droid.Helpers.Advert;

namespace UTubeSave.Droid.Helpers
{
    public class Advertistment
    {
        const string _appId = "ca-app-pub-6653353220256677~5913709845";

        public static Advertistment Instance;

        private Advertistment(){}

        static Advertistment()
        {
            Instance = new Advertistment();
        }

        public void InitApp(Context context)
        {
            MobileAds.Initialize(context, _appId);
        }

        public async Task<bool> ShowRewardAd(Context context)
        {
            var rewardAd = new RewardAd(context);
            var result = await rewardAd.LoadAd();

            if (result == RewardAdStatus.NotLoaded)
            {
                return await ShowBetweenPagesAd(context);
            }

            Tracker.TrackRewardAdShown();
            return true;
        }

        public Task<bool> ShowBetweenPagesAd(Context context)
        {
            var betweenAd = new BetweenPagesAd(context);
            return betweenAd.LoadAd();
        }

        public void LoadAd(AdView view)
        {
            var adRequest = new AdRequest.Builder().Build();
            view.LoadAd(adRequest);
        }
    }
}
