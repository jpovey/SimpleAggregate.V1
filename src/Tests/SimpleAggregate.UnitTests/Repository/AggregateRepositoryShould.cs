namespace SimpleAggregate.UnitTests.Repository
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using AutoFixture;
    using Domain.Savings;
    using Domain.Savings.Events;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using SimpleAggregate.Repository;

    [TestFixture]
    public class AggregateRepositoryShould
    {
        private readonly Fixture _fixture = new Fixture();
        private AggregateRepository _sut;
        private Mock<IEventSource> _eventSourceMock;
        private string _accountId;
        private int _savingsTarget;

        [SetUp]
        public void Setup()
        {
            _accountId = _fixture.Create<string>();
            _savingsTarget = _fixture.Create<int>();
            _eventSourceMock = new Mock<IEventSource>();
            _sut = new AggregateRepository(_eventSourceMock.Object);
        }

        [Test]
        public async Task LoadNewAggregateFromEventSource_GivenEventListIsNull()
        {
            var result = await _sut.GetAggregate<SavingsAccount>(_accountId);

            _eventSourceMock.Verify(x => x.LoadEvents(_accountId), Times.Once);
            result.HydratedEventCount.Should().Be(0);

        }

        [Test]
        public async Task LoadAggregateFromEventSource_GivenEventListIsEmpty()
        {
            _eventSourceMock.Setup(x => x.LoadEvents(_accountId)).ReturnsAsync(new List<object>());

            var result = await _sut.GetAggregate<SavingsAccount>(_accountId);

            _eventSourceMock.Verify(x => x.LoadEvents(_accountId), Times.Once);
            result.HydratedEventCount.Should().Be(0);
        }
        
        [Test]
        public async Task HydrateAggregate_WhenLoadingFromEventSource_GivenEventListIsNotEmpty()
        {
            var savingsAccountCreated = new SavingsAccountCreated(_accountId, _savingsTarget);
            _eventSourceMock.Setup(x => x.LoadEvents(_accountId)).ReturnsAsync(new List<object>{ savingsAccountCreated });

            var result = await _sut.GetAggregate<SavingsAccount>(_accountId);

            result.AccountId.Should().Be(_accountId);
            result.SavingsTarget.Should().Be(_savingsTarget);
        }

        [Test]
        public async Task SaveUncommittedEvents()
        {
            var savingsAccountCreated = new SavingsAccountCreated(_accountId, _savingsTarget);
            _eventSourceMock.Setup(x => x.LoadEvents(_accountId)).ReturnsAsync(new List<object> { savingsAccountCreated });
            var aggregate = await _sut.GetAggregate<SavingsAccount>(_accountId);
            aggregate.AddSavings(50);

            var expectedUncommittedEvents = new List<object>
            {
                new SavingsAdded(50)
            };

            await _sut.Save(aggregate);

            _eventSourceMock.Verify(x => x.AppendEvents(_accountId, 1,
                It.Is<ReadOnlyCollection<object>>(y => VerifyUncommittedEvents(y, expectedUncommittedEvents))));

        }

        private static bool VerifyUncommittedEvents(IEnumerable<object> events, IEnumerable<object> expectedEvents)
        {
           // var readOnlyExpectedEvents = expectedEvents.AsReadOnly();

            events.Should().BeEquivalentTo(expectedEvents);

            return true;
        }
    }
}
