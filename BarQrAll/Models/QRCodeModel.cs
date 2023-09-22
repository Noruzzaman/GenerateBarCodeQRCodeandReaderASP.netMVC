namespace BarQrAll.Models
{
    public class QRCodeModel
    {
         public string BarQRId { get; set; }

        public int QRCodeType { get; set; }
        public string QRImageURL { get; set; }

        public string QRText { get; set; }
        //Bookmark qr code
        public string BookmarkTitle { get; set; }
        public string BookmarkURL { get; set; }
        // Email qr code
        public string ReceiverEmailAddress { get; set; }
        public string EmailSubject { get; set; }
        public string EmailMessage { get; set; }
        //Sms qr code
        public string SMSPhoneNumber { get; set; }
        public string SMSBody { get; set; }
        //Website code
        public string WebsiteURL { get; set; }
        //Whatsapp qr message code
        public string WhatsAppNumber { get; set; }
        public string WhatsAppMessage { get; set; }
        //Wifi qr code
        public string WIFIName { get; set; }
        public string WIFIPassword { get; set; }
    }

}
