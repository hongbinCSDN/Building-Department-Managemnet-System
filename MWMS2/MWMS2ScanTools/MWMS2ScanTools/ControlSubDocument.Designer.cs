namespace MWMS2ScanTools
{
    partial class ControlSubDocument
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlSubDocument));
            this.buttonThumbnails = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label5 = new System.Windows.Forms.Label();
            this.fDOC_DESC = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.fDSN_SUB = new System.Windows.Forms.TextBox();
            this.fFOLDER_TYPE = new System.Windows.Forms.ComboBox();
            this.fFORM_TYPE = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.fDOCUMENT_TYPE = new System.Windows.Forms.ComboBox();
            this.fSUBMIT_TYPE = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.axAcroPDF1 = new AxAcroPDFLib.AxAcroPDF();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axAcroPDF1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonThumbnails
            // 
            this.buttonThumbnails.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonThumbnails.Location = new System.Drawing.Point(197, 7);
            this.buttonThumbnails.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonThumbnails.Name = "buttonThumbnails";
            this.buttonThumbnails.Size = new System.Drawing.Size(232, 40);
            this.buttonThumbnails.TabIndex = 70;
            this.buttonThumbnails.Text = "Show Thumbnails";
            this.buttonThumbnails.UseVisualStyleBackColor = true;
            this.buttonThumbnails.Click += new System.EventHandler(this.buttonThumbnails_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.buttonThumbnails);
            this.splitContainer1.Panel1.Controls.Add(this.fDOC_DESC);
            this.splitContainer1.Panel1.Controls.Add(this.label37);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.fDSN_SUB);
            this.splitContainer1.Panel1.Controls.Add(this.fFOLDER_TYPE);
            this.splitContainer1.Panel1.Controls.Add(this.fFORM_TYPE);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.fDOCUMENT_TYPE);
            this.splitContainer1.Panel1.Controls.Add(this.fSUBMIT_TYPE);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.axAcroPDF1);
            this.splitContainer1.Size = new System.Drawing.Size(1263, 795);
            this.splitContainer1.SplitterDistance = 438;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 78;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(14, 232);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(175, 20);
            this.label5.TabIndex = 88;
            this.label5.Text = "Document Description :";
            // 
            // fDOC_DESC
            // 
            this.fDOC_DESC.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fDOC_DESC.Location = new System.Drawing.Point(197, 229);
            this.fDOC_DESC.Multiline = true;
            this.fDOC_DESC.Name = "fDOC_DESC";
            this.fDOC_DESC.Size = new System.Drawing.Size(237, 123);
            this.fDOC_DESC.TabIndex = 87;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.ForeColor = System.Drawing.Color.Black;
            this.label37.Location = new System.Drawing.Point(96, 57);
            this.label37.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(93, 20);
            this.label37.TabIndex = 77;
            this.label37.Text = "Sub - DSN :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(89, 198);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 20);
            this.label4.TabIndex = 86;
            this.label4.Text = "Folder Type :";
            // 
            // fDSN_SUB
            // 
            this.fDSN_SUB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fDSN_SUB.Enabled = false;
            this.fDSN_SUB.Location = new System.Drawing.Point(197, 57);
            this.fDSN_SUB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fDSN_SUB.Name = "fDSN_SUB";
            this.fDSN_SUB.Size = new System.Drawing.Size(237, 26);
            this.fDSN_SUB.TabIndex = 78;
            // 
            // fFOLDER_TYPE
            // 
            this.fFOLDER_TYPE.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fFOLDER_TYPE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fFOLDER_TYPE.FormattingEnabled = true;
            this.fFOLDER_TYPE.Location = new System.Drawing.Point(197, 195);
            this.fFOLDER_TYPE.Name = "fFOLDER_TYPE";
            this.fFOLDER_TYPE.Size = new System.Drawing.Size(237, 28);
            this.fFOLDER_TYPE.TabIndex = 85;
            // 
            // fFORM_TYPE
            // 
            this.fFORM_TYPE.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fFORM_TYPE.Enabled = false;
            this.fFORM_TYPE.Location = new System.Drawing.Point(197, 127);
            this.fFORM_TYPE.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fFORM_TYPE.Name = "fFORM_TYPE";
            this.fFORM_TYPE.Size = new System.Drawing.Size(237, 26);
            this.fFORM_TYPE.TabIndex = 80;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(60, 164);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 20);
            this.label3.TabIndex = 84;
            this.label3.Text = "Document Type :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(97, 130);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 20);
            this.label1.TabIndex = 79;
            this.label1.Text = "Form Type :";
            // 
            // fDOCUMENT_TYPE
            // 
            this.fDOCUMENT_TYPE.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fDOCUMENT_TYPE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fDOCUMENT_TYPE.FormattingEnabled = true;
            this.fDOCUMENT_TYPE.Location = new System.Drawing.Point(197, 161);
            this.fDOCUMENT_TYPE.Name = "fDOCUMENT_TYPE";
            this.fDOCUMENT_TYPE.Size = new System.Drawing.Size(237, 28);
            this.fDOCUMENT_TYPE.TabIndex = 83;
            // 
            // fSUBMIT_TYPE
            // 
            this.fSUBMIT_TYPE.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fSUBMIT_TYPE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fSUBMIT_TYPE.FormattingEnabled = true;
            this.fSUBMIT_TYPE.Location = new System.Drawing.Point(197, 91);
            this.fSUBMIT_TYPE.Name = "fSUBMIT_TYPE";
            this.fSUBMIT_TYPE.Size = new System.Drawing.Size(237, 28);
            this.fSUBMIT_TYPE.TabIndex = 81;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(52, 94);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 20);
            this.label2.TabIndex = 82;
            this.label2.Text = "Submission Type :";
            // 
            // axAcroPDF1
            // 
            this.axAcroPDF1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axAcroPDF1.Enabled = true;
            this.axAcroPDF1.Location = new System.Drawing.Point(0, 0);
            this.axAcroPDF1.Name = "axAcroPDF1";
            this.axAcroPDF1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAcroPDF1.OcxState")));
            this.axAcroPDF1.Size = new System.Drawing.Size(819, 795);
            this.axAcroPDF1.TabIndex = 0;
            // 
            // ControlSubDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ControlSubDocument";
            this.Size = new System.Drawing.Size(1263, 795);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axAcroPDF1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Button buttonThumbnails;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label37;
        public System.Windows.Forms.TextBox fDSN_SUB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox fDOC_DESC;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox fFOLDER_TYPE;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox fDOCUMENT_TYPE;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox fSUBMIT_TYPE;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox fFORM_TYPE;
        public AxAcroPDFLib.AxAcroPDF axAcroPDF1;
    }
}
