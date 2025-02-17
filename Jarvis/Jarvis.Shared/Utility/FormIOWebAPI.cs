using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Jarvis.Shared.Helper.FormIOResponsemodel;
using Newtonsoft.Json;
using System.Security.Policy;
using System.IO;
using System.Linq;

namespace Jarvis.Shared.Utility
{
    public class FormIOWebAPI
    {
       
        public static string URL = "http://formiodev-env.eba-mmqr5ivu.us-east-2.elasticbeanstalk.com";

        public struct ApiFunciton
        {
            public struct GetURL
            {

            }
            public struct PostURL
            {
                public static readonly string UserLogin = "/formio/user/login/submission?live=1";
                public static readonly string ExporterPDF = "/pdf-proxy/pdf/641ab70ed27f8a2d584a2b84/download";
            }
        }
        public static async Task<Stream> PostApiCallWithJson<T>(string command, string  sendobj , string api_token)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    var content = sendobj;// JsonConvert.SerializeObject(sendobj);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    var tst = new StringContent(content, Encoding.Default, "application/json");
                    tst.Headers.ContentType.CharSet = "";
                    if (!String.IsNullOrEmpty(api_token))
                    {
                        tst.Headers.Add("x-jwt-token", api_token);
                    }

                    HttpResponseMessage response = await client.PostAsync(command, tst);
                    var statuscode = response.StatusCode;

                    var responseString = await response.Content.ReadAsStringAsync();
                    if (statuscode != System.Net.HttpStatusCode.OK)
                    {
                        return null;
                    }
                    else
                    {
                        var contentStream = await response.Content.ReadAsStreamAsync(); // get the actual content stream

                        return contentStream;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<string> FormIOUserLogin<T>(string command, string sendobj)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    var content = sendobj;// JsonConvert.SerializeObject(sendobj);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    var tst = new StringContent(content, Encoding.Default, "application/json");
                    tst.Headers.ContentType.CharSet = "";
                    

                    HttpResponseMessage response = await client.PostAsync(command, tst);
                    var statuscode = response.StatusCode;

                    var responseString = await response.Content.ReadAsStringAsync();
                    if (statuscode != System.Net.HttpStatusCode.OK)
                    {
                        return null;
                    }
                    else
                    {
                        HttpHeaders headers = response.Headers;
                        IEnumerable<string> values;
                        string token = null;
                        if (headers.TryGetValues("x-jwt-token", out values))
                        {
                             token = values.First();
                        }
                        return token;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
