using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public string TransactionType { get; set; } // "Deposit", "Withdrawal", "Transfer"
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
