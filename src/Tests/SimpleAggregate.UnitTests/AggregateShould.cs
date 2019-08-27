namespace SimpleAggregate.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Domain.Savings;
    using Domain.Savings.Events;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class AggregateShould
    {
        private readonly Fixture _fixture = new Fixture();
        private SavingsAccount _sut;
        private string _accountId;
        private const int SavingsTarget = 500;

        [SetUp]
        public void Setup()
        {
            _sut = new SavingsAccount();
            _accountId = _fixture.Create<string>();
        }

        [Test]
        public void AddEventToUncommittedEvents_WhenApplying()
        {
            _sut.CreateSavingsAccount(_accountId, SavingsTarget);

            _sut.UncommittedEvents.Count.Should().Be(1);
            _sut.UncommittedEvents.First().Should().BeEquivalentTo(new SavingsAccountCreated(_accountId, SavingsTarget));
        }

        [Test]
        public void ClearUncommittedEvents()
        {
            _sut.CreateSavingsAccount(_accountId, SavingsTarget);

            _sut.ClearUncommittedEvents();

            _sut.UncommittedEvents.Count.Should().Be(0);
        }

        [Test]
        public void ApplyEvent()
        {
            _sut.CreateSavingsAccount(_accountId, SavingsTarget);

            _sut.AccountId.Should().Be(_accountId);
            _sut.SavingsTarget.Should().Be(SavingsTarget);
        }

        [Test]
        public void SetAggregateId()
        {
            _sut.CreateSavingsAccount(_accountId, SavingsTarget);

            _sut.AggregateId.Should().Be(_accountId);
        }

        [Test]
        public void RehydrateAggregate()
        {
            var events = new List<object>
            {
                new SavingsAccountCreated(_accountId, SavingsTarget)
            };

            _sut.Hydrate(events);

            _sut.AccountId.Should().Be(_accountId);
            _sut.SavingsTarget.Should().Be(SavingsTarget);
            _sut.HydratedEventCount.Should().Be(1);
        }

        [Test]
        public void IncrementHydratedEventCount_WhenRehydrateAggregate()
        {
            var events = new List<object>
            {
                new SavingsAccountCreated(_accountId, SavingsTarget)
            };

            _sut.Hydrate(events);

            _sut.HydratedEventCount.Should().Be(1);
        }

        [Test]
        public void NotAddHydratedEventsToUncommittedEvents_WhenHydratingAggregate()
        {
            var events = new List<object>
            {
                new SavingsAccountCreated(_accountId, SavingsTarget)
            };

            _sut.Hydrate(events);

            _sut.UncommittedEvents.Count.Should().Be(0);
        }

        [Test]
        public void ThrowException_WhenEventNotRegistered_GivenIgnoreUnregisteredEventsIsFalse()
        {
            var events = new List<object>
            {
                new UnregisteredEvent()
            };

            Action act = () => _sut.Hydrate(events);

            act.Should().Throw<UnregisteredEventException>();
        }

        [Test]
        public void NotThrowException_WhenEventNotRegistered_GivenIgnoreUnregisteredEventsIsTrue()
        {
            _sut.IgnoreUnregisteredEvents();
            var events = new List<object>
            {
                new UnregisteredEvent()
            };

            Action act = () => _sut.Hydrate(events);

            act.Should().NotThrow<UnregisteredEventException>();
        }

        [Test]
        public void ThrowException_WhenApplyingNullEvent()
        {
            Action act = () => _sut.ApplyNullEvent();

            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void AddUncommittedEventsInOrder()
        {
            const int savingsTarget = 100;
            _sut.CreateSavingsAccount(_accountId, savingsTarget);

            _sut.AddSavings(50);
            _sut.SavingsTargetMet.Should().BeFalse();

            _sut.AddSavings(40);
            _sut.SavingsTargetMet.Should().BeFalse();

            _sut.AddSavings(20);
            _sut.SavingsTargetMet.Should().BeTrue();

            _sut.UncommittedEvents[0].Should().BeEquivalentTo(new SavingsAccountCreated(_accountId, savingsTarget));
            _sut.UncommittedEvents[1].Should().BeEquivalentTo(new SavingsAdded(50));
            _sut.UncommittedEvents[2].Should().BeEquivalentTo(new SavingsAdded(40));
            _sut.UncommittedEvents[3].Should().BeEquivalentTo(new SavingsAdded(20));
            _sut.UncommittedEvents[4].Should().BeEquivalentTo(new SavingsTargetMet(110));
        }
    }
}