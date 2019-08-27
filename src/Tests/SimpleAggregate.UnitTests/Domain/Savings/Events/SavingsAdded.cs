namespace SimpleAggregate.UnitTests.Domain.Savings.Events
{
    internal class SavingsAdded
    {
        public readonly int AmountAdded;

        public SavingsAdded(int amount)
        {
            AmountAdded = amount;
        }
    }
}