//-----------------------------------------------------------------------
// <copyright file="AddressValidationService.cs" company="Procare Software, LLC">
//     Copyright © 2021-2023 Procare Software, LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Procare.AddressValidation.Tester
{
    using System;
    using System.Net.Http;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Text.Json;

    public class AddressValidationService : BaseHttpService
    {
        public AddressValidationService(IHttpClientFactory httpClientFactory, bool disposeFactory, Uri baseUrl)
            : this(httpClientFactory, disposeFactory, baseUrl, null, false)
        {
        }

        protected AddressValidationService(IHttpClientFactory httpClientFactory, bool disposeFactory, Uri baseUrl, HttpMessageHandler? httpMessageHandler, bool disposeHandler)
            : base(httpClientFactory, disposeFactory, baseUrl, httpMessageHandler, disposeHandler)
        {
        }

        /// <summary>
        /// This api call will retry 3 times until it gets a good response from the address validation service. An exception will be thrown on the third time if not sucessful.
        /// </summary>
        /// <param name="request">The address being validated.</param>
        /// <param name="token">The cancellation token for the first request. This will expire after the first request.</param>
        /// <returns>JSON string response.</returns>
        public async Task<string> GetAddressesAsync(AddressValidationRequest request, CancellationToken token = default)
        {
            // Keeps track of how many times the request has been called in this instance.
            int retries = 0;
            while (retries < 3)
            {
                try
                {
                    retries++;
                    using var httpRequest = request.ToHttpRequest(this.BaseUrl);

                    // This is here to verify that the cancellation is happening after 700ms.
                    Console.WriteLine("Request started at: " + DateTime.Now.ToString("HH:mm:ss.fff"));
                    using var response = await this.CreateClient().SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);

                    if ((int)response.StatusCode >= 500 && (int)response.StatusCode <= 599)
                    {
                        Console.WriteLine("Hit 5xx");

                        if (retries >= 3)
                        {
                            var res = new Response();
                            res.Message = "Looks like something went wrong on our end! Please try again. If the problem persists, please contact customer service.";
                            throw new HttpRequestException(JsonSerializer.Serialize(res));
                        }

                        continue;
                    }

                    // All other responses are returned below including 400 responses.
                    return await response.Content.ReadAsStringAsync(token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    Console.WriteLine("Hit Cancellation");

                    // This verifies the cancellation timing.
                    Console.WriteLine("Request completed at: " + DateTime.Now.ToString("HH:mm:ss.fff"));
                    if (retries >= 3)
                    {
                        var res = new Response();
                        res.Message = "Request to address validation has timed out. Please try again. If the problem persists, please contact customer service.";

                        // This will return only if three request are unsuccessful.
                        return JsonSerializer.Serialize(res);
                    }
                }
                catch (HttpRequestException ex)
                {
                    // This catches the 5xx exception. Will only return if three request are unsuccessful.
                    return ex.Message;
                }

                // The cancellation token is no longer valid after the first iteration of the loop. This resets the value of the token after a cancellation.
                var t = new CancellationTokenSource(700);
                token = t.Token;
            }

            return "";
        }

    }
}
