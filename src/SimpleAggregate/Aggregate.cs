namespace SimpleAggregate
{
    using System;
    using System.Collections.Generic;
    using Domain;

    public abstract class Aggregate
    {
        private readonly Dictionary<Type, Action<IEvent>> _registeredEvents = new Dictionary<Type, Action<IEvent>>();
        public readonly List<IEvent> UncommittedEvents = new List<IEvent>();
        protected bool IgnoreUnregisteredEvents;

        protected void RegisterEvent<TEvent>(Action<TEvent> eventHandler) where TEvent : class
        {
            _registeredEvents.Add(typeof(TEvent), theEvent => eventHandler(theEvent as TEvent));
        }

        protected void Apply(IEvent @event)
        {
            this.ApplyEvent(@event);
            UncommittedEvents.Add(@event);
        }

        private void ApplyEvent(IEvent @event)
        {
            var eventType = @event.GetType();
            _registeredEvents.TryGetValue(eventType, out var eventHandler);

            if (!IgnoreUnregisteredEvents && eventHandler == null)
                throw new UnregisteredEventException($"The requested event '{eventType.FullName}' is not registered in '{GetType().FullName}'");

            eventHandler?.Invoke(@event);
        }

        public void Rehydrate(IEnumerable<IEvent> history)
        {
            foreach (var @event in history) ApplyEvent(@event);
        }

        public void ClearUncommittedEvents()
        {
            UncommittedEvents.Clear();
        }
    }
}
