using System;

namespace SqlStreamStoreExample.Events
{
    public class Deposited : AccountEvent
    {
        public Deposited(Guid transactionId, decimal amount, DateTime dateTime)
            : base(transactionId, amount, dateTime)
        { }
    }
}
