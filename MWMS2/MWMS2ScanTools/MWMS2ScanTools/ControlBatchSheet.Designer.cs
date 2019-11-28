namespace MWMS2ScanTools
{
    partial class ControlBatchSheet
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
            this.buttonUpload = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.buttonClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonUpload
            // 
            this.buttonUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUpload.Location = new System.Drawing.Point(499, 3);
            this.buttonUpload.Name = "buttonUpload";
            this.buttonUpload.Size = new System.Drawing.Size(126, 23);
            this.buttonUpload.TabIndex = 0;
            this.buttonUpload.Text = "Batch Upload Sheet";
            this.buttonUpload.UseVisualStyleBackColor = true;
            this.buttonUpload.Click += new System.EventHandler(this.buttonUpload_Click);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(3, 32);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(622, 366);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(3, 3);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close sheet";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // ControlBatchSheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.buttonUpload);
            this.Name = "ControlBatchSheet";
            this.Size = new System.Drawing.Size(628, 401);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonUpload;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button buttonClose;
    }
}
