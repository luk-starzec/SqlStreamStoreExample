using SqlStreamStore;
using SqlStreamStore.Streams;
using SqlStreamStoreExample.Events;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SqlStreamStoreExample.Models
{
    public class AccountBalance
    {
        public Balance Balance { get; private set; } = new Balance(0, DateTime.UtcNow);

        public AccountBalance(IStreamStore streamStore, StreamId streamId)
        {
            streamStore.SubscribeToStream(streamId, null, StreamMessageReceived);
        }

        private async Task StreamMessageReceived(IStreamSubscription subscription, StreamMessage streamMessage, CancellationToken cancellationToken)
        {
            var json = await streamMessage.GetJsonData(cancellationToken);
            switch (streamMessage.Type)
            {
                case nameof(Deposited):
                    var deposited = JsonSerializer.Deserialize<Deposited>(json);
                    Balance = Balance.Add(deposited.Amount);
                    break;

                case nameof(Withdrawn):
                    var withdrawn = JsonSerializer.Deserialize<Withdrawn>(json);
                    Balance = Balance.Subtract(withdrawn.Amount);
                    break;
            }
        }
    }
}
