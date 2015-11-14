using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace NumberSpeaker.Common
{
    public class TranslationService
    {
        private static string _targetLanguageCode;
        private static string _textToTranslate;
        private CoreDispatcher _dispatcher;

        public string TranslatedText { get; private set; }

        public TranslationService(Windows.UI.Core.CoreDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void PerformTranslation(string txtToTrans)
        {
            // Initialize the strTextToTranslate here, while we're on the UI thread
            _textToTranslate = txtToTrans;
            // STEP 1: Create the request for the OAuth service that will
            // get us our access tokens.
            var strTranslatorAccessURI = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
            var serviceUri = new Uri("https://api.datamarket.azure.com/Bing/MicrosoftTranslator/");
            //var accountKey = 
            var req = System.Net.WebRequest.Create(strTranslatorAccessURI);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            // Important to note -- the call back here isn't that the request was processed by the server
            // but just that the request is ready for you to do stuff to it (like writing the details)
            // and then post it to the server.
            IAsyncResult writeRequestStreamCallback =
              (IAsyncResult)req.BeginGetRequestStream(new AsyncCallback(RequestStreamReady), req);

        }

        private void RequestStreamReady(IAsyncResult ar)
        {
            // STEP 2: The request stream is ready. Write the request into the POST stream
            var clientID = "TranslateAndMore_01";
            var clientSecret = "b96cf00da97d5ae8d8d0d9fa0637046b";
            var strRequestDetails = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com",
              System.Net.WebUtility.UrlEncode(clientID), System.Net.WebUtility.UrlEncode(clientSecret));

            // note, this isn't a new request -- the original was passed to beginrequeststream, so we're pulling a reference to it back out. It's the same request

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)ar.AsyncState;
            // now that we have the working request, write the request details into it
            var bytes = System.Text.Encoding.UTF8.GetBytes(strRequestDetails);
            using (System.IO.Stream postStream = request.EndGetRequestStream(ar))
            {
                postStream.Write(bytes, 0, bytes.Length);
            }
            // now that the request is good to go, let's post it to the server
            // and get the response. When done, the async callback will call the
            // GetResponseCallback function
            request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
        }

        private void GetResponseCallback(IAsyncResult ar)
        {
            // STEP 3: Process the response callback to get the token
            // we'll then use that token to call the translator service
            // Pull the request out of the IAsynch result
            var request = (System.Net.HttpWebRequest)ar.AsyncState;
            // The request now has the response details in it (because we've called back having gotten the response
            var response = (System.Net.HttpWebResponse)request.EndGetResponse(ar);
            // Using JSON we'll pull the response details out, and load it into an AdmAccess token class (defined earlier in this module)
            // IMPORTANT (and vague) -- despite the name, in Windows Phone, this is in the System.ServiceModel.Web library,
            // and not the System.Runtime.Serialization one -- so you will need to have a reference to System.ServiceModel.Web
            try
            {
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(AdmAccessToken));
                var token = (AdmAccessToken)serializer.ReadObject(response.GetResponseStream());

                var uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + System.Net.WebUtility.UrlEncode(_textToTranslate) + "&from=en&to=" + _targetLanguageCode;
                var translationWebRequest = System.Net.HttpWebRequest.Create(uri);
                // The authorization header needs to be "Bearer" + " " + the access token
                var headerValue = "Bearer " + token.access_token;
                translationWebRequest.Headers["Authorization"] = headerValue;
                // And now we call the service. When the translation is complete, we'll get the callback
                IAsyncResult writeRequestStreamCallback = (IAsyncResult)translationWebRequest.BeginGetResponse(new AsyncCallback(TranslationReady), translationWebRequest);
            }
            catch (Exception ex)
            {
                // Do nothing
            }
        }

        internal async void TranslationReady(IAsyncResult ar)
        {
            // STEP 4: Process the translation
            // Get the request details
            var request = (System.Net.HttpWebRequest)ar.AsyncState;
            // Get the response details
            var response = (System.Net.HttpWebResponse)request.EndGetResponse(ar);
            // Read the contents of the response into a string
            var streamResponse = response.GetResponseStream();
            var streamRead = new System.IO.StreamReader(streamResponse);
            var responseString = streamRead.ReadToEnd();
            // Translator returns XML, so load it into an XDocument
            var xTranslation = System.Xml.Linq.XDocument.Parse(responseString);
            var strTest = xTranslation.Root.FirstNode.ToString();

            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                TranslatedText = strTest;

            });
        }

    }
}
