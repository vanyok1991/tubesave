using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AutoService.Core.Services
{
    public class RemoteDataService
    {
        #region singleton
        public static RemoteDataService Instance;

        RemoteDataService()
        {

        }

        static RemoteDataService()
        {
            Instance = new RemoteDataService();
        }
        #endregion

        public event EventHandler<string> DataLoaded;
        public event EventHandler<DataError> DataFailed;

        const string _remoteVersionUrl = "";//TODO add url
        const string _remoteDataUrl = "";//TODO add url

        public Task RequestVersion()
        {
            return GetDataAsync(_remoteVersionUrl);
        }

        public Task RequestData()
        {
            return GetDataAsync(_remoteDataUrl);
        }

        async Task GetDataAsync(string dataUrl)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = new TimeSpan(0, 0, 30);

                    using (var response = await httpClient.GetAsync(dataUrl))
                    {
                        string content = string.Empty;
                        if (response != null && response.Content != null)
                        {
                            content = await response.Content.ReadAsStringAsync();
                        }

                        if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                        {
                            DataLoaded?.Invoke(this, content);
                        }

                        DataFailed?.Invoke(this, DataError.DataException);
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine(ex.Message);
                DataFailed?.Invoke(this, DataError.Timeout);
            }
            catch (HttpRequestException ex)
            {
                var webException = ex.InnerException as WebException;

                if (webException?.Status == WebExceptionStatus.ConnectFailure)
                {
                    DataFailed?.Invoke(this, DataError.ConnectFailure);
                }

                throw ex;
            }
            catch
            {
                DataFailed?.Invoke(this, DataError.DataException);
            }
        }
    }
}
