using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace ContosoUniversity.Tests
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// This method provides a way to post form data to an MVC controller
        /// </summary>
        /// <typeparam name="T">The view model type that will be posted.</typeparam>
        /// <param name="client">The HttpClient that will be posting the data.</param>
        /// <param name="url">The url of the controller</param>
        /// <param name="viewModel">The view model data that will be passed in the request.</param>
        /// <param name="antiForgeryToken"></param>
        /// <returns></returns>
        public async static Task<HttpResponseMessage> PostFormDataAsync<T>(this HttpClient client, string url, T viewModel, KeyValuePair<string, string>? antiForgeryToken = null)
        {
            var list = viewModel.GetType()
                .GetProperties()
                .Select(t => new KeyValuePair<string, string>(t.Name, (t.GetValue(viewModel) ?? new object()).ToString()))
                .ToList();
            if (antiForgeryToken.HasValue)
                list.Add(antiForgeryToken.Value);
            var formData = new FormUrlEncodedContent(list);
            return await client.PostAsync(url, formData);
        }
    }
}
