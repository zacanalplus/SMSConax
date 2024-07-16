using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.IdentityModel.Tokens;
using System.Security;
using System.Net.Security;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Converters;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace ConaxSMS
{
    public class RequestState
    {
        // This class stores the state of the request.
        public const int BUFFER_SIZE = 8192;
        public StringBuilder requestData;
        public byte[] bufferRead;
        public WebRequest request;
        public WebResponse response;
        public Stream responseStream;
        public RequestState()
        {
            bufferRead = new byte[BUFFER_SIZE];
            requestData = new StringBuilder("");
            request = null;
            responseStream = null;
        }
    }
    class SoapCaller
    {
        //private static XmlDocument xResp = new XmlDocument();

        public static string CallWebService(AppConfigurator cfg, List<string> scList, string msg, int durSecs, int seqNum)
        {
            try
            {
                SoapEnvelop senvp = new SoapEnvelop();
                //// Start Old XML Generation
                XElement reqDet = senvp.AddRequest(cfg, scList, msg, durSecs, seqNum);
                XmlDocument soapEnvelopeXml = CreateSoapEnvelope(reqDet);
                //Logger.Write(soapEnvelopeXml.Value.ToString());
                //// End Old XML Generation

                //// Start New XML Generation
                //string strXml = senvp.GetRequestXml(cfg, scList, msg, durSecs, seqNum);
                //XmlDocument soapEnvelopeXml = new XmlDocument();
                //soapEnvelopeXml.LoadXml(strXml);

                //Logger.Write("Request XML Content Length = " + Encoding.UTF8.GetBytes(strXml).Length);
                //Logger.Write(" Request XML");
                //Logger.Write(strXml);
                //// End New XML Generation

                HttpWebRequest webRequest = CreateWebRequest(cfg, cfg.GetConfigValue("wsURIL"), cfg.GetConfigValue("SoapActionVerb"), 
                    cfg.GetConfigValue("authHead"), cfg.GetConfigValue("user"), cfg.GetConfigValue("pwd"));
                InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
                Logger.Write(webRequest);
                //AddCertificate(cfg, webRequest, cfg.GetConfigValue("CertSerial"));

                RequestState myRequestState = new RequestState();
                myRequestState.request = webRequest;
               // begin async call to web request.
               IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();

                // get the response from the completed web request.
                //string soapResult;
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    myRequestState.response = webResponse;
                    Stream responseStream = myRequestState.response.GetResponseStream();
                    myRequestState.responseStream = responseStream;
                    
                    // Using big buffer length
                    //IAsyncResult asynchronousResultRead = responseStream.BeginRead(myRequestState.bufferRead, 0, RequestState.BUFFER_SIZE, null, myRequestState);
                    // Using response actual length
                    IAsyncResult asynchronousResultRead = responseStream.BeginRead(myRequestState.bufferRead, 0, (int)myRequestState.response.ContentLength, null, myRequestState);
                    int read = responseStream.EndRead(asynchronousResultRead);

                    string respMsg = System.Text.Encoding.Default.GetString(myRequestState.bufferRead);
                    Logger.Write("response by response stream reader\r\n" + respMsg);

                    // Json log entry is deprecated on 23 Feb 2018 
                    //Logger.Write("Get response by Json reader");
                    //Logger.Write(JsonConvert.SerializeObject(webResponse, Newtonsoft.Json.Formatting.Indented));

                    // Just return normal
                    return respMsg;

                    // Returning Json string
                    //return (JsonConvert.SerializeObject(webResponse, Newtonsoft.Json.Formatting.Indented));

                    //XmlDocument xmlDoc = new XmlDocument();
                    //xmlDoc.Load(webResponse.GetResponseStream());
                    //using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    //{
                    //    soapResult = rd.ReadToEnd();
                    //}
                    //XmlDocument xResp = new XmlDocument();
                    //xResp.LoadXml(soapResult);
                    //XmlNamespaceManager manager = new XmlNamespaceManager(xResp.NameTable);
                    //manager.AddNamespace(cfg.GetConfigValue("resNSTypeTag"), cfg.GetConfigValue("respNSType"));

                    //Logger.Write("Response From ");
                    //Logger.Write(soapResult);
                    //return xmlDoc.DocumentElement.SelectSingleNode(cfg.GetConfigValue("RetCodTag")).Value;
                    //return (soapResult);
                }
            }
            catch (System.Net.WebException wEx)
            {
                Logger.Write("Web Error: " + wEx.Message);
                LogWebExceptionDetails(wEx);
                return wEx.Message;
            }
            catch(XmlException ex)
            {
                Logger.Write(Properties.Resources.RespXMLErr + " " + ex.Message);
                return ex.Message;
            }
        }

        public static string PostXMLRequest(AppConfigurator cfg, List<string> scList, string msg, int durSecs, int seqNum)
        {
            try
            {
                //Uri uri = new Uri("https://134.47.4.3:44302/ca-server/webservices/caclient/user-text-messaging");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cfg.GetConfigValue("wsURIL"));
                

                string postData = GetRequestXml(cfg, scList, msg, durSecs, seqNum);

                Logger.Write("----------- Begin Request XML -----------");
                Logger.Write(postData);
                Logger.Write("----------- End Request XML -----------");
                byte[] data = Encoding.UTF8.GetBytes(postData);
                XmlDocument soapEnvelopeXml = new XmlDocument();
                soapEnvelopeXml.LoadXml(postData);
                //Logger.Write()

                request.Method = "POST";
                
                request.ContentType = "application/xml";
                request.KeepAlive = true;
                request.UserAgent = "Apache-HttpClient/4.5.2";
                
                //request.ContentLength = data.Length;
                ServicePointManager.ServerCertificateValidationCallback = new
                    RemoteCertificateValidationCallback
                    (
                        delegate { return true; }
                    );

                request.Credentials = new NetworkCredential(cfg.GetConfigValue("user"), cfg.GetConfigValue("pwd"));
                request.ClientCertificates.Add(GetCertificate(cfg));
                //InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, request);
                using (Stream stream = request.GetRequestStream())
                {
                    soapEnvelopeXml.Save(stream);
                }

                //using (var stream = request.GetRequestStream())
                //{
                //    stream.Write(data, 0, data.Length);
                //}
                //RequestState myRequestState = new RequestState();
                //myRequestState.request = request;
                // begin async call to web request.
                //IAsyncResult asyncResult = request.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                //asyncResult.AsyncWaitHandle.WaitOne();
                //HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch(WebException ex)
            {
                Logger.Write("Web Error: " + ex.Message);

                LogWebExceptionDetails(ex);
                return "Error";
            }
        }
        public static void LogWebExceptionDetails(WebException ex)
        {
            Logger.Write(ex.StackTrace);
            //Logger.Write(ex.Data.)
            WebResponse errResp = ex.Response;
            using (Stream respStream = errResp.GetResponseStream())
            {
                StreamReader reader = new StreamReader(respStream);
                string text = reader.ReadToEnd();
                Logger.Write("Error Details: " + text);
            }
        }

        public static string GetRequestXml(AppConfigurator cfg, List<string> scList, string msg, int durSecs, int seqNum)
        {
            int defaultSCLEN = 11;
            // Request Xml
            defaultSCLEN = int.Parse(cfg.GetConfigValue("ConaxSCLengthAccepted"));
            SendBarkMessageToClientsSendBarkMessageToClientsRequest req = new SendBarkMessageToClientsSendBarkMessageToClientsRequest(scList.Count);
            for (int i = 0; i < scList.Count; i++)
            {
                req.CaClientIds[i] = scList[i].Substring(0, defaultSCLEN);
            }
            req.DisplayDurationInSeconds = durSecs;
            req.Message = msg;
            req.SequenceNumber = seqNum;
            // Envelope and body
            SendBarkMessageToClients cmd = new SendBarkMessageToClients();
            cmd.SendBarkMessageToClientsRequest = req;

            //XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            //ns.Add("s", "http://schemas.xmlsoap.org/soap/envelope/");

            EnvelopeBody xbody = new EnvelopeBody();
            xbody.SendBarkMessageToClients = cmd;

            Envelope e = new Envelope();
            e.Body = xbody;
            //Converting Xml to String
            XmlSerializer serializer = new XmlSerializer(typeof(Envelope));
            StringWriter sw = new StringWriter();
            serializer.Serialize(sw, e);
            return sw.ToString();
        }

        public static string CallWebService(AppConfigurator cfg, string url, XElement reqDet, string action, string authHead, string uname, string pwd)
        {
            try
            {
                XmlDocument soapEnvelopeXml = CreateSoapEnvelope(reqDet);
                Logger.Write("Request XML ");
                Logger.Write(soapEnvelopeXml.ToString());
                HttpWebRequest webRequest = CreateWebRequest(cfg, url, action, authHead, uname, pwd);
                InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
                Logger.Write(webRequest.ToString());

                // begin async call to web request.
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();

                // get the response from the completed web request.
                string soapResult;
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }
                    Logger.Write("Response From ");
                    Logger.Write(soapResult);
                    return (soapResult);
                }
            }
            catch (System.Net.WebException wEx)
            {
                return wEx.Message;
            }


            //return "Nothing";
        }
        public static HttpWebRequest CreateWebRequest(AppConfigurator cfg, string url, string action, string authHead, string uname, string pwd)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add(AuthenticationToken(authHead, uname, pwd));
            //webRequest.Headers.Add(Properties.Resources.SoapActionVerb, action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            
            //webRequest.Accept = "text/xml";
            //webRequest.ContentLength=?
            webRequest.Method = "POST";
            webRequest.Credentials = new NetworkCredential(cfg.GetConfigValue("user"), cfg.GetConfigValue("pwd"));
            webRequest.ClientCertificates.Add(GetCertificate(cfg));

            ServicePointManager.ServerCertificateValidationCallback = new
                RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

            return webRequest;
        }
        public static X509Certificate GetCertificate(AppConfigurator cfg)
        {
            X509Store certStore = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);
            if (certStore.Certificates.Count > 0)
            {
                X509Certificate2Collection col = certStore.Certificates.Find(X509FindType.FindBySerialNumber, cfg.GetConfigValue("CertSerial"), true);
                if (col.Count == 1)
                {
                    return col[0];
                }
            }
            throw new CertifcateNotFound();
        }
        public static string AuthenticationToken(string header, string uname, string pwd)
        {
            return header + Base64Encode(uname + ":" + pwd);
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static XmlDocument CreateSoapEnvelope(XElement reqDet)
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();

            string requestXml = reqDet.ToString();
            string adhoc_search_text = "<SendBarkMessageToClientsRequest xmlns=\"\">";
            string adhoc_replace_text = "<SendBarkMessageToClientsRequest>";
            requestXml = requestXml.Replace(adhoc_search_text, adhoc_replace_text);
            soapEnvelopeDocument.LoadXml(requestXml);
            Logger.Write(requestXml);
            return soapEnvelopeDocument;
        }
        private string Replace(string inputString, string replacementString, string pattern, int backreferenceGroupNumber)
        {
            return inputString.Replace(Regex.Match(inputString, pattern).Groups[backreferenceGroupNumber].Value, replacementString);
        }
        public X509SecurityToken GetSecurityToken(string cerSerial)
        {
            X509SecurityToken stok = null;
            X509Store certStore = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);
            if (certStore.Certificates.Count > 0)
            {
                X509Certificate2Collection col = certStore.Certificates.Find(X509FindType.FindBySerialNumber, cerSerial, true);
                
                if (col.Count > 0)
                {
                    X509Certificate2 cert = col[0];
                    if (cert != null)
                    {
                        stok = new X509SecurityToken(cert);
                    }
                }
            }

            return stok;
        }
        //public void SecureMessage(SoapEnvelope envelope, Security security, AppConfigurator cfg)
        //{
        //    X509SecurityToken signatureToken = GetSecurityToken(cfg.GetConfigValue("CertSerial"));
        //    if (signatureToken == null)
        //    {
        //        throw new CertifcateNotFound();
        //    }

        //    // Add the security token.                
        //    security.Tokens.Add(signatureToken);
        //    // Specify the security token to sign the message with.
        //    MessageSignature sig = new MessageSignature(signatureToken);

        //    security.Elements.Add(sig);

        //}
        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
            Logger.Write("Webrequest Content Length = " + webRequest.ContentLength.ToString());
        }
    }
    class SoapEnvelop
    {
        XmlDocument tmpl;
        public string xmlFilePath { get; set; }
        //private string defaultRoot = "root";
        private XElement root;

        //public const string nsURIKey = "NamespaceURI";

        public SoapEnvelop() { }

        public SoapEnvelop(string xmlPath)
        {
            xmlFilePath = xmlPath;
            if (!File.Exists(xmlFilePath))
                throw new XmlFileNotFoundException("Xml File " + xmlFilePath + " not found");
            tmpl = new XmlDocument();
            tmpl.Load(xmlFilePath);

        }
        public SoapEnvelop(string xmlPath, string rootName) : this(xmlPath)
        {
            //root = tmpl.GetElementById(rootName != "" ? rootName : defaultRoot);
        }

        public SoapEnvelop(AppConfigurator cfg)
        {
            XNamespace s = cfg.GetConfigValue("envlope");
            //XElement child = new XElement(s + "Body", new XAttribute(XNamespace.Xmlns+ "xsd", cfg.GetConfigValue("bodyxsd")));
            //child.Add(new XAttribute(XNamespace.Xmlns + "xsi", cfg.GetConfigValue("bodyxsi")));
            root = new XElement(s + "Envelope",
                new XAttribute(XNamespace.Xmlns + "s", cfg.GetConfigValue("envlope")),
                new XElement(s + "Body", new XAttribute(XNamespace.Xmlns + "xsd", cfg.GetConfigValue("bodyxsd")),
                new XAttribute(XNamespace.Xmlns + "xsi", cfg.GetConfigValue("bodyxsi")))
            );

        }
        public string GetRequestXmlWithoutEnvelope(AppConfigurator cfg, List<string> scList, string msg, int durSecs, int seqNum)
        {
            int defaultSCLEN = 11;
            // Request Xml
            defaultSCLEN = int.Parse(cfg.GetConfigValue("ConaxSCLengthAccepted"));
            SendBarkMessageToClientsSendBarkMessageToClientsRequest req = new SendBarkMessageToClientsSendBarkMessageToClientsRequest(scList.Count);
            for (int i = 0; i < scList.Count; i++)
            {
                req.CaClientIds[i] = scList[i].Substring(0, defaultSCLEN);
            }
            req.DisplayDurationInSeconds = durSecs;
            req.Message = msg;
            req.SequenceNumber = seqNum;
            // Envelope and body
            SendBarkMessageToClients cmd = new SendBarkMessageToClients();
            cmd.SendBarkMessageToClientsRequest = req;
            //Converting Xml to String
            XmlSerializer serializer = new XmlSerializer(typeof(SendBarkMessageToClients));
            StringWriter sw = new StringWriter();
            serializer.Serialize(sw, cmd);
            Logger.Write(" Raw request created: \r\n" + sw.ToString());
            return sw.ToString();
        }

        public string GetRequestXml(AppConfigurator cfg, List<string> scList, string msg, int durSecs, int seqNum)
        {
            int defaultSCLEN = 11;
            // Request Xml
            defaultSCLEN = int.Parse(cfg.GetConfigValue("ConaxSCLengthAccepted"));
            SendBarkMessageToClientsSendBarkMessageToClientsRequest req = new SendBarkMessageToClientsSendBarkMessageToClientsRequest(scList.Count);
            for(int i=0; i<scList.Count; i++)
            {
                req.CaClientIds[i] = scList[i].Substring(0, defaultSCLEN);
            }
            req.DisplayDurationInSeconds = durSecs;
            req.Message = msg;
            req.SequenceNumber = seqNum;
            // Envelope and body
            SendBarkMessageToClients cmd = new SendBarkMessageToClients();
            cmd.SendBarkMessageToClientsRequest = req;

            EnvelopeBody xbody = new EnvelopeBody();
            xbody.SendBarkMessageToClients = cmd;

            Envelope e = new Envelope();
            e.Body = xbody;
            //Converting Xml to String
            XmlSerializer serializer = new XmlSerializer(typeof(Envelope));
            StringWriter sw = new StringWriter();
            serializer.Serialize(sw, e);
            return sw.ToString();
        }

        // Heavily tied to XML format of Conax request!!!
        public XElement AddRequest(AppConfigurator cfg, List<string> scList, string msg, int durSecs, int seqNum)
        {
            XNamespace s = cfg.GetConfigValue("envlope");
            //XElement child = new XElement(s + "Body", new XAttribute(XNamespace.Xmlns+ "xsd", cfg.GetConfigValue("bodyxsd")));
            //child.Add(new XAttribute(XNamespace.Xmlns + "xsi", cfg.GetConfigValue("bodyxsi")));
            XElement caclis = new XElement(cfg.GetConfigValue("SCIDSKey"));
            //XElement funcEle = new XElement(cfg.GetConfigValue("funcName"));
            if (scList.Count > 0)
            {
                foreach (string scid in scList)
                {
                    caclis.Add(new XElement(cfg.GetConfigValue("SCIDKey"), scid.Substring(0,11)));
                }
            }
            XNamespace defaultNS = XNamespace.None;
            //XName empty="";
            XElement funcEle = new XElement(cfg.GetConfigValue("funcName"), caclis,
                new XElement(cfg.GetConfigValue("MsgKey"), msg),
                new XElement(cfg.GetConfigValue("DurationKey"), durSecs),
                // new XElement(cfg.GetConfigValue("DTKey"), stdt.ToString("s")),  // Removed by comment of Imran@Conax
                new XElement(cfg.GetConfigValue("SeqKey"), seqNum.ToString())
               );
            //funcEle.Add();
            //funcEle.Add();
            //funcEle.Add();

            XNamespace reqNSpace = cfg.GetConfigValue("funcNamespaceVal");
            root = new XElement(s + "Envelope",
                new XAttribute(XNamespace.Xmlns + "s", cfg.GetConfigValue("envlope")),
                new XElement(s + "Body", new XAttribute(XNamespace.Xmlns + "xsd", cfg.GetConfigValue("bodyxsd")),
                new XAttribute(XNamespace.Xmlns + "xsi", cfg.GetConfigValue("bodyxsi")),
                new XElement(reqNSpace + cfg.GetConfigValue("funcNamespace"), funcEle))
            );
            //Logger.Write(root.ToString());
            
            return root;
        }

        public void DebugOnTheFlyXml()
        {
            System.IO.File.WriteAllText(xmlFilePath, root.ToString());
        }

        public void SaveXml()
        {
            tmpl.Save(xmlFilePath);
        }
        public void AddElement(string parentName, string attrName, string attrVal)
        {
            XmlNode parent = tmpl.SelectSingleNode("/" + parentName);
            XmlNode child = tmpl.CreateElement(attrName);
            child.InnerText = attrVal;
            parent.AppendChild(child);
        }
    }
    class XmlFileNotFoundException : FileNotFoundException
    {
        public XmlFileNotFoundException() : base("Missing Xml template file") { }
        public XmlFileNotFoundException(string msg) : base(msg) { }
        public XmlFileNotFoundException(string msg, Exception inner) : base(msg, inner) { }
    }
    class CertifcateNotFound : Exception
    {
        public CertifcateNotFound() : base(Properties.Resources.CertificateNotFoundDesc) { }
        public CertifcateNotFound(string msg) : base(msg) { }
        public CertifcateNotFound(string msg, Exception inner) : base(msg, inner) { }
    }
}
