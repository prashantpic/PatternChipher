using System;
using System.Collections.Generic;
using UnityEngine;

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

        private readonly Dictionary<Type, List<Delegate>> _subscribers;

        private GlobalEventBus()
        {
            _subscribers = new Dictionary<Type, List<Delegate>>();
        }

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : GameEvent
        {
            Type eventType = typeof(TEvent);
            if (!_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType] = new List<Delegate>();
            }

            if (!_subscribers[eventType].Contains(handler))
            {
                _subscribers[eventType].Add(handler);
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : GameEvent
        {
            Type eventType = typeof(TEvent);
            if (_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType].Remove(handler);
                if (_subscribers[eventType].Count == 0)
                {
                    _subscribers.Remove(eventType);
                }
            }
        }

        public void Publish<TEvent>(TEvent eventInstance) where TEvent : GameEvent
        {
            Type eventType = typeof(TEvent);
            if (_subscribers.ContainsKey(eventType))
            {
                // Create a copy of the list to avoid issues if a handler unsubscribes itself
                List<Delegate> handlers = new List<Delegate>(_subscribers[eventType]);
                foreach (Delegate handlerDelegate in handlers)
                {
                    try
                    {
                        if (handlerDelegate is Action<TEvent> typedHandler)
                        {
                            typedHandler.Invoke(eventInstance);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error invoking event handler for {eventType.Name}: {ex}");
                    }
                }
            }
        }
    }
}