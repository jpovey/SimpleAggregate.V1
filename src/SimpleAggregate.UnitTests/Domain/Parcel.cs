namespace SimpleAggregate.UnitTests.Domain
{
    using Events;

    internal class Parcel : Aggregate
    {
        private bool _delivered;
        private bool _cancelled;
        private string _deliveredBy;
        private string _deliveryAddress;

        public Parcel()
        {
            RegisterEvents();
        }

        internal void SetDeliveryAddress(string address)
        {
            this.Apply(new DeliveryAddressSet
            {
                Address = address
            });
        }

        internal void CancelDelivery(string address)
        {
            if (!_delivered)
            {
                this.Apply(new DeliveryCancelled());
            }
        }

        private void Apply(ParcelDelivered parcelDelivered)
        {
            _delivered = true;
            _deliveredBy = parcelDelivered.DeliveredBy;
        }

        private void Apply(DeliveryAddressSet deliveryAddressSet)
        {
            _deliveryAddress = deliveryAddressSet.Address;
        }

        private void Apply(DeliveryCancelled deliveryCancelled)
        {
            _cancelled = true;
        }

        private void RegisterEvents()
        {
           // this.RegisterEvent<DeliveryAddressSet>(this.Apply);
           // this.RegisterEvent<ParcelDelivered>(this.Apply);
           // this.RegisterEvent<DeliveryCancelled>(this.Apply);
        }
    }
}
