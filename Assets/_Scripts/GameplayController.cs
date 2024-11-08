using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public static GameplayController instance;
    public GameEvent startEvent;

    private void Awake()
    {
        instance = this;
    }

    public void OnStartGameplay(string npcQuantity)
    {
        int _npcQuantity = int.Parse(npcQuantity);

        startEvent.TriggerListener(this, _npcQuantity);

    }
}
