﻿using OrangedataRequest.Helpers;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OrangedataRequest.DataService
{
    internal sealed class ODDataService
    {
        public ODDataService(string keyXml, byte[] cert, string certPassword, string apiUrl)
        {
            _keyXml = keyXml;
            _cert = new X509Certificate2(cert, certPassword);
            _apiUrl = apiUrl;
        }

        private readonly string _apiUrl;

        private readonly string _keyXml;
        private readonly X509Certificate2 _cert;

        #region Public methods

        public async Task<ODResponse> SendCheckAsync(ReqCreateCheck check)
        {
            var requestBody = SerializationHelper.Serialize(check);
            var signature = ComputeSignature(requestBody);

            return await SendRequestAsync<RespCreateCheck>($"{_apiUrl}/documents", HttpMethod.Post, requestBody, signature);
        }

        public async Task<ODResponse> CreateCorrectionsCheckAsync(ReqCreateCorrectionCheck correctionCheck)
        {
            var requestBody = SerializationHelper.Serialize(correctionCheck);
            var signature = ComputeSignature(requestBody);

            return await SendRequestAsync<RespCreateCheck>($"{_apiUrl}/corrections", HttpMethod.Post, requestBody, signature);
        }

        public async Task<ODResponse> GetCheckStateAsync(string INN, string documentId)
        {
            return await SendRequestAsync<RespCheckStatus>($"{_apiUrl}/documents/{INN}/status/{documentId}", HttpMethod.Get);
        }

        public async Task<ODResponse> GetCorrectionCheckStateAsync(string INN, string documentId)
        {
            return await SendRequestAsync<RespCorrectionCheckStatus>($"{_apiUrl}/corrections/{INN}/status/{documentId}", HttpMethod.Get);
        }

        #endregion Public methods

        #region Helpers

        private async Task<ODResponse> SendRequestAsync<T>(string uri, HttpMethod method, string requestBody = null, string signature = null)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException(nameof(uri));
            }

            using (var client = new HttpClient(new HttpClientHandler
            {
                ClientCertificates =
                {
                    _cert,
                },
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true
            })) 
            {
                var request = new HttpRequestMessage(method, uri);
                if (!string.IsNullOrWhiteSpace(signature))
                {
                    request.Headers.Add("X-Signature", signature);
                }
                //httpRequest.KeepAlive = true;
                //httpRequest.UserAgent = "OrangeDataClient";
                //httpRequest.PreAuthenticate = true;
                if (!string.IsNullOrEmpty(requestBody))
                {
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                }

                var response = await client.SendAsync(request);
                return await ExtractResponseAsync<T>(response);
            }
        }

        private async Task<ODResponse> ExtractResponseAsync<T>(HttpResponseMessage response)
        {
            var res = new ODResponse();
            using (response)
            {
                res.StatusCode = response.StatusCode;
                var text = await response.Content.ReadAsStringAsync();
                res.Response = text;
                try
                {
                    res.ResponseObject = SerializationHelper.Deserialize<T>(text);
                }
                catch(Exception ex)
                {
                    res.ResponseObject = ex;
                }
            }
            return res;
        }

        private string ComputeSignature(string document)
        {
            var data = Encoding.UTF8.GetBytes(document);

            using (var rsa = RSA.Create())
            {
                rsa.ImportFromXml(_keyXml);
                return Convert.ToBase64String(rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
            }
        }

        #endregion Helpers
    }
}
