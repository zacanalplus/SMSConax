using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConaxSMS
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class Envelope
    {
        private EnvelopeBody bodyField;

        /// <remarks/>
        public EnvelopeBody Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }

    /// <remarks/>
    /// 
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "http://www.w3.org/2001/XMLSchema")]
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("xsi", "http://www.w3.org/2001/XMLSchema")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    //[System.Xml.Serialization.XmlTypeAttribute("xsd", Namespace = "http://www.w3.org/2001/XMLSchema")]
    //[System.Xml.Serialization.XmlTypeAttribute("xsi", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    public partial class EnvelopeBody
    {
        private SendBarkMessageToClients sendBarkMessageToClientsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.conax.com/cas/xsd/user-text-messaging/v1")]
        public SendBarkMessageToClients SendBarkMessageToClients
        {
            get
            {
                return this.sendBarkMessageToClientsField;
            }
            set
            {
                this.sendBarkMessageToClientsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.conax.com/cas/xsd/user-text-messaging/v1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.conax.com/cas/xsd/user-text-messaging/v1", IsNullable = false)]
    public partial class SendBarkMessageToClients
    {

        private SendBarkMessageToClientsSendBarkMessageToClientsRequest sendBarkMessageToClientsRequestField;

        /// <remarks/>
        public SendBarkMessageToClientsSendBarkMessageToClientsRequest SendBarkMessageToClientsRequest
        {
            get
            {
                return this.sendBarkMessageToClientsRequestField;
            }
            set
            {
                this.sendBarkMessageToClientsRequestField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.conax.com/cas/xsd/user-text-messaging/v1")]
    public partial class SendBarkMessageToClientsSendBarkMessageToClientsRequest
    {

        private string[] caClientIdsField;

        private string messageField;

        private int displayDurationInSecondsField;

        //private System.DateTime startIndicationTimeField;

        private int sequenceNumberField;

        public SendBarkMessageToClientsSendBarkMessageToClientsRequest()
        {
            caClientIdsField = new string[1];
        }
        public SendBarkMessageToClientsSendBarkMessageToClientsRequest(int ncards)
        {
            caClientIdsField = new string[ncards];
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("CaClientId", IsNullable = false)]
        public string[] CaClientIds
        {
            get
            {
                return this.caClientIdsField;
            }
            set
            {
                this.caClientIdsField = value;
            }
        }

        /// <remarks/>
        public string Message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
            }
        }

        /// <remarks/>
        public int DisplayDurationInSeconds
        {
            get
            {
                return this.displayDurationInSecondsField;
            }
            set
            {
                this.displayDurationInSecondsField = value;
            }
        }

        /// <remarks/>
        //public System.DateTime StartIndicationTime
        //{
        //    get
        //    {
        //        return this.startIndicationTimeField;
        //    }
        //    set
        //    {
        //        this.startIndicationTimeField = value;
        //    }
        //}

        /// <remarks/>
        public int SequenceNumber
        {
            get
            {
                return this.sequenceNumberField;
            }
            set
            {
                this.sequenceNumberField = value;
            }
        }
    }
}
