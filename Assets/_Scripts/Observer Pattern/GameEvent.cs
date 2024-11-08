using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Game Event", menuName = "Game Event")]
public class GameEvent : ScriptableObject
{
    List<GameEventListener> listeners = new List<GameEventListener>();


    public void TriggerListener(Component eventComponent, object data)
    {
        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i].OnTriggerEvent(eventComponent, data);
        }
    }

    public void AddListener(GameEventListener gameEventListener)
    {
        listeners.Add(gameEventListener);
    }

    public void RemoveListener(GameEventListener gameEventListener)
    {
        listeners.Remove(gameEventListener);
    }
}
