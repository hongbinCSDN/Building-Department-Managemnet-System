namespace MWMS2ScanTools
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.buttonScan = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageScan = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxScanner = new System.Windows.Forms.ComboBox();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonMerge = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.textBoxSearchKeyword = new System.Windows.Forms.TextBox();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.tabControlDoc = new System.Windows.Forms.TabControl();
            this.tabPageBatch = new System.Windows.Forms.TabPage();
            this.controlBatch1 = new MWMS2ScanTools.ControlBatch();
            this.tabPageSearch = new System.Windows.Forms.TabPage();
            this.controlSearch1 = new MWMS2ScanTools.ControlSearch();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1.SuspendLayout();
            this.tabPageScan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabPageBatch.SuspendLayout();
            this.tabPageSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonScan
            // 
            this.buttonScan.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonScan.Location = new System.Drawing.Point(6, 46);
            this.buttonScan.Name = "buttonScan";
            this.buttonScan.Size = new System.Drawing.Size(76, 30);
            this.buttonScan.TabIndex = 0;
            this.buttonScan.Text = "Scan";
            this.buttonScan.UseVisualStyleBackColor = false;
            this.buttonScan.Click += new System.EventHandler(this.buttonScan_Click);
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Location = new System.Drawing.Point(0, 25);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(194, 274);
            this.treeView1.TabIndex = 45;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageScan);
            this.tabControl1.Controls.Add(this.tabPageBatch);
            this.tabControl1.Controls.Add(this.tabPageSearch);
            this.tabControl1.Location = new System.Drawing.Point(2, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(950, 457);
            this.tabControl1.TabIndex = 46;
            // 
            // tabPageScan
            // 
            this.tabPageScan.Controls.Add(this.splitContainer1);
            this.tabPageScan.Location = new System.Drawing.Point(4, 22);
            this.tabPageScan.Name = "tabPageScan";
            this.tabPageScan.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageScan.Size = new System.Drawing.Size(942, 431);
            this.tabPageScan.TabIndex = 0;
            this.tabPageScan.Text = "Scan";
            this.tabPageScan.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(936, 425);
            this.splitContainer1.SplitterDistance = 120;
            this.splitContainer1.TabIndex = 69;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxScanner);
            this.groupBox1.Controls.Add(this.buttonEdit);
            this.groupBox1.Controls.Add(this.listView1);
            this.groupBox1.Controls.Add(this.buttonLoad);
            this.groupBox1.Controls.Add(this.buttonScan);
            this.groupBox1.Controls.Add(this.buttonMerge);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(934, 118);
            this.groupBox1.TabIndex = 68;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Image(s)";
            // 
            // comboBoxScanner
            // 
            this.comboBoxScanner.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxScanner.FormattingEnabled = true;
            this.comboBoxScanner.Location = new System.Drawing.Point(6, 19);
            this.comboBoxScanner.Name = "comboBoxScanner";
            this.comboBoxScanner.Size = new System.Drawing.Size(76, 21);
            this.comboBoxScanner.TabIndex = 68;
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonEdit.Location = new System.Drawing.Point(811, 10);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(117, 39);
            this.buttonEdit.TabIndex = 67;
            this.buttonEdit.Text = "Edit";
            this.buttonEdit.UseVisualStyleBackColor = false;
            this.buttonEdit.Visible = false;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(88, 10);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(717, 102);
            this.listView1.TabIndex = 66;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // buttonLoad
            // 
            this.buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonLoad.Location = new System.Drawing.Point(6, 82);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(76, 30);
            this.buttonLoad.TabIndex = 65;
            this.buttonLoad.Text = "Load..";
            this.buttonLoad.UseVisualStyleBackColor = false;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonMerge
            // 
            this.buttonMerge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMerge.Location = new System.Drawing.Point(811, 10);
            this.buttonMerge.Name = "buttonMerge";
            this.buttonMerge.Size = new System.Drawing.Size(117, 102);
            this.buttonMerge.TabIndex = 52;
            this.buttonMerge.Text = "Merge to Document";
            this.buttonMerge.UseVisualStyleBackColor = false;
            this.buttonMerge.Click += new System.EventHandler(this.buttonMerge_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.textBoxSearchKeyword);
            this.splitContainer2.Panel1.Controls.Add(this.buttonSearch);
            this.splitContainer2.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControlDoc);
            this.splitContainer2.Size = new System.Drawing.Size(936, 301);
            this.splitContainer2.SplitterDistance = 196;
            this.splitContainer2.TabIndex = 70;
            // 
            // textBoxSearchKeyword
            // 
            this.textBoxSearchKeyword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSearchKeyword.Location = new System.Drawing.Point(0, 2);
            this.textBoxSearchKeyword.Name = "textBoxSearchKeyword";
            this.textBoxSearchKeyword.Size = new System.Drawing.Size(140, 20);
            this.textBoxSearchKeyword.TabIndex = 47;
            // 
            // buttonSearch
            // 
            this.buttonSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSearch.Location = new System.Drawing.Point(139, 1);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(55, 22);
            this.buttonSearch.TabIndex = 46;
            this.buttonSearch.Text = "Search";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // tabControlDoc
            // 
            this.tabControlDoc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlDoc.Location = new System.Drawing.Point(0, 0);
            this.tabControlDoc.Name = "tabControlDoc";
            this.tabControlDoc.SelectedIndex = 0;
            this.tabControlDoc.Size = new System.Drawing.Size(734, 299);
            this.tabControlDoc.TabIndex = 67;
            // 
            // tabPageBatch
            // 
            this.tabPageBatch.Controls.Add(this.controlBatch1);
            this.tabPageBatch.Location = new System.Drawing.Point(4, 22);
            this.tabPageBatch.Name = "tabPageBatch";
            this.tabPageBatch.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBatch.Size = new System.Drawing.Size(942, 431);
            this.tabPageBatch.TabIndex = 1;
            this.tabPageBatch.Text = "Batch Upload/ Update";
            this.tabPageBatch.UseVisualStyleBackColor = true;
            // 
            // controlBatch1
            // 
            this.controlBatch1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlBatch1.Location = new System.Drawing.Point(3, 3);
            this.controlBatch1.Name = "controlBatch1";
            this.controlBatch1.Size = new System.Drawing.Size(936, 425);
            this.controlBatch1.TabIndex = 0;
            // 
            // tabPageSearch
            // 
            this.tabPageSearch.Controls.Add(this.controlSearch1);
            this.tabPageSearch.Location = new System.Drawing.Point(4, 22);
            this.tabPageSearch.Name = "tabPageSearch";
            this.tabPageSearch.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSearch.Size = new System.Drawing.Size(942, 431);
            this.tabPageSearch.TabIndex = 2;
            this.tabPageSearch.Text = "Search";
            this.tabPageSearch.UseVisualStyleBackColor = true;
            // 
            // controlSearch1
            // 
            this.controlSearch1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlSearch1.Location = new System.Drawing.Point(3, 3);
            this.controlSearch1.Name = "controlSearch1";
            this.controlSearch1.Size = new System.Drawing.Size(936, 425);
            this.controlSearch1.TabIndex = 0;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.RestoreDirectory = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(954, 470);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "MWM2 Scanning Tool";
            this.tabControl1.ResumeLayout(false);
            this.tabPageScan.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabPageBatch.ResumeLayout(false);
            this.tabPageSearch.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonScan;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageScan;
        private System.Windows.Forms.Button buttonMerge;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ListView listView1;
        public System.Windows.Forms.TabControl tabControlDoc;
        private System.Windows.Forms.TabPage tabPageBatch;
        private System.Windows.Forms.TabPage tabPageSearch;
        private ControlSearch controlSearch1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private ControlBatch controlBatch1;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.TextBox textBoxSearchKeyword;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.ComboBox comboBoxScanner;
    }
}

