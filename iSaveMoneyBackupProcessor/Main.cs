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
                    string text = System.IO.File.ReadAllText(ofd.FileName);
                    txtBkpPath.Text = ofd.FileName;

                    bkp = JsonConvert.DeserializeObject<RootNode>(text);

                    foreach (var account in bkp.accounts)
                    {
                        chkLstAccounts.Items.Add(account.name);
                    }
                }
            }
        }

        private void chkLstAccounts_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string account = chkLstAccounts.Items[e.Index].ToString();
            int accountId = bkp.accounts.Single(x => x.name == account).id;

            if (e.NewValue == CheckState.Checked) //if an account is being checked
            {
                foreach (var transfer in bkp.transfers.Where(x => x.from == accountId))
                {
                    dgvMain.Rows.Add(
                        account,
                        DateTimeOffset.FromUnixTimeSeconds(transfer.transaction_date).DateTime.ToString("dd-MMM-yyyy HH:mm"),
                        transfer.comment,
                        -1 * transfer.amount,
                        "Transfer",
                        bkp.accounts.Single(x => x.id == transfer.to).name
                    );
                }

                foreach (var transfer in bkp.transfers.Where(x => x.to == accountId))
                {
                    dgvMain.Rows.Add(
                        account,
                        DateTimeOffset.FromUnixTimeSeconds(transfer.transaction_date).DateTime.ToString("dd-MMM-yyyy HH:mm"),
                        transfer.comment,
                        transfer.amount,
                        "Transfer",
                        bkp.accounts.Single(x => x.id == transfer.from).name
                    );
                }

                foreach (var income in bkp.budgets.SelectMany(x => x.incomes).Where(x => x.account_id == accountId))
                {
                    dgvMain.Rows.Add(
                        account,
                        DateTimeOffset.FromUnixTimeSeconds(income.transaction_date).DateTime.ToString("dd-MMM-yyyy HH:mm"),
                        income.title,
                        income.amount,
                        "Income",
                        bkp.payers.FirstOrDefault(x => x.id == income.payer_id) == null ? "" : bkp.payers.FirstOrDefault(x => x.id == income.payer_id).name
                    );
                }

                var aa = bkp.budgets.SelectMany(x => x.categories).SelectMany(x => x.expenses).ToList();

                foreach (var expense in bkp.budgets.SelectMany(x => x.categories).SelectMany(x => x.expenses).Where(x => x.account.GetType() == typeof(Newtonsoft.Json.Linq.JObject) && ((Newtonsoft.Json.Linq.JObject)(x.account)).ToObject<Account>().id == accountId))
                {
                    dgvMain.Rows.Add(
                        account,
                        DateTimeOffset.FromUnixTimeSeconds(expense.transaction_date).DateTime.ToString("dd-MMM-yyyy HH:mm"),
                        expense.title,
                        -1 * expense.amount,
                        "Expense",
                        expense.payee.GetType() == typeof(Newtonsoft.Json.Linq.JObject) ? ((Newtonsoft.Json.Linq.JObject)expense.payee).ToObject<Payee>().name : ""
                    );
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
