namespace SimpleAggregate
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public abstract class AggregateBase
    {
        public abstract string AggregateId { get; }
        public int HydratedEventCount { get; private set; }
        public ReadOnlyCollection<object> UncommittedEvents => _uncommittedEvents.AsReadOnly();
        private readonly List<object> _uncommittedEvents = new List<object>();
        private readonly Dictionary<Type, Action<object>> _eventHandlers = new Dictionary<Type, Action<object>>();
        protected bool IgnoreUnregisteredEvents;

        protected void RegisterEventHandler<TEvent>(Action<TEvent> eventHandler) where TEvent : class
        {
            _eventHandlers.Add(typeof(TEvent), theEvent => eventHandler(theEvent as TEvent));
        }

        protected void Apply(object @event)
        {
            _uncommittedEvents.Add(@event);
            this.ApplyEvent(@event);
        }

        private void ApplyEvent(object @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event), "The event to be applied is null");

            var eventType = @event.GetType();
            _eventHandlers.TryGetValue(eventType, out var eventHandler);

            if (!IgnoreUnregisteredEvents && eventHandler == null)
                throw new UnregisteredEventException($"The requested event '{eventType.FullName}' is not registered in '{GetType().FullName}'");

            eventHandler?.Invoke(@event);
        }

        public void Hydrate(IEnumerable<object> history)
        {
            foreach (var @event in history)
            {
                ApplyEvent(@event);
                HydratedEventCount += 1;
            }
        }

        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }
    }
}