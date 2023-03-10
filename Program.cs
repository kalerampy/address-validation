//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Procare Software, LLC">
//     Copyright © 2021-2023 Procare Software, LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Procare.AddressValidation.Tester
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class Program
    {
        private static async Task Main()
        {
            var addressValidationBaseUrl = new Uri("https://addresses.dev-procarepay.com");

            using var factory = new HttpClientFactory();
            using var addressService = new AddressValidationService(factory, false, addressValidationBaseUrl);
            string? input;
            while (true)
            {
                Console.WriteLine("To start a test request press enter.");
                input = Console.ReadLine();
                if (input.ToLower() == "exit")
                {
                    break;
                }

                CancellationTokenSource cts = new CancellationTokenSource(700);
                //var request = new AddressValidationRequest { Line1 = "1 W Main", City = "Medford", StateCode = "OR", ZipCodeLeading5 = "97501" };
                //var request = new AddressValidationRequest();
                var request = new AddressValidationRequest { Line1 = "1125 17th St Ste 1800", City = "Denver", StateCode = "CO", ZipCodeLeading5 = "80202" };
                var response = await addressService.GetAddressesAsync(request, cts.Token).ConfigureAwait(false);
                Console.WriteLine(response);

            }
        }
    }
}
