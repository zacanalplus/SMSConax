using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;

namespace ConaxSMS
{
    public partial class frmConaxSMS : Form
    {
        public bool CSV_LoadOK { get; set; }
        private List<string> scList = new List<string>();
        private SoapEnvelop envp = new SoapEnvelop();
        private AppConfigurator cfg = new AppConfigurator(Application.ExecutablePath);
        private int scserialLEN, scserialLENOK;
        int maxSTB;

        public bool hasSC
        {
            get
            {
                if (scList == null)
                    return false;
                return scList.Count > 0;
            }
        }
        private bool validateMsgLen()
        {
            try
            {
                int maxMsgLen = int.Parse(cfg.GetConfigValue(Properties.Resources.MsgLenCfgKey));
                if (txtMsg.Text.Length > maxMsgLen || txtMsg.Text.Length < 0)
                {
                    return false;
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
        public frmConaxSMS()
        {
            //MessageBox.Show(Properties.Resources.WelcomeMessage, "Welcome Message", MessageBoxButtons.OK);
            InitializeComponent();
            
            try
            {
                sequenceNo.Value = int.Parse(cfg.GetConfigValue("DefaultSequenceNo"));
                int delaySecs = int.Parse(cfg.GetConfigValue("DefaultDelaySec"));
                int defaultDurSecs = int.Parse(cfg.GetConfigValue("DurationSecs"));
                scserialLEN = int.Parse(cfg.GetConfigValue("ConaxSCLengthInCSV"));
                scserialLENOK = int.Parse(cfg.GetConfigValue("ConaxSCLengthAccepted"));
                maxSTB = int.Parse(cfg.GetConfigValue("MaxSTB")); // to Number of maximun STB function can send message
                //dateTimeTransmit.Value = DateTime.Now.AddSeconds(delaySecs);
                duration2Display.Value = defaultDurSecs;
            }
            catch (Exception)
            {
                sequenceNo.Value = 11;
                duration2Display.Value = 10;
            }
            Logger.Open(cfg.GetConfigValue("logpath"));
            Logger.Debugging = (cfg.GetConfigValue("logging") == "1");
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            openCSVDialog.ShowDialog();
            if (openCSVDialog.FileName != "")
            {
                txtCSVPath.Text = openCSVDialog.FileName;
                if (File.Exists(txtCSVPath.Text))
                {
                    CSVParser p = new CSVParser(txtCSVPath.Text);
                    scList = p.LoadL();
                }
                //removeSCLastDigit();
            }
        }
        private int composeRequest(DateTime dtStamp, int durSecs, int seqNum)
        {
            if (scList == null || scList.Count < 1)
                return -1;
            envp.AddRequest(cfg, scList, txtMsg.Text, durSecs, seqNum);
            return 0;
        }
        //private void removeSCLastDigit()
        //{
        //    if (scserialLEN <= scserialLENOK)
        //        return;
        //    for(int i=0; i<scList.Count; i++)
        //    {
        //        if(scList[i].Length == scserialLEN)
        //            scList[i] = scList[i].Substring(0, scserialLENOK);
        //    }
        //}

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!validateMsgLen())
            {
                MessageBox.Show(Properties.Resources.MessageLenError, "Error");
                return;
            }
            /* Future Day Delay by Interface
            if (nFutureDay.Value > 0)
            {
                DialogResult usrResp = MessageBox.Show("You are asking program to delay for "+ nFutureDay.Value+ " days. " + Properties.Resources.DelayMsg
                    , "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if(usrResp == DialogResult.OK)
                {
                    TimeSpan ts = new TimeSpan((int)nFutureDay.Value, 0, 0, 0);
                    System.Threading.Thread.Sleep(ts);
                }
            }*/
            List<string> chunkSCList = new List<string>();
            bool errInSending = false;
            string oneErrMsg = "";
            string oneOKMsg = "";
            for (int i = 0; i < scList.Count; i += maxSTB)
            {
                chunkSCList = scList.GetRange(i, Math.Min(maxSTB, scList.Count - i));

                string retVal = SoapCaller.CallWebService(cfg, chunkSCList, txtMsg.Text, (int)duration2Display.Value, (int)sequenceNo.Value);
                //string retVal = SoapCaller.PostXMLRequest(cfg, scList, txtMsg.Text, (int)duration2Display.Value, (int)sequenceNo.Value);
                int pFrom = retVal.IndexOf(Properties.Resources.ReturnMsgOpen) + Properties.Resources.ReturnMsgOpen.Length;
                int pTo = retVal.LastIndexOf(Properties.Resources.ReturnMsgClose);
                String retMsg = "No Error";
                if (pTo - pFrom > 0)
                    retMsg = retVal.Substring(pFrom, pTo - pFrom);

                if (retVal.Contains(cfg.GetConfigValue("SUCCESS")))
                {
                    oneOKMsg = "Message delivered for Smarcart list from item No." + (i + 1) + " to " + chunkSCList.Count + " is OK!";
                    Logger.Write(oneOKMsg);
                }
                else
                {
                    oneErrMsg = Properties.Resources.ErrorResponse + " ; Error Reason -> " + retMsg + " while sending message item No." + (i + 1) + " to " + chunkSCList.Count;
                    Logger.Write(oneErrMsg);
                    errInSending = true;
                }

                chunkSCList.Clear();
            }
            if (errInSending)
            {
                MessageBox.Show(oneErrMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(oneOKMsg, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
                
            Logger.Flush();
        }
        private void IncreaseSequence()
        {

        }

        private void txtMsg_TextChanged(object sender, EventArgs e)
        {
            tsStatusLabel.Text = "The character count is " + Encoding.UTF8.GetBytes(txtMsg.Text).Length;
        }

        private void frmConaxSMS_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Logger.Close();
        }
    }
}
