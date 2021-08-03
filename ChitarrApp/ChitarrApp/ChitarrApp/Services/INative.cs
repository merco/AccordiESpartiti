using System;
using System.Collections.Generic;
using System.Text;

namespace ChitarrApp.Services
{
    public interface INative
    {
        void SendLink(string newUrl);
        void OpenPDFLink(string newUrl);
        String getPath(string fname);
        String getDownloadPath();
        String convertToMp3(string fname);
        void OpenFolder(string pathbase);
    }
}
