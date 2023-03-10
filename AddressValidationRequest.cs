//-----------------------------------------------------------------------
// <copyright file="AddressValidationRequest.cs" company="Procare Software, LLC">
//     Copyright © 2021-2023 Procare Software, LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Procare.AddressValidation.Tester
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    public struct AddressValidationRequest : IEquatable<AddressValidationRequest>
    {
        public string? CompanyName { get; set; }

        public string? Line1 { get; set; }

        public string? Line2 { get; set; }

        public string? City { get; set; }

        public string? StateCode { get; set; }

        public string? Urbanization { get; set; }

        public string? ZipCodeLeading5 { get; set; }

        public string? ZipCodeTrailing4 { get; set; }

        public static bool operator ==(AddressValidationRequest left, AddressValidationRequest right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AddressValidationRequest left, AddressValidationRequest right)
        {
            return !(left == right);
        }

        public string ToQueryString()
        {
            var result = new StringBuilder();

            foreach (var prop in this.GetType().GetProperties())
            {
                var value = (string?)prop.GetMethod!.Invoke(this, Array.Empty<object>());
                if (!string.IsNullOrEmpty(value))
                {
                    result.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}={2}", result.Length == 0 ? "?" : "&", WebUtility.UrlEncode(prop.Name), WebUtility.UrlEncode(value));
                }
            }

            return result.ToString();
        }

        public HttpRequestMessage ToHttpRequest(Uri baseUri)
        {
            return new HttpRequestMessage(HttpMethod.Get, new Uri(baseUri, this.ToQueryString()));
        }

        public bool Equals(AddressValidationRequest other)
        {
            return this.CompanyName == other.CompanyName &&
                this.Line1 == other.Line1 &&
                this.Line2 == other.Line2 &&
                this.City == other.City &&
                this.StateCode == other.StateCode &&
                this.Urbanization == other.Urbanization &&
                this.ZipCodeLeading5 == other.ZipCodeLeading5 &&
                this.ZipCodeTrailing4 == other.ZipCodeTrailing4;
        }

        public override bool Equals(object? obj)
        {
            return obj is AddressValidationRequest other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.CompanyName, this.Line1, this.Line2, this.City, this.StateCode, this.Urbanization, this.ZipCodeLeading5, this.ZipCodeTrailing4);
        }
    }
}
