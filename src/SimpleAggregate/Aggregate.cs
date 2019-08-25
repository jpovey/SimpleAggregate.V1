﻿namespace SimpleAggregate
{
    using System;
    using System.Collections.Generic;

    public abstract class Aggregate
    {
        private readonly Dictionary<Type, Action<object>> _registeredEvents = new Dictionary<Type, Action<object>>();
        public readonly List<object> UncommittedEvents = new List<object>();
        protected bool IgnoreUnregisteredEvents;

        protected void RegisterEvent<TEvent>(Action<TEvent> eventHandler) where TEvent : class
        {
            _registeredEvents.Add(typeof(TEvent), theEvent => eventHandler(theEvent as TEvent));
        }

        protected void Apply(object @event)
        {
            this.ApplyEvent(@event);
            UncommittedEvents.Add(@event);
        }

        private void ApplyEvent(object @event)
        {
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
            UncommittedEvents.Clear();
        }
    }
}
