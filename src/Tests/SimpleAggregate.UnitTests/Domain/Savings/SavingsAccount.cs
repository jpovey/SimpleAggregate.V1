namespace SimpleAggregate.UnitTests.Domain.Savings
{
    using Events;

    internal class SavingsAccount : Aggregate
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

        public void Handle(SavingsAccountCreated savingsAccountCreated)
        {
            AccountId = savingsAccountCreated.AccountId;
            SavingsTarget = savingsAccountCreated.SavingsTarget;
        }

        public void Handle(SavingsAdded savingsAdded)
        {
            SavingsAmount += savingsAdded.AmountAdded;
            if (SavingsAmount > SavingsTarget)
            {
                this.Apply(new SavingsTargetMet(SavingsAmount));
            }
        }

        public void Handle(SavingsTargetMet savingsTargetMet)
        {
            SavingsTargetMet = true;
        }

        private void RegisterEventHandlers()
        {
            this.RegisterEvent<SavingsAccountCreated>(Handle);
            this.RegisterEvent<SavingsAdded>(Handle);
            this.RegisterEvent<SavingsTargetMet>(Handle);
        }
    }
}
