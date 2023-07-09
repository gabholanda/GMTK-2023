using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    [HideInInspector]
    public readonly List<GameEventListener> listeners =
        new List<GameEventListener>();

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised();
    }

    public void RaiseListenerOnly(GameEventListener listener)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            if (listener == listeners[i])
            {
                listeners[i].OnEventRaised();
                break;
            }
    }

    public void RegisterListener(GameEventListener listener)
    { listeners.Add(listener); }

    public void UnregisterListener(GameEventListener listener)
    { listeners.Remove(listener); }
}