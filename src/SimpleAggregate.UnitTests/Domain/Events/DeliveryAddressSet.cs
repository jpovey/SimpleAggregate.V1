namespace SimpleAggregate.UnitTests.Domain.Events
{
    using SimpleAggregate.Domain;

    internal class DeliveryAddressSet: IEvent
    {
        internal string Address;
    }
}