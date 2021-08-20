using System;

namespace SqlStreamStoreExample.Models
{
    public record Balance(decimal Amount, DateTime DateTime)
    {
        public Balance Add(decimal amount)
            => new(Amount + amount, DateTime.UtcNow);

        public Balance Subtract(decimal amount)
            => new(Amount - amount, DateTime.UtcNow);
    }
}
