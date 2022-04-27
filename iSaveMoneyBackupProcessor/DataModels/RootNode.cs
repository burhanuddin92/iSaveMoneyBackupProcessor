using System.Collections.Generic;

namespace iSaveMoneyBackupProcessor.DataModels
{
    public class RootNode
    {
        public Dictionary<string, object> preferences { get; set; }
        public List<Budget> budgets {get;set;}
        public List<Account> accounts { get; set; }
        public List<Payee> payees { get; set; }
        public List<Payer> payers { get; set; }
        public List<Transfer> transfers { get; set; }
        public List<Label> labels { get; set; }
    }
}
