namespace SimpleAggregate
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public abstract class Aggregate
    {
        public ReadOnlyCollection<object> UncommittedEvents => _uncommittedEvents.AsReadOnly();

        private readonly List<object> _uncommittedEvents = new List<object>();
        private readonly Dictionary<Type, Action<object>> _registeredEvents = new Dictionary<Type, Action<object>>();
        protected bool IgnoreUnregisteredEvents;

        protected void RegisterEvent<TEvent>(Action<TEvent> eventHandler) where TEvent : class
        {
            _registeredEvents.Add(typeof(TEvent), theEvent => eventHandler(theEvent as TEvent));
        }

        protected void Apply(object @event)
        {
            this.ApplyEvent(@event);
            _uncommittedEvents.Add(@event);
        }

        private void ApplyEvent(object @event)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event), "The event to be applied is null");

            var eventType = @event.GetType();
            _registeredEvents.TryGetValue(eventType, out var eventHandler);

            if (!IgnoreUnregisteredEvents && eventHandler == null)
                throw new UnregisteredEventException($"The requested event '{eventType.FullName}' is not registered in '{GetType().FullName}'");

            eventHandler?.Invoke(@event);
        }

        public void Rehydrate(IEnumerable<object> history)
        {
            foreach (var @event in history) ApplyEvent(@event);
        }

        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }
    }
}
