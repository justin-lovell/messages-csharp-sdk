/*
 * MessageMediaMessages.PCL
 *
 */
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using APIMATIC.SDK.Common;
using APIMATIC.SDK.Http.Request;
using APIMATIC.SDK.Http.Response;
using APIMATIC.SDK.Http.Client;

namespace MessageMedia.Messages.Controllers
{
    public partial class DeliveryReportsController: BaseController, IDeliveryReportsController
    {
        #region Singleton Pattern

        //private static variables for the singleton pattern
        private static object syncObject = new object();
        private static DeliveryReportsController instance = null;

        /// <summary>
        /// Singleton pattern implementation
        /// </summary>
        internal static DeliveryReportsController Instance
        {
            get
            {
                lock (syncObject)
                {
                    if (null == instance)
                    {
                        instance = new DeliveryReportsController();
                    }
                }
                return instance;
            }
        }

        #endregion Singleton Pattern

        /// <summary>
        /// Check for any delivery reports that have been received.
        /// Delivery reports are a notification of the change in status of a message as it is being processed.
        /// Each request to the check delivery reports endpoint will return any delivery reports received that
        /// have not yet been confirmed using the confirm delivery reports endpoint. A response from the check
        /// delivery reports endpoint will have the following structure:
        /// ```json
        /// {
        ///     "delivery_reports": [
        ///         {
        ///             "callback_url": "https://my.callback.url.com",
        ///             "delivery_report_id": "01e1fa0a-6e27-4945-9cdb-18644b4de043",
        ///             "source_number": "+61491570157",
        ///             "date_received": "2017-05-20T06:30:37.642Z",
        ///             "status": "enroute",
        ///             "delay": 0,
        ///             "submitted_date": "2017-05-20T06:30:37.639Z",
        ///             "original_text": "My first message!",
        ///             "message_id": "d781dcab-d9d8-4fb2-9e03-872f07ae94ba",
        ///             "vendor_account_id": {
        ///                 "vendor_id": "MessageMedia",
        ///                 "account_id": "MyAccount"
        ///             },
        ///             "metadata": {
        ///                 "key1": "value1",
        ///                 "key2": "value2"
        ///             }
        ///         },
        ///         {
        ///             "callback_url": "https://my.callback.url.com",
        ///             "delivery_report_id": "0edf9022-7ccc-43e6-acab-480e93e98c1b",
        ///             "source_number": "+61491570158",
        ///             "date_received": "2017-05-21T01:46:42.579Z",
        ///             "status": "enroute",
        ///             "delay": 0,
        ///             "submitted_date": "2017-05-21T01:46:42.574Z",
        ///             "original_text": "My second message!",
        ///             "message_id": "fbb3b3f5-b702-4d8b-ab44-65b2ee39a281",
        ///             "vendor_account_id": {
        ///                 "vendor_id": "MessageMedia",
        ///                 "account_id": "MyAccount"
        ///             },
        ///             "metadata": {
        ///                 "key1": "value1",
        ///                 "key2": "value2"
        ///             }
        ///         }
        ///     ]
        /// }
        /// ```
        /// Each delivery report will contain details about the message, including any metadata specified
        /// and the new status of the message (as each delivery report indicates a change in status of a
        /// message) and the timestamp at which the status changed. Every delivery report will have a 
        /// unique delivery report ID for use with the confirm delivery reports endpoint.
        /// *Note: The source number and destination number properties in a delivery report are the inverse of
        /// those specified in the message that the delivery report relates to. The source number of the
        /// delivery report is the destination number of the original message.*
        /// Subsequent requests to the check delivery reports endpoint will return the same delivery reports
        /// and a maximum of 100 delivery reports will be returned in each request. Applications should use the
        /// confirm delivery reports endpoint in the following pattern so that delivery reports that have been
        /// processed are no longer returned in subsequent check delivery reports requests.
        /// 1. Call check delivery reports endpoint
        /// 2. Process each delivery report
        /// 3. Confirm all processed delivery reports using the confirm delivery reports endpoint
        /// *Note: It is recommended to use the Webhooks feature to receive reply messages rather than
        /// polling the check delivery reports endpoint.*
        /// </summary>
        /// <return>Returns the Models.CheckDeliveryReportsResponse response from the API call</return>
        public Models.CheckDeliveryReportsResponse GetCheckDeliveryReports()
        {
            Task<Models.CheckDeliveryReportsResponse> t = GetCheckDeliveryReportsAsync();
            APIHelper.RunTaskSynchronously(t);
            return t.Result;
        }

        /// <summary>
        /// Check for any delivery reports that have been received.
        /// Delivery reports are a notification of the change in status of a message as it is being processed.
        /// Each request to the check delivery reports endpoint will return any delivery reports received that
        /// have not yet been confirmed using the confirm delivery reports endpoint. A response from the check
        /// delivery reports endpoint will have the following structure:
        /// ```json
        /// {
        ///     "delivery_reports": [
        ///         {
        ///             "callback_url": "https://my.callback.url.com",
        ///             "delivery_report_id": "01e1fa0a-6e27-4945-9cdb-18644b4de043",
        ///             "source_number": "+61491570157",
        ///             "date_received": "2017-05-20T06:30:37.642Z",
        ///             "status": "enroute",
        ///             "delay": 0,
        ///             "submitted_date": "2017-05-20T06:30:37.639Z",
        ///             "original_text": "My first message!",
        ///             "message_id": "d781dcab-d9d8-4fb2-9e03-872f07ae94ba",
        ///             "vendor_account_id": {
        ///                 "vendor_id": "MessageMedia",
        ///                 "account_id": "MyAccount"
        ///             },
        ///             "metadata": {
        ///                 "key1": "value1",
        ///                 "key2": "value2"
        ///             }
        ///         },
        ///         {
        ///             "callback_url": "https://my.callback.url.com",
        ///             "delivery_report_id": "0edf9022-7ccc-43e6-acab-480e93e98c1b",
        ///             "source_number": "+61491570158",
        ///             "date_received": "2017-05-21T01:46:42.579Z",
        ///             "status": "enroute",
        ///             "delay": 0,
        ///             "submitted_date": "2017-05-21T01:46:42.574Z",
        ///             "original_text": "My second message!",
        ///             "message_id": "fbb3b3f5-b702-4d8b-ab44-65b2ee39a281",
        ///             "vendor_account_id": {
        ///                 "vendor_id": "MessageMedia",
        ///                 "account_id": "MyAccount"
        ///             },
        ///             "metadata": {
        ///                 "key1": "value1",
        ///                 "key2": "value2"
        ///             }
        ///         }
        ///     ]
        /// }
        /// ```
        /// Each delivery report will contain details about the message, including any metadata specified
        /// and the new status of the message (as each delivery report indicates a change in status of a
        /// message) and the timestamp at which the status changed. Every delivery report will have a 
        /// unique delivery report ID for use with the confirm delivery reports endpoint.
        /// *Note: The source number and destination number properties in a delivery report are the inverse of
        /// those specified in the message that the delivery report relates to. The source number of the
        /// delivery report is the destination number of the original message.*
        /// Subsequent requests to the check delivery reports endpoint will return the same delivery reports
        /// and a maximum of 100 delivery reports will be returned in each request. Applications should use the
        /// confirm delivery reports endpoint in the following pattern so that delivery reports that have been
        /// processed are no longer returned in subsequent check delivery reports requests.
        /// 1. Call check delivery reports endpoint
        /// 2. Process each delivery report
        /// 3. Confirm all processed delivery reports using the confirm delivery reports endpoint
        /// *Note: It is recommended to use the Webhooks feature to receive reply messages rather than
        /// polling the check delivery reports endpoint.*
        /// </summary>
        /// <return>Returns the Models.CheckDeliveryReportsResponse response from the API call</return>
        public async Task<Models.CheckDeliveryReportsResponse> GetCheckDeliveryReportsAsync()
        {
            //the base uri for api requests
            string _baseUri = Configuration.BaseUri;

            //prepare query string for API call
            StringBuilder _queryBuilder = new StringBuilder(_baseUri);
            _queryBuilder.Append("/v1/delivery_reports");


            //validate and preprocess url
            string _queryUrl = APIHelper.CleanUrl(_queryBuilder);

            //append request with appropriate headers and parameters
            var _headers = new Dictionary<string,string>()
            {
                { "user-agent", "messagemedia-messages-csharp-sdk-1.0.0" },
                { "accept", "application/json" }
            };

            //prepare the API call request to fetch the response
            HttpRequest _request = ClientInstance.Get(_queryUrl,_headers, Configuration.BasicAuthUserName, Configuration.BasicAuthPassword);

            //invoke request and get response
            HttpStringResponse _response = (HttpStringResponse) await ClientInstance.ExecuteAsStringAsync(_request).ConfigureAwait(false);
            HttpContext _context = new HttpContext(_request,_response);
            //handle errors defined at the API level
            base.ValidateResponse(_response, _context);

            try
            {
                return APIHelper.JsonDeserialize<Models.CheckDeliveryReportsResponse>(_response.Body);
            }
            catch (Exception _ex)
            {
                throw new APIException("Failed to parse the response: " + _ex.Message, _context);
            }
        }

        /// <summary>
        /// Mark a delivery report as confirmed so it is no longer return in check delivery reports requests.
        /// The confirm delivery reports endpoint is intended to be used in conjunction with the check delivery
        /// reports endpoint to allow for robust processing of delivery reports. Once one or more delivery
        /// reports have been processed, they can then be confirmed using the confirm delivery reports endpoint so they
        /// are no longer returned in subsequent check delivery reports requests.
        /// The confirm delivery reports endpoint takes a list of delivery report IDs as follows:
        /// ```json
        /// {
        ///     "delivery_report_ids": [
        ///         "011dcead-6988-4ad6-a1c7-6b6c68ea628d",
        ///         "3487b3fa-6586-4979-a233-2d1b095c7718",
        ///         "ba28e94b-c83d-4759-98e7-ff9c7edb87a1"
        ///     ]
        /// }
        /// ```
        /// Up to 100 delivery reports can be confirmed in a single confirm delivery reports request.
        /// </summary>
        /// <param name="body">Required parameter: Example: </param>
        /// <return>Returns the dynamic response from the API call</return>
        public dynamic CreateConfirmDeliveryReportsAsReceived(Models.ConfirmDeliveryReportsAsReceivedRequest body)
        {
            Task<dynamic> t = CreateConfirmDeliveryReportsAsReceivedAsync(body);
            APIHelper.RunTaskSynchronously(t);
            return t.Result;
        }

        /// <summary>
        /// Mark a delivery report as confirmed so it is no longer return in check delivery reports requests.
        /// The confirm delivery reports endpoint is intended to be used in conjunction with the check delivery
        /// reports endpoint to allow for robust processing of delivery reports. Once one or more delivery
        /// reports have been processed, they can then be confirmed using the confirm delivery reports endpoint so they
        /// are no longer returned in subsequent check delivery reports requests.
        /// The confirm delivery reports endpoint takes a list of delivery report IDs as follows:
        /// ```json
        /// {
        ///     "delivery_report_ids": [
        ///         "011dcead-6988-4ad6-a1c7-6b6c68ea628d",
        ///         "3487b3fa-6586-4979-a233-2d1b095c7718",
        ///         "ba28e94b-c83d-4759-98e7-ff9c7edb87a1"
        ///     ]
        /// }
        /// ```
        /// Up to 100 delivery reports can be confirmed in a single confirm delivery reports request.
        /// </summary>
        /// <param name="body">Required parameter: Example: </param>
        /// <return>Returns the dynamic response from the API call</return>
        public async Task<dynamic> CreateConfirmDeliveryReportsAsReceivedAsync(Models.ConfirmDeliveryReportsAsReceivedRequest body)
        {
            //the base uri for api requests
            string _baseUri = Configuration.BaseUri;

            //prepare query string for API call
            StringBuilder _queryBuilder = new StringBuilder(_baseUri);
            _queryBuilder.Append("/v1/delivery_reports/confirmed");


            //validate and preprocess url
            string _queryUrl = APIHelper.CleanUrl(_queryBuilder);

            //append request with appropriate headers and parameters
            var _headers = new Dictionary<string,string>()
            {
                { "user-agent", "messagemedia-messages-csharp-sdk-1.0.0" },
                { "accept", "application/json" },
                { "content-type", "application/json; charset=utf-8" }
            };

            //append body params
            var _body = APIHelper.JsonSerialize(body);

            //prepare the API call request to fetch the response
            HttpRequest _request = ClientInstance.PostBody(_queryUrl, _headers, _body, Configuration.BasicAuthUserName, Configuration.BasicAuthPassword);

            //invoke request and get response
            HttpStringResponse _response = (HttpStringResponse) await ClientInstance.ExecuteAsStringAsync(_request).ConfigureAwait(false);
            HttpContext _context = new HttpContext(_request,_response);

            //Error handling using HTTP status codes
            if (_response.StatusCode == 400)
                throw new APIException(@"", _context);

            //handle errors defined at the API level
            base.ValidateResponse(_response, _context);

            try
            {
                return APIHelper.JsonDeserialize<dynamic>(_response.Body);
            }
            catch (Exception _ex)
            {
                throw new APIException("Failed to parse the response: " + _ex.Message, _context);
            }
        }

    }
} 