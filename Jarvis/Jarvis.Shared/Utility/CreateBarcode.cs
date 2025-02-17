using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ZXing.QrCode;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Threading.Tasks;
using Jarvis.Shared.StatusEnums;

namespace Jarvis.Shared.Utility
{
    public static class CreateBarcode
    {
        public static async System.Threading.Tasks.Task<string> GetQRCode(string uuid, string awsaccesskey, string awssecretkey, string bucketname)
        {

            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions { Height = 200, Width = 200, Margin = 0 }
            };

            var pixelData = qrCodeWriter.Write(uuid);
            using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    try
                    {
                        // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image
                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0,
                        pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                    // save to stream as PNG
                    var imageName = DateTime.Now.Ticks.ToString() + ".png";
                    //bitmap.Save(imageName);

                    var filename = await AwsS3BucketUtil.UploadBarcode(awsaccesskey, awssecretkey, bitmap, uuid + ".png", bucketname);

                    return filename;
                }
            }
        }

        public static byte[] CreatePDF(AssetBarcodeGenerator assetlist,CreatePDFTypes types)
        {
            byte[] x = null;
            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.LETTER, 0, 0, -13, 0);
            MemoryStream PDFData = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, PDFData);
            try
            {
                //MemoryStream
                document.Open();
                document.SetMargins(0, 0, 0, 0);
                var count = 0;

                PdfPTable pdfTableBlank1 = new PdfPTable(1);
                PdfPCell cell = new PdfPCell();
                cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                Phrase elements = new Phrase();
                elements.Add(
                       new Chunk("\n")
                   );
                elements.Add(
                       new Chunk("\n")
                   );
                cell.AddElement(elements);
                pdfTableBlank1.AddCell(cell);
                    

                document.Add(pdfTableBlank1);

                PdfPTable pdfTableBlank = new PdfPTable(5);
                float[] FirstRowWidths = new float[] { 30f, 50f, 15f, 30f, 50f };
                pdfTableBlank.SetWidths(FirstRowWidths);
                pdfTableBlank.WidthPercentage = 92.5f;
                for (int i = 1; i <= assetlist.asset.Count; i++)
                {
                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(assetlist.asset[i - 1].asset_barcode_image);
                    count++;
                    jpg.Alignment = Element.ALIGN_LEFT;
                    jpg.SpacingBefore = 38f;
                    jpg.ScaleToFit(93f, 95f);
                    jpg.SpacingAfter = 10.5f;

                    PdfPCell logo = new PdfPCell();
                    logo.AddElement(jpg);
                    logo.HorizontalAlignment = Element.ALIGN_LEFT;
                    pdfTableBlank.AddCell(new PdfPCell(logo) { Rowspan = 3, HorizontalAlignment = Element.ALIGN_LEFT , Border = iTextSharp.text.Rectangle.NO_BORDER });
                    pdfTableBlank.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    Phrase phrase = new Phrase();
                    phrase.Add(
                        new Chunk(assetlist.asset[i - 1].asset_name, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD))
                    );

                    phrase.Add(
                        new Chunk("\n")
                    );

                    phrase.Add(
                        new Chunk(assetlist.asset[i - 1].internal_asset_id, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL))
                    );

                    if (CreatePDFTypes.Asset == types)
                    {
                        phrase.Add(
                            new Chunk("\n")
                        );
                    }

                    phrase.Add(
                        new Chunk("\n")
                    );

                    phrase.Add(
                        new Chunk(assetlist.asset[i - 1].site_name, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL))
                    );

                    PdfPCell cellNum2 = new PdfPCell();
                    cellNum2.AddElement(phrase);
                    cellNum2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cellNum2.Rowspan = 3;
                    cellNum2.PaddingTop = 40f;
                    pdfTableBlank.AddCell(cellNum2);

                    if ((i == assetlist.asset.Count && i / 2 != 0) || (i < 2 && i == assetlist.asset.Count))
                    {
                        pdfTableBlank.AddCell("");
                        pdfTableBlank.AddCell("");
                        pdfTableBlank.AddCell("");
                        pdfTableBlank.AddCell("");
                        pdfTableBlank.AddCell("");
                        pdfTableBlank.AddCell("");
                    }
                    if (i % 2 != 0)
                    {
                        PdfPCell cellNum3 = new PdfPCell(new Phrase());
                        cellNum3.VerticalAlignment = Element.ALIGN_CENTER;
                        cellNum3.HorizontalAlignment = Element.ALIGN_CENTER;
                        cellNum3.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        cellNum3.Rowspan = 3;
                        cellNum3.PaddingTop = 40f;
                        //cellNum3.PaddingLeft = -20f;
                        //cellNum3.PaddingRight = -20f;
                        pdfTableBlank.AddCell(cellNum3);
                    }

                }
                document.Add(pdfTableBlank);
                //}


            }
            catch (Exception e)
            {

            }

            finally
            {
                if (document.IsOpen())
                {
                    document.Close();
                }
                writer.Close();
                PDFData.Close();
            }
            x = PDFData.ToArray();
            return x;
        }

        public static byte[] CreateRandomBarcodePDF(AssetBarcodeGenerator assetlist, CreatePDFTypes types)
        {
            byte[] x = null;
            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.LETTER, 0, 0, -13, 0);
            MemoryStream PDFData = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, PDFData);
            try
            {
                //MemoryStream
                document.Open();
                document.SetMargins(0, 0, 0, 0);
                
                var count = 0;

                PdfPTable pdfTableBlank1 = new PdfPTable(1);
                PdfPCell cell = new PdfPCell();
                cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                Phrase elements = new Phrase();
                elements.Add(
                       new Chunk("\n")
                   );
                elements.Add(
                       new Chunk("\n")
                   );
                /*elements.Add(
                       new Chunk("\n")
                   );
                elements.Add(
                       new Chunk("\n")
                   );*/
                cell.AddElement(elements);
                pdfTableBlank1.AddCell(cell);


                document.Add(pdfTableBlank1);

                PdfPTable pdfTableBlank = new PdfPTable(5);
                float[] FirstRowWidths = new float[] { 30f, 50f, 15f, 30f, 50f };
                pdfTableBlank.SetWidths(FirstRowWidths);
              //  iTextSharp.text.Image jpg_logo = iTextSharp.text.Image.GetInstance("https://s3-us-east-2.amazonaws.com//conduit-dev.assetimages/638078178549142372.png");
              //  iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(assetlist.asset[0].asset_barcode_image);
                pdfTableBlank.WidthPercentage = 92.5f;
                for (int i = 1; i <= assetlist.asset.Count; i++)
                {
                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(assetlist.asset[i - 1].asset_barcode_image);

                    count++;
                    jpg.Alignment = Element.ALIGN_LEFT;
                    jpg.SpacingBefore = 35f;
                    jpg.ScaleToFit(93f, 95f);
                   // jpg.SpacingAfter = 10.5f;

                    PdfPCell logo = new PdfPCell();
                    logo.AddElement(jpg);
                    logo.HorizontalAlignment = Element.ALIGN_LEFT;
                    pdfTableBlank.AddCell(new PdfPCell(logo) { Rowspan = 3, HorizontalAlignment = Element.ALIGN_LEFT , VerticalAlignment = Element.ALIGN_CENTER
                        , Border = iTextSharp.text.Rectangle.NO_BORDER 
                    });
                    pdfTableBlank.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    Phrase phrase = new Phrase();
                    Phrase phrase1 = new Phrase();

                      iTextSharp.text.Image jpg_logo = iTextSharp.text.Image.GetInstance("https://s3-us-east-2.amazonaws.com//conduit-dev.assetimages/638078178549142372.png");

                    // count++;
                    jpg_logo.Alignment = Element.ALIGN_LEFT;
                    jpg_logo.SpacingBefore = 35f;
                    jpg_logo.ScaleToFit(93f, 95f);
                    jpg_logo.SpacingAfter = 10.5f;
                    jpg_logo.ScaleAbsoluteHeight(98);
                    jpg_logo.ScaleAbsoluteWidth(100);

                    PdfPCell site_logo = new PdfPCell();
                    site_logo.AddElement(jpg_logo);


                   
                    phrase1.Add(
                      new Chunk(" " + assetlist.asset[i - 1].internal_asset_id, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL))
                     );
                    phrase.Add(
                       new Chunk(" CVS Caremark | ©eGalvanic 2023", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL))
                     );
                    phrase.Add(
                      new Chunk("\n", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL))
                    );
                    phrase.Add(
                      new Chunk("\n", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL))
                    );
                    
                    site_logo.AddElement(phrase1);
                    site_logo.AddElement(phrase);
                    pdfTableBlank.AddCell(new PdfPCell(site_logo) { Rowspan = 3, HorizontalAlignment = Element.ALIGN_LEFT , VerticalAlignment = Element.ALIGN_TOP
                        , Border = iTextSharp.text.Rectangle.NO_BORDER 
                    });
                    pdfTableBlank.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    if ((i == assetlist.asset.Count && i / 2 != 0) || (i < 2 && i == assetlist.asset.Count))
                    {
                        pdfTableBlank.AddCell("");
                        pdfTableBlank.AddCell("");
                        pdfTableBlank.AddCell("");
                        pdfTableBlank.AddCell("");
                        pdfTableBlank.AddCell("");
                        pdfTableBlank.AddCell("");
                    }
                    if (i % 2 != 0)
                    {
                        PdfPCell cellNum3 = new PdfPCell(new Phrase());
                        cellNum3.VerticalAlignment = Element.ALIGN_CENTER;
                        cellNum3.HorizontalAlignment = Element.ALIGN_CENTER;
                        cellNum3.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        cellNum3.Rowspan = 3;
                        cellNum3.PaddingTop = 40f;
                        //cellNum3.PaddingLeft = -20f;
                        //cellNum3.PaddingRight = -20f;
                        pdfTableBlank.AddCell(cellNum3);
                    }

                }
                document.Add(pdfTableBlank);
                //}


            }
            catch (Exception e)
            {

            }

            finally
            {
                if (document.IsOpen())
                {
                    document.Close();
                }
                writer.Close();
                PDFData.Close();
            }
            x = PDFData.ToArray();
            return x;
        }

        
        // ------------------------------------------ DONE  -----------------------------------------------//


        //public static byte[] CreatePDF(AssetBarcodeGenerator assetlist)
        //{
        //    byte[] x = null;
        //    iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 0, 0, 36, 0);
        //    MemoryStream PDFData = new MemoryStream();
        //    PdfWriter writer = PdfWriter.GetInstance(document, PDFData);
        //    try
        //    {
        //        //MemoryStream
        //        document.Open();
        //        document.SetMargins(0, 0, 0, 0);
        //        var count = 0;

        //        PdfPTable pdfTableBlank = new PdfPTable(5);
        //        float[] FirstRowWidths = new float[] { 100f, 100f, 25f, 100f, 100f };
        //        pdfTableBlank.SetWidths(FirstRowWidths);
        //        pdfTableBlank.WidthPercentage = 95f;
        //        for (int i = 1; i <= assetlist.asset.Count; i++)
        //        {
        //            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(assetlist.asset[i - 1].asset_barcode_image);
        //            count++;
        //            jpg.Alignment = Element.ALIGN_LEFT;
        //            jpg.SpacingBefore = 16f;
        //            //jpg.SpacingAfter = 10f;

        //            PdfPCell logo = new PdfPCell();
        //            logo.AddElement(jpg);
        //            logo.HorizontalAlignment = Element.ALIGN_LEFT;
        //            pdfTableBlank.AddCell(new PdfPCell(logo) { Rowspan = 3, HorizontalAlignment = Element.ALIGN_LEFT, Border = iTextSharp.text.Rectangle.NO_BORDER });
        //            pdfTableBlank.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

        //            Phrase phrase = new Phrase();
        //            phrase.Add(
        //                new Chunk(assetlist.asset[i - 1].asset_name, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD))
        //            );

        //            phrase.Add(
        //                new Chunk("\n")
        //            );

        //            phrase.Add(
        //                new Chunk(assetlist.asset[i - 1].internal_asset_id, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL))
        //            );

        //            phrase.Add(
        //                new Chunk("\n")
        //            );

        //            phrase.Add(
        //                new Chunk("\n")
        //            );


        //            phrase.Add(
        //                new Chunk(assetlist.asset[i - 1].site_name, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL))
        //            );

        //            PdfPCell cellNum2 = new PdfPCell();
        //            cellNum2.AddElement(phrase);
        //            cellNum2.Border = iTextSharp.text.Rectangle.NO_BORDER;
        //            cellNum2.Rowspan = 3;
        //            cellNum2.PaddingTop = 28f;
        //            pdfTableBlank.AddCell(cellNum2);

        //            if ((i == assetlist.asset.Count && i / 2 != 0) || (i < 2 && i == assetlist.asset.Count))
        //            {
        //                pdfTableBlank.AddCell("");
        //                pdfTableBlank.AddCell("");
        //                pdfTableBlank.AddCell("");
        //                pdfTableBlank.AddCell("");
        //                pdfTableBlank.AddCell("");
        //                pdfTableBlank.AddCell("");
        //            }
        //            if (i % 2 != 0)
        //            {
        //                PdfPCell cellNum3 = new PdfPCell(new Phrase());
        //                cellNum3.VerticalAlignment = Element.ALIGN_CENTER;
        //                cellNum3.HorizontalAlignment = Element.ALIGN_CENTER;
        //                cellNum3.Border = iTextSharp.text.Rectangle.NO_BORDER;
        //                cellNum3.Rowspan = 3;
        //                cellNum3.PaddingTop = 28f;
        //                //cellNum3.PaddingLeft = -20f;
        //                //cellNum3.PaddingRight = -20f;
        //                pdfTableBlank.AddCell(cellNum3);
        //            }

        //        }
        //        document.Add(pdfTableBlank);
        //        //}


        //    }
        //    catch (Exception e)
        //    {

        //    }

        //    finally
        //    {
        //        if (document.IsOpen())
        //        {
        //            document.Close();
        //        }
        //        writer.Close();
        //        PDFData.Close();
        //    }
        //    x = PDFData.ToArray();
        //    return x;
        //}

        // ------------------------------------------  DONE  -----------------------------------------------//






        //public static byte[] CreatePDF(AssetBarcodeGenerator assetlist)
        //{
        //    byte[] x = null;
        //    iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 0, 0, 45, 0);
        //    MemoryStream PDFData = new MemoryStream();
        //    PdfWriter writer = PdfWriter.GetInstance(document, PDFData);
        //    try
        //    {
        //        //MemoryStream
        //        document.Open();
        //        document.SetMargins(0, 0, 50, 20);
        //        var count = 0;

        //        PdfPTable pdfTableBlank = new PdfPTable(4);
        //        float[] FirstRowWidths = new float[] { 130f , 130f, 130f, 130f };
        //        pdfTableBlank.SetWidths(FirstRowWidths);
        //        for (int i = 1; i <= assetlist.asset.Count; i++)
        //        {
        //            //var imagename = GetQRCode(assetIDs[count]);

        //            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(assetlist.asset[i-1].asset_barcode_image);
        //            count++;
        //            //Resize image depend upon your need
        //            //jpg.ScaleToFit(140f, 150f);
        //            jpg.Alignment = Element.ALIGN_LEFT;
        //            jpg.SpacingBefore = 16f;
        //            jpg.SpacingAfter = 16f;

        //            PdfPCell logo = new PdfPCell();
        //            //logo.CellEvent = new RoundedBorder();
        //            logo.AddElement(jpg);
        //            logo.HorizontalAlignment = Element.ALIGN_LEFT;
        //            //pdfTableBlank.AddCell(new PdfPCell(logo) { Rowspan = 3, VerticalAlignment = Element.ALIGN_CENTER , HorizontalAlignment = Element.ALIGN_LEFT,  Border = iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.TOP_BORDER  });
        //            pdfTableBlank.AddCell(new PdfPCell(logo) { Rowspan = 3, VerticalAlignment = Element.ALIGN_CENTER , HorizontalAlignment = Element.ALIGN_LEFT , Border = iTextSharp.text.Rectangle.NO_BORDER});
        //            pdfTableBlank.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

        //            //pdfTableBlank.DefaultCell.CellEvent = new RoundedBorder();

        //            Phrase phrase = new Phrase();
        //            phrase.Add(
        //                new Chunk(assetlist.asset[i-1].asset_name, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD))
        //            );

        //            phrase.Add(
        //                new Chunk("\n")
        //            );

        //            phrase.Add(
        //                new Chunk(assetlist.asset[i-1].internal_asset_id, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL))
        //            );

        //            phrase.Add(
        //                new Chunk("\n")
        //            );

        //            phrase.Add(
        //                new Chunk("\n")
        //            );


        //            phrase.Add(
        //                new Chunk(assetlist.asset[i-1].site_name, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL))
        //            );

        //            PdfPCell cellNum2 = new PdfPCell();
        //            cellNum2.AddElement(phrase);
        //            cellNum2.VerticalAlignment = Element.ALIGN_CENTER;
        //            cellNum2.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cellNum2.Border = iTextSharp.text.Rectangle.NO_BORDER;
        //            //cellNum2.Border = iTextSharp.text.Rectangle.TOP_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.BOTTOM_BORDER;
        //            cellNum2.Rowspan = 3;
        //            cellNum2.PaddingTop = 17f;
        //            //cellNum2.CellEvent = new RoundedBorder();
        //            pdfTableBlank.AddCell(cellNum2);

        //            if ((i == assetlist.asset.Count && i / 2 != 0) || (i < 2 && i == assetlist.asset.Count))
        //            {
        //                pdfTableBlank.AddCell("");
        //                pdfTableBlank.AddCell("");
        //                pdfTableBlank.AddCell("");
        //                pdfTableBlank.AddCell("");
        //                pdfTableBlank.AddCell("");
        //                pdfTableBlank.AddCell("");
        //            }
        //            //if (i % 2 != 0)
        //            //{   
        //            //    PdfPCell cellNum3 = new PdfPCell(new Phrase());
        //            //    cellNum3.VerticalAlignment = Element.ALIGN_CENTER;
        //            //    cellNum3.HorizontalAlignment = Element.ALIGN_CENTER;
        //            //    cellNum3.Border = iTextSharp.text.Rectangle.NO_BORDER;
        //            //    cellNum3.Rowspan = 3;
        //            //    cellNum3.PaddingTop = 10f;
        //            //    cellNum3.PaddingLeft = -20f;
        //            //    cellNum3.PaddingRight = -20f;
        //            //    pdfTableBlank.AddCell(cellNum3);
        //            //}

        //        }
        //        document.Add(pdfTableBlank);
        //        //}


        //    }
        //    catch (Exception e)
        //    {

        //    }

        //    finally
        //    {
        //        if (document.IsOpen())
        //        {
        //            document.Close();
        //        }
        //        writer.Close();
        //        PDFData.Close();
        //    }
        //    x = PDFData.ToArray();
        //    return x;
        //}

        class RoundedBorder : IPdfPCellEvent
        {
            public void CellLayout(PdfPCell cell, iTextSharp.text.Rectangle rect, PdfContentByte[] canvas)
            {
                //PdfContentByte cb = canvas[PdfPTable.BACKGROUNDCANVAS];
                //cb.RoundRectangle(
                //  rect.Left + 200f,
                //  rect.Bottom + 200f,
                //  rect.Width - 3,
                //  rect.Height - 3, 0
                //);
                //cb.Stroke();
                PdfContentByte cb = canvas[PdfPTable.BACKGROUNDCANVAS];
                //rect = new iTextSharp.text.Rectangle(200, 200, 100, 100);
                rect.Border = iTextSharp.text.Rectangle.TOP_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.BOTTOM_BORDER;
                rect.BorderWidth = 1;
                rect.BorderColor = new BaseColor(255, 255, 255);
                cb.RoundRectangle(rect.Left - 22.0f, rect.Bottom + 3f, 250, 129, 20);
                cb.Stroke();
            }
        }

        public async static Task<bool> ByteArrayToFile(string _FileName, byte[] _ByteArray, string awsAccessKey, string awsSecretKey, string bucketName)
        {
            try
            {
                // Open file for reading
               
                using (FileStream _FileStream = new FileStream(_FileName, FileMode.Create, FileAccess.Write))
                {
                    // Writes a block of bytes to this stream using data from  a byte array.
                    _FileStream.Write(_ByteArray, 0, _ByteArray.Length);
                    //Logger.Log("Created PDF" + _FileName);
                    // Close file stream
                    _FileStream.Close();
                    var result =  await AwsS3BucketUtil.UploadBarcodePdf(_FileName, awsAccessKey, awsSecretKey, bucketName);
                    //File.Delete(_FileName);
                    return true;
                }
            }
            catch (Exception _Exception)
            {
                throw _Exception;
                //Console.WriteLine("Exception caught in process while trying to save : {0}", _Exception.ToString());
            }

            return false;
        }
    }
}
