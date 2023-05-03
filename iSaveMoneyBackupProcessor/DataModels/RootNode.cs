using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace iSaveMoneyBackupProcessor.DataModels
{
    public class RootNode
    {
        public Dictionary<string, object> preferences { get; set; }
        public List<Budget> budgets { get; set; }
        public List<Account> accounts { get; set; }
        public List<Payee> payees { get; set; }
        public List<Payer> payers { get; set; }
        public List<Transfer> transfers { get; set; }
        public List<Label> labels { get; set; }

        public List<(int Id, string Name)> ListAllAccounts()
        {
            return this.accounts.Select(x => (x.id, x.name)).ToList();
        }

        private int GetAccountIdByName(string Name)
        {
            return this.accounts.Single(x => x.name == Name).id;
        }

        public List<Transaction> GetAllTransactionsForAccount(string Name)
        {
            int AccountId = GetAccountIdByName(Name);

            List<Transaction> transactions = new List<Transaction>();

            foreach (var transaction in this.transfers.Where(x => x.from == AccountId))
            {
                transactions.Add(new Transaction()
                {
                    Account = Name,
                    Date = DateTimeOffset.FromUnixTimeSeconds(transaction.transaction_date).DateTime,
                    Title = transaction.comment,
                    Amount = -1 * transaction.amount,
                    Type = "Transfer",
                    Details = this.accounts.Single(x => x.id == transaction.to).name
                });
            }

            foreach (var transaction in this.transfers.Where(x => x.to == AccountId))
            {
                transactions.Add(new Transaction()
                {
                    Account = Name,
                    Date = DateTimeOffset.FromUnixTimeSeconds(transaction.transaction_date).DateTime,
                    Title = transaction.comment,
                    Amount = transaction.amount,
                    Type = "Transfer",
                    Details = this.accounts.Single(x => x.id == transaction.from).name
                });
            }

            foreach (var transaction in this.budgets.SelectMany(x => x.incomes).Where(x => x.account_id == AccountId))
            {
                transactions.Add(new Transaction()
                {
                    Account = Name,
                    Date = DateTimeOffset.FromUnixTimeSeconds(transaction.transaction_date).DateTime,
                    Title = transaction.title,
                    Amount = transaction.amount,
                    Type = "Income",
                    Details = this.payers.FirstOrDefault(x => x.id == transaction.payer_id) == null ? "" : this.payers.FirstOrDefault(x => x.id == transaction.payer_id).name
                });
            }

            foreach (var transaction in this.budgets.SelectMany(x => x.categories).SelectMany(x => x.expenses).Where(x => x.account.GetType() == typeof(Newtonsoft.Json.Linq.JObject) && ((Newtonsoft.Json.Linq.JObject)(x.account)).ToObject<Account>().id == AccountId))
            {
                transactions.Add(new Transaction()
                {
                    Account = Name,
                    Date = DateTimeOffset.FromUnixTimeSeconds(transaction.transaction_date).DateTime,
                    Title = transaction.title,
                    Amount = -1 * transaction.amount,
                    Type = "Expense",
                    Details = transaction.payee.GetType() == typeof(Newtonsoft.Json.Linq.JObject) ? ((Newtonsoft.Json.Linq.JObject)transaction.payee).ToObject<Payee>().name : ""
                });
            }

            return transactions;
        }
    }
}
