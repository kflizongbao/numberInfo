using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace drawDong
{
    public class HttpClass
    {
        private HttpWebRequest Request = null;
        private WebResponse Response = null;
        private System.IO.Stream Stream = null;
        private System.IO.StreamReader Read = null;
        private System.Byte[] Byte = null;
        private System.Text.Encoding Encode = System.Text.Encoding.UTF8;
        private System.Text.RegularExpressions.Match Match = null;
        public System.Net.WebProxy Proxy = new System.Net.WebProxy();
        public string IPFor = null;
        public bool HideInfo = false;

        public void available()
        {
            Uri uri = new Uri("http://www.litasoft.com/door/dongwan/door.txt");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri); //构建http request
            request.Method = "get";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();

                string content = "";
                using (StreamReader sr = new StreamReader(resStream))
                {
                    content = sr.ReadToEnd();
                }
                Console.WriteLine(content);

                if (content.StartsWith("test_end"))
                {
                    System.Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {

            }
        }














    }


}
