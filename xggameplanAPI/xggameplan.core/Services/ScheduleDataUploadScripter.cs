using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using xggameplan.common.Utilities;

namespace xggameplan.Services
{
    /// <summary>
    /// Takes the schedule data previously uploaded (HTTP request log & content files) and prepares it so that it can be uploaded
    /// a different environment by running the Upload Data script.
    /// </summary>
    public class ScheduleDataUploadScripter
    {
        private class RequestDetails
        {
            public Guid Id { get; set; }

            public string Method { get; set; }
            public string URL { get; set; }
        }

        private class RequestMatch
        {
            public enum ActionTypes : byte
            {
                DeleteAll = 0,   // Delete all
                PostBatch = 1,   // Post batch of data
                PostRun = 2      // Post run
            }

            public string Description { get; set; }

            public string Method { get; set; }

            public string URLPattern { get; set; }

            public string TargetEntity { get; set; }    // Spot, Break etc

            public ActionTypes ActionType { get; set; }
        }

        public void Build(string requestLogFile, string responseLogFile, string contentFolder, string outputFolder)
        {
            // Load requests file            
            DataTable requestDataTable = CSVUtilities.LoadCSVInToDataTable(requestLogFile, (Char)9, -1, -1);

            // Get request match list
            List<RequestMatch> requestMatchList = GetRequestMatchList();

            // Process requests
            for (int requestRowIndex = 0; requestRowIndex < requestDataTable.Rows.Count; requestRowIndex++)
            {
                // Get request details 
                RequestDetails requestDetails = GetRequestDetails(requestDataTable, requestRowIndex);

                // Identify request type
                RequestMatch requestMatch = GetRequestMatch(requestDetails, requestMatchList);

                if (requestMatch != null)    // Request type that we handle
                {
                    switch (requestMatch.ActionType)
                    {
                        case RequestMatch.ActionTypes.PostBatch:
                            ProcessPostBatch(requestDetails, requestMatch, contentFolder, outputFolder);
                            break;
                        case RequestMatch.ActionTypes.DeleteAll:
                            ProcessDeleteAll(requestDetails, requestMatch, contentFolder, outputFolder);
                            break;
                        case RequestMatch.ActionTypes.PostRun:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Processes the request to upload a batch of data
        /// </summary>
        /// <param name="requestDetails"></param>
        /// <param name="requestMatch"></param>
        /// <param name="contentFolder"></param>
        /// <param name="outputFolder"></param>
        private void ProcessPostBatch(RequestDetails requestDetails, RequestMatch requestMatch, string contentFolder, string outputFolder)
        {
            // Create folder for this category of data
            string entityOutputFolder = string.Format(@"{0}\{1}", outputFolder, requestMatch.TargetEntity);
            Directory.CreateDirectory(entityOutputFolder);
            
            string srcContentFile = string.Format(@"{0}\{1}.request.content", contentFolder, requestDetails.Id);

            // Create data file, sequence number ensures processing in same order, assume compressed format
            int fileSequence = Directory.GetFiles(entityOutputFolder, "*.*").Length + 1;
            string dstContentFile = string.Format(@"{0}\{1}.{2}.gz", entityOutputFolder, requestMatch.TargetEntity, fileSequence.ToString("0000"));

            // Copy data file
            File.Copy(srcContentFile, dstContentFile);
        }

        private void ProcessDeleteAll(RequestDetails requestDetails, RequestMatch requestMatch, string contentFolder, string outputFolder)
        {
            // Create folder for this category of data, causes DELETE to be executed. This might be necessary if there's no
            // data to upload. E.g. Schedule.
            string entityOutputFolder = string.Format(@"{0}\{1}", outputFolder, requestMatch.TargetEntity);
            Directory.CreateDirectory(entityOutputFolder);

            // string srcContentFile = string.Format(@"{0}\{1}.request.content", contentFolder, requestDetails.Id);

            // Create data file, sequence number ensures processing in same order, assume compressed format
            //int fileSequence = Directory.GetFiles(entityOutputFolder, "*.*").Length + 1;
            //string dstContentFile = string.Format(@"{0}\{1}.{2}.gz", entityOutputFolder, requestMatch.TargetEntity, fileSequence.ToString("0000"));            
        }

        private RequestDetails GetRequestDetails(DataTable requestDataTable, int rowIndex)
        {
            return new RequestDetails()
            {
                Id = new Guid(requestDataTable.Rows[rowIndex]["RequestID"].ToString()),
                Method = requestDataTable.Rows[rowIndex]["Method"].ToString(),
                URL = requestDataTable.Rows[rowIndex]["URI"].ToString()
            };
        }

        private RequestMatch GetRequestMatch(RequestDetails requestDetails, List<RequestMatch> requestMatchList)
        {
            return requestMatchList.Where(rq => IsMatch(requestDetails, rq)).FirstOrDefault();            
        }        

        private bool IsMatch(RequestDetails requestDetails, RequestMatch requestMatch)
        {
            if (requestDetails.Method.ToUpper() == requestMatch.Method.ToUpper())
            {
                if (requestDetails.URL.ToLower().EndsWith(requestMatch.URLPattern.ToLower()))   // Currently only supports ends with
                {
                    return true;
                }
            }
            return false;
        }

        private List<RequestMatch> GetRequestMatchList()
        {
            List<RequestMatch> requestMatchList = new List<RequestMatch>();

            // Delete all requests
            requestMatchList.Add(new RequestMatch() { Description = "Breaks (Delete All)", TargetEntity = "Breaks", Method = "DELETE", URLPattern = "/breaks/DeleteAll", ActionType = RequestMatch.ActionTypes.DeleteAll });  // Yes
            requestMatchList.Add(new RequestMatch() { Description = "Campaigns (Delete All)", TargetEntity = "Campaigns", Method = "DELETE", URLPattern = "/campaigns/DeleteAll", ActionType = RequestMatch.ActionTypes.DeleteAll });   // Yes
            requestMatchList.Add(new RequestMatch() { Description = "Clashes (Delete All)", TargetEntity = "Clashes", Method = "DELETE", URLPattern = "/clash/DeleteAll", ActionType = RequestMatch.ActionTypes.DeleteAll });  //  Yes
            requestMatchList.Add(new RequestMatch() { Description = "Products (Delete All)", TargetEntity = "Products", Method = "DELETE", URLPattern = "/product/DeleteAll", ActionType = RequestMatch.ActionTypes.DeleteAll });   // Yes
            requestMatchList.Add(new RequestMatch() { Description = "Programmes (Delete All)", TargetEntity = "Programmes", Method = "DELETE", URLPattern = "/programmes/DeleteAll", ActionType = RequestMatch.ActionTypes.DeleteAll });    // Yes
            requestMatchList.Add(new RequestMatch() { Description = "Ratings (Delete All)", TargetEntity = "Ratings", Method = "DELETE", URLPattern = "/ratings/DeleteAll", ActionType = RequestMatch.ActionTypes.DeleteAll }); 
            requestMatchList.Add(new RequestMatch() { Description = "Schedules (Delete All)", TargetEntity = "Schedules", Method = "DELETE", URLPattern = "/schedules/DeleteAll", ActionType = RequestMatch.ActionTypes.DeleteAll });   // Yes
            requestMatchList.Add(new RequestMatch() { Description = "Spots (Delete All)", TargetEntity = "Spots", Method = "DELETE", URLPattern = "/spots/DeleteAll", ActionType = RequestMatch.ActionTypes.DeleteAll });   // Yes

            // Post batch requests
            requestMatchList.Add(new RequestMatch() { Description = "Breaks (Upload)", TargetEntity = "Breaks", Method = "POST", URLPattern = "/breaks", ActionType = RequestMatch.ActionTypes.PostBatch });
            requestMatchList.Add(new RequestMatch() { Description = "Campaigns (Upload)", TargetEntity = "Campaigns", Method = "POST", URLPattern = "/campaigns", ActionType = RequestMatch.ActionTypes.PostBatch });
            requestMatchList.Add(new RequestMatch() { Description = "Clashes (Upload)", TargetEntity = "Clashes", Method = "POST", URLPattern = "/clash", ActionType = RequestMatch.ActionTypes.PostBatch });
            requestMatchList.Add(new RequestMatch() { Description = "Products (Upload)", TargetEntity = "Products", Method = "POST", URLPattern = "/product", ActionType = RequestMatch.ActionTypes.PostBatch });
            requestMatchList.Add(new RequestMatch() { Description = "Programmes (Upload)", TargetEntity = "Programmes", Method = "POST", URLPattern = "/programmes", ActionType = RequestMatch.ActionTypes.PostBatch });
            requestMatchList.Add(new RequestMatch() { Description = "Ratings (Upload)", TargetEntity = "Ratings", Method = "POST", URLPattern = "/ratings", ActionType = RequestMatch.ActionTypes.PostBatch });
            requestMatchList.Add(new RequestMatch() { Description = "Spots (Upload)", TargetEntity = "Spots", Method = "POST", URLPattern = "/spots", ActionType = RequestMatch.ActionTypes.PostBatch });

            // Post run requests
            requestMatchList.Add(new RequestMatch() { Description = "Run (Upload)", TargetEntity = "Runs", Method = "POST", URLPattern = "/runs", ActionType = RequestMatch.ActionTypes.PostRun });

            return requestMatchList;
        }
    }
}
