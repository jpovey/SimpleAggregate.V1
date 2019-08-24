namespace SimpleAggregate.UnitTests.Domain.Events
{
    using SimpleAggregate.Domain;

    internal class ParcelDelivered : IEvent
    {
        internal string DeliveredBy;
    }
}
