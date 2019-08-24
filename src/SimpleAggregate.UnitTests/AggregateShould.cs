namespace SimpleAggregate.UnitTests
{ 
    using Domain;
    using Domain.Events;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class AggregateShould
    {
        [Test]
        public void AddEventToUncommittedEvents_WhenApplying()
        {
            var sut = new Parcel();
            sut.Apply(new ParcelDelivered());
            sut.UncommittedEvents.Count.Should().Be(1);
        }

        [Test]
        public void ClearUncommittedEvents()
        {
            var sut = new Parcel();
            sut.Apply(new ParcelDelivered());
            sut.ClearUncommittedEvents();
            sut.UncommittedEvents.Count.Should().Be(0);
        }
    }
}
