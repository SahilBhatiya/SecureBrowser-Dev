using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SecureBrowser.Functions
{
    public class GetIpAddress
    {
        public static string Fetch()
        {
            String address = "";
            WebRequest request = null;
            try
            {
                request = WebRequest.Create("http://checkip.dyndns.org/");
                using (WebResponse response = request.GetResponse())
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    address = stream.ReadToEnd();
                }

                int first = address.IndexOf("Address: ") + 9;
                int last = address.LastIndexOf("</body>");
                address = address.Substring(first, last - first);
            }
            catch
            {
                address = "Offline";
            }
            return address;
        }
    }
}
