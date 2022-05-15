using Microsoft.AspNetCore.WebUtilities;
using TSDC.ApiHelper.Enums;

namespace TSDC.ApiHelper
{
    public static class ApiHelper<T>
    {
        public static async Task<BaseResult<T>?> ExecuteAsync(string apiUrl, Dictionary<string, string>? parameters, object? obj, Method method, string baseUrl, string? accessToken = null)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(accessToken))
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer" + accessToken);
                }

                if (!string.IsNullOrEmpty(apiUrl))
                {
                    if (apiUrl.StartsWith("/") || apiUrl.StartsWith("\\"))
                    {
                        apiUrl = apiUrl.Remove(0);
                    }

                    if (apiUrl.EndsWith("/") || apiUrl.EndsWith("\\"))
                    {
                        apiUrl = apiUrl.Remove(apiUrl.Length - 1);
                    }
                }

                switch (method)
                {                    
                    case Method.GET:
                        return await GetExecuteAsync(client, apiUrl, parameters, obj);
                    case Method.POST:
                        return await PostExecuteAsync(client, apiUrl, parameters, obj);
                    case Method.PUT:
                        return await PutExecuteAsync(client, apiUrl, parameters, obj);
                    case Method.DELETE:
                        return await DeleteExecuteAsync(client, apiUrl, parameters, obj);
                    default:
                        break;
                }
            }            

            return default;
        }

        #region GET
        private static async Task<BaseResult<T>?> GetExecuteAsync(HttpClient client, string apiUrl, Dictionary<string, string>? parameters, object? obj)
        {
            HttpResponseMessage response = parameters != null ? await client.GetAsync(QueryHelpers.AddQueryString(apiUrl, parameters)) : await client.GetAsync(apiUrl);            

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<BaseResult<T>>();
                return result;
            }
            else
            {
                return new BaseResult<T>
                {
                    Status = false,
                    StatusCode = (int)response.StatusCode,
                    Message = response.Headers.ToString()
                };
            }
        }
        #endregion

        #region POST
        private static async Task<BaseResult<T>?> PostExecuteAsync(HttpClient client, string apiUrl, Dictionary<string, string>? parameters, object? obj)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, obj);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<BaseResult<T>>();
                return result;
            }
            else
            {
                return new BaseResult<T>
                {
                    Status = false,
                    StatusCode = (int)response.StatusCode,
                    Message = response.Headers.ToString()
                };
            }
        }
        #endregion

        #region PUT
        private static async Task<BaseResult<T>?> PutExecuteAsync(HttpClient client, string apiUrl, Dictionary<string, string>? parameters, object? obj)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(apiUrl, obj);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<BaseResult<T>>();
                return result;
            }
            else
            {
                return new BaseResult<T>
                {
                    Status = false,
                    StatusCode = (int)response.StatusCode,
                    Message = response.Headers.ToString()
                };
            }
        }
        #endregion

        #region DELETE
        private static async Task<BaseResult<T>?> DeleteExecuteAsync(HttpClient client, string apiUrl, Dictionary<string, string>? parameters, object? obj)
        {
            HttpResponseMessage response = parameters != null ? await client.DeleteAsync(QueryHelpers.AddQueryString(apiUrl, parameters)) : await client.DeleteAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<BaseResult<T>>();
                return result;
            }
            else
            {
                return new BaseResult<T>
                {
                    Status = false,
                    StatusCode = (int)response.StatusCode,
                    Message = response.Headers.ToString()
                };
            }
        }
        #endregion
    }
}
