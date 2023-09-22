using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ZXing.QrCode;
using System.Drawing;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using static QRCoder.PayloadGenerator;
using System.Drawing.Imaging;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using BarQrAll.Models;


namespace BarQrAll.Tags
{
    [HtmlTargetElement("barcode")]
        public class BarCodeTagHelper : TagHelper
        {
            public override void Process(TagHelperContext context, TagHelperOutput output)
            {
                var content = context.AllAttributes["content"].Value.ToString();
                var width = int.Parse(context.AllAttributes["width"].Value.ToString());
                var height = int.Parse(context.AllAttributes["height"].Value.ToString());
                var barcodeWriterPixelData = new ZXing.BarcodeWriterPixelData
                {
                    Format = ZXing.BarcodeFormat.CODE_128,
                    Options = new QrCodeEncodingOptions
                    {
                        Height = height,
                        Width = width,
                        Margin = 0
                    }
                };
                var pixelData = barcodeWriterPixelData.Write(content);
                using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                        try
                        {
                            System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                        }
                        finally
                        {
                            bitmap.UnlockBits(bitmapData);
                        }
                        bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                        string fileGuid = Guid.NewGuid().ToString().Substring(0, 4);                              
                        bitmap.Save(@"wwwroot/Images/file-" + fileGuid + ".png", System.Drawing.Imaging.ImageFormat.Png);

                        output.TagName = "img";
                        output.Attributes.Clear();
                        output.Attributes.Add("width", width);
                        output.Attributes.Add("height", height);
                        output.Attributes.Add("src", String.Format("data:image/png;base64,{0}", Convert.ToBase64String(memoryStream.ToArray())));


                    using (MemoryStream ms = new MemoryStream())
                    {
                        Document document = new Document();
                        PdfWriter pdfWriter = PdfWriter.GetInstance(document, new FileStream(@"wwwroot/Pdf/fileBarCode-" + fileGuid + ".pdf", FileMode.Create));
                        PdfWriter.GetInstance(document, ms);
                        pdfWriter.CloseStream = false;
                        document.Open();
                        string imageURL = Path.Combine(@"wwwroot/Images", "file-" + fileGuid + ".png");
                        iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);


                        jpg.ScaleToFit(140f, 120f);

                        //Give space before image

                        jpg.SpacingBefore = 10f;

                        //Give some space after the image

                        jpg.SpacingAfter = 1f;

                        jpg.Alignment = Element.ALIGN_LEFT;


                        document.Add(jpg);
                        document.Close();

                        ms.Flush(); //Always catches me out
                    }



                }
            }
            }
        }
    }