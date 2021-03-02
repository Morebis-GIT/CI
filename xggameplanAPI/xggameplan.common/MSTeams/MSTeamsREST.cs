using System.Collections.Generic;
using System.Net;
using System.Text;
using xggameplan.common.Services;

namespace xggameplan.common.MSTeams
{
    /// <summary>
    /// API for interacting with MS Teams via REST API.
    /// </summary>
    public class MSTeamsREST : InterfaceToREST
    {
        /// <summary>
        /// Posts a simple message to the channel URL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public void PostSimpleMessage(string url, string title, string message)
        {
            const char quotes = '"';

            string json = "{" +
                        quotes + "title" + quotes + ": " + quotes + title + quotes + "," +
                        quotes + "text" + quotes + ": " + quotes + message + quotes +
                    "}";

            var webRequest = CreateHttpWebRequest(url, "POST", GetEmptyHeaders(), ContentTypeJSON, Encoding.UTF8.GetBytes(json));
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });
        }

        /// <summary>
        /// Posts a message card to the channel URL
        /// </summary>
        /// <param name="channelUrl"></param>
        /// <param name="title"></param>
        /// <param name="messageTitle"></param>
        /// <param name="message"></param>
        /// <param name="detailsUrl"></param>
        public void PostMessageCard(string url, string title, string messageTitle, string message, List<string> detailsTextList, List<string> detailsUrlList)
        {
            const char quotes = '"';

            string json = "{" +
              quotes + "summary" + quotes + ": " + quotes + title + quotes + "," +
              quotes + "themeColor" + quotes + ": " + quotes + "0078D7" + quotes + "," +
              quotes + "sections" + quotes + ": [" +
              "{" +
                   quotes + "activityTitle" + quotes + ": " + quotes + messageTitle + quotes + "," +
                   quotes + "text" + quotes + ": " + quotes + message + quotes +
              "}]";

            // Add links
            if (detailsTextList.Count > 0)
            {
                json += ",";
                json += quotes + "potentialAction" + quotes + ": [";
                for (int index = 0; index < detailsTextList.Count; index++)
                {
                    json += (index == 0 ? "" : ",") + "{" +
                          quotes + "@type" + quotes + ": " + quotes + "OpenUri" + quotes + "," +
                          quotes + "name" + quotes + ": " + quotes + detailsTextList[index] + quotes + "," +
                          quotes + "targets" + quotes + ": [" +
                                  "{" + quotes + "os" + quotes + ": " + quotes + "default" + quotes + "," + quotes + "uri" + quotes + ": " + quotes + detailsUrlList[index] + quotes + " }]" +
                        "}";
                }
                json += "]";
            }
            json += "}";

            var webRequest = CreateHttpWebRequest(url, "POST", GetEmptyHeaders(), ContentTypeJSON, Encoding.UTF8.GetBytes(json));
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });
        }
    }
}
