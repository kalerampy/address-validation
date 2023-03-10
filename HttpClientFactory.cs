//-----------------------------------------------------------------------
// <copyright file="HttpClientFactory.cs" company="Procare Software, LLC">
//     Copyright © 2021-2023 Procare Software, LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Procare.AddressValidation.Tester
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    public class HttpClientFactory : IHttpClientFactory
    {
        private HttpClient? defaultClient;
        private IDictionary<HttpMessageHandler, HttpClient>? specificClients = new Dictionary<HttpMessageHandler, HttpClient>();

        ~HttpClientFactory()
        {
            this.Dispose(false);
        }

        public HttpClient CreateClient()
        {
            return this.CreateClient(default, default);
        }

        public HttpClient CreateClient(HttpMessageHandler? handler, bool disposeHandler)
        {
            if (this.specificClients == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            HttpClient? client;

            if (handler == null)
            {
                client = this.defaultClient ??= new HttpClient();
            }
            else if (!this.specificClients.TryGetValue(handler, out client))
            {
                client = new HttpClient(handler, disposeHandler);
                this.specificClients.Add(handler, client);
            }

            return client;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.defaultClient?.Dispose();

                if (this.specificClients != null)
                {
                    foreach (var client in this.specificClients.Values)
                    {
                        client?.Dispose();
                    }
                }
            }

            this.defaultClient = null;
            this.specificClients = null;
        }
    }
}
