using System;
using System.Collections.Generic;
using UnityEngine;

namespace PatternCipher.Client.Events
{
    /// <summary>
    /// A static, centralized event bus that allows different parts of the application
    /// to communicate by publishing and subscribing to events. This decouples components,
    /// improves maintainability, and supports an event-driven architecture.
    /// Systems can react to game state changes without holding direct references to each other.
    /// </summary>
    public static class GameEventSystem
    {
        private static readonly Dictionary<Type, Delegate> _eventListeners = new Dictionary<Type, Delegate>();

        /// <summary>
        /// Subscribes a listener to a specific type of event.
        /// </summary>
        /// <typeparam name="T">The type of the event to listen for.</typeparam>
        /// <param name="listener">The action to execute when the event is published.</param>
        public static void Subscribe<T>(Action<T> listener)
        {
            Type eventType = typeof(T);
            if (_eventListeners.TryGetValue(eventType, out Delegate existingDelegate))
            {
                _eventListeners[eventType] = Delegate.Combine(existingDelegate, listener);
            }
            else
            {
                _eventListeners[eventType] = listener;
            }
        }

        /// <summary>
        /// Unsubscribes a listener from a specific type of event.
        /// </summary>
        /// <typeparam name="T">The type of the event to stop listening for.</typeparam>
        /// <param name="listener">The action that was previously subscribed.</param>
        public static void Unsubscribe<T>(Action<T> listener)
        {
            Type eventType = typeof(T);
            if (_eventListeners.TryGetValue(eventType, out Delegate existingDelegate))
            {
                Delegate resultingDelegate = Delegate.Remove(existingDelegate, listener);
                if (resultingDelegate == null)
                {
                    _eventListeners.Remove(eventType);
                }
                else
                {
                    _eventListeners[eventType] = resultingDelegate;
                }
            }
        }

        /// <summary>
        /// Publishes an event to all subscribed listeners.
        /// </summary>
        /// <typeparam name="T">The type of the event being published.</typeparam>
        /// <param name="eventData">The event data payload to send to listeners.</param>
        public static void Publish<T>(T eventData)
        {
            Type eventType = typeof(T);
            if (_eventListeners.TryGetValue(eventType, out Delegate existingDelegate))
            {
                // Using DynamicInvoke is slightly slower than a direct call but necessary for a generic event system.
                // For performance-critical events, a dedicated event system might be considered.
                try
                {
                    existingDelegate.DynamicInvoke(eventData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error invoking event listener for {eventType.Name}: {ex}");
                }
            }
        }
    }
}