namespace SimpleAggregate
{
    using System.Collections.Generic;
    using Domain;

    public abstract class Aggregate
    {
        public readonly List<IEvent> UncommittedEvents = new List<IEvent>();

        private void Apply(IEvent @event)
        {
            UncommittedEvents.Add(@event);
        }

        public void ClearUncommittedEvents()
        {
            UncommittedEvents.Clear();
        }
    }
}
