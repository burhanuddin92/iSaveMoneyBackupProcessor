namespace iSaveMoneyBackupProcessor.DataModels
{
    public class Income
    {
        public int id { get; set; }
        public int label_id { get; set; }
        public int payer_id { get; set; }
        public int account_id { get; set; }
        public string title { get; set; }
        public float amount { get; set; }
        public string comment { get; set; }
        public int transaction_date { get; set; }
        public bool active { get; set; }
        public bool state { get; set; }
    }
}