using System;
using System.Collections.Generic;

namespace PatternCipher.Client.Core.Events
{
    public class GlobalEventBus
    {
        private static GlobalEventBus _instance;
        public static GlobalEventBus Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GlobalEventBus();
                }
                return _instance;
            }
        }

        private readonly Dictionary<Type, List<Delegate>> _eventHandlers;

        private GlobalEventBus()
        {
            _eventHandlers = new Dictionary<Type, List<Delegate>>();
        }

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : GameEvent
        {
            Type eventType = typeof(TEvent);
            if (!_eventHandlers.ContainsKey(eventType))
            {
                _eventHandlers[eventType] = new List<Delegate>();
            }
            _eventHandlers[eventType].Add(handler);
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : GameEvent
        {
            Type eventType = typeof(TEvent);
            if (_eventHandlers.ContainsKey(eventType))
            {
                _eventHandlers[eventType].Remove(handler);
                if (_eventHandlers[eventType].Count == 0)
                {
                    _eventHandlers.Remove(eventType);
                }
            }
        }

        public void Publish<TEvent>(TEvent eventInstance) where TEvent : GameEvent
        {
            Type eventType = typeof(TEvent);
            if (_eventHandlers.ContainsKey(eventType))
            {
                // Create a copy of the list to avoid issues if a handler unsubscribes itself during invocation
                List<Delegate> handlers = new List<Delegate>(_eventHandlers[eventType]);
                foreach (Delegate handlerDelegate in handlers)
                {
                    if (handlerDelegate is Action<TEvent> typedHandler)
                    {
                        try
                        {
                            typedHandler.Invoke(eventInstance);
                        }
                        catch (Exception ex)
                        {
                            // Log the exception, e.g., using UnityEngine.Debug.LogError
                            UnityEngine.Debug.LogError($"Error invoking event handler for {eventType.Name}: {ex.Message}\n{ex.StackTrace}");
                        }
                    }
                }
            }
        }
    }
}