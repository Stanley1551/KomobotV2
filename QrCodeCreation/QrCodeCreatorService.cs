using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using QRCoder;

namespace QrCodeCreation
{
    public class QrCodeCreatorService : IQrCodeCreatorService
    {
        public bool UriToQrCode(string url)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                qrCodeImage.Save("temp.jpg", ImageFormat.Jpeg);
                return true;
            }
            catch(Exception e) { throw e; }
            
        }

        public bool UriToQrCode(Uri uri)
        {
            return UriToQrCode(uri.OriginalString);
        }
    }
}
