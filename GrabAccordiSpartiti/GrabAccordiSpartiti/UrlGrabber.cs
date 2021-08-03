
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GrabAccordiSpartiti
{
    public class UrlGrabber
    {
        public static string GetURL(String URL)
        {
            WebRequest request = WebRequest.Create(URL);
         
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();
            string html = String.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            return html;
        }
        public async static Task< string > GetURLChrome(String URL)
        {
            Browser browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            try
            {

                Page page = await browser.NewPageAsync();
                await page.GoToAsync(URL);
                string content = await page.GetContentAsync();
                await browser.CloseAsync();
                return content;
            } catch (Exception E)
            {
                return "";
            }

        }
    }
}
