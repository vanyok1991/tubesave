using System;
using AutoService.Core.Models;
using Newtonsoft.Json;

namespace AutoService.Core.Services
{
    public class DataService
    {
        #region singleton
        public static DataService Instance;

        DataService()
        {
            RemoteDataService.Instance.DataLoaded += RemoteDataServiceDataLoaded;
            RemoteDataService.Instance.DataFailed += RemoteDataServiceDataFailed;
        }

        static DataService()
        {
            Instance = new DataService();
        }
        #endregion

        public event EventHandler<ServiceRoot> DataUpdated;
        public event EventHandler<DataFailedArgs> UpdatingError;

        public void RequestDataUpdate()
        {
            RemoteDataService.Instance.RequestData();
        }

        public ServiceRoot StorageData
        {
            get
            {
                var stringData = StorageService.Instance.ServiceData;
                return JsonConvert.DeserializeObject<ServiceRoot>(stringData);
            }
        }

        void RemoteDataServiceDataLoaded(object sender, string e)
        {
            ServiceRoot rootData = null;
            try
            {
                rootData = JsonConvert.DeserializeObject<ServiceRoot>(e);
            }catch
            {
                DateTime lastDate;
                DateTime.TryParse(StorageService.Instance.UpdatedDate, out lastDate);

                UpdatingError?.Invoke(this, new DataFailedArgs(DataError.DataException, lastDate));
            }

            if (rootData != null)
            {
                StorageService.Instance.ServiceData = e;
                StorageService.Instance.UpdatedDate = DateTime.Now.ToString();

                DataUpdated?.Invoke(this, rootData);
            }
        }

        void RemoteDataServiceDataFailed(object sender, DataError e)
        {
            DateTime lastDate;
            DateTime.TryParse(StorageService.Instance.UpdatedDate, out lastDate);

            UpdatingError?.Invoke(this, new DataFailedArgs(e, lastDate));
        }
    }
}
