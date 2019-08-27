namespace SimpleAggregate.Repository
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    public interface IEventSource
    {
        Task<List<object>> LoadEvents(string aggregateId);
        Task AppendEvents(string aggregateId, int expectedEventCount, ReadOnlyCollection<object> uncommittedEvents);
    }
}