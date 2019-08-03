using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace QrCodeCreation
{
    public interface IQrCodeCreatorService
    {
        bool UriToQrCode(string url);
        bool UriToQrCode(Uri uri);
    }
}
