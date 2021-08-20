using System;

namespace SqlStreamStoreExample.Events
{
    public class Withdrawn : AccountEvent
    {
        public Withdrawn(Guid transactionId, decimal amount, DateTime dateTime)
            : base(transactionId, amount, dateTime)
        { }
    }
}
