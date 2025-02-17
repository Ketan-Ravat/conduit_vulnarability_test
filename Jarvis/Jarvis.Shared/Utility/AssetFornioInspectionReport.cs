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
    public static class AssetFornioInspectionReport
    {
        private static Logger _logger;

        public static async Task GenerateReport(string awsAccessKey, string awsSecretKey, string  request_model, Logger logger)
        {
            try
            {
                _logger = logger;

                AmazonLambdaClient client = new AmazonLambdaClient(awsAccessKey, awsSecretKey, RegionEndpoint.USWest2);

               // ReportRequestModel requestModel = new ReportRequestModel();
               // requestModel.report_id = report_id;

                string jsonString = request_model;

                var SqsConfig = new AmazonSQSConfig
                {
                    //ServiceURL = "https://sqs.us-west-2.amazonaws.com"
                    ServiceURL = ConfigurationManager.AppSettings["Formio_SQS_Service_Base_Url"]
                };
                var SqsClient = new AmazonSQSClient(awsAccessKey, awsSecretKey, SqsConfig);

                //var QueueUrl = ConfigurationManager.AppSettings["Formio_SQS_Service_Url"];// old lambda and SQS where custom python was used to create a report
                var QueueUrl = ConfigurationManager.AppSettings["jinja_formio_report_SQS_Service_Url"];// new lambda and SQS where jinja library is used to create report

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

        public static async Task GenerateIRWOReport(string awsAccessKey, string awsSecretKey, string request_model, Logger logger)
        {
            try
            {
                _logger = logger;

                AmazonLambdaClient client = new AmazonLambdaClient(awsAccessKey, awsSecretKey, RegionEndpoint.USWest2);

                // ReportRequestModel requestModel = new ReportRequestModel();
                // requestModel.report_id = report_id;

                string jsonString = request_model;

                var SqsConfig = new AmazonSQSConfig
                {
                    //ServiceURL = "https://sqs.us-west-2.amazonaws.com"
                    ServiceURL = ConfigurationManager.AppSettings["Formio_SQS_Service_Base_Url"]
                };
                var SqsClient = new AmazonSQSClient(awsAccessKey, awsSecretKey, SqsConfig);

                var QueueUrl = ConfigurationManager.AppSettings["IR_WO_Formio_SQS_Service_Url"];

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
                    _logger.LogInformation("IR WO Inspection Report SQS Service Call Successfully");
                }
                else
                {
                    _logger.LogInformation("Error in call IR WO Inspection Report SQS Service");
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

        public static async Task GenerateMWOReport(string awsAccessKey, string awsSecretKey, string request_model, Logger logger)
        {
            try
            {
                _logger = logger;

                AmazonLambdaClient client = new AmazonLambdaClient(awsAccessKey, awsSecretKey, RegionEndpoint.USWest2);

                // ReportRequestModel requestModel = new ReportRequestModel();
                // requestModel.report_id = report_id;

                string jsonString = request_model;

                var SqsConfig = new AmazonSQSConfig
                {
                    //ServiceURL = "https://sqs.us-west-2.amazonaws.com"
                    ServiceURL = ConfigurationManager.AppSettings["Formio_SQS_Service_Base_Url"]
                };
                var SqsClient = new AmazonSQSClient(awsAccessKey, awsSecretKey, SqsConfig);

                var QueueUrl = ConfigurationManager.AppSettings["MWO_Report_SQS_Service_Url"];

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
                    _logger.LogInformation("IR WO Inspection Report SQS Service Call Successfully");
                }
                else
                {
                    _logger.LogInformation("Error in call IR WO Inspection Report SQS Service");
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

        public static async Task GenerateOBWOReport(string awsAccessKey, string awsSecretKey, string request_model, Logger logger)
        {
            try
            {
                _logger = logger;

                AmazonLambdaClient client = new AmazonLambdaClient(awsAccessKey, awsSecretKey, RegionEndpoint.USWest2);

                // ReportRequestModel requestModel = new ReportRequestModel();
                // requestModel.report_id = report_id;

                string jsonString = request_model;

                var SqsConfig = new AmazonSQSConfig
                {
                    //ServiceURL = "https://sqs.us-west-2.amazonaws.com"
                    ServiceURL = ConfigurationManager.AppSettings["Formio_SQS_Service_Base_Url"]
                };
                var SqsClient = new AmazonSQSClient(awsAccessKey, awsSecretKey, SqsConfig);

                var QueueUrl = ConfigurationManager.AppSettings["OBWO_Report_SQS_Service_Url"];

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
                    _logger.LogInformation("IR WO Inspection Report SQS Service Call Successfully");
                }
                else
                {
                    _logger.LogInformation("Error in call IR WO Inspection Report SQS Service");
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

        public static async Task BulkImportAssetFormIO(string awsAccessKey, string awsSecretKey, string request_model, Logger logger)
        {
            try
            {
                _logger = logger;

                AmazonLambdaClient client = new AmazonLambdaClient(awsAccessKey, awsSecretKey, RegionEndpoint.USWest2);

                // ReportRequestModel requestModel = new ReportRequestModel();
                // requestModel.report_id = report_id;

                string jsonString = request_model;

                var SqsConfig = new AmazonSQSConfig
                {
                    //ServiceURL = "https://sqs.us-west-2.amazonaws.com"
                    ServiceURL = ConfigurationManager.AppSettings["Formio_SQS_Service_Base_Url"]
                };
                var SqsClient = new AmazonSQSClient(awsAccessKey, awsSecretKey, SqsConfig);

                var QueueUrl = ConfigurationManager.AppSettings["Bulk_Import_AssetFormIO_SQS_Service_Url"];

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
                    _logger.LogInformation("IR WO Inspection Report SQS Service Call Successfully");
                }
                else
                {
                    _logger.LogInformation("Error in call IR WO Inspection Report SQS Service");
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

        public static async Task UpdateWOOffline(string awsAccessKey, string awsSecretKey, string request_model, Logger logger)
        {
            try
            {
                _logger = logger;

                AmazonLambdaClient client = new AmazonLambdaClient(awsAccessKey, awsSecretKey, RegionEndpoint.USEast2);

                // ReportRequestModel requestModel = new ReportRequestModel();
                // requestModel.report_id = report_id;

                string jsonString = request_model;

                var SqsConfig = new AmazonSQSConfig
                {
                    //ServiceURL = "https://sqs.us-west-2.amazonaws.com"
                    ServiceURL = ConfigurationManager.AppSettings["Formio_SQS_Service_Base_Url"]
                };
                var SqsClient = new AmazonSQSClient(awsAccessKey, awsSecretKey, SqsConfig);

                var QueueUrl = ConfigurationManager.AppSettings["Offlline_wo_Offline_SQS_Service_Url"];

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
                    _logger.LogInformation("IR WO Inspection Report SQS Service Call Successfully");
                }
                else
                {
                    _logger.LogInformation("Error in call IR WO Inspection Report SQS Service");
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

        public static async Task GenerateReportForBulkNetaReport(string awsAccessKey, string awsSecretKey, string request_model, Logger logger)
        {
            try
            {
                _logger = logger;

                AmazonLambdaClient client = new AmazonLambdaClient(awsAccessKey, awsSecretKey, RegionEndpoint.USWest2);

                // ReportRequestModel requestModel = new ReportRequestModel();
                // requestModel.report_id = report_id;

                string jsonString = request_model;

                var SqsConfig = new AmazonSQSConfig
                {
                    //ServiceURL = "https://sqs.us-west-2.amazonaws.com"
                    ServiceURL = ConfigurationManager.AppSettings["Formio_SQS_Service_Base_Url"]
                };
                var SqsClient = new AmazonSQSClient(awsAccessKey, awsSecretKey, SqsConfig);

                //var QueueUrl = ConfigurationManager.AppSettings["Formio_SQS_Service_Url"];// old lambda and SQS where custom python was used to create a report
                var QueueUrl = ConfigurationManager.AppSettings["conduit-dev-bulk-neta-inspection"];// new lambda and SQS where jinja library is used to create report

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


        public static async Task GenerateVisualImageFromFLIRCamera(string awsAccessKey, string awsSecretKey, string request_model, Logger logger)
        {
            try
            {
                _logger = logger;

                AmazonLambdaClient client = new AmazonLambdaClient(awsAccessKey, awsSecretKey, RegionEndpoint.USWest2);

                // ReportRequestModel requestModel = new ReportRequestModel();
                // requestModel.report_id = report_id;

                string jsonString = request_model;

                var SqsConfig = new AmazonSQSConfig
                {
                    //ServiceURL = "https://sqs.us-west-2.amazonaws.com"
                    ServiceURL = ConfigurationManager.AppSettings["Formio_SQS_Service_Base_Url"]
                };
                var SqsClient = new AmazonSQSClient(awsAccessKey, awsSecretKey, SqsConfig);

                //var QueueUrl = ConfigurationManager.AppSettings["Formio_SQS_Service_Url"];// old lambda and SQS where custom python was used to create a report
                var QueueUrl = ConfigurationManager.AppSettings["conduit-visual-image-generator-dev-ohio"];// new lambda and SQS where jinja library is used to create report

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
