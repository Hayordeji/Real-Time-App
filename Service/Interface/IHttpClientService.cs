using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IHttpClientService
    {
        void SetAuthorizationHeader(string scheme, string token);
        void AddJsonHeader();
        void AddSeamlessHRHeader();

        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> PostAsync<T>(string url, T data);
        Task<HttpResponseMessage> PutAsync<T>(string url, T data);
        Task<HttpResponseMessage> DeleteAsync(string url);

    }
}
