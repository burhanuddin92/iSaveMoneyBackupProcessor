using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iSaveMoneyBackupProcessor.DataModels;
using Newtonsoft.Json;

namespace iSaveMoneyBackupProcessor
{
    public partial class Main : Form
    {
        RootNode bkp;

        public Main()
        {
            InitializeComponent();

            InitializeDGVMain();
        }

        private void InitializeDGVMain()
        {
            dgvMain.Columns.Add("dgvKey", "Account");
            dgvMain.Columns.Add("dgvDate", "Date");
            dgvMain.Columns.Add("dgvText", "Title");
            dgvMain.Columns.Add("dgvAmount", "Amount");
            dgvMain.Columns.Add("dgvType", "Type");
            dgvMain.Columns.Add("dgvDetails", "Details");
        }

        private void btnBrowseBkp_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "JSON File|*.json";
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    chkLstAccounts.Items.Clear();

                    string text = System.IO.File.ReadAllText(ofd.FileName);
                    txtBkpPath.Text = ofd.FileName;

                    bkp = JsonConvert.DeserializeObject<RootNode>(text);

                    chkLstAccounts.Items.AddRange(bkp.ListAllAccounts().Select(x => x.Name).ToArray());

                    ProcessDataToCsvs(bkp, System.IO.Path.GetDirectoryName(ofd.FileName));
                }
            }
        }

        private void ProcessDataToCsvs(RootNode bkp, string path)
        {
            List<string> accounts = bkp.ListAllAccounts().Select(x => x.Name).ToList();
            List<Transaction> allTransactions = new List<Transaction>();

            foreach (var account in accounts)
            {
                allTransactions.AddRange(bkp.GetAllTransactionsForAccount(account));
            }

            allTransactions = allTransactions.OrderByDescending(x => x.Date).ToList();

            string allTransactionText = "Account,Date,Title,Amount,Type,Details" + Environment.NewLine + string.Join(Environment.NewLine, allTransactions);
            System.IO.File.WriteAllText(System.IO.Path.Combine(path, "AllTransactions.csv"), allTransactionText);

            var allAccounts = allTransactions.GroupBy(x => x.Account, y => y, (x, y) => new
            {
                Name = x,
                TransactionCount = y.Count(),
                Opening = bkp.accounts.Single(z => z.name == x).balance,
                TransactionValue = y.Sum(z => z.Amount)
            }).OrderBy(x => x.Name).Select(x => $"{x.Name},{x.TransactionCount},{x.Opening},{x.TransactionValue},{x.Opening + x.TransactionValue}").ToList();

            string allAccountsText = "Account,Tx Count,Opening,Tx Value,Closing" + Environment.NewLine + string.Join(Environment.NewLine, allAccounts);
            System.IO.File.WriteAllText(System.IO.Path.Combine(path, "AllAccounts.csv"), allAccountsText);

            var allBudgets = bkp.budgets.Select(x => new
            {
                Name = x.comment,
                StartDate = DateTimeOffset.FromUnixTimeSeconds(x.start_date).DateTime.ToString("dd-MMM-yyyy"),
                EndDate = DateTimeOffset.FromUnixTimeSeconds(x.end_date).DateTime.ToString("dd-MMM-yyyy"),
                x.TotalIncome,
                x.TotalExpense,
                Difference = x.TotalIncome - x.TotalExpense
            }).Select(x => $"{x.Name},{x.StartDate},{x.EndDate},{x.TotalIncome},{x.TotalExpense},{x.Difference}").ToList();

            string allBudgetsText = "Name,StartDate,EndDate,TotalIncome,TotalExpense,Difference" + Environment.NewLine + string.Join(Environment.NewLine, allBudgets);
            System.IO.File.WriteAllText(System.IO.Path.Combine(path, "AllBudgets.csv"), allBudgetsText);
        }

        private void chkLstAccounts_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string account = chkLstAccounts.Items[e.Index].ToString();

            if (e.NewValue == CheckState.Checked) //if an account is being checked
            {
                foreach (var transaction in bkp.GetAllTransactionsForAccount(account))
                {
                    dgvMain.Rows.Add(new string[] { transaction.Account, transaction.Date.ToString("dd-MMM-yyyy"), transaction.Title, transaction.Amount.ToString(), transaction.Type, transaction.Details });
                }
            }
            else
            {
                var rows = dgvMain.Rows.Cast<DataGridViewRow>().Where(x => x.Cells[0].Value.ToString() == account).ToList();

                foreach (var row in rows)
                {
                    dgvMain.Rows.Remove(row);
                }
            }
        }
    }
}
