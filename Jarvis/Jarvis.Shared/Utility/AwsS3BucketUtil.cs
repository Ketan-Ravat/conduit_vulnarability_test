using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Jarvis.Shared.Utility
{
    public static class AwsS3BucketUtil
    {
        public static async Task<string> UploadBarcode(string awsAccessKey, string awsSecretKey, Bitmap bitmap, string filename, string bucketname)
        {
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };
                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {

                    var stream = new System.IO.MemoryStream();
                    bitmap.Save(stream, ImageFormat.Bmp);
                    stream.Position = 0;

                    var request = new PutObjectRequest()
                    {
                        BucketName = bucketname,
                        CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                        Key = filename,
                        InputStream = stream    //SEND THE FILE STREAM
                    };
                    var response = await client.PutObjectAsync(request);
                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                    {
                        return request.Key;
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }

        public static async Task<string> UploadBarcodePdf(string fileStream, string awsAccessKey, string awsSecretKey, string bucketName)
        {
            var result = string.Empty;
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {

                    //using (Stream fileStream = filename.OpenReadStream())
                    //{

                    //var filestream = new FileStream("abcjshdja.pdf", FileMode.Create, FileAccess.Write);
                    var filename = DateTime.UtcNow.ToString() + ".pdf";
                    var request = new PutObjectRequest()
                    {
                        BucketName = bucketName,
                        CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                        Key = fileStream,
                        FilePath = fileStream  //SEND THE FILE STREAM
                    };
                    var response = await client.PutObjectAsync(request);
                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                    {
                        //result.Add(request.Key);
                        return request.Key;
                    }
                    //}
                }
            }
            catch (Exception e)
            {
                //Logger.Log("error while upload inspection images in aws ", e.Message);
                throw e;
            }

            return result;
        }

       


        public static Stream ToStream(Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }
    }
}
