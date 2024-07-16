namespace ConaxSMS
{
    partial class frmConaxSMS
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConaxSMS));
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtCSVPath = new System.Windows.Forms.TextBox();
            this.txtMsg = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.openCSVDialog = new System.Windows.Forms.OpenFileDialog();
            this.sequenceNo = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.duration2Display = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.nFutureDay = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.sequenceNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.duration2Display)).BeginInit();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nFutureDay)).BeginInit();
            this.SuspendLayout();
            // 
            // btnBrowse
            // 
            resources.ApplyResources(this.btnBrowse, "btnBrowse");
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtCSVPath
            // 
            resources.ApplyResources(this.txtCSVPath, "txtCSVPath");
            this.txtCSVPath.Name = "txtCSVPath";
            // 
            // txtMsg
            // 
            resources.ApplyResources(this.txtMsg, "txtMsg");
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.TextChanged += new System.EventHandler(this.txtMsg_TextChanged);
            // 
            // btnSend
            // 
            resources.ApplyResources(this.btnSend, "btnSend");
            this.btnSend.Name = "btnSend";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // openCSVDialog
            // 
            this.openCSVDialog.FileName = "*.csv";
            // 
            // sequenceNo
            // 
            resources.ApplyResources(this.sequenceNo, "sequenceNo");
            this.sequenceNo.Name = "sequenceNo";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // duration2Display
            // 
            resources.ApplyResources(this.duration2Display, "duration2Display");
            this.duration2Display.Name = "duration2Display";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsStatusLabel});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // tsStatusLabel
            // 
            this.tsStatusLabel.Name = "tsStatusLabel";
            resources.ApplyResources(this.tsStatusLabel, "tsStatusLabel");
            // 
            // nFutureDay
            // 
            resources.ApplyResources(this.nFutureDay, "nFutureDay");
            this.nFutureDay.Name = "nFutureDay";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // frmConaxSMS
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nFutureDay);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.duration2Display);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sequenceNo);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtMsg);
            this.Controls.Add(this.txtCSVPath);
            this.Controls.Add(this.btnBrowse);
            this.MaximizeBox = false;
            this.Name = "frmConaxSMS";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmConaxSMS_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.sequenceNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.duration2Display)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nFutureDay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtCSVPath;
        private System.Windows.Forms.TextBox txtMsg;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.OpenFileDialog openCSVDialog;
        private System.Windows.Forms.NumericUpDown sequenceNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown duration2Display;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsStatusLabel;
        private System.Windows.Forms.NumericUpDown nFutureDay;
        private System.Windows.Forms.Label label1;
    }
}

