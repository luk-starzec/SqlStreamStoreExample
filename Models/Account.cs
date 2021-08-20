using SqlStreamStore;
using SqlStreamStore.Streams;
using SqlStreamStoreExample.Events;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace SqlStreamStoreExample.Models
{
    public class Account
    {
        private readonly IStreamStore streamStore;
        private readonly StreamId streamId;

        public Account(IStreamStore streamStore, StreamId streamId)
        {
            this.streamStore = streamStore;
            this.streamId = streamId;
        }


        public async Task<Guid> Deposit(decimal amount)
        {
            var tid = Guid.NewGuid();
            var deposit = new Deposited(tid, amount, DateTime.UtcNow);
            var msg = new NewStreamMessage(tid, nameof(Deposited), JsonSerializer.Serialize(deposit));

            await streamStore.AppendToStream(streamId, ExpectedVersion.Any, msg);

            return tid;
        }

        public async Task<Guid> Withdraw(decimal amount)
        {
            var tid = Guid.NewGuid();
            var withdraw = new Withdrawn(tid, amount, DateTime.UtcNow);
            var msg = new NewStreamMessage(tid, nameof(Withdrawn), JsonSerializer.Serialize(withdraw));

            await streamStore.AppendToStream(streamId, ExpectedVersion.Any, msg);

            return tid;

        }

        public async Task<string[]> GetTransactions()
        {
            var result = new List<string>();
            var balance = 0m;

            var endOfStream = false;
            var fromVersion = 0;
            var maxCount = 5;

            while (!endOfStream)
            {
                var stream = await streamStore.ReadStreamForwards(streamId, fromVersion, maxCount);
                endOfStream = stream.IsEnd;
                fromVersion = stream.NextStreamVersion;

                foreach (var msg in stream.Messages)
                {
                    var json = await msg.GetJsonData();
                    switch (msg.Type)
                    {
                        case nameof(Deposited):
                            var deposited = JsonSerializer.Deserialize<Deposited>(json);
                            result.Add($"{deposited.DateTime} - deposited: {deposited.Amount:C}");
                            balance += deposited.Amount;
                            break;

                        case nameof(Withdrawn):
                            var withdrawn = JsonSerializer.Deserialize<Withdrawn>(json);
                            result.Add($"{withdrawn.DateTime} - withdrawn: {withdrawn.Amount:C}");
                            balance -= withdrawn.Amount;
                            break;
                    }
                }
            }
            result.Add($"Balance: {balance:C}");
            return result.ToArray();
        }
    }
}
