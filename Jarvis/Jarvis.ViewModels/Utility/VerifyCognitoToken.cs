using Jarvis.ViewModels.RequestResponseViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Jarvis.ViewModels.Utility
{
    public static class VerifyCognitoToken
    {
        public static VerifyCognitoTokenResponseModel Verify(string token, string domain_name , int app_brand)
        {
            try
            {
                VerifyCognitoTokenResponseModel response_model = new VerifyCognitoTokenResponseModel();
                //Response_Message response_model = new Response_Message();
                HttpClient client = new HttpClient();
                string BaseAddress = ConfigurationManager.AppSettings["Cognito_Base_Url"];

                // Add an Accept header for JSON format.
                //client.DefaultRequestHeaders.Accept.Add(
                //new MediaTypeWithQualityHeaderValue("application/json"));
                TokenVerificationRequestModel requestModel = new TokenVerificationRequestModel();
                requestModel.token = token;
                requestModel.app_brand = app_brand;
                requestModel.domain_name = domain_name.ToLower();

                var content = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");
                // List data response.
                HttpResponseMessage response = client.PostAsync(BaseAddress, content).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body.
                    var dataObjects = response.Content.ReadAsStringAsync().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                    response_model = JsonConvert.DeserializeObject<VerifyCognitoTokenResponseModel>(dataObjects);
                    //response_model = JsonConvert.DeserializeObject<Response_Message>(dataObjects);

                    //foreach (var d in dataObjects)
                    //{
                    //    Console.WriteLine("{0}", d.Name);
                    //}
                }
                else
                {
                    Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                }

                //Make any other calls using HttpClient here.

                //Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
                client.Dispose();
                return response_model;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
    public class TokenVerificationRequestModel
    {
        public string token { get; set; }
        public string domain_name { get; set; }
        public int app_brand { get; set; }
    }
}
