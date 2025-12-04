using System;
using System.Collections.Generic;
using ChronoHeist.Player;
using UnityEngine;

public static class EventManager
{
    #region Event Types

    public interface IEventType
    { }
    
    public struct OnPlayerInitialized : IEventType
    {
        public ChronoHeist.Player.PlayerController player;

        public OnPlayerInitialized(PlayerController player)
        {
            this.player = player;
        }
    }

    public struct OnGridGenerationFinished : IEventType
    {
        
    }

    public struct OnGameLose : IEventType
    {
        
    }
    
    #endregion

    private static Dictionary<Type, Delegate> eventList = new();

    public static void RegisterEvent<T>(Action<T> eventToAdd) where T : IEventType
    {
        Type eventType = typeof(T);
        if (eventList.TryGetValue(eventType, out var existingDelegate))
        {
            eventList[eventType] = Delegate.Combine(existingDelegate, eventToAdd);
        }
        else
        {
            eventList[eventType] = eventToAdd;
        }
    }
    
    public static void DeregisterEvent<T>(Action<T> eventToRemove) where T : IEventType
    {
        Type eventType = typeof(T);
        if (eventList.TryGetValue(eventType, out var existingDelegate))
        {
            eventList[eventType] = Delegate.Remove(existingDelegate, eventToRemove);

            if (eventList[eventType] == null)
            {
                eventList.Remove(eventType);
            }
        }
    }

    public static void TriggerEvent<T>(T eventToTrigger) where T : IEventType
    {
        if (eventList.TryGetValue(typeof(T), out var registeredDelegate) && registeredDelegate is Action<T> action)
        {
            action(eventToTrigger);
        }
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        eventList.Clear();
    }
}