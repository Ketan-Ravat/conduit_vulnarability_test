using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using iTextSharp.text.log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Jarvis.Shared.Utility
{
    public static class AssetInspectionReportLamda
    {
        private static Logger _logger;

        public static async Task GenerateReport(string awsAccessKey, string awsSecretKey, Guid report_id, Logger logger)
        {
            try
            {
                _logger = logger;

                AmazonLambdaClient client = new AmazonLambdaClient(awsAccessKey, awsSecretKey, RegionEndpoint.USWest2);

                ReportRequestModel requestModel = new ReportRequestModel();
                requestModel.report_id = report_id;

                string jsonString = JsonSerializer.Serialize(requestModel);

                var SqsConfig = new AmazonSQSConfig
                {
                    //ServiceURL = "https://sqs.us-west-2.amazonaws.com"
                    ServiceURL = ConfigurationManager.AppSettings["SQS_Service_Base_Url"]
                };
                var SqsClient = new AmazonSQSClient(awsAccessKey, awsSecretKey, SqsConfig);

                var QueueUrl = ConfigurationManager.AppSettings["SQS_Service_Url"];

                SendMessageRequest sendMessageRequest = new SendMessageRequest
                {
                    QueueUrl = QueueUrl,
                    MessageBody = jsonString
                };

                var response = await SqsClient.SendMessageAsync(sendMessageRequest).ConfigureAwait(false);


                //var ir = new Amazon.Lambda.Model.InvokeRequest
                //{
                //    FunctionName = "si-lambda-ac-report-qa-oregon",
                //    Payload = jsonString
                //};


                //InvokeResponse response = await client.InvokeAsync(ir).ConfigureAwait(false);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("Asset Inspection Report SQS Service Call Successfully");
                }
                else
                {
                    _logger.LogInformation("Error in call Asset Inspection Report SQS Service");
                }
                //var sr = new StreamReader(response.Payload);
                //JsonReader reader = new JsonTextReader(sr);

                //var serilizer = new JsonSerializer();
                //var op = serilizer.Deserialize(reader);

                //Console.WriteLine(op);
                //Console.ReadLine();
            }
            catch (Exception e)
            {
                _logger.LogInformation("Exception in call Asset Inspection Report SQS Service" + e.Message);
            }
        }

        public class ReportRequestModel
        {
            public Guid report_id { get; set; }
        }
    }
}
