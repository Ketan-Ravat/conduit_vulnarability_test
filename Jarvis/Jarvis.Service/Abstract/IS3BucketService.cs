using Amazon.S3.Model;
using Jarvis.db.ExcludePropertiesfromDBHelper;
using Jarvis.ViewModels.RequestResponseViewModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Service.Abstract
{
    public interface IS3BucketService
    {
        Task<List<string>> UploadImage(List<IFormFile> images, string awsAccessKey, string awsSecretKey, string bucketName);
        Task<List<string>> UploadIRPhotos(List<IFormFile> images, string awsAccessKey, string awsSecretKey, string bucketName, string order_number , string string_site_id , List<GetIRImageFilePathExclude> db_file_names,int? camera_type,int? ir_img_flag);
        Task<string> UploadAsStremForPDF(System.IO.Stream file, string awsAccessKey, string awsSecretKey, string bucketName, string filename);
        Task<UploadAssetImageResponsemodel> UploadAssetImage(List<IFormFile> images, string awsAccessKey, string awsSecretKey, string bucketName);
        Task<List<UploadAssetImageResponsemodel>> UploadAssetMultipleImage(List<IFormFile> images, string awsAccessKey, string awsSecretKey, string bucketName);
        Task<UploadAssetImageResponsemodel> UploadWOPMFormImages(List<IFormFile> images, string awsAccessKey, string awsSecretKey, string bucketName);

        Task<List<string>> UploadOfflineImage(List<IFormFile> images, string awsAccessKey, string awsSecretKey, string bucketName, int height, int width, string folderPathName);

        Task<bool> Exists(string fileKey, string bucketName, string awsAccessKey, string awsSecretKey);

        Task<string> UploadBarcode(string awsAccessKey, string awsSecretKey, Bitmap bitmap, string filename, string bucketname);

        Task<string> GetQuickSightEmbedUrl(string accessKey, string secretKey, string AwsAccountId, string DashboardId, string roleArnToAssume);

        Task<List<string>> UploadThumbNailImagesForExisting(List<string> images, string awsAccessKey, string awsSecretKey, string bucketName, string folderPathName, int height, int width);

        Task<List<string>> UploadAttachment(List<IFormFile> attachments, string awsAccessKey, string awsSecretKey, string bucketName);
        Task<List<S3UploadAssetAttachmentResponsemodel>> UploadAssetAttachment(List<IFormFile> attachments, string awsAccessKey, string awsSecretKey, string bucketName);
        Task<List<S3UploadAssetAttachmentResponsemodel>> UploadDocumentByFilename(List<IFormFile> attachments, string awsAccessKey, string awsSecretKey, string bucketName);
        public Task<DeleteObjectsResponse> deleteObjects(List<string> objectsToBeDeleted, string awsAccessKey, string awsSecretKey, string bucketName);
        Task<int> CopyImagesToAnotherBucket(string awsAccessKey, string awsSecretKey, string source_bucketName, string destination_bucketName, string image_name);

        Task<int> CopyImagesToAnotherFolderForIR(string awsAccessKey, string awsSecretKey, string source_bucketName, string destination_bucketName, string sourceFolderName, string destinationFolderName);

        Task<string> UploadClusterOneLinePdfService(IFormFile attachment, string awsAccessKey, string awsSecretKey, string bucketName, string site_id);

        Task<GetImageInfoByTextRactResponsemodel> DetectSampleAsync(string awsAccessKey, string awsSecretKey, string bucketName,string image_name, List<string> question_list);
        Task<string> UploadJsonfileAsStrem(System.IO.MemoryStream file, string awsAccessKey, string awsSecretKey, string bucketName, string filename);

        Task<List<string>> UploadmobilelogsToS3(List<IFormFile> log_file, string awsAccessKey, string awsSecretKey, string bucketName, string folder_name);
        Task<UploadAssetImageResponsemodel> UploadSiteProfileImage(List<IFormFile> images, string awsAccessKey, string awsSecretKey, string bucketName, string company_id, string site_id);
        Task<List<string>> UploadSiteDocument(List<IFormFile> attachments, string awsAccessKey, string awsSecretKey, string bucketName);
    }
}
