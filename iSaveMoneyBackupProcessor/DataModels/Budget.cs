using System.Collections.Generic;

namespace iSaveMoneyBackupProcessor.DataModels
{
    public class Budget
    {
        public int id { get; set; }
        public int start_date { get; set; }
        public int end_date { get; set; }
        public int type { get; set; }
        public string comment { get; set; }
        public bool active { get; set; }
        public List<Income> incomes { get; set; }
        public List<Category> categories { get; set; }
    }
}