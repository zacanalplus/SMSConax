using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConaxSMS
{
    public static class Logger
    {
        private static System.IO.StreamWriter file;
        static bool OK2Write = false;
        public static bool Debugging = true;
        public static string FilePath = "";
        //public static string FilePath(string fpath)
        //{
        //    return path = fpath;
        //}
        //static Logger()
        //{
            
        //}
        static public void Open(string logpath)
        {
            FilePath = logpath;
            if (FilePath.Length > 0)
            {
                file = new System.IO.StreamWriter(FilePath, true);
                OK2Write = true;
            }
        }
        static public void Write(string lines)
        {
            if (OK2Write && Debugging)
                file.WriteLine(DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString() + lines);
        }
        static public void Write(HttpWebRequest request)
        {
            if (OK2Write && Debugging)
            {
                file.WriteLine(Properties.Resources.BeginLogEntry);
                file.WriteLine("Address " + request.Address);
                file.WriteLine("Authentication Level " + request.AuthenticationLevel);
                file.WriteLine("Connection Header " + request.Connection);
                file.WriteLine("Content Type " + request.ContentType);
                file.WriteLine("Content Length " + request.ContentLength);
                file.WriteLine("Host " + request.Host);
                file.WriteLine("Method " + request.Method);
                file.WriteLine("User Agent " + request.UserAgent);
                file.WriteLine("Request URI " + request.RequestUri);
                file.WriteLine("Proxy " + request.Proxy);
                file.WriteLine(Properties.Resources.EndLogEntry);
            }
        }
        static public void Write(HttpWebResponse resp)
        {
            if (OK2Write && Debugging)
            {
                file.WriteLine(Properties.Resources.BeginLogEntry);
                file.WriteLine("Character Set " + resp.CharacterSet);
                file.WriteLine("Encoding " + resp.ContentEncoding);
                file.WriteLine("Content Type " + resp.ContentType);
                file.WriteLine("Content Length " + resp.ContentLength);
                file.WriteLine("Header " + resp.Headers);
                file.WriteLine("Method " + resp.Method);
                file.WriteLine("Protocol version " + resp.ProtocolVersion);
                file.WriteLine("Response URI " + resp.ResponseUri);
                file.WriteLine("Server " + resp.Server);
                file.WriteLine("Status code " + resp.StatusCode);
                file.WriteLine("Status description " + resp.StatusDescription);
                file.WriteLine("Supports Header " + resp.SupportsHeaders);
                file.WriteLine(Properties.Resources.EndLogEntry);
            }
        }
        static public void Write(HttpWebResponse response, bool dump)
        {
            if (OK2Write && Debugging)
            {
                file.WriteLine(Properties.Resources.BeginLogEntry);
                Stream input = response.GetResponseStream();

                byte[] buffer = new byte[8192];
                int bytesRead;
                while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    file.Write(buffer);
                }
                file.WriteLine();
                file.WriteLine(Properties.Resources.EndLogEntry);
            }
        }
        static public void Write(WebResponse resp)
        {

            if (OK2Write && Debugging)
            {
                file.WriteLine(Properties.Resources.BeginLogEntry);
                file.WriteLine("Content Type " + resp.ContentType);
                file.WriteLine("Content Length " + resp.ContentLength);
                file.WriteLine("Header " + resp.Headers);
                file.WriteLine("Response URI " + resp.ResponseUri);
                file.WriteLine("Supports Header " + resp.SupportsHeaders);
                file.WriteLine(Properties.Resources.EndLogEntry);
                //    file.WriteLine(Properties.Resources.BeginLogEntry);
                //    Stream input = response.GetResponseStream();

                //    byte[] buffer = new byte[8192];
                //    int bytesRead;
                //    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                //    {
                //        file.Write(buffer);
                //    }
                //    file.WriteLine();
                //    file.WriteLine(Properties.Resources.EndLogEntry);
            }
        }
        static public void Write(WebResponse resp, bool dump)
        {
            if (OK2Write && Debugging)
            {
                file.WriteLine(Properties.Resources.BeginLogEntry);
                Stream input = resp.GetResponseStream();

                byte[] buffer = new byte[8192];
                int bytesRead;
                while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    file.Write(buffer);
                }
                file.WriteLine();
                file.WriteLine(Properties.Resources.EndLogEntry);
            }
        }
        static public void Close()
        {
            file.Close();
        }
        static public void Flush()
        {
            file.Flush();
        }
    }
}
