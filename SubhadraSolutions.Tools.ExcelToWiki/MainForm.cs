using System.Data;
using SubhadraSolutions.Utils.OpenXml;

namespace Microsoft.Tools.ExcelToWiki
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        
        private void Process()
        {
            var dataSet = ExcelHelper.GetDataFromExcelFile(this.excelFileTextBox.Text);
            var tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };
            foreach (DataTable table in dataSet.Tables)
            {
                var tabpage = new TabPage
                {
                    Text = table.TableName
                };

                tabpage.Controls.Add(new DataTableAndWikiCode(table)
                {
                    Dock = DockStyle.Fill
                });
                tabControl.TabPages.Add(tabpage);
            }
            panel1.Controls.Add(tabControl);
        }
        private void browseButton_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog()
            {
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                Title = "Select Excel file",
                Filter = "Excel files (*.xlsx)|*.xlsx"

            };
            if(ofd.ShowDialog()==DialogResult.OK)
            {
                this.excelFileTextBox.Text = ofd.FileName;
                Process();
            }
        }
    }
}
