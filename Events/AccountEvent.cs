using System;

namespace SqlStreamStoreExample.Events
{
    public abstract class AccountEvent
    {
        public Guid TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }

        protected AccountEvent(Guid transactionId, decimal amount, DateTime dateTime)
        {
            TransactionId = transactionId;
            Amount = amount;
            DateTime = dateTime;
        }
    }
}
