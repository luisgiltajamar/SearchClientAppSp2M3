using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search;
using Microsoft.SharePoint.Client.Search.Query;

namespace SearchClientApp
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public DataTable ConvertToDataTable(ResultTable rt)
        {
            DataTable queryDataTable = new DataTable();

            try
            {
                IDictionary<string, object> r = rt.ResultRows.ToArray()[0];
                foreach (string id in r.Keys)
                {
                    queryDataTable.Columns.Add(id);
                }
                foreach (var row in rt.ResultRows)
                {
                    DataRow dr = queryDataTable.NewRow();
                    foreach (DataColumn dc in queryDataTable.Columns)
                    {
                        dr[dc.ColumnName] = row[dc.ColumnName];
                    }
                    queryDataTable.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
            }
            return queryDataTable;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            this.btnSearch.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            var searchResults = Task.Factory.StartNew(() =>
            {
                ClientContext clientContext = new ClientContext("http://intranet.contoso.com");
                KeywordQuery keywordQuery = new KeywordQuery(clientContext);
                keywordQuery.QueryText = txtSearchTerm.Text;

                DataTable queryDataTable = null;
                SearchExecutor searchExecutor = new SearchExecutor(clientContext);
                ClientResult<ResultTableCollection> results = searchExecutor.ExecuteQuery(keywordQuery);
                clientContext.ExecuteQuery();

                foreach (ResultTable resultsTable in results.Value)
                {
                    queryDataTable = ConvertToDataTable(resultsTable);
                }

                return queryDataTable;
            });

            this.dataGridView1.DataSource = await searchResults;
            this.dataGridView1.Refresh();
            this.btnSearch.Enabled = true;
            this.Cursor = Cursors.Default;
        }
    }
}
