using Jarvis.Shared.Helper;
using Jarvis.ViewModels;
using Jarvis.ViewModels.ViewModels;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Jarvis.ViewModels.RequestResponseViewModel;
using System.Linq;

namespace Jarvis.Shared.Utility {
    public static class SendEmail {
        private static String Email, SendGrid_Email, Password, HostName, LoginUrl;
        private static int PortNumber;
        private static Logger _logger;
        public static MimeMessage prepareMimeMessage(String subject, String htmlBody)
        {
            MimeMessage message = new MimeMessage();
            MailboxAddress from = new MailboxAddress("Sensaii CE AssetCare", ConfigurationManager.AppSettings["Email"]);
            message.From.Add(from);
            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlBody;
            message.Subject = subject;
            message.Body = bodyBuilder.ToMessageBody();
            return message;
        }

        //public SendEmail()
        //{
        //    Email = ConfigurationManager.AppSettings["Email"];
        //    Password = ConfigurationManager.AppSettings["Password"];
        //    PortNumber = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
        //    HostName = ConfigurationManager.AppSettings["SmtpHost"];
        //    LoginUrl = ConfigurationManager.AppSettings["LoginURL"];
        //    _logger = Logger.GetInstance<SendEmail>();
        //}

        public static void GetEmailSettings()
        {

            Email = ConfigurationManager.AppSettings["Email"];
            SendGrid_Email = ConfigurationManager.AppSettings["SendGrid_Email"];
            Password = ConfigurationManager.AppSettings["Password"];
            PortNumber = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
            HostName = ConfigurationManager.AppSettings["SmtpHost"];
        }

        public static void SendReserPasswordEmail(List<string> email, string callbackurl, Logger logger)
        {
            try
            {
                _logger = logger;
                BodyBuilder bodyBuilder = new BodyBuilder();
                string htmlbody = string.Empty;

                var subject = "Reset Password";
                string htmlContent = PrepareResetPasswordEmail(callbackurl);

                email.ForEach(x =>
                {
                    Thread thread = new Thread(() => SendGridEmail(x, subject, htmlContent));
                    thread.Start();
                });
            }
            catch (Exception e)
            {
                ApiException apiException = new ApiException(e);
                var jsonString = JsonConvert.SerializeObject(apiException);
                _logger.LogWarning("Exception in Prepare Email: " + jsonString);
            }
        }

        public static string PrepareResetPasswordEmail(string callbackurl)
        {
            string htmlbody = string.Empty;
            BodyBuilder bodyBuilder = new BodyBuilder();
            using (StreamReader SourceReader = System.IO.File.OpenText(UrlGenerator.GetResetPasswordTempDirectory()))
            {
                bodyBuilder.HtmlBody = SourceReader.ReadToEnd();
            }
            bodyBuilder.HtmlBody = bodyBuilder.HtmlBody.Replace("{", "{{").Replace("}", "}}");
            bodyBuilder.HtmlBody = bodyBuilder.HtmlBody.Replace("{{{{{{", "{").Replace("}}}}}}", "}");
            StringBuilder builder = new StringBuilder();
            htmlbody = string.Format(@bodyBuilder.HtmlBody,
                callbackurl,
                ConfigurationManager.AppSettings["BaseURL"],
                builder
            );

            return htmlbody;
        }

        public static async Task SendGridEmail(string email, string subject, string htmlContent)
        {
            try
            {
                GetEmailSettings();
                var apiKey = ConfigurationManager.AppSettings["SendGrid_API_KEY"];
                //var apiKey = "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";
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(SendGrid_Email, "Conduit");
                var to = new EmailAddress(email);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
                _logger.LogInformation("email send " + subject + " : " + email + ": " + response.StatusCode);
            }
            catch (Exception e)
            {
                ApiException apiException = new ApiException(e);
                var jsonString = JsonConvert.SerializeObject(apiException);
                _logger.LogWarning("Exception in Send Email: " + jsonString);
            }
        }
        public static async Task<string> SendGridEmailToAddNotification(string email, string subject, string htmlContent)
        {
            try
            {
                GetEmailSettings();
                var apiKey = ConfigurationManager.AppSettings["SendGrid_API_KEY"];
                var GroupID = ConfigurationManager.AppSettings["Unsubscribe_Group_ID"];
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(SendGrid_Email, "CE AssetCare");
                var to = new EmailAddress(email);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
                msg.SetAsm(Convert.ToInt32(GroupID));
                msg.Asm.GroupId = Convert.ToInt32(GroupID);
                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
                //_logger.LogInformation("email send " + subject + " : " + email + ": " + response.StatusCode.ToString());
                return response.StatusCode.ToString();
            }
            catch (Exception e)
            {
                ApiException apiException = new ApiException(e);
                var jsonString = JsonConvert.SerializeObject(apiException);
                _logger.LogWarning("Exception in Send mail: " + jsonString);
                return "Error";
            }
        }
        public static async Task<string> SendGridEmailWithTemplate(string email, string subject, object templateData, string templateId, Stream memoryStream = null, string filename = null)
        {
            try
            {
                var jsonobjString = JsonConvert.SerializeObject(templateData);
                GetEmailSettings();
                var apiKey = ConfigurationManager.AppSettings["SendGrid_API_KEY"];
                //var GroupID = ConfigurationManager.AppSettings["Unsubscribe_Group_ID"];
                //var apiKey = "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";
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(SendGrid_Email, "Egalvanic");
                var to = new EmailAddress(email);
                var msg = new SendGridMessage();
                //var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
                msg.SetFrom(from);
                msg.AddTo(to);
                //msg.SetAsm(Convert.ToInt32(GroupID));
                //msg.Asm.GroupId = Convert.ToInt32(GroupID);
                msg.Subject = subject;
                //msg.AddSubstitution("asmGroupUnsubscribeUrl", "<%asm_group_unsubscribe_url%>");
                msg.SetTemplateId(templateId);
                msg.SetTemplateData(templateData);
                if (memoryStream != null)
                {
                    await msg.AddAttachmentAsync(filename, memoryStream);
                }
                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
                //_logger.LogInformation("email send " + subject + " : " + email + ": " + response.StatusCode);
                var statuscode = response.StatusCode;
                var Status = response.StatusCode.ToString();
                return response.StatusCode.ToString();
            }
            catch (Exception e)
            {
                ApiException apiException = new ApiException(e);
                var jsonString = JsonConvert.SerializeObject(apiException);
                _logger.LogWarning("Exception in Send Email: " + jsonString);
                return "Error";
            }
        }
        //public static void PendingInspection(List<ManagerPendingInspectionResponseModel> users, Logger logger)
        //{
        //    try
        //    {
        //        _logger = logger;
        //        BodyBuilder bodyBuilder = new BodyBuilder();
        //        string htmlbody = string.Empty;
        //        string callbackUrl = ConfigurationManager.AppSettings["LoginURL"];

        //        using (StreamReader SourceReader = System.IO.File.OpenText(UrlGenerator.GetPendingInspectionTempDirectory()))
        //        {
        //            bodyBuilder.HtmlBody = SourceReader.ReadToEnd();
        //        }

        //        bodyBuilder.HtmlBody = bodyBuilder.HtmlBody.Replace("{", "{{").Replace("}", "}}");
        //        bodyBuilder.HtmlBody = bodyBuilder.HtmlBody.Replace("{{{{{{", "{").Replace("}}}}}}", "}");

        //        users.ForEach(x =>
        //        {
        //            List<string> inspectiontabel = new List<string>();
        //            x.inspections.ForEach(y =>
        //            {
        //                string str = @"<tr>
        //                            <td style = ""font-family:'Open Sans', sans-serif; color: #666;  line-height: 18px;  vertical-align: top;""class=""article"">
        //                                " + y.internal_asset_id + @"
        //                            </td>
        //                            <td style = ""font-family: 'Open Sans', sans-serif; color: #666;  line-height: 18px;  vertical-align: top;"" class=""article"">
        //                                " + y.asset_name + @"
        //                            </td>
        //                            <td style = ""font -family: 'Open Sans', sans-serif; color: #666;  line-height: 18px;  vertical-align: top;"" class=""article"">
        //                                " + y.meter_hours + @"
        //                            </td>
        //                            <td style = ""font -family: 'Open Sans', sans-serif; color: #666;  line-height: 18px;  vertical-align: top;"" class=""article"">
        //                                " + y.time_elapsed + @"
        //                            </td>
        //                            <td style = ""font-family: 'Open Sans', sans-serif; color: #666;  line-height: 18px;  vertical-align: top;"" class=""article"">
        //                                " + y.operator_name + @"
        //                            </td>
        //                        </tr>";
        //                inspectiontabel.Add(str);
        //            });
        //            StringBuilder builder = new StringBuilder();
        //            foreach (string table in inspectiontabel)
        //            {
        //                // Append each int to the StringBuilder overload.
        //                builder.Append(table).Append(" ");
        //            }
        //            htmlbody = string.Format(@bodyBuilder.HtmlBody,
        //                callbackUrl,
        //                builder
        //            );
        //            string subject = x.inspections.Count == 1 ? "1 Pending Inspection Review" : x.inspections.Count.ToString() + " Pending Inspection Reviews";
        //            //var message = SendEmail.prepareMimeMessage(subject, htmlbody);
        //            if (!String.IsNullOrEmpty(x.email))
        //            {
        //                Thread th1 = new Thread(() => SendGridEmail(x.email, subject, htmlbody));
        //                th1.Start();
        //            }
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        ApiException apiException = new ApiException(e);
        //        var jsonString = JsonConvert.SerializeObject(apiException);
        //        _logger.LogWarning("Exception in Prepare Email: " + jsonString);
        //    }
        //}

        public static void SendEmails(string toemail, MimeMessage message)
        {
            try
            {
                GetEmailSettings();
                MailboxAddress to = new MailboxAddress("User", toemail);
                message.To.Add(to);
                SmtpClient client = new SmtpClient();
                //var cts = new CancellationTokenSource();
                //client.Timeout = 1800000;
                client.Connect(HostName, PortNumber, false);
                client.Authenticate(Email, Password);
                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
            }
            catch (Exception e)
            {
                ApiException apiException = new ApiException(e);
                var jsonString = JsonConvert.SerializeObject(apiException);
                _logger.LogWarning("Exception in Send Email: " + jsonString);
            }
        }

        //public static async Task OperatorUsageReport(List<OperatorUsageWeeklyReportResponseModel> users, Logger logger)
        //{
        //    try
        //    {
        //        _logger = logger;
        //        BodyBuilder bodyBuilder = new BodyBuilder();
        //        string htmlbody = string.Empty;
        //        string callbackUrl = ConfigurationManager.AppSettings["LoginURL"];
        //        foreach(var x in users)
        //        {
        //            List<string> sitetablerows = new List<string>();
        //            x.SiteWiseReports.ForEach(y =>
        //            {
        //                if (y.operatorWiseReports?.Count > 0)
        //                {
        //                    y.operatorWiseReports.ForEach(op =>
        //                    {
        //                        if (op.OperatorUsage?.Count > 0)
        //                        {
        //                            var count = 0;
        //                            op.OperatorUsage.ForEach(ins => {
        //                                if (count == 0)
        //                                {
        //                                    ins.isFirst = true;
        //                                    ins.totalCount = op.OperatorUsage.Count;
        //                                }
        //                                count++;
        //                            });
        //                        }
        //                    });
        //                }
        //            });
        //            string subject = "Operator Usage Report Email - From " + x.fromdate + " To " + x.todate + "";
        //            if (!String.IsNullOrEmpty(x.email_id))
        //            {
        //                x.callbackUrl = callbackUrl;
        //                var response = await SendGridEmailWithTemplate(x.email_id, subject, x);
        //            }
        //        }


        //        users.ForEach(x =>
        //        {
        //            List<string> sitetablerows = new List<string>();
        //            x.SiteWiseReports.ForEach(y =>
        //            {
        //                string sitedetails = @"<tr>
        //                        <td>
        //                            <table width=""700"" border=""0"" align=""center"" class=""fullPadding"" style=""border-collapse: collapse;"">
        //                                <tr>
        //                                    <td style=""color: #333; font-family: 'Open Sans', sans-serif; font-size: 14px; font-weight: 600;"">
        //                                        <p style=""margin: 0;"">Site Name: " + y.site_name + @"</p>
        //                                    </td>
        //                                </tr>
        //                                <tr>
        //                                    <td height=""10px""></td>
        //                                </tr>
        //                            </table>
        //                        </td>
        //                    </tr>";
        //                sitetablerows.Add(sitedetails);
        //                if (y.operatorWiseReports?.Count > 0)
        //                {
        //                    string operatorusage = @"<tr>
        //                        <td>
        //                            <table width=""700"" border=""1"" cellpadding=""10"" cellspacing=""10"" align=""center"" class=""fullPadding"" style=""border-collapse: collapse; margin: 0 auto 30px;"">
        //                                <tbody>
        //                                    <tr>
        //                                        <th style=""font-size: 14px; font-family: 'Open Sans', sans-serif; color: #333;  line-height: 1; width:100px;"" align=""center"">
        //                                            Operator
        //                                        </th>
        //                                        <th style=""font-size: 14px; font-family: 'Open Sans', sans-serif; color: #333;  line-height: 1;"" align=""center"">
        //                                            Equipment Inspected
        //                                        </th>
        //                                        <th style=""font-size: 14px; font-family: 'Open Sans', sans-serif; color: #333;  line-height: 1;"" align=""center"">
        //                                            Recent Inspection on equipment
        //                                        </th>
        //                                        <th style=""font-size: 14px; font-family: 'Open Sans', sans-serif; color: #333; line-height: 1;"" align=""center"">
        //                                            Total Inspections on equipment
        //                                        </th>
        //                                    </tr>";
        //                    y.operatorWiseReports.ForEach(op =>
        //                    {
        //                        if (op.OperatorUsage?.list?.Count > 0)
        //                        {
        //                            var count = 0;
        //                            op.OperatorUsage.list.ForEach(ins =>
        //                            {
        //                                if (count == 0)
        //                                {
        //                                    operatorusage = operatorusage + @"<tr>
        //                                        <td style=""font-size: 14px; font-family: 'Open Sans', sans-serif; font-weight: 400;"" rowspan=""" + op.OperatorUsage.list.Count + @""">" +
        //                                        ins.operator_name + @"</td>
        //                                        <td style=""font-size: 14px; font-family: 'Open Sans', sans-serif; font-weight: 400;"">" +
        //                                        ins.asset_name + @"</td>
        //                                        <td style=""font-size: 14px; font-family: 'Open Sans', sans-serif; font-weight: 400;"">" +
        //                                        ins.lastinspectiondate + @"</td>
        //                                        <td align=""right"" style=""font-size: 14px; font-family: 'Open Sans', sans-serif; font-weight: 400;"">" +
        //                                        ins.totalinspection + @"</td>
        //                                    </tr>";
        //                                }
        //                                else
        //                                {
        //                                    operatorusage = operatorusage + @"<tr>
        //                                        <td style=""font-size: 14px; font-family: 'Open Sans', sans-serif; font-weight: 400;"">" +
        //                                        ins.asset_name + @"</td>
        //                                        <td style=""font-size: 14px; font-family: 'Open Sans', sans-serif; font-weight: 400;"">" +
        //                                        ins.lastinspectiondate + @"</td>
        //                                        <td align=""right"" style=""font-size: 14px; font-family: 'Open Sans', sans-serif; font-weight: 400;"">" +
        //                                        ins.totalinspection + @"</td>
        //                                    </tr>";
        //                                }
        //                                count++;
        //                            });
        //                        }

        //                    });
        //                    operatorusage = operatorusage + @"</tbody>
        //                                            </table>
        //                                        </td>
        //                                    </tr>";
        //                    sitetablerows.Add(operatorusage);
        //                }
        //                if (y.OperatorWithoutInspectionList?.Count > 0)
        //                {
        //                    var operatorsNameList = String.Join(", ", y.OperatorWithoutInspectionList.Select(x => x.operator_fname + " " + x.operator_lname));
        //                    string withoutInspectionOps = @"<tr>
        //                        <td>
        //                            <table width=""700"" border=""1"" cellpadding=""10"" cellspacing=""10"" align=""center"" class=""fullPadding"" style=""border-collapse: collapse; margin: 0 auto 30px;"">
        //                                <tbody>
        //                                    <tr>
        //                                        <th style=""font-size: 14px; font-family: 'Open Sans', sans-serif; color: #333;  line-height: 1; vertical-align: top;"" align=""left"">
        //                                            Operators who did not perform any inspections
        //                                        </th>
        //                                    </tr>
        //                                    <tr>
        //                                        <td style=""font-size: 14px; font-family: 'Open Sans', sans-serif; font-weight: 400;"">" + operatorsNameList + "</td></tr>";
        //                    //y.OperatorWithoutInsoection.ForEach(withoutins =>
        //                    //{
        //                    //    withoutInspectionOps = withoutInspectionOps + @"<tr>
        //                    //                    <td style=""font-size: 14px; font-family: 'Open Sans', sans-serif; font-weight: 400;"">"
        //                    //                       +withoutins.operator_fname+" "+withoutins.operator_lname+@"</td>
        //                    //                </tr>";
        //                    //});
        //                    withoutInspectionOps = withoutInspectionOps + @"</tbody>
        //                            </table>
        //                        </td>
        //                    </tr>";
        //                    sitetablerows.Add(withoutInspectionOps);
        //                }
        //            });
        //            StringBuilder builder = new StringBuilder();
        //            foreach (string table in sitetablerows)
        //            {
        //                // Append each int to the StringBuilder overload.
        //                builder.Append(table).Append(" ");
        //            }
        //            htmlbody = string.Format(@bodyBuilder.HtmlBody,
        //                x.manager_name,
        //                callbackUrl,
        //                builder
        //            );
        //            string subject = "Operator Usage Report Email - From " + x.fromdate + " To " + x.todate + "";
        //            if (!String.IsNullOrEmpty(x.email_id))
        //            {
        //                SendGridEmail(x.email_id, subject, htmlbody);
        //            }
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        ApiException apiException = new ApiException(e);
        //        var jsonString = JsonConvert.SerializeObject(apiException);
        //        _logger.LogWarning("Exception in Prepare Email: " + jsonString);
        //    }
        //}
    }
}
