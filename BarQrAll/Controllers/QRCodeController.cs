using Microsoft.AspNetCore.Mvc;
using QRCoder;
using static QRCoder.PayloadGenerator;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using BarQrAll.Models;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace BarQrAll.Controllers
{
    public class QRCodeController : Controller
    {
        public IActionResult Index()
        {
            QRCodeModel model = new QRCodeModel();
            return View(model);
        }


        [HttpPost]
        public IActionResult Index(QRCodeModel model)
        {



            ViewBag.productId = model.BarQRId;


            Payload payload = null;
            if (model.QRCodeType > 0 && model.QRCodeType != 7)
            {
                switch (model.QRCodeType)
                {
                    case 1: // website url
                        payload = new Url(model.WebsiteURL);
                        break;
                    case 2: // bookmark url
                        payload = new Bookmark(model.BookmarkURL, model.BookmarkURL);
                        break;
                    case 3: // compose sms
                        payload = new SMS(model.SMSPhoneNumber, model.SMSBody);
                        break;
                    case 4: // compose whatsapp message
                        payload = new WhatsAppMessage(model.WhatsAppNumber, model.WhatsAppMessage);
                        break;
                    case 5: //compose email
                        payload = new Mail(model.ReceiverEmailAddress, model.EmailSubject, model.EmailMessage);
                        break;
                    case 6: // wifi qr code
                        payload = new WiFi(model.WIFIName, model.WIFIPassword, WiFi.Authentication.WPA);
                        break;
                }
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload);
                QRCode qrCode = new QRCode(qrCodeData);
                var qrCodeAsBitmap = qrCode.GetGraphic(20);
                string base64String = Convert.ToBase64String(BitmapToByteArray(qrCodeAsBitmap));
                model.QRImageURL = "data:image/png;base64," + base64String;


            }
            else
            {
                if (model.QRCodeType == 7)
                {

                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(model.QRText, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    var qrCodeAsBitmap = qrCode.GetGraphic(20);
                    string base64String = Convert.ToBase64String(BitmapToByteArray(qrCodeAsBitmap));
                    model.QRImageURL = "data:image/png;base64," + base64String;




                }

            }
            return View("Index", model);
        }
        private byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                
                    string fileGuid = Guid.NewGuid().ToString().Substring(0, 4);
                    bitmap.Save(ms, ImageFormat.Png);
                    bitmap.Save(@"wwwroot/Images/file-" + fileGuid + ".png", ImageFormat.Png);

                   // string imageURL = @"wwwroot/qrr/file - 5eb9.png";
                Document document = new Document();
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, new FileStream(@"wwwroot/Pdf/fileQRCode-" + fileGuid + ".pdf", FileMode.Create));
                PdfWriter.GetInstance(document, ms);
                pdfWriter.CloseStream = false;
                document.Open();
                string imageURL = Path.Combine(@"wwwroot/Images", "file-"+fileGuid+".png");
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
          

                return ms.ToArray();
            }
        }

         
        
    }
}

