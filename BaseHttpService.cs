//-----------------------------------------------------------------------
// <copyright file="BaseHttpService.cs" company="Procare Software, LLC">
//     Copyright © 2021-2023 Procare Software, LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Procare.AddressValidation.Tester
{
    using System;
    using System.Net.Http;

    public class BaseHttpService : IDisposable
    {
        private readonly bool disposeFactory;
        private readonly bool disposeHandler;
        private IHttpClientFactory? httpClientFactory;
        private HttpMessageHandler? httpMessageHandler;

        protected BaseHttpService(IHttpClientFactory httpClientFactory, bool disposeFactory, Uri baseUrl, HttpMessageHandler? httpMessageHandler, bool disposeHandler)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.disposeFactory = disposeFactory;
            this.BaseUrl = baseUrl;
            this.httpMessageHandler = httpMessageHandler;
            this.disposeHandler = disposeHandler;
        }

        ~BaseHttpService()
        {
            this.Dispose(false);
        }

        public Uri BaseUrl { get; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected HttpClient CreateClient()
        {
            if (this.httpClientFactory == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            return this.httpClientFactory.CreateClient(this.httpMessageHandler, this.disposeHandler);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.disposeFactory)
                {
                    this.httpClientFactory?.Dispose();
                }
            }

            this.httpMessageHandler = null;
            this.httpClientFactory = null;
        }
    }
}
