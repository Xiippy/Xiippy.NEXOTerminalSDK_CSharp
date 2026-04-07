// *******************************************************************************************
// Copyright © 2019 Xiippy.ai. All rights reserved. Australian patents awarded. PCT patent pending.
//
// NOTES:
//
// - No payment gateway SDK function is consumed directly. Interfaces are defined out of such interactions and then the interface is implemented for payment gateways. Design the interface with the most common members and data structures between different gateways. 
// - A proper factory or provider must instantiate an instance of the interface that is interacted with.
// - Any major change made to SDKs should begin with the c sharp SDK with the mindset to keep the high-level syntax, structures and class names the same to minimise porting efforts to other languages. Do not use language specific features that do not exist in other languages. We are not in the business of doing the same thing from scratch multiple times in different forms.
// - Pascal Case for naming conventions should be used for all languages
// - No secret or passwords or keys must exist in the code when checked in
//
// *******************************************************************************************

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xiippy.NEXOTerminalSDK.Models;
using Xiippy.NEXOTerminalSDK.Utils;

namespace Xiippy.NEXOTerminalSDK
{
    public class NexoApiClient
    {
        private const string ApplicationJson = "application/json";
        private readonly string _apiBaseUrl;
        private readonly string _clientId;
        private readonly byte[] _ed25519PrivateKey;
        private readonly HttpClient _httpClient;

        public NexoApiClient(string apiBaseUrl, string clientId, byte[] ed25519PrivateKey)
        {
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
                throw new ArgumentException("API base URL cannot be null or empty", nameof(apiBaseUrl));
            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentException("Client ID cannot be null or empty", nameof(clientId));
            if (ed25519PrivateKey == null || ed25519PrivateKey.Length == 0)
                throw new ArgumentException("Ed25519 private key cannot be null or empty", nameof(ed25519PrivateKey));

            _apiBaseUrl = apiBaseUrl.TrimEnd('/');
            _clientId = clientId;
            _ed25519PrivateKey = ed25519PrivateKey;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Creates a payment request. The response is sent asynchronously via webhook.
        /// </summary>
        /// <param name="paymentRequest">The payment request containing transaction details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>HTTP response message containing the initial response</returns>
        public async Task<HttpResponseMessage> CreatePaymentAsync(
            NexoPaymentRequest paymentRequest,
            CancellationToken cancellationToken = default)
        {
            if (paymentRequest == null)
                throw new ArgumentNullException(nameof(paymentRequest));

            return await SendRequestAsync(HttpMethod.Post, "/api/Nexo/payment", paymentRequest, cancellationToken);
        }

  
        public async Task<HttpResponseMessage> CreateAbortionAsync(
         NexoAbortRequest abortRequest,
         CancellationToken cancellationToken = default)
        {
            if (abortRequest == null)
                throw new ArgumentNullException(nameof(abortRequest));

            return await SendRequestAsync(HttpMethod.Post, "/api/Nexo/abort", abortRequest, cancellationToken);
        }

        /// <summary>
        /// Sends a request to the NEXO API endpoint
        /// </summary>
        private async Task<HttpResponseMessage> SendRequestAsync(
            HttpMethod method,
            string endpoint,
            object requestBody,
            CancellationToken cancellationToken)
        {
            var jsonContent = JsonConvert.SerializeObject(requestBody, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var bodyBytes = Encoding.UTF8.GetBytes(jsonContent);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, ApplicationJson);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(ApplicationJson);

            using (var request = new HttpRequestMessage(method, $"{_apiBaseUrl}{endpoint}"))
            {
                request.Content = httpContent;
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJson));


                long requestMoment = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                
                byte[] signature = RequestSignatureHandlerClientSide.GenerateSignatureForRequest(
                    bodyBytes,
                    requestMoment,
                    _ed25519PrivateKey
                );

                string signatureHex = ByteArrayToHex(signature);

                request.Headers.Add("client-id", _clientId);
                request.Headers.Add("client-request-signature", signatureHex);
                request.Headers.Add("request-moment", requestMoment.ToString());

                var response = await _httpClient.SendAsync(request, cancellationToken);
                return response;
            }
        }

        /// <summary>
        /// Converts a byte array to a hexadecimal string representation
        /// </summary>
        private static string ByteArrayToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
