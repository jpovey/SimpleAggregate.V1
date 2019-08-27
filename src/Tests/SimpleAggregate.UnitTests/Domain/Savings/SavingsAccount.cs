namespace SimpleAggregate.UnitTests.Domain.Savings
{
    using Events;

    internal class SavingsAccount : AggregateBase
    {
        public override string AggregateId => AccountId;
        public string AccountId { get; private set; }
        public int SavingsTarget { get; private set; }
        public int SavingsAmount { get; private set; }
        public bool SavingsTargetMet { get; private set; }

        public SavingsAccount()
        {
            RegisterEventHandlers();
        }

        internal new void IgnoreUnregisteredEvents()
        {
            base.IgnoreUnregisteredEvents = true;
        }

        public void CreateSavingsAccount(string accountId, int savingsTarget)
        {
            this.Apply(new SavingsAccountCreated(accountId, savingsTarget));
        }

        public void AddSavings(int amount)
        {
            this.Apply(new SavingsAdded(amount));
        }
        
        internal void ApplyNullEvent()
        {
            this.Apply(null);
        }

        private void Handle(SavingsAccountCreated savingsAccountCreated)
        {
            AccountId = savingsAccountCreated.AccountId;
            SavingsTarget = savingsAccountCreated.SavingsTarget;
        }

        private void Handle(SavingsAdded savingsAdded)
        {
            SavingsAmount += savingsAdded.AmountAdded;
            if (SavingsAmount > SavingsTarget)
            {
                this.Apply(new SavingsTargetMet(SavingsAmount));
            }
        }

        private void Handle(SavingsTargetMet savingsTargetMet)
        {
            SavingsTargetMet = true;
        }

        private void RegisterEventHandlers()
        {
            this.RegisterEventHandler<SavingsAccountCreated>(Handle);
            this.RegisterEventHandler<SavingsAdded>(Handle);
            this.RegisterEventHandler<SavingsTargetMet>(Handle);
        }
    }
}
