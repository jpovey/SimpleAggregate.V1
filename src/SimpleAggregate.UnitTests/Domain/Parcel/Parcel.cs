namespace SimpleAggregate.UnitTests.Domain.Parcel
{
    using Events;

    internal class Parcel : Aggregate
    {
        public bool Delivered { get; private set; }
        public string DeliveredBy { get; private set; }

        public Parcel()
        {
            RegisterEventHandlers();
        }

        internal new void IgnoreUnregisteredEvents()
        {
            base.IgnoreUnregisteredEvents = true;
        }

        internal void DeliverParcel(string deliveredBy)
        {
            this.Apply(new ParcelDelivered { DeliveredBy = deliveredBy });
        }
        
        private void Handle(ParcelDelivered parcelDelivered)
        {
            DeliveredBy = parcelDelivered.DeliveredBy;
            Delivered = true;
        }

        private void RegisterEventHandlers()
        {
           this.RegisterEvent<ParcelDelivered>(Handle);
        }
    }
}
