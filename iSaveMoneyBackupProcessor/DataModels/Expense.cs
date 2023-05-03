namespace iSaveMoneyBackupProcessor.DataModels
{
    public class Expense
    {
        public int id { get; set; }
        public int label_id { get; set; }
        public object payee { get; set; } // maybe 0, or DataModels.Payee
        public string title { get; set; }
        public double amount { get; set; }
        public int transaction_date { get; set; }
        public bool active { get; set; }
        public bool state { get; set; }
        public object account { get; set; } // maybe 0, or DataModels.Account
    }
}