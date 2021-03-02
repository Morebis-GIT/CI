using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.common.Services;
using xggameplan.Model;

namespace xggameplan.AutoBooks.AWS
{
    /// <summary>
    /// AutoBooks API for managing AutoBook instances in AWS using the Provisioning API, uses REST
    /// </summary>
    public class AWSAutoBooksAPI : InterfaceToREST, IAutoBooksAPI<AWSPAAutoBook, AWSPACreateAutoBook>
    {
        private readonly string _baseUrl;
        private readonly string _apibaseUrl;
        private readonly string _accessToken;
        private const string AuthType = "bearer";
        private readonly IAuditEventRepository _auditEventRepository;

        public AWSAutoBooksAPI(string baseUrl, string accessToken, IAuditEventRepository auditEventRepository)
        {
            _baseUrl = $"{baseUrl}/autobooks";
            _apibaseUrl = baseUrl;
            _accessToken = accessToken;
            _auditEventRepository = auditEventRepository;
        }

        public AWSPAAutoBook Create(AWSPACreateAutoBook paCreateAutoBook)
        {
            AWSPAAutoBook autoBookObject = null;
            byte[] contentBody = SerializeContentBody(paCreateAutoBook);
            var webRequest = CreateHttpWebRequest(_baseUrl, "POST", GetAuthHeaders(AuthType, _accessToken), ContentTypeJSON, contentBody);
            using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });
                autoBookObject = null;
                using var reader = new StreamReader(webResponse.GetResponseStream());
                string data = reader.ReadToEnd();
                autoBookObject = DeserializeContentBody<AWSPAAutoBook>(data);
            }

            return autoBookObject;
        }

        public AWSPAAutoBook Get(string paAutoBookId)
        {
            paAutoBookId = paAutoBookId.Replace("AutoBooks/", String.Empty);
            string url = $"{_baseUrl}/{paAutoBookId}";
            var webRequest = CreateHttpWebRequest(url, "GET", GetAuthHeaders(AuthType, _accessToken), String.Empty, null);
            try
            {
                using var webResponse = (HttpWebResponse)webRequest.GetResponse();
                ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });

                AWSPAAutoBook autoBook = null;
                using (var reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    string data = reader.ReadToEnd();
                    autoBook = DeserializeContentBody<AWSPAAutoBook>(data);
                }
                return autoBook;
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw ex;
            }
        }

        public List<AWSPAAutoBook> GetAll()
        {
            var webRequest = CreateHttpWebRequest(_baseUrl, "GET", GetAuthHeaders(AuthType, _accessToken), String.Empty, null);
            using var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });

            List<AWSPAAutoBook> paAutoBooks = null;
            using (var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                string data = reader.ReadToEnd();
                paAutoBooks = DeserializeContentBody<List<AWSPAAutoBook>>(data);
            }
            return paAutoBooks;
        }

        public void Update(AWSPAAutoBook paAutoBook)
        {
            byte[] contentBody = SerializeContentBody(paAutoBook);
            string url = $"{_baseUrl}/{paAutoBook.Id}";
            var webRequest = CreateHttpWebRequest(url, "PUT", GetAuthHeaders(AuthType, _accessToken), ContentTypeJSON, contentBody);
            using var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });
        }

        /// <summary>
        /// Deletes AutoBook
        /// </summary>
        /// <param name="autoBookId"></param>
        public void Delete(string autoBookId)
        {
            string url = $"{_baseUrl}/{autoBookId}";
            var webRequest = CreateHttpWebRequest(url, "DELETE", GetAuthHeaders(AuthType, _accessToken), String.Empty, null);
            using var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });
        }

        /// <summary>
        /// Restarts AutoBook
        /// </summary>
        /// <param name="autoBookId"></param>
        public void Restart(string autoBookId)
        {
            string url = $"{_baseUrl}/{autoBookId}/restart";
            var webRequest = CreateHttpWebRequest(url, "POST", GetAuthHeaders(AuthType, _accessToken), String.Empty, null);
            using var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });
        }

        /// <summary>
        /// Requests an autobookRequest model
        /// </summary>
        /// <param name="autoBookRequest"></param>
        public string AutoBookRequestRun(AutoBookRequestModel autoBookRequest)
        {
            string url = $"{_apibaseUrl}/run";
            byte[] contentBody = SerializeContentBody(autoBookRequest);
            int len = 0;
            len = contentBody.Length;

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                $"Creating WebRequest for {nameof(AutoBookRequestRun)}, content body len after serialization: {len.ToString()}, url: {url}, autoBookRequest.mock: {autoBookRequest.mock}"));

            try
            {
                var webRequest = CreateHttpWebRequest(url, "POST", GetAuthHeaders(AuthType, _accessToken), ContentTypeJSON, contentBody);

                using var webResponse = (HttpWebResponse)webRequest.GetResponse();
                ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });
                using var reader = new StreamReader(webResponse.GetResponseStream());
                {
                    var data = reader.ReadToEnd();
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                        $"Created WebRequest for {nameof(AutoBookRequestRun)}, url: {url}, data: {data})"));
                    return data;
                }
            }
            catch (WebException ex)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $"Error thrown Creating WebRequest for {nameof(AutoBookRequestRun)}, url: {url}, ex: {ex})"));
                throw ex;
            }
        }
    }
}
