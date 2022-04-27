using System.Collections.Generic;

namespace iSaveMoneyBackupProcessor.DataModels
{
    public class Category
    {
        public int id { get; set; }
        public string title { get; set; }
        public float amount { get; set; }
        public float initial_amount { get; set; }
        public float spent { get; set; }
        public bool rollover { get; set; }
        public string comment { get; set; }
        public bool active { get; set; }
        public int type { get; set; }
        public List<Expense> expenses { get; set; }
    }
}