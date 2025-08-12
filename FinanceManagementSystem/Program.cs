using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Bank Transfer] Processed GHC{transaction.Amount:N2} for {transaction.Category}.");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Mobile Money] Processed GHC{transaction.Amount:N2} for {transaction.Category}.");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Crypto Wallet] Processed GHC{transaction.Amount:N2} for {transaction.Category}.");
        }
    }

    public class Account
    {
        public string AccountNumber { get; private set; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. New balance: GHC{Balance:N2}");
        }
    }

    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
            }
            else
            {
                Balance -= transaction.Amount;
                Console.WriteLine($"Transaction applied. Updated balance: GHC{Balance:N2}");
            }
        }
    }

    public class FinanceApp
    {
        private List<Transaction> _transactions = new List<Transaction>();

        public void Run()
        {
            var savingsAccount = new SavingsAccount("CAL10001", 5000m);

            var t1 = new Transaction(1, DateTime.Now, 500m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 250m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 100m, "Entertainment");

            ITransactionProcessor mobileMoney = new MobileMoneyProcessor();
            ITransactionProcessor bankTransfer = new BankTransferProcessor();
            ITransactionProcessor cryptoWallet = new CryptoWalletProcessor();

            mobileMoney.Process(t1);
            bankTransfer.Process(t2);
            cryptoWallet.Process(t3);

            savingsAccount.ApplyTransaction(t1);
            savingsAccount.ApplyTransaction(t2);
            savingsAccount.ApplyTransaction(t3);

            _transactions.Add(t1);
            _transactions.Add(t2);
            _transactions.Add(t3);

            Console.WriteLine("\nTransaction log:");
            foreach (var tx in _transactions)
            {
                Console.WriteLine($"{tx.Id}: {tx.Category} - GHC{tx.Amount:N2} on {tx.Date}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            FinanceApp app = new FinanceApp();
            app.Run();
        }
    }
}

