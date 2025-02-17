using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace Jarvis.Shared.Helper
{
    public class UrlGenerator
    {
        public static List<string> GetInspectionImagesURL(List<string> imagename)
        {
            List<string> UrlGeneratedImages = new List<string>();
            foreach (var item in imagename)
            {
                var image = string.Concat("https://s3-us-east-2.amazonaws.com",
                                   "/", "conduit-prod.inspectionimages",
               "/", item);
                UrlGeneratedImages.Add(image);
            }
            return UrlGeneratedImages;
        }
        public static List<string> GetInspectionThumbnailImagesURL(List<string> imagename)
        {
            List<string> UrlGeneratedImages = new List<string>();
            foreach (var item in imagename)
            {
                var image = string.Concat("https://s3-us-east-2.amazonaws.com",
                                "/", "conduit-prod.inspectionimages/thumbnails",
               "/", item);
                UrlGeneratedImages.Add(image);
            }
            return UrlGeneratedImages;
        }
        public static string GetAssetBarcodeImagesURL(string imagename)
        {


            var image = string.Concat("https://s3-us-east-2.amazonaws.com/",
                           "/", "conduit-prod.barcodesimages",
           "/", imagename);

            return image;
        }

        public static string GetEmailTempalateDirectory()
        {
            return Directory.GetCurrentDirectory() + @"/EmailTemplete/";
        }

        public static string GetPendingInspectionTempDirectory()
        {
            return GetEmailTempalateDirectory() + "pending-inspection-template.html";
        }

        public static string GetResetPasswordTempDirectory()
        {
            return GetEmailTempalateDirectory() + "reset-password-template.html";
        }

        public static string GetAssetImagesURL(string imagename)
        {
            var image = string.Concat("https://s3-us-east-2.amazonaws.com/",
                            "/", "conduit-prod.assetimages",
           "/", imagename);

            return image;
        }

        public static string GetAsconduitsetImagesURL(string imagename)
        {
            var image = string.Concat("https://s3-us-east-2.amazonaws.com/",
                            "/", "conduit-prod.assetimages",
           "/", imagename);

            return image;
        }
        public static string GetIssueImagesURL(string imagename)
        {
            var image = string.Concat("https://s3-us-east-2.amazonaws.com/"
                               , "conduit-prod-issuephotos",
           "/", imagename);

            return image;
        }
        public static string GetIRImagesURL(string imagename, string folder_name)
        {
            string image = null;
            if (!String.IsNullOrEmpty(imagename))
            {
                image = string.Concat("https://s3-us-east-2.amazonaws.com/",
                                   "/", "conduit-prod.obwoirphotos",
                     "/", folder_name, "/", imagename);
            }
            //if (!String.IsNullOrEmpty(imagename))
            //{
            //    image = string.Concat("https://",
            //                        "conduit-prod.obwoirphotos", "s3-us-east-2.amazonaws.com",
            //         "/", folder_name, "/", imagename);
            //}
            return image;
        }
        public static string GetFormIOPDFUrl(string pdf_name, string folder_name)
        {
            string image = null;
            if (!String.IsNullOrEmpty(pdf_name))
            {
                image = string.Concat("https://s3-us-east-2.amazonaws.com/",
                     "/", "formio-pdf-bucket",
                 "/", folder_name, "/", pdf_name);
            }
            return image;
        }
        public static string GetOBAssetPdfUrl(string pdf_name, string folder_name)
        {
            string image = null;
            if (!String.IsNullOrEmpty(pdf_name))
            {
                image = string.Concat("https://s3-us-east-2.amazonaws.com/",
                                   "/", "prod-wo-ob-asset-pdf-lambda-python",
                     "/", folder_name, "/", pdf_name);
            }
            return image;
        }
        public static string GetFEChangePasswordUrl(string token)
        {
            var url = string.Concat("https://app.qa.sensaii.com",
            "/", "accountrecovery",
            "/", token);

            return url;
        }

        public static string GetOperatorUsageReportTempDirectory()
        {
            return GetEmailTempalateDirectory() + "operator-usage-weekly-report.html";
        }

        public static string GetPMAttachmentURL(string filename)
        {
            var file = string.Concat("https://s3-us-east-2.amazonaws.com/",
                            "/", "conduit-prod.pmattachments",
           "/", filename);

            return file;
        }

        public static string GetWorkOrderAttachmentURL(string filename)
        {
            var file = string.Concat("https://s3-us-east-2.amazonaws.com/",
                          "/", "conduit-prod.workorderattachments",
               "/", filename);
            return file;
        }

        public static string GetAssetAttachmentURL(string filename)
        {
            var file = string.Concat("https://s3-us-east-2.amazonaws.com",
                          "/", "conduit-prod-assetattachments",
           "/", filename);
            return file;
        }

        public static string GetClusterOnelinePdfURL(string filename)
        {
            var myfile = string.Concat("https://s3-us-east-2.amazonaws.com/",
                              "/", "conduit-prod-digital-oneline-pdf",
           "/", filename);  

            return myfile;
        }
        public static string GetAssetDetailsURL(Guid assetId,string domain_name)
        {
            //var myAsset = string.Concat("https://", domain_name, ConfigurationManager.AppSettings["WebAppDomainForMail"]
            //         , "/assets/details", "/", assetId.ToString());

            var myAsset = string.Concat( domain_name , "assets/details", "/", assetId.ToString());

            return myAsset;
        }


        public static string GetProfilePictureURL(string imagename)
        {
            var image = string.Concat("https://s3-us-east-2.amazonaws.com/",
                               "/", "conduit-prod-userprofile",
           "/", imagename);

            return image;
        }
        public static string GetAssetNameplateImageURL(string imagename)
        {
            var image = string.Concat("https://s3-us-east-2.amazonaws.com/",
                            "/", "conduit-prod.assetimages",
           "/", imagename);

            return image;
        }
        public static string GetAssetClassJinjaTemplateURL(string filename,string bucket_name)
        {
            //https://conduit-jinja-report-templates.s3.us-east-2.amazonaws.com/PITR_old.docx

            var url = string.Concat("https://", bucket_name, ".s3.us-east-2.amazonaws.com", "/", filename);
            return url;
        }
        public static string GetSiteDocumentUrl(string document_name, string folder_name)
        {
            string image = null;
            if (!String.IsNullOrEmpty(document_name))
            {
                image = string.Concat("https://s3-us-east-2.amazonaws.com/",
                                    "conduit-prod-sitedocument",
                     "/", folder_name, "/", document_name);
            }
            return image;
        }
    }
}