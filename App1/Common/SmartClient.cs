using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace FanaticAdminApp.Persistency
{
    /// <summary>
    /// Class made by Nikolaj Bang.
	/// https://github.com/Bang0123
    /// This class implements simple princips of designs patterns.
    /// Facade, everything is static, and you just need to call one method to utilize the full potential of the class.
    /// </summary>
    public static class SmartClient
    {
        /// <summary>
        /// Set or get the timeout time in miliseconds
        /// default 10 sec
        /// </summary>
        public static int TimeoutTime { get; set; } = 10000;

        #region private static methods

        /// <summary>
        /// returns a handler that atm doesnt know if other creds are needed.
        /// </summary>
        /// <param name="creds">default true, meaning it will use default credentials</param>
        /// <param name="userName">if creds is false, please specify a networking username</param>
        /// <param name="password">if creds is false, please specify a networking password</param>
        /// <returns></returns>
        private static HttpClientHandler GetHandler(bool creds = true, string userName = null, string password = null)
        {
            if (!creds && userName != null && password != null)
            {
                HttpClientHandler hand = new HttpClientHandler
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(userName, password),
                };
                return hand;
            }
            HttpClientHandler handler = new HttpClientHandler() { UseDefaultCredentials = true };
            return handler;
        }

        /// <summary>
        /// Sets a handlers settings to return json, and sets baseUri.
        /// </summary>
        /// <param name="client">HttpClient that needs settings changed.</param>
        /// <param name="baseUri">baseUri end it with "/".</param>
        /// <param name="setJson">default true, ensures replay to app with json.</param>
        private static void SetClientSettings(HttpClient client, string baseUri, bool setJson = true)
        {
            client.BaseAddress = new Uri(baseUri);
            if (setJson)
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }
        #endregion

        #region public static methods


        /// <summary>
        /// Checks to see if theres internet
        /// </summary>
        /// <returns>true if theres internet false if not</returns>
        public static bool CheckForInternetConnection()
        {
            try
            {
                var network = NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel();
                return network == NetworkConnectivityLevel.InternetAccess;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// Simple Post and its generic.
        /// </summary>
        /// <typeparam name="T">please specify to ensure typesafety.</typeparam>
        /// <param name="baseUri">baseUri end it with "/".</param>
        /// <param name="apiRoute">apiRoute dont end it with "/".</param>
        /// <param name="obj">obj is object to post.</param>
        /// <param name="creds">true by default, false if you want to use other networking credentials.</param>
        /// <param name="setJson">Indicates wether you want application json or not, true by default.</param>
        /// <param name="userNameCred">if creds is false, this need a username.</param>
        /// <param name="passwordCred">if creds is false, this need a password.</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> Post<T>(string baseUri, string apiRoute, T obj, bool setJson = true, bool creds = true, string userNameCred = null, string passwordCred = null)
        {
            using (var client = new HttpClient(GetHandler(creds, userNameCred, passwordCred)))
            {
                try
                {
                    SetClientSettings(client, baseUri, setJson);
                    var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(TimeoutTime)).Token;
                    cts.ThrowIfCancellationRequested();
                    return await client.PostAsJsonAsync(apiRoute, obj, cts).ConfigureAwait(false);
                }
                catch (Exception e)
                {

                    if (e.GetType() == typeof(TaskCanceledException) || e.GetType() == typeof(OperationCanceledException))
                    {
                        return new HttpResponseMessage(HttpStatusCode.RequestTimeout);
                    }
                    return null;
                }
            }
        }

        /// <summary>
        /// Updates a specific element.
        /// </summary>
        /// <typeparam name="T">please specify to ensure typesafety.</typeparam>
        /// <param name="baseUri">baseUri end it with "/".</param>
        /// <param name="apiRoute">apiRoute dont end it with "/".</param>
        /// <param name="objToUpdate">object to update.</param>
        /// <param name="num">num is for the obj id.</param>
        /// <param name="creds">true by default, false if you want to use other networking credentials.</param>
        /// <param name="setJson">Indicates wether you want application json or not, true by default.</param>
        /// <param name="userNameCred">if creds is false, this need a username.</param>
        /// <param name="passwordCred">if creds is false, this need a password.</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> Update<T>(string baseUri, string apiRoute, T objToUpdate, int? num = null, bool setJson = true, bool creds = true, string userNameCred = null, string passwordCred = null)
        {
            using (var client = new HttpClient(GetHandler(creds, userNameCred, passwordCred)))
            {
                try
                {
                    SetClientSettings(client, baseUri, setJson);
                    var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(TimeoutTime)).Token;
                    cts.ThrowIfCancellationRequested();
                    if (num != null && objToUpdate != null)
                    {
                        return await client.PutAsJsonAsync(apiRoute + "/" + num, objToUpdate, cts).ConfigureAwait(false);
                    }
                    return null;
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(TaskCanceledException) || e.GetType() == typeof(OperationCanceledException))
                    {
                        return new HttpResponseMessage(HttpStatusCode.RequestTimeout);
                    }
                    return null;
                }
            }
        }

        /// <summary>
        /// Deletes an element.
        /// </summary>
        /// <param name="baseUri">baseUri end it with "/".</param>
        /// <param name="apiRoute">apiRoute dont end it with "/".</param>
        /// <param name="numToDelete">Id of the element to delete.</param>
        /// <param name="creds">true by default, false if you want to use other networking credentials.</param>
        /// <param name="setJson">Indicates wether you want application json or not, true by default.</param>
        /// <param name="userNameCred">if creds is false, this need a username.</param>
        /// <param name="passwordCred">if creds is false, this need a password.</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> Delete(string baseUri, string apiRoute, int? numToDelete = null, bool setJson = true, bool creds = true, string userNameCred = null, string passwordCred = null)
        {
            using (var client = new HttpClient(GetHandler(creds, userNameCred, passwordCred)))
            {
                try
                {
                    SetClientSettings(client, baseUri, setJson);
                    var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(TimeoutTime)).Token;
                    cts.ThrowIfCancellationRequested();
                    if (numToDelete != null)
                    {
                        return await client.DeleteAsync(apiRoute + "/" + numToDelete, cts).ConfigureAwait(false);
                    }
                    return null;
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(TaskCanceledException) || e.GetType() == typeof(OperationCanceledException))
                    {
                        return new HttpResponseMessage(HttpStatusCode.RequestTimeout);
                    }
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets a list of elements.
        /// </summary>
        /// <typeparam name="T">please specify to ensure typesafety.</typeparam>
        /// <param name="baseUri">baseUri end it with "/".</param>
        /// <param name="apiRoute">apiRoute dont end it with "/".</param>
        /// <param name="creds">true by default, false if you want to use other networking credentials.</param>
        /// <param name="setJson">Indicates wether you want application json or not, true by default.</param>
        /// <param name="userNameCred">if creds is false, this need a username.</param>
        /// <param name="passwordCred">if creds is false, this need a password.</param>
        /// <returns></returns>
        public static async Task<List<T>> Get<T>(string baseUri, string apiRoute, bool setJson = true, bool creds = true, string userNameCred = null, string passwordCred = null)
        {
            using (var client = new HttpClient(GetHandler(creds, userNameCred, passwordCred)))
            {
                try
                {
                    SetClientSettings(client, baseUri, setJson);
                    var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(TimeoutTime)).Token;
                    cts.ThrowIfCancellationRequested();

                    var response = await client.GetAsync(apiRoute, cts).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsAsync<List<T>>(cts).ConfigureAwait(false);
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get a single object by specifyin a ID
        /// </summary>
        /// <typeparam name="T">what type should be returned.</typeparam>
        /// <param name="baseUri">baseUri end it with "/".</param>
        /// <param name="apiRoute">apiRoute dont end it with "/".</param>
        /// <param name="num"></param>
        /// <param name="creds">true by default, false if you want to use other networking credentials.</param>
        /// <param name="setJson">Indicates wether you want application json or not, true by default.</param>
        /// <param name="userNameCred">if creds is false, this need a username.</param>
        /// <param name="passwordCred">if creds is false, this need a password.</param>
        /// <returns></returns>
        public static async Task<object> Get<T>(string baseUri, string apiRoute, int num, bool setJson = true, bool creds = true, string userNameCred = null, string passwordCred = null)
        {
            using (var client = new HttpClient(GetHandler(creds, userNameCred, passwordCred)))
            {
                try
                {
                    SetClientSettings(client, baseUri, setJson);
                    var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(TimeoutTime)).Token;
                    cts.ThrowIfCancellationRequested();

                    var response = await client.GetAsync(apiRoute + "/" + num, cts).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsAsync<T>(cts).ConfigureAwait(false);
                    }

                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #endregion
    }
}