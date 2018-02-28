using Plugin.SecureStorage;

namespace AutoService.Core.Services
{
    public class StorageService
    {
        #region singleton
        public static StorageService Instance;

        StorageService()
        {

        }

        static StorageService()
        {
            Instance = new StorageService();
        }
        #endregion

        const string _serviceDataKey = "_serviceDataKey";

        public string ServiceData 
        {
            get
            {
                return CrossSecureStorage.Current.GetValue(_serviceDataKey);
            }
            set
            {
                CrossSecureStorage.Current.SetValue(_serviceDataKey, value);
            }
        }

        const string _dataVersionKey = "_dataVersionKey";

        public string DataVersion
        {
            get
            {
                return CrossSecureStorage.Current.GetValue(_dataVersionKey);
            }
            set
            {
                CrossSecureStorage.Current.SetValue(_dataVersionKey, value);
            }
        }

        const string _updatedDateKey = "_updatedDateKey";

        public string UpdatedDate
        {
            get
            {
                return CrossSecureStorage.Current.GetValue(_updatedDateKey);
            }
            set
            {
                CrossSecureStorage.Current.SetValue(_updatedDateKey, value);
            }
        }
    }
}
