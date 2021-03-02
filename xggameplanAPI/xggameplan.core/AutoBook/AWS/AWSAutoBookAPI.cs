using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using xggameplan.common.Services;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.Model;

namespace xggameplan.AutoBooks.AWS
{
    /// <summary>
    /// API to AutoBook instance, uses REST
    /// </summary>
    internal class AWSAutoBookAPI : InterfaceToREST, IAutoBookAPI
    {
        private AutoBook _autoBook;
        private AutoBookSettings _autoBookSettings;
        private string _accessToken;
        private const string authType = "bearer";

        public AWSAutoBookAPI(AutoBook autoBook, AutoBookSettings autoBookSettings, string accessToken)
        {
            _autoBook = autoBook;
            _autoBookSettings = autoBookSettings;
            _accessToken = accessToken;
        }

        /// <summary>
        /// Post REST message to start running task
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="scenarioId"></param>
        /// <param name="real"></param>
        public GetAutoBookStatusModel StartAutoBookRun(Guid runId, Guid scenarioId)
        {
            var startAutoBookTaskModel = new StartAutoBookTaskModel()
            {
                runId = runId,
                scenarioId = scenarioId,
                binariesVersion = _autoBookSettings.BinariesVersion
            };
            string url = $"{_autoBook.Api}/task";
            byte[] contentBody = SerializeContentBody(startAutoBookTaskModel);
            var webRequest = CreateHttpWebRequest(url, "POST", GetAuthHeaders(authType, _accessToken), ContentTypeJSON, contentBody);
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });

            // Get response
            GetAutoBookStatusModel status = null;
            using (var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                string data = reader.ReadToEnd().ToLower();
                status = DeserializeContentBody<GetAutoBookStatusModel>(data);
            }
            return status;
        }

        /// <summary>
        /// Deletes run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="scenarioId"></param>
        public void DeleteRun(Guid runId, Guid scenarioId)
        {
            string url = $"{_autoBook.Api}/task";
            var webRequest = CreateHttpWebRequest(url, "DELETE", GetAuthHeaders(authType, _accessToken), string.Empty, null);
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });
        }

        public GetAutoBookSnapshotModel GetSnapshot(Guid scenarioId)
        {
            // Get snapshot URL
            string url = $"{_autoBook.Api}/snapshot/{scenarioId}";
            var webRequest = CreateHttpWebRequest(url, "GET", GetAuthHeaders(authType, _accessToken), string.Empty, null);
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });

            string snapshotUrl = "";
            using (var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                snapshotUrl = reader.ReadToEnd();
            }
            if (string.IsNullOrEmpty(snapshotUrl))
            {
                throw new Exception("No snapshot URL returned");
            }

            // Get snapshot
            var webRequest2 = CreateHttpWebRequest(snapshotUrl, "GET", GetAuthHeaders(authType, _accessToken), string.Empty, null);
            var webResponse2 = (HttpWebResponse)webRequest2.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse2, new List<HttpStatusCode>() { HttpStatusCode.OK });

            using (var stream = webResponse2.GetResponseStream())
            {
                var binaryReader = new BinaryReader(stream);
                var autoBookSnapshotModel = new GetAutoBookSnapshotModel() { Data = binaryReader.ReadBytes((int)webResponse2.ContentLength) };
                return autoBookSnapshotModel;
            }
        }

        /// <summary>
        /// Returns status
        /// </summary>
        /// <returns></returns>
        public AutoBookStatuses GetStatus()
        {
            string url = $"{_autoBook.Api}/status";
            var webRequest = CreateHttpWebRequest(url, "GET", GetAuthHeaders(authType, _accessToken), string.Empty, null);
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });

            // Get response
            GetAutoBookStatusModel status = null;
            using (var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                string data = reader.ReadToEnd().ToLower();
                status = DeserializeContentBody<GetAutoBookStatusModel>(data);
            }
            return status.Status;
        }

        public GetAutoBookVersionModel GetVersion()
        {
            string url = $"{_autoBook.Api}/version";
            var webRequest = CreateHttpWebRequest(url, "GET", GetAuthHeaders(authType, _accessToken), string.Empty, null);
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });

            // Get response
            GetAutoBookVersionModel version = null;
            using (var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                string data = reader.ReadToEnd().ToLower();
                version = DeserializeContentBody<GetAutoBookVersionModel>(data);
            }
            return version;
        }

        public GetAutoBookAuditTrailModel GetAuditTrail(Guid scenarioId)
        {
            string url = $"{_autoBook.Api}/audit-trail/{scenarioId}";
            var webRequest = CreateHttpWebRequest(url, "GET", GetAuthHeaders(authType, _accessToken), string.Empty, null);
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });

            // Get response
            GetAutoBookAuditTrailModel model = null;
            using (var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                string data = reader.ReadToEnd().ToLower();
                model = DeserializeContentBody<GetAutoBookAuditTrailModel>(data);
            }
            return model;

        }

        public GetAutoBookLogsModel GetLogs(Guid scenarioId)
        {
            string url = $"{_autoBook.Api}/logs/{scenarioId}";
            var webRequest = CreateHttpWebRequest(url, "GET", GetAuthHeaders(authType, _accessToken), string.Empty, null);
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });

            // Get response
            GetAutoBookLogsModel model = null;
            using (var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                string data = reader.ReadToEnd().ToLower();
                model = DeserializeContentBody<GetAutoBookLogsModel>(data);
            }
            return model;
        }

        public GetAutoBookStorageInfoModel GetStorageInfo()
        {
            string url = $"{_autoBook.Api}/storage-info";
            var webRequest = CreateHttpWebRequest(url, "GET", GetAuthHeaders(authType, _accessToken), string.Empty, null);
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });

            // Get response
            GetAutoBookStorageInfoModel storageInfo = null;
            using (var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                string data = reader.ReadToEnd().ToLower();
                storageInfo = DeserializeContentBody<GetAutoBookStorageInfoModel>(data);
            }
            return storageInfo;
        }
    }
}
