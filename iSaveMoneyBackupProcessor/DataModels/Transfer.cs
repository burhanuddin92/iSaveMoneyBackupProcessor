namespace iSaveMoneyBackupProcessor.DataModels
{
    public class Transfer
    {
        public int id { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public int type { get; set; }
        public float amount { get; set; }
        public string comment { get; set; }
        public int transaction_date { get; set; }
        public int insert_date { get; set; }
        public bool active { get; set; }

    }
}