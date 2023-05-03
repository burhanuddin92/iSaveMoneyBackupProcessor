namespace iSaveMoneyBackupProcessor.DataModels
{
    public class Account
    {
        public int id { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public double balance { get; set; }
        public bool active { get; set; }
    }
}