namespace SimpleAggregate.Repository
{
    using System.Threading.Tasks;

    public class AggregateRepository
    {
        private readonly IEventSource _eventSource;

        public AggregateRepository(IEventSource eventSource)
        {
            _eventSource = eventSource;
        }

        public async Task<T> GetAggregate<T>(string aggregateId) where T : AggregateBase, new()
        {
            var events = await _eventSource.LoadEvents(aggregateId);
            var aggregate = new T();
            if (events?.Count > 0)
            {
                aggregate.Hydrate(events);
            }
            return aggregate;
        }

        public Task Save(AggregateBase aggregate)
        {
            return _eventSource.AppendEvents(aggregate.AggregateId, aggregate.HydratedEventCount, aggregate.UncommittedEvents);
        }
    }
}
