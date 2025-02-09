using System.Data;
using SubhadraSolutions.Utils.Data;

namespace Microsoft.Tools.ExcelToWiki
{
    public partial class DataTableAndWikiCode : UserControl
    {
        public DataTableAndWikiCode(DataTable table)
        {
            InitializeComponent();
            this.dataGridView1.DataSource = table;
            this.wikiCodeTextBox.Text = WikiHelper.ExportDataTableToWiki(table);
        }
    }
}
