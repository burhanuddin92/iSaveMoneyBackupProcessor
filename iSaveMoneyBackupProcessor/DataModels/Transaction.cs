using System;

namespace iSaveMoneyBackupProcessor.DataModels
{
    public class Transaction
    {
        public string Account { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public double Amount { get; set; }
        public string Type { get; set; }
        public string Details { get; set; }

        public override string ToString()
        {
            return $"{Account},{Date.ToString("dd-MMM-yyyy")},{Title},{Amount},{Type},{Details}";
        }
    }
}
