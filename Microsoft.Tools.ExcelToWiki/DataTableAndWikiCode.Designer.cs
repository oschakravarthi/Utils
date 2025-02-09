namespace Microsoft.Tools.ExcelToWiki
{
    partial class DataTableAndWikiCode
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
            tabControl1 = new TabControl();
            dataTabPage = new TabPage();
            dataGridView1 = new DataGridView();
            wikiCodeTabPage = new TabPage();
            wikiCodeTextBox = new TextBox();
            tabControl1.SuspendLayout();
            dataTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            wikiCodeTabPage.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(dataTabPage);
            tabControl1.Controls.Add(wikiCodeTabPage);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(928, 462);
            tabControl1.TabIndex = 0;
            // 
            // dataTabPage
            // 
            dataTabPage.Controls.Add(dataGridView1);
            dataTabPage.Location = new Point(4, 24);
            dataTabPage.Name = "dataTabPage";
            dataTabPage.Padding = new Padding(3);
            dataTabPage.Size = new Size(920, 434);
            dataTabPage.TabIndex = 0;
            dataTabPage.Text = "Data";
            dataTabPage.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(3, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.Size = new Size(914, 428);
            dataGridView1.TabIndex = 0;
            // 
            // wikiCodeTabPage
            // 
            wikiCodeTabPage.Controls.Add(wikiCodeTextBox);
            wikiCodeTabPage.Location = new Point(4, 24);
            wikiCodeTabPage.Name = "wikiCodeTabPage";
            wikiCodeTabPage.Padding = new Padding(3);
            wikiCodeTabPage.Size = new Size(920, 434);
            wikiCodeTabPage.TabIndex = 1;
            wikiCodeTabPage.Text = "Wiki Code";
            wikiCodeTabPage.UseVisualStyleBackColor = true;
            // 
            // wikiCodeTextBox
            // 
            wikiCodeTextBox.Dock = DockStyle.Fill;
            wikiCodeTextBox.Location = new Point(3, 3);
            wikiCodeTextBox.Multiline = true;
            wikiCodeTextBox.Name = "wikiCodeTextBox";
            wikiCodeTextBox.ScrollBars = ScrollBars.Both;
            wikiCodeTextBox.Size = new Size(914, 428);
            wikiCodeTextBox.TabIndex = 0;
            // 
            // DataTableAndWikiCode
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabControl1);
            Name = "DataTableAndWikiCode";
            Size = new Size(928, 462);
            tabControl1.ResumeLayout(false);
            dataTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            wikiCodeTabPage.ResumeLayout(false);
            wikiCodeTabPage.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage dataTabPage;
        private DataGridView dataGridView1;
        private TabPage wikiCodeTabPage;
        private TextBox wikiCodeTextBox;
    }
}
