using Amazon;
using Amazon.QuickSight;
using Amazon.QuickSight.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.Textract.Model;
using Amazon.Textract;
using DocumentFormat.OpenXml.InkML;
using Jarvis.db.Models;
using Jarvis.Service.Abstract;
using Jarvis.Shared;
using Jarvis.Shared.StatusEnums;
using Jarvis.Shared.Utility;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office2016.Excel;
using Jarvis.db.ExcludePropertiesfromDBHelper;
using ImageMagick;

namespace Jarvis.Service.Concrete
{
    public class S3BucketService : IS3BucketService
    {
        public async Task<List<string>> UploadImage(List<IFormFile> images, string awsAccessKey, string awsSecretKey, string bucketName)
        {
            var result = new List<string>();
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    foreach (var image in images)
                    {
                        using (Stream fileStream = image.OpenReadStream())
                        {

                            var request = new PutObjectRequest()
                            {
                                BucketName = bucketName,
                                CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                                Key = DateTime.Now.Ticks.ToString() + Path.GetExtension(image.FileName),
                                InputStream = fileStream    //SEND THE FILE STREAM
                            };
                            var response = await client.PutObjectAsync(request);
                            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                            {
                                result.Add(request.Key);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //Logger.Log("error while upload inspection images in aws ", e.Message);
                throw e;
            }

            return result;
        }

        /*  UploadIRPhotos Changes for FLIR/FLUKE Camera type
         * 
        public async Task<List<string>> UploadIRPhotos(List<IFormFile> images, string awsAccessKey, string awsSecretKey, string bucketName, string order_number, string string_site_id, List<GetIRImageFilePathExclude> db_file_names)
        {
            var result = new List<string>();
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    foreach (var image in images)
                    {
                        using (Stream fileStream = image.OpenReadStream())
                        {
                            string file_name = string_site_id + "/" + order_number + "/" + image.FileName;
                            var get_db_s3_filepath = db_file_names.Where(x => (x.ir_image_label == image.FileName || x.visual_image_label == image.FileName)).Select(x => x.s3_image_folder_name).FirstOrDefault();
                            if (!String.IsNullOrEmpty(get_db_s3_filepath))
                            {
                                file_name = get_db_s3_filepath + "/" + image.FileName;
                            }

                            var request = new PutObjectRequest()
                            {
                                BucketName = bucketName,
                                CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                                Key = file_name,// DateTime.Now.Ticks.ToString() + Path.GetExtension(image.FileName),
                                InputStream = fileStream    //SEND THE FILE STREAM
                            };
                            var response = await client.PutObjectAsync(request);
                            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                            {
                                result.Add(request.Key);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //Logger.Log("error while upload inspection images in aws ", e.Message);
                throw e;
            }

            return result;
        }
        */

        public async Task<List<string>> UploadIRPhotos(List<IFormFile> images, string awsAccessKey, string awsSecretKey, string bucketName, string order_number, string string_site_id, List<GetIRImageFilePathExclude> db_file_names,int? camera_type, int? ir_img_flag)
        {
            var result = new List<string>();
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    foreach (var image in images)
                    {
                        string filename_str = null;
                        if (camera_type == (int)ir_visual_camera_type.FLUKE && ir_img_flag == (int)ir_visual_image_type.Visual_Image_Only)
                        {
                            filename_str = image.FileName;
                            filename_str = filename_str.Insert(filename_str.LastIndexOf('.'), "-visual");
                        }
                        else
                        {
                            filename_str = image.FileName;
                        }

                        using (Stream fileStream = image.OpenReadStream())
                        {
                            string file_name = string_site_id + "/" + order_number + "/" + filename_str;
                            var get_db_s3_filepath = db_file_names.Where(x => (x.ir_image_label == filename_str || x.visual_image_label == filename_str)).Select(x=>x.s3_image_folder_name).FirstOrDefault();
                            if (!String.IsNullOrEmpty(get_db_s3_filepath))
                            {
                                file_name = get_db_s3_filepath + "/" + filename_str;
                            }
                            
                            var request = new PutObjectRequest()
                            {
                                BucketName = bucketName,
                                CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                                Key = file_name,// DateTime.Now.Ticks.ToString() + Path.GetExtension(image.FileName),
                                InputStream = fileStream    //SEND THE FILE STREAM
                            };
                            var response = await client.PutObjectAsync(request);
                            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                            {
                                result.Add("https://" + bucketName + ".s3.us-east-2.amazonaws.com/" + request.Key);
                                //result.Add("https://s3.us-east-2.amazonaws.com/" + bucketName+"/" + request.Key);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //Logger.Log("error while upload inspection images in aws ", e.Message);
                throw e;
            }

            return result;
        }

        public async Task<UploadAssetImageResponsemodel> UploadAssetImage(List<IFormFile> images, string awsAccessKey, string awsSecretKey, string bucketName)
        {
            UploadAssetImageResponsemodel UploadAssetPhotoRequestModel = new UploadAssetImageResponsemodel();
            var result = new List<string>();
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    foreach (var image in images)
                    {
                        using (Stream fileStream = image.OpenReadStream())
                        {
                            byte[] byte_data = null;
                            string file_name = image.FileName;
                            string file_extention = Path.GetExtension(image.FileName).ToLower();

                            using (var memoryStream = new MemoryStream())
                            {
                                fileStream.CopyTo(memoryStream);
                                byte_data = memoryStream.ToArray();
                            }

                            if (file_extention == ".heic")
                            {
                                file_name = file_name.Replace("heic", "jpeg");
                                file_name = file_name.Replace("HEIC", "jpeg");
                                file_name = file_name.Replace("HEIF", "jpeg");
                                file_name = file_name.Replace("heif", "jpeg");

                                var out_stream = ConvertHeicToJpeg(fileStream);
                                file_extention = ".jpeg";
                                using (var memoryStream = new MemoryStream())
                                {
                                    out_stream.CopyTo(memoryStream);
                                    byte_data = memoryStream.ToArray();
                                }
                            }

                            string thumbnail_img_name = "thumbnail_" + DateTime.Now.Ticks.ToString() + file_extention;//Path.GetExtension(image.FileName);
                            using (MemoryStream mem = new MemoryStream(byte_data))
                            {
                                var resized_image = Crop(320, 240, mem, thumbnail_img_name, awsAccessKey, awsSecretKey, bucketName, file_extention);
                                if (!String.IsNullOrEmpty(resized_image))
                                {
                                    UploadAssetPhotoRequestModel.thumbnail_image_file = resized_image;
                                }
                                resized_image = null;
                            }
                            using (MemoryStream uploadStream = new MemoryStream(byte_data))
                            {
                                var request = new PutObjectRequest()
                                {
                                    BucketName = bucketName,
                                    CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                                    Key = DateTime.Now.Ticks.ToString() + file_extention,//Path.GetExtension(image.FileName),
                                    InputStream = uploadStream    //SEND THE FILE STREAM
                                };
                                var response = await client.PutObjectAsync(request);
                                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                                {
                                    UploadAssetPhotoRequestModel.original_imege_file = request.Key;
                                    result.Add(request.Key);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Shared.Logger.Log("error while upload inspection images in aws UploadAssetImage function ", e.Message);
                //Logger.Log("error while upload inspection images in aws ", e.Message);
                throw e;
            }

            return UploadAssetPhotoRequestModel;
        }
        public async Task<List<UploadAssetImageResponsemodel>> UploadAssetMultipleImage(List<IFormFile> images, string awsAccessKey, string awsSecretKey, string bucketName)
        {
            List<UploadAssetImageResponsemodel> responsemodel2 = new List<UploadAssetImageResponsemodel>();
            var result = new List<string>();
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    foreach (var image in images)
                    {
                        UploadAssetImageResponsemodel UploadAssetPhotoRequestModel = new UploadAssetImageResponsemodel();
                        using (Stream fileStream = image.OpenReadStream())
                        {
                            byte[] byte_data = null;
                            string file_name = image.FileName;
                            string file_extention = Path.GetExtension(image.FileName).ToLower();

                            using (var memoryStream = new MemoryStream())
                            {
                                fileStream.CopyTo(memoryStream);
                                byte_data = memoryStream.ToArray();
                            }

                            if (file_extention == ".heic" || file_extention == ".heif")
                            {
                                file_name = file_name.Replace("heic", "jpeg");
                                file_name = file_name.Replace("HEIC", "jpeg");
                                file_name = file_name.Replace("HEIF", "jpeg");
                                file_name = file_name.Replace("heif", "jpeg");

                                var out_stream = ConvertHeicToJpeg(fileStream);
                                file_extention = ".jpeg";
                                using (var memoryStream = new MemoryStream())
                                {
                                    out_stream.CopyTo(memoryStream);
                                    byte_data = memoryStream.ToArray();
                                }
                            }

                            string thumbnail_img_name = "thumbnail_" + DateTime.Now.Ticks.ToString() + file_extention;//Path.GetExtension(image.FileName);
                            using (MemoryStream mem = new MemoryStream(byte_data))
                            {
                                var resized_image = Crop(320, 240, mem, thumbnail_img_name, awsAccessKey, awsSecretKey, bucketName, file_extention);
                                if (!String.IsNullOrEmpty(resized_image))
                                {
                                    UploadAssetPhotoRequestModel.thumbnail_image_file = resized_image;
                                }
                                resized_image = null;
                            }

                            using (MemoryStream uploadStream = new MemoryStream(byte_data))
                            {
                                var request = new PutObjectRequest()
                                {
                                    BucketName = bucketName,
                                    CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                                    Key = DateTime.Now.Ticks.ToString() + file_extention,//Path.GetExtension(image.FileName),
                                    InputStream = uploadStream    //SEND THE FILE STREAM
                                };
                                var response = await client.PutObjectAsync(request);
                                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                                {
                                    UploadAssetPhotoRequestModel.user_uploaded_filename = file_name;//image.FileName;
                                    UploadAssetPhotoRequestModel.original_imege_file = request.Key;
                                    result.Add(request.Key);
                                    responsemodel2.Add(UploadAssetPhotoRequestModel);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Shared.Logger.Log("error while upload inspection images in aws UploadAssetImage function ", e.Message);
                //Logger.Log("error while upload inspection images in aws ", e.Message);
                throw e;
            }

            return responsemodel2;
        }
        public static Stream ConvertHeicToJpeg(Stream heicStream)
        {
            heicStream.Seek(0, SeekOrigin.Begin); // Ensure the stream is at the beginning

            using var image = new MagickImage(heicStream);
            image.Format = MagickFormat.Jpeg; // Specify the output format
            image.Quality = 75;

            var jpegStream = new MemoryStream();
            image.Write(jpegStream);
            jpegStream.Seek(0, SeekOrigin.Begin); // Reset stream position for later use
            return jpegStream;
        }

        public async Task<UploadAssetImageResponsemodel> UploadWOPMFormImages(List<IFormFile> images, string awsAccessKey, string awsSecretKey, string bucketName)
        {
            UploadAssetImageResponsemodel UploadAssetPhotoRequestModel = new UploadAssetImageResponsemodel();
            var result = new List<string>();
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    foreach (var image in images)
                    {
                        using (Stream fileStream = image.OpenReadStream())
                        {

                            string file_extention = Path.GetExtension(image.FileName);

                            byte[] byte_data = null;
                            using (var memoryStream = new MemoryStream())
                            {
                                fileStream.CopyTo(memoryStream);
                                byte_data = memoryStream.ToArray();
                            }
                            /*string thumbnail_img_name = "thumbnail_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(image.FileName);
                            using (MemoryStream mem = new MemoryStream(byte_data))
                            {
                                var resized_image = Crop(320, 240, mem, thumbnail_img_name, awsAccessKey, awsSecretKey, bucketName);
                                if (!String.IsNullOrEmpty(resized_image))
                                {
                                    UploadAssetPhotoRequestModel.thumbnail_image_file = resized_image;
                                }
                                resized_image = null;
                            }*/

                            var request = new PutObjectRequest()
                            {

                                BucketName = bucketName,
                                CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                                Key = DateTime.Now.Ticks.ToString() + Path.GetExtension(image.FileName),
                                InputStream = fileStream    //SEND THE FILE STREAM
                            };
                            var response = await client.PutObjectAsync(request);
                            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                            {
                                UploadAssetPhotoRequestModel.original_imege_file = request.Key;
                                result.Add(request.Key);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Shared.Logger.Log("error while upload inspection images in aws UploadAssetImage function ", e.Message);
                //Logger.Log("error while upload inspection images in aws ", e.Message);
                throw e;
            }

            return UploadAssetPhotoRequestModel;
        }
        public async Task<string> UploadAsStremForPDF(System.IO.Stream file, string awsAccessKey, string awsSecretKey, string bucketName, string filename)
        {
            string result = null;
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    // using (Stream fileStream = image.OpenReadStream())
                    // {
                    var request = new PutObjectRequest()
                    {
                        BucketName = bucketName,
                        CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                        Key = filename,
                        InputStream = file    //SEND THE FILE STREAM
                    };
                    var response = await client.PutObjectAsync(request);
                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                    {
                        result = request.Key;
                    }
                    //  }
                }
            }
            catch (Exception e)
            {
            }
            return result;
        }
        public static string Crop(int Width, int Height, Stream streamImg, string fileName, string awsAccessKey, string awsSecretKey, string bucketName  , string file_extention)
        {
            string return_file_name = "";
            try
            {
                Bitmap sourceImage = new Bitmap(streamImg);
                // var img = Image.FromStream(streamImg);
                // var temp = ImageHelper.RotateImageByExifOrientationData(img, true);
                try
                {
                    RotateFlipType fType = ImageHelper.RotateImageByExifOrientationData(sourceImage, true);
                    if (fType != RotateFlipType.RotateNoneFlipNone)
                    {

                    }
                }
                catch (Exception ex)
                {
                    Shared.Logger.Log("error while rotating image ", ex.Message);
                }

                try
                {

                    Width = sourceImage.Width;
                    Height = sourceImage.Height;
                    Width = (int)(Width * 0.25);
                    Height = (int)(Height * 0.25);
                    //Bitmap objBitmap;
                    using (var objBitmap = new Bitmap(Width, Height))
                    {
                        Shared.Logger.Log("error while upload inspection images in aws Crop function after using objBitmap ", "");
                        // objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                        Shared.Logger.Log("error while upload inspection images in aws Crop function after using SetResolution ", "");
                        using (Graphics objGraphics = Graphics.FromImage(objBitmap))
                        {
                            // Set the graphic format for better result cropping   
                            objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            objGraphics.DrawImage(sourceImage, 0, 0, Width, Height);
                            Shared.Logger.Log("error while upload inspection images in aws Crop function after using deaw ", "");
                            // Save the file path, note we use png format to support png file   
                            // objBitmap.Save(saveFilePath);
                            using (MemoryStream m = new MemoryStream())
                            {
                                if (file_extention.ToLower() == ".png")
                                    objBitmap.Save(m, ImageFormat.Png);
                                else
                                    objBitmap.Save(m, ImageFormat.Jpeg);
                                var a = new MemoryStream(m.GetBuffer());
                                return_file_name = UploadAsStrem(a, awsAccessKey, awsSecretKey, bucketName, fileName).Result;

                            }

                        }

                    }

                }
                catch (Exception x)
                {
                    Shared.Logger.Log("error while upload inspection images in aws Crop function exception " + x.Message, "");
                }
                finally
                {
                    Shared.Logger.Log("error while upload inspection images in aws crop function finally", "");
                    sourceImage = null;
                }
            }
            catch (Exception e)
            {
            }
            return return_file_name;
        }

        public static async Task<string> UploadAsStrem(MemoryStream image, string awsAccessKey, string awsSecretKey, string bucketName, string filename)
        {
            string result = null;
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    // using (Stream fileStream = image.OpenReadStream())
                    // {
                    var request = new PutObjectRequest()
                    {
                        BucketName = bucketName,
                        CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                        Key = filename,
                        InputStream = image    //SEND THE FILE STREAM
                    };
                    var response = await client.PutObjectAsync(request);
                    // LogHelper.Log<AwsS3BucketUtil>(LogLevel.Information, "UploadAsStrem FileName: " + filename + " Response: " + response.HttpStatusCode);
                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                    {
                        result = request.Key;

                    }
                    //  }
                }
            }
            catch (Exception e)
            {
                Shared.Logger.Log("error while upload inspection images in aws UploadAsStrem function exception ", e.Message);
            }
            return result;
        }

        public async Task<string> UploadJsonfileAsStrem(MemoryStream image, string awsAccessKey, string awsSecretKey, string bucketName, string filename)
        {
            string result = null;
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    // using (Stream fileStream = image.OpenReadStream())
                    // {
                    var request = new PutObjectRequest()
                    {
                        BucketName = bucketName,
                        CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                        Key = filename,
                        InputStream = image    //SEND THE FILE STREAM
                    };
                    var response = await client.PutObjectAsync(request);
                    // LogHelper.Log<AwsS3BucketUtil>(LogLevel.Information, "UploadAsStrem FileName: " + filename + " Response: " + response.HttpStatusCode);
                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                    {
                        result = request.Key;

                    }
                    //  }
                }
            }
            catch (Exception e)
            {
                Shared.Logger.Log("error while upload inspection images in aws UploadAsStrem function exception ", e.Message);
            }
            return result;
        }
        public async Task<List<string>> UploadOfflineImage(List<IFormFile> images, string awsAccessKey, string awsSecretKey, string bucketName, int height, int width, string folderPathName)
        {
            var result = new List<string>();
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    foreach (var image in images)
                    {
                        using (Stream fileStream = image.OpenReadStream())
                        {
                            var request = new PutObjectRequest()
                            {
                                BucketName = bucketName,
                                CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                                Key = Path.GetFileNameWithoutExtension(image.FileName) + Path.GetExtension(image.FileName),
                                InputStream = fileStream    //SEND THE FILE STREAM
                            };
                            var response = await client.PutObjectAsync(request);
                            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                            {
                                // create/upload image as a thumbnail in s3 bucket

                                using (var memoryStream = new MemoryStream())
                                {
                                    await image.CopyToAsync(memoryStream);
                                    using (var img = Image.FromStream(memoryStream))
                                    {
                                        var thumbnailImage = img.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
                                        var thumbFileStream = Shared.Utility.AwsS3BucketUtil.ToStream(thumbnailImage, ImageFormat.Jpeg);
                                        var putrequest = new PutObjectRequest()
                                        {
                                            BucketName = bucketName,
                                            CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                                            Key = folderPathName + "/" + request.Key,
                                            InputStream = thumbFileStream    //SEND THE FILE STREAM
                                        };
                                        var thumbResponse = await client.PutObjectAsync(putrequest);
                                        if (thumbResponse.HttpStatusCode == System.Net.HttpStatusCode.OK || thumbResponse.HttpStatusCode == System.Net.HttpStatusCode.Created)
                                        {
                                            result.Add(request.Key);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //Logger.Log("error while upload inspection images in aws ", e.Message);
                throw e;
            }
            return result;
        }

        public async Task<bool> Exists(string fileKey, string bucketName, string awsAccessKey, string awsSecretKey)
        {

            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                ForcePathStyle = true // MUST be true to work correctly with MinIO server
            };
            string responseBody = "";
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileKey
                };
                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    using (GetObjectResponse response = await client.GetObjectAsync(request))
                    using (Stream responseStream = response.ResponseStream)
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        string title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
                        string contentType = response.Headers["Content-Type"];
                        Console.WriteLine("Object metadata, Title: {0}", title);
                        Console.WriteLine("Content type: {0}", contentType);

                        responseBody = reader.ReadToEnd(); // Now you process the response body.
                        return true;
                    }
                }

            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered ***. Message:'{0}' when writing an object", e.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                return false;
            }

        }

        public async Task<string> UploadBarcode(string awsAccessKey, string awsSecretKey, Bitmap bitmap, string filename, string bucketname)
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
        public async Task<string> GetQuickSightEmbedUrl(string accessKey, string secretKey, string AwsAccountId, string DashboardId, string roleArnToAssume)
        {
            var client = new AmazonQuickSightClient(
                accessKey,
                secretKey,
                RegionEndpoint.USEast2);

            var urlReponse = await client.GetDashboardEmbedUrlAsync(new GetDashboardEmbedUrlRequest
            {
                AwsAccountId = AwsAccountId,
                DashboardId = DashboardId,
                IdentityType = EmbeddingIdentityType.QUICKSIGHT,
                ResetDisabled = true,
                Namespace = "default",
                SessionLifetimeInMinutes = 600,
                UndoRedoDisabled = false,
                UserArn = roleArnToAssume
            });
            return urlReponse.EmbedUrl;
        }

        public async Task<List<string>> UploadThumbNailImagesForExisting(List<string> images, string awsAccessKey, string awsSecretKey, string bucketName, string folderPathName, int height, int width)
        {
            var result = new List<string>();
            List<string> notUploaded = new List<string>();
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    foreach (var image in images)
                    {
                        GetObjectRequest request = new GetObjectRequest
                        {
                            BucketName = bucketName,
                            Key = Path.GetFileName(new Uri(image).AbsolutePath)
                        };
                        try
                        {
                            using (var getObjectResponse = await client.GetObjectAsync(request))
                            {
                                using (var responseStream = getObjectResponse.ResponseStream)
                                {
                                    var stream = new MemoryStream();
                                    await responseStream.CopyToAsync(stream);
                                    stream.Position = 0;

                                    using (var img = Image.FromStream(stream))
                                    {
                                        var thumbnailImage = img.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
                                        var fileStream = Shared.Utility.AwsS3BucketUtil.ToStream(thumbnailImage, ImageFormat.Jpeg);
                                        var putrequest = new PutObjectRequest()
                                        {
                                            BucketName = bucketName,
                                            CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                                            Key = folderPathName + "/" + Path.GetFileName(new Uri(image).AbsolutePath),
                                            InputStream = fileStream    //SEND THE FILE STREAM
                                        };
                                        var response = await client.PutObjectAsync(putrequest);
                                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                                        {
                                            result.Add(putrequest.Key);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            //Logger.Log("error while upload inspection images in aws ", e.Message);
                            // Logger.Log("Image Name ", image);
                            notUploaded.Add(image);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //Logger.Log("error while upload inspection images in aws ", e.Message);
                throw e;
            }

            return result;
        }

        public async Task<List<string>> UploadAttachment(List<IFormFile> attachments, string awsAccessKey, string awsSecretKey, string bucketName)
        {
            var result = new List<string>();
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    foreach (var attachment in attachments)
                    {
                        using (Stream fileStream = attachment.OpenReadStream())
                        {
                            var request = new PutObjectRequest()
                            {
                                BucketName = bucketName,
                                CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                                Key = DateTime.Now.Ticks.ToString() + Path.GetExtension(attachment.FileName),
                                InputStream = fileStream    //SEND THE FILE STREAM
                            };
                            var response = await client.PutObjectAsync(request);
                            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                            {
                                result.Add(request.Key);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //  Logger.Log("error while upload inspection images in aws ", e.Message);
                throw e;
            }

            return result;
        }

        public async Task<List<S3UploadAssetAttachmentResponsemodel>> UploadAssetAttachment(List<IFormFile> attachments, string awsAccessKey, string awsSecretKey, string bucketName)
        {
            var result = new List<S3UploadAssetAttachmentResponsemodel>();
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    foreach (var attachment in attachments)
                    {
                        var file_name = attachment.FileName;
                        using (Stream fileStream = attachment.OpenReadStream())
                        {
                            var request = new PutObjectRequest()
                            {
                                BucketName = bucketName,
                                CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                                Key = DateTime.Now.Ticks.ToString() + Path.GetExtension(attachment.FileName),
                                InputStream = fileStream    //SEND THE FILE STREAM
                            };
                            var response = await client.PutObjectAsync(request);
                            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                            {
                                S3UploadAssetAttachmentResponsemodel S3UploadAssetAttachmentResponsemodel = new S3UploadAssetAttachmentResponsemodel();
                                S3UploadAssetAttachmentResponsemodel.s3_file_name = request.Key;
                                S3UploadAssetAttachmentResponsemodel.user_uploaded_file_name = file_name;
                                result.Add(S3UploadAssetAttachmentResponsemodel);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //  Logger.Log("error while upload inspection images in aws ", e.Message);
                throw e;
            }

            return result;
        }

        public async Task<List<S3UploadAssetAttachmentResponsemodel>> UploadDocumentByFilename(List<IFormFile> attachments, string awsAccessKey, string awsSecretKey, string bucketName)
        {
            var result = new List<S3UploadAssetAttachmentResponsemodel>();
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    foreach (var attachment in attachments)
                    {
                        var file_name = attachment.FileName;
                        using (Stream fileStream = attachment.OpenReadStream())
                        {
                            var request = new PutObjectRequest()
                            {
                                BucketName = bucketName,
                                CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                                Key = attachment.FileName,//DateTime.Now.Ticks.ToString() + Path.GetExtension(attachment.FileName),
                                InputStream = fileStream    //SEND THE FILE STREAM
                            };
                            var response = await client.PutObjectAsync(request);
                            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                            {
                                S3UploadAssetAttachmentResponsemodel S3UploadAssetAttachmentResponsemodel = new S3UploadAssetAttachmentResponsemodel();
                                S3UploadAssetAttachmentResponsemodel.s3_file_name = request.Key;
                                S3UploadAssetAttachmentResponsemodel.user_uploaded_file_name = file_name;
                                result.Add(S3UploadAssetAttachmentResponsemodel);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //  Logger.Log("error while upload inspection images in aws ", e.Message);
                throw e;
            }

            return result;
        }

        public async Task<DeleteObjectsResponse> deleteObjects(List<string> objectsToBeDeleted, string awsAccessKey, string awsSecretKey, string bucketName)
        {
            try
            {
                AmazonS3Client Client = new AmazonS3Client(awsAccessKey, awsSecretKey, RegionEndpoint.USEast2);
                var deleteObjectsRequest = new DeleteObjectsRequest();
                deleteObjectsRequest.BucketName = bucketName;
                foreach (string obj in objectsToBeDeleted)
                {
                    deleteObjectsRequest.AddKey(obj);
                }
                var response = await Client.DeleteObjectsAsync(deleteObjectsRequest);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;

        }

        public async Task<int> CopyImagesToAnotherBucket(string awsAccessKey, string awsSecretKey, string source_bucketName, string destination_bucketName, string image_name)
        {
            string accessKey = awsAccessKey;
            string secretKey = awsSecretKey;
            RegionEndpoint region = RegionEndpoint.USEast2;

            // Set the source and destination bucket names
            string sourceBucketName = source_bucketName;
            string destinationBucketName = destination_bucketName;

            // Set the key of the image file to copy
            string source_image = image_name;
            string destination_image = image_name;
            if (destination_image != null)
            {
                string[] substrings = destination_image.Split('/');
                destination_image = substrings.Last();
            }


            // Create an instance of the S3 client
            var s3Client = new AmazonS3Client(accessKey, secretKey, region);

            try
            {
                // Create a CopyObjectRequest
                var copyRequest = new CopyObjectRequest
                {
                    SourceBucket = sourceBucketName,
                    SourceKey = source_image,
                    DestinationBucket = destinationBucketName,
                    DestinationKey = destination_image,
                    CannedACL = S3CannedACL.PublicRead,
                };

                // Copy the image from the source bucket to the destination bucket
                var copyResponse = await s3Client.CopyObjectAsync(copyRequest);

                if (copyResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("Image copied successfully!");
                }
                else
                {
                    Console.WriteLine("Failed to copy image.");
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on Amazon S3: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return 1;
        }


        public async Task<int> CopyImagesToAnotherFolderForIR(string awsAccessKey, string awsSecretKey, string source_bucketName, string destination_bucketName, string sourceFolderName, string destinationFolderName)
        {
            try
            {
                var credentials = new Amazon.Runtime.BasicAWSCredentials(awsAccessKey, awsSecretKey);
                var config = new AmazonS3Config { RegionEndpoint = RegionEndpoint.USEast2 }; // Change to your desired region

                using var s3Client = new AmazonS3Client(credentials, config);

                var listRequest = new Amazon.S3.Model.ListObjectsRequest
                {
                    BucketName = source_bucketName,
                    Prefix = sourceFolderName
                };

                var listResponse = await s3Client.ListObjectsAsync(listRequest);

                foreach (var s3Object in listResponse.S3Objects)
                {
                    var copyRequest = new CopyObjectRequest
                    {
                        SourceBucket = source_bucketName,
                        SourceKey = s3Object.Key,
                        DestinationBucket = destination_bucketName,
                        DestinationKey = destinationFolderName + s3Object.Key.Substring(sourceFolderName.Length),
                        CannedACL = S3CannedACL.PublicRead,
                    };

                    await s3Client.CopyObjectAsync(copyRequest);

                    var deleteRequest = new Amazon.S3.Model.DeleteObjectRequest
                    {
                        BucketName = source_bucketName,
                        Key = s3Object.Key
                    };

                    await s3Client.DeleteObjectAsync(deleteRequest);
                }

            }
            catch (Exception ex)
            {

            }



            return 1;
        }


        public async Task<string> UploadClusterOneLinePdfService(IFormFile attachment, string awsAccessKey, string awsSecretKey, string bucketName, string site_id)
        {
            var result = string.Empty;//new List<string>();
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    using (Stream fileStream = attachment.OpenReadStream())
                    {
                        var request = new PutObjectRequest()
                        {
                            BucketName = bucketName,
                            CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                            Key = site_id + Path.GetExtension(attachment.FileName),
                            InputStream = fileStream    //SEND THE FILE STREAM
                        };

                        var response = await client.PutObjectAsync(request);
                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                        {
                            result = request.Key;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        public async Task<GetImageInfoByTextRactResponsemodel> DetectSampleAsync(string awsAccessKey, string awsSecretKey, string bucketName, string image_name, List<string> question_list)
        {
            GetImageInfoByTextRactResponsemodel response = new GetImageInfoByTextRactResponsemodel();
            try
            {
                // Set your AWS credentials and region
                var region = RegionEndpoint.USEast2;

                var textractClient = new AmazonTextractClient(awsAccessKey, awsSecretKey, region);

                var analyzeRequest = new AnalyzeDocumentRequest();
                analyzeRequest.Document = new Document()
                {
                    S3Object = new Amazon.Textract.Model.S3Object
                    {
                        Bucket = bucketName,
                        Name = image_name
                    }
                };
                analyzeRequest.FeatureTypes = new List<string> { "TABLES", "FORMS" };

                if (question_list != null)
                {
                    analyzeRequest.FeatureTypes.Add("QUERIES");
                    analyzeRequest.QueriesConfig = new QueriesConfig();
                 
                    List<Query> Queries = new List<Query>();
                    foreach (var item in question_list)
                    {
                        Query Query = new Query();
                        Query.Text = item;
                        Queries.Add(Query);
                    }
                    analyzeRequest.QueriesConfig.Queries = Queries;
                }
                

                // Create the request to analyze the document
               /* var analyzeRequest = new AnalyzeDocumentRequest
                {
                    Document = new Document
                    {
                        S3Object = new Amazon.Textract.Model.S3Object
                        {
                            Bucket = bucketName,
                            Name = image_name
                        }
                    },
                    FeatureTypes = new List<string> { "QUERIES" }, // Adjust features based on your needs

                    QueriesConfig = new QueriesConfig()
                    {
                        
                        Queries = new List<Query>
                        {
                            new Query {Text = "who is the manufacturer?"},
                            new Query {Text = "what is the model number?"},
                            new Query {Text = "what is the high voltage or HV rating?"},
                            new Query {Text = "what is the high voltage current?"},
                            new Query {Text = "what is the low voltage or LV rating?"},
                            new Query {Text = "what is the low voltage or current?"},
                            new Query {Text = "what is the percent impedance?"},
                            new Query {Text = "what is the frequency?"},
                        }
                    }
                };
                */
                // Call the AnalyzeDocument API
                var analyzeResponse = await textractClient.AnalyzeDocumentAsync(analyzeRequest);
                response.textract_output_json = JsonConvert.SerializeObject(analyzeResponse, Formatting.Indented);

            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public async Task<List<string>> UploadmobilelogsToS3(List<IFormFile> log_file, string awsAccessKey, string awsSecretKey, string bucketName , string folder_name)
        {
            UploadAssetImageResponsemodel UploadAssetPhotoRequestModel = new UploadAssetImageResponsemodel();
            var result = new List<string>();
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    foreach (var image in log_file)
                    {
                        using (Stream fileStream = image.OpenReadStream())
                        {

                            string file_extention = Path.GetExtension(image.FileName);

                            
                            using (var memoryStream = new MemoryStream())
                            {
                                fileStream.CopyTo(memoryStream);
                                
                            }

                            var request = new PutObjectRequest()
                            {

                                BucketName = bucketName,
                                CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                                Key = folder_name + "/" + DateTime.Now.Ticks.ToString()+ "-" + image.FileName,
                                InputStream = fileStream    //SEND THE FILE STREAM
                            };
                            var response = await client.PutObjectAsync(request);
                            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                            {
                                UploadAssetPhotoRequestModel.original_imege_file = request.Key;
                                result.Add(request.Key);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Shared.Logger.Log("error while upload inspection images in aws UploadAssetImage function ", e.Message);
                //Logger.Log("error while upload inspection images in aws ", e.Message);
                throw e;
            }

            return result;
        }

        public async Task<UploadAssetImageResponsemodel> UploadSiteProfileImage(List<IFormFile> images, string awsAccessKey, string awsSecretKey, string bucketName, string company_id, string site_id)
        {
            UploadAssetImageResponsemodel UploadAssetPhotoRequestModel = new UploadAssetImageResponsemodel();
            var result = new List<string>();
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    foreach (var image in images)
                    {
                        using (Stream fileStream = image.OpenReadStream())
                        {
                            string file_name = company_id + "/" + site_id + "/" + image.FileName;
                            //string file_extention = Path.GetExtension(image.FileName);

                            byte[] byte_data = null;
                            using (var memoryStream = new MemoryStream())
                            {
                                fileStream.CopyTo(memoryStream);
                                byte_data = memoryStream.ToArray();
                            }
                            /*
                             * //string thumbnail_img_name = "thumbnail_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(image.FileName);
                            //using (MemoryStream mem = new MemoryStream(byte_data))
                            //{
                            //    var resized_image = Crop(320, 240, mem, thumbnail_img_name, awsAccessKey, awsSecretKey, bucketName, file_extention);
                            //    if (!String.IsNullOrEmpty(resized_image))
                            //    {
                            //        UploadAssetPhotoRequestModel.thumbnail_image_file = resized_image;
                            //    }
                            //    resized_image = null;
                            //}*/

                            var request = new PutObjectRequest()
                            {

                                BucketName = bucketName,
                                CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                                Key = file_name,//DateTime.Now.Ticks.ToString() + Path.GetExtension(image.FileName),
                                InputStream = fileStream    //SEND THE FILE STREAM
                            };
                            var response = await client.PutObjectAsync(request);
                            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                            {
                                UploadAssetPhotoRequestModel.user_uploaded_filename = image.FileName;//request.Key;
                                UploadAssetPhotoRequestModel.original_imege_file = "https://" + bucketName + ".s3.us-east-2.amazonaws.com/" + request.Key;
                                //result.Add("https://" + bucketName + ".s3.us-east-2.amazonaws.com/" + request.Key);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Shared.Logger.Log("error while upload inspection images in aws UploadAssetImage function ", e.Message);
                //Logger.Log("error while upload inspection images in aws ", e.Message);
                throw e;
            }

            return UploadAssetPhotoRequestModel;
        }

        public async Task<List<string>> UploadSiteDocument(List<IFormFile> attachments, string awsAccessKey, string awsSecretKey, string bucketName)
        {
            var result = new List<string>();
            try
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast2, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` enviroment variable.
                    ForcePathStyle = true // MUST be true to work correctly with MinIO server
                };

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    foreach (var attachment in attachments)
                    {
                        string file_name = DateTime.Now.Ticks.ToString() + Path.GetExtension(attachment.FileName);
                        using (Stream fileStream = attachment.OpenReadStream())
                        {
                            var request = new PutObjectRequest()
                            {
                                BucketName = bucketName,
                                CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
                                Key = UpdatedGenericRequestmodel.CurrentUser.site_id + "/" + file_name,
                                InputStream = fileStream    //SEND THE FILE STREAM
                            };
                            var response = await client.PutObjectAsync(request);
                            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                            {
                                result.Add(file_name);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //  Logger.Log("error while upload inspection images in aws ", e.Message);
                throw e;
            }

            return result;
        }

    }
}
