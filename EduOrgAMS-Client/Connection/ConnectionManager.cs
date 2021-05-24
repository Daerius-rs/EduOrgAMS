using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RIS;
using RIS.Randomizing;

namespace EduOrgAMS.Client.Connection
{
    public static class ConnectionManager
    {
        public static event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        private static readonly Random RandomGenerator;
        private static readonly SocketsHttpHandler HttpHandler;

        public static HttpClient HttpClient { get; }
        private static int _connectionState;
        public static ConnectionStateType ConnectionState
        {
            get
            {
                return (ConnectionStateType)_connectionState;
            }
            private set
            {
                Interlocked.Exchange(ref _connectionState, (int)value);
            }
        }

        static ConnectionManager()
        {
            RandomGenerator = Rand.CreateRandom();

            HttpHandler = new SocketsHttpHandler
            {
                MaxConnectionsPerServer = int.MaxValue
            };
            HttpClient = new HttpClient(HttpHandler)
            {
                Timeout = TimeSpan.FromMinutes(5)
            };

            ConnectionState = ConnectionStateType.Connected;
        }

        public static void OnConnectionStateChanged(ConnectionStateType newState)
        {
            OnConnectionStateChanged(null, newState);
        }
        public static void OnConnectionStateChanged(object sender, ConnectionStateType newState)
        {
            var oldState = Interlocked.Exchange(ref _connectionState, (int)newState);

            if (oldState == (int)newState)
                return;

            ConnectionStateChanged?.Invoke(sender,
                new ConnectionStateChangedEventArgs((ConnectionStateType)oldState, newState));
        }

        private static void OnConnectionStateChanged(ConnectionStateChangedEventArgs e)
        {
            OnConnectionStateChanged(null, e);
        }
        private static void OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            ConnectionStateChanged?.Invoke(sender, e);
        }



        public static Uri CreateUri(string uri,
            UriKind kind = UriKind.RelativeOrAbsolute)
        {
            return !string.IsNullOrEmpty(uri)
                ? new Uri(uri, kind)
                : null;
        }

        public static HttpRequestMessage CreateRequestMessage(Uri uri, HttpMethod method)
        {
            return new HttpRequestMessage(method, uri)
            {
                Version =
#if uap
                    HttpVersion.Version20
#else
                    HttpVersion.Version11
#endif
            };
        }



        public static Task<HttpResponseMessage> GetAsync(
            string requestUri,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            return GetAsync(CreateUri(requestUri),
                CancellationToken.None, completionOption);
        }
        public static Task<HttpResponseMessage> GetAsync(
            Uri requestUri,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            return GetAsync(requestUri,
                CancellationToken.None, completionOption);
        }
        public static Task<HttpResponseMessage> GetAsync(
            string requestUri, CancellationToken cancellationToken,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            return GetAsync(CreateUri(requestUri),
                cancellationToken, completionOption);
        }
        public static Task<HttpResponseMessage> GetAsync(
            Uri requestUri, CancellationToken cancellationToken,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            HttpRequestMessage request = CreateRequestMessage(
                requestUri, HttpMethod.Get);
            return SendAsync(request,
                cancellationToken, completionOption);
        }

        public static Task<HttpResponseMessage> PostAsync(
            string requestUri, HttpContent content)
        {
            return PostAsync(CreateUri(requestUri),
                content, CancellationToken.None);
        }
        public static Task<HttpResponseMessage> PostAsync(
            Uri requestUri, HttpContent content)
        {
            return PostAsync(requestUri,
                content, CancellationToken.None);
        }
        public static Task<HttpResponseMessage> PostAsync(
            string requestUri, HttpContent content,
            CancellationToken cancellationToken)
        {
            return PostAsync(CreateUri(requestUri),
                content, cancellationToken);
        }
        public static Task<HttpResponseMessage> PostAsync(
            Uri requestUri, HttpContent content,
            CancellationToken cancellationToken)
        {
            HttpRequestMessage request = CreateRequestMessage(
                requestUri, HttpMethod.Post);
            request.Content = content;
            return SendAsync(request,
                cancellationToken);
        }

        public static Task<HttpResponseMessage> PutAsync(
            string requestUri, HttpContent content)
        {
            return PutAsync(CreateUri(requestUri),
                content, CancellationToken.None);
        }
        public static Task<HttpResponseMessage> PutAsync(
            Uri requestUri, HttpContent content)
        {
            return PutAsync(requestUri,
                content, CancellationToken.None);
        }
        public static Task<HttpResponseMessage> PutAsync(
            string requestUri, HttpContent content,
            CancellationToken cancellationToken)
        {
            return PutAsync(CreateUri(requestUri),
                content, cancellationToken);
        }
        public static Task<HttpResponseMessage> PutAsync(
            Uri requestUri, HttpContent content,
            CancellationToken cancellationToken)
        {
            HttpRequestMessage request = CreateRequestMessage(
                requestUri, HttpMethod.Put);
            request.Content = content;
            return SendAsync(request,
                cancellationToken);
        }

        public static Task<HttpResponseMessage> PatchAsync(
            string requestUri, HttpContent content)
        {
            return PatchAsync(CreateUri(requestUri),
                content, CancellationToken.None);
        }
        public static Task<HttpResponseMessage> PatchAsync(
            Uri requestUri, HttpContent content)
        {
            return PatchAsync(requestUri,
                content, CancellationToken.None);
        }
        public static Task<HttpResponseMessage> PatchAsync(
            string requestUri, HttpContent content,
            CancellationToken cancellationToken)
        {
            return PatchAsync(CreateUri(requestUri),
                content, cancellationToken);
        }
        public static Task<HttpResponseMessage> PatchAsync(
            Uri requestUri, HttpContent content,
            CancellationToken cancellationToken)
        {
            HttpRequestMessage request = CreateRequestMessage(
                requestUri, HttpMethod.Patch);
            request.Content = content;
            return SendAsync(request,
                cancellationToken);
        }

        public static Task<HttpResponseMessage> DeleteAsync(
            string requestUri)
        {
            return DeleteAsync(CreateUri(requestUri),
                CancellationToken.None);
        }
        public static Task<HttpResponseMessage> DeleteAsync(
            Uri requestUri)
        {
            return DeleteAsync(requestUri,
                CancellationToken.None);
        }
        public static Task<HttpResponseMessage> DeleteAsync(
            string requestUri, CancellationToken cancellationToken)
        {
            return DeleteAsync(CreateUri(requestUri),
                cancellationToken);
        }
        public static Task<HttpResponseMessage> DeleteAsync(
            Uri requestUri, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = CreateRequestMessage(
                requestUri, HttpMethod.Delete);
            return SendAsync(request,
                cancellationToken);
        }



        public static Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            return SendAsync(request,
                CancellationToken.None, completionOption);
        }
        public static async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            HttpResponseMessage response = null;
            int responseCode = -1;

            while (true)
            {
                try
                {
                    try
                    {
                        response = await HttpClient.SendAsync(
                                request, completionOption, cancellationToken)
                            .ConfigureAwait(false);
                        responseCode = (int)response.StatusCode;
                    }
                    catch (Exception)
                    {
                        responseCode = -1;
                        throw;
                    }

                    if (responseCode == 429)
                        throw new HttpRequestException();

                    OnConnectionStateChanged(ConnectionStateType.Connected);
                }
                catch (HttpRequestException ex)
                    when (responseCode == 429)
                {
                    Events.OnError(null, new RErrorEventArgs(
                        ex,
                        $"Request[Uri={request?.RequestUri?.ToString() ?? "Unknown"}] rejected (too many requests). " +
                        $"\nCode={responseCode}"));

                    await Task.Delay(TimeSpan.FromMilliseconds(
                            RandomGenerator.Next(300, 1500)))
                        .ConfigureAwait(false);
                }
                catch (HttpRequestException ex)
                {
                    OnConnectionStateChanged(ConnectionStateType.Disconnected);

                    Events.OnError(null, new RErrorEventArgs(
                        ex,
                        $"Request[Uri={request?.RequestUri?.ToString() ?? "Unknown"}] sending error. " +
                        $"\nCode={responseCode}"));

                    await Task.Delay(TimeSpan.FromSeconds(1))
                        .ConfigureAwait(false);
                }
            }
        }



        public static void CancelPendingRequests()
        {
            HttpClient.CancelPendingRequests();
        }
    }
}
