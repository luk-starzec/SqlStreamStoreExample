using SqlStreamStore;
using SqlStreamStore.Streams;
using SqlStreamStoreExample.Models;
using System;
using System.Threading.Tasks;

namespace SqlStreamStoreExample
{
    class Program
    {
        private const string keyExit = "X";
        private const string keyDeposit = "+";
        private const string keyWithdraw = "-";
        private const string keyBalance = "=";
        private const string keyTransactions = "*";

        private static InMemoryStreamStore streamStore;
        private static Account account;
        private static AccountBalance accountBalance;

        static async Task Main(string[] args)
        {
            Init();

            var key = string.Empty;
            while (key != keyExit)
            {
                PrintOptions();
                Console.Write("operation: ");

                key = Console.ReadLine()?.ToUpperInvariant();
                switch (key)
                {
                    case keyDeposit:
                        var depositAmount = GetAmount();
                        if (depositAmount.HasValue)
                        {
                            var tid = await account.Deposit(depositAmount.Value);
                            Console.WriteLine($"Deposited: {depositAmount:C} (id: {tid })");
                        }
                        break;

                    case keyWithdraw:
                        var withdrawAmount = GetAmount();
                        if (withdrawAmount.HasValue)
                        {
                            var tid = await account.Withdraw(withdrawAmount.Value);
                            Console.WriteLine($"Withdrawn: {withdrawAmount:C} (id: {tid })");
                        }
                        break;

                    case keyBalance:
                        PrintBalance();
                        break;

                    case keyTransactions:
                        await PrintLogs();
                        break;
                }

                Console.WriteLine();
            }
        }

        private static void Init()
        {
            var streamId = new StreamId($"{Guid.NewGuid()}");
            streamStore = new InMemoryStreamStore();

            account = new Account(streamStore, streamId);
            accountBalance = new AccountBalance(streamStore, streamId);
        }

        private static void PrintOptions()
        {
            Console.WriteLine("---------------");
            Console.WriteLine("--- Options ---");
            Console.WriteLine("---------------");
            Console.WriteLine($"- Deposit: {keyDeposit}");
            Console.WriteLine($"- Withdraw: {keyWithdraw}");
            Console.WriteLine($"- Balance: {keyBalance}");
            Console.WriteLine($"- Logs: {keyTransactions}");
            Console.WriteLine($"- Exit: {keyExit}");
            Console.WriteLine("---------------");
        }

        private static decimal? GetAmount()
        {
            Console.Write("Amount: ");
            if (decimal.TryParse(Console.ReadLine(), out var amount))
                return amount;

            Console.WriteLine("Invalid amount");
            return null;
        }

        private static void PrintBalance()
        {
            Console.WriteLine($"Balance: {accountBalance.Balance.Amount:C} on {accountBalance.Balance.DateTime}");
        }

        private static async Task PrintLogs()
        {
            var logs = await account.GetTransactions();
            foreach (var log in logs)
                Console.WriteLine(log);
        }
    }
}
