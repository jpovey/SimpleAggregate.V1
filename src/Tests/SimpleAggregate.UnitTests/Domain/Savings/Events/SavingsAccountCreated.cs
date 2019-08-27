namespace SimpleAggregate.UnitTests.Domain.Savings.Events
{
    internal class SavingsAccountCreated
    {
        public readonly string AccountId;
        public readonly int SavingsTarget;

        public SavingsAccountCreated(string accountId, int savingsTarget)
        {
            AccountId = accountId;
            SavingsTarget = savingsTarget;
        }
    }
}