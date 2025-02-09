using System.Data;
using System.IO;

namespace SubhadraSolutions.Utils.Data
{
    public static class WikiHelper
    {
        public static string ExportDataTableToWiki(DataTable table)
        {
            using (var sw = new StringWriter())
            {
                ExportDataTableToWiki(table, sw);
                return sw.ToString();
            }
        }

        public static void ExportDataTableToWiki(DataTable table, TextWriter writer)
        {
            writer.WriteLine(@"{| class=""wikitable""");
            writer.WriteLine("|+");
            var cc = table.Columns.Count;
            var rc = table.Rows.Count;
            for (int i = 0; i < cc; i++)
            {
                writer.WriteLine($"!{table.Columns[i].ColumnName}");
            }
            writer.WriteLine("|-");
            for (int i = 0; i < rc; i++)
            {
                writer.WriteLine("|+");
                var row = table.Rows[i];
                for (int j = 0; j < cc; j++)
                {
                    writer.WriteLine($"|{row[j]}");
                }
                writer.WriteLine("|-");
            }
            writer.WriteLine("|}");
        }
    }
}