namespace SimpleAggregate.UnitTests.Domain.Savings.Events
{
    internal class SavingsTargetMet
    {
        public readonly int AmountReached;

        public SavingsTargetMet(int amountReached)
        {
            AmountReached = amountReached;
        }
    }
}