namespace SimpleAggregate.UnitTests
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using Domain;
    using Domain.Events;
    using FluentAssertions;
    using NUnit.Framework;
    using SimpleAggregate.Domain;

    [TestFixture]
    public class AggregateShould
    {
        private readonly Fixture _fixture = new Fixture();
        private Parcel _sut = new Parcel();

        [SetUp]
        public void Setup()
        {
            _sut = new Parcel();
        }

        [Test]
        public void AddEventToUncommittedEvents_WhenApplying()
        {
            _sut.DeliverParcel("Mr Delivery Driver");

            _sut.UncommittedEvents.Count.Should().Be(1);
        }

        [Test]
        public void ClearUncommittedEvents()
        {
            _sut.DeliverParcel("Mr Delivery Driver");

            _sut.ClearUncommittedEvents();

            _sut.UncommittedEvents.Count.Should().Be(0);
        }

        [Test]
        public void ApplyEvent()
        {
            var deliveredBy = _fixture.Create<string>();

            _sut.DeliverParcel(deliveredBy);

            _sut.DeliveredBy.Should().Be(deliveredBy);
            _sut.Delivered.Should().BeTrue();
        }

        [Test]
        public void RehydrateAggregate()
        {
            var deliveredBy = _fixture.Create<string>();
            var events = new List<IEvent>()
            {
                new ParcelDelivered
                {
                    DeliveredBy = deliveredBy
                }
            };

            _sut.Rehydrate(events);

            _sut.DeliveredBy.Should().Be(deliveredBy);
        }

        [Test]
        public void NotAddHydratedEventsToUncommittedEvents_WhenHydratingAggregate()
        {
            var deliveredBy = _fixture.Create<string>();
            var events = new List<IEvent>()
            {
                new ParcelDelivered
                {
                    DeliveredBy = deliveredBy
                }
            };

            _sut.Rehydrate(events);

            _sut.UncommittedEvents.Count.Should().Be(0);
        }

        [Test]
        public void ThrowException_WhenApplyingEvent_GivenEventNotRegistered()
        {
            var events = new List<IEvent>()
            {
                new UnregisteredEvent()
            };

            Action act = () => _sut.Rehydrate(events);

            act.Should().Throw<UnregisteredEventException>();
        }
    }
}
